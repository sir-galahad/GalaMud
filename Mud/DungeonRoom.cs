/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/13/2017
 * Time: 7:09 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Mud.Actions;
using Mud.Characters;
using Mud.Characters.NpcCharacters;
namespace Mud
{
	/// <summary>
	/// Description of Room.
	/// </summary>
	public class DungeonRoom
	{
		public string Message{get;set;}
		public string Status{get;private set;}
		List<string> SpawnList=new List<string>();
		int timeoutSeconds=10;
		Object lockobject=new Object();
		List<MudCharacter> NonPlayersInRoom=new List<MudCharacter>();
		List<PlayerCharacter> PlayersInRoom=new List<PlayerCharacter>();
		Queue<CharacterAction> ActionQueue=new Queue<CharacterAction>();
		DateTime time=DateTime.MinValue;
		public readonly DungeonPosition Position;
		Dungeon parentDungeon;
		Task ActionTask=null;
		public DungeonRoom(Dungeon parent,DungeonPosition pos)
		{
			Message="Another dirty, damp dungeon chamber";
			parentDungeon=parent;
			Position=pos;
		}
		
		public MudCharacter[] GetCharactersInRoom()
		{
			MudCharacter[] characters=new MudCharacter[PlayersInRoom.Count+NonPlayersInRoom.Count];
			PlayersInRoom.ToArray().CopyTo(characters,0);
			NonPlayersInRoom.ToArray().CopyTo(characters,PlayersInRoom.Count);
			return characters;
		}
		
		public void NotifyPlayers(string msg,params string[] arguments)
		{
			foreach(PlayerCharacter c in PlayersInRoom){
			
				c.NotifyPlayer(msg,arguments);
				
			}
		}
		
		public virtual void AddActionToQueue(CharacterAction action)
		{
			lock(lockobject)
			{	foreach(CharacterAction a in ActionQueue){
					if(a.Character==action.Character){
						if(a.Character is PlayerCharacter){
							(a.Character as PlayerCharacter).NotifyPlayer("you've already registered an action for this round");
						}
						return;
					}
				}
				ActionQueue.Enqueue(action);
			}
		}
		
		public virtual void AddCharacter(MudCharacter character)
		{
			lock(lockobject){
				
				if(character is PlayerCharacter){
					if(PlayersInRoom.Count==0)
					{
						SpawnNpcs();
					}
					PlayersInRoom.Add(character as PlayerCharacter);
					Status=GenerateStatus();
					int characterCount=PlayersInRoom.Count+NonPlayersInRoom.Count;
					(character as PlayerCharacter).NotifyPlayer("Entering the room you find: {0}",Message);
				}else{
					NonPlayersInRoom.Add(character); 
					character.SetRoom(this);
					return;
				}
				
				Status=GenerateStatus();
				character.SetRoom(this);
				if(ActionTask==null){
					if(time==DateTime.MinValue)time=DateTime.Now.AddSeconds(timeoutSeconds);
					ActionTask=Task.Factory.StartNew(new Action(ExecuteQueue));
				}
				
				foreach(PlayerCharacter p in PlayersInRoom)
				{
					if(p!=character)
					{
						p.NotifyPlayer("{0} has entered the room",character.StatusString());
					}
				}
				//character.SetRoom(this);
			}
		}
		
		string GenerateStatus()
		{
			StringBuilder sb=new StringBuilder();
			foreach(MudCharacter c in PlayersInRoom)
			{
				sb.Append(c.StatusString());
				sb.Append(' ');
			}
			foreach(MudCharacter c in NonPlayersInRoom)
			{
				sb.Append(c.StatusString());
				sb.Append(' ');
			}
			return sb.ToString();
		}
		
		public void RemoveCharacter(MudCharacter character)
		{
			lock(lockobject){
				if(PlayersInRoom.Contains(character as PlayerCharacter)){
					PlayersInRoom.Remove(character as PlayerCharacter);
					if(PlayersInRoom.Count==0){
						NonPlayersInRoom.Clear();
					}
				}
				else{NonPlayersInRoom.Remove(character);}
				Status = GenerateStatus();
				
				if(PlayersInRoom.Count==0){
					NonPlayersInRoom.Clear();
				}
			}
		}
		
		public PlayerCharacter[] GetPlayersInRoom()
		{
			return PlayersInRoom.ToArray();
		}
		
		public MudCharacter[] GetNonPlayersInRoom()
		{
			return NonPlayersInRoom.ToArray();
		}
		void ExecuteQueue()
		{	
			try{
				bool ready=false;
				foreach(MudCharacter character in NonPlayersInRoom)
				{
					character.StartTurn();
				}
				foreach(MudCharacter character in PlayersInRoom)
				{
					character.StartTurn();
				}
				//give all characters a chance to put in an action
				while(!ready)
				{
					lock(lockobject){
						foreach(MudCharacter character in PlayersInRoom) //test for an action from all characters
						{
							ready=false;
							foreach(CharacterAction action in ActionQueue)
							{
								if(action.Character==character)
								{
									ready=true;
									break;
								}
								
							}
							if(!ready)
							{
								break;
							}
						}
						if(DateTime.Now>time) //if time is up execute all actions in the queue 
						{
							ready=true;
						}
					}
					Thread.Sleep(100);
				}	
				
				lock(lockobject)
				{
					while(ActionQueue.Count>0)
					{
						CharacterAction action=ActionQueue.Dequeue();
						//skip actions by characters no longer in room(probably dead)
						if(action.Character is PlayerCharacter)
						{
							if(!PlayersInRoom.Contains(action.Character as PlayerCharacter))
							{
								continue;
							}
						}else{
							if(!NonPlayersInRoom.Contains(action.Character))
							{
								continue;
							}
						}
						//skip actions on targets no longer in room
						if(action is TargetedAction)
						{
							TargetedAction ta=(action as TargetedAction);
							if(ta.Target is PlayerCharacter)
							{
								if(!PlayersInRoom.Contains(ta.Target as PlayerCharacter))
								{
									continue;
								}
							}else{
								if(!NonPlayersInRoom.Contains(ta.Target))
								{
									continue;
								}
							}
						}
						
						string msg=action.DoAction();
						NotifyPlayers(msg);
						int x=0;
						//test for death
						//funky loops because the lists we're iterating might be changed
						
						while(x<PlayersInRoom.Count)
						{
							PlayerCharacter c=PlayersInRoom[x];
							if(c.HitPoints<=0)
							{
								NotifyPlayers("\t{0} has died.",c.StatusString());
								this.RemoveCharacter(c);
								c.OnDeath();
								continue;
 							}
							x++;
						}
						x=0;
						while(x<NonPlayersInRoom.Count)
						{
							MudCharacter c=NonPlayersInRoom[x];
							if(c.HitPoints<=0)
							{
								NotifyPlayers("\t{0} has died",c.StatusString());
								NonPlayersInRoom.Remove(c);
								c.OnDeath();
								continue;
							}
							x++;
						}
					}
					Status=GenerateStatus();
				}	
				
				Thread.Sleep(100);
				
				lock(lockobject)
				{
					if(PlayersInRoom.Count==0){
						ActionQueue.Clear();
						ActionTask=null;
						time=DateTime.MinValue;
					
					}else{
						time=DateTime.Now.AddSeconds(timeoutSeconds);
						ActionTask=Task.Factory.StartNew(new Action(ExecuteQueue));
					}
				}
			
			}catch(Exception ex){
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
		}
		
		public void AddNpcsToSpawn(string[] mobs)
		{
			SpawnList.AddRange(mobs);
		}
		
		public virtual void SpawnNpcs()
		{
			NpcFactory factory=NpcFactory.GetInstance();
			foreach(string s in SpawnList)
			{
				AddCharacter(factory.GetCharacter(s));
			}
		}
		
		public bool HasCharacter(MudCharacter c)
		{
			MudCharacter[] characters=GetCharactersInRoom();
			foreach(MudCharacter other in characters)
			{
				if(c==other){return true;}
			}
			return false;
		}
	}
}
