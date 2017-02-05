/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/4/2017
 * Time: 2:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
using Mud.Actions;
using Mud;
using System.Text;
namespace TestClient
{
	/// <summary>
	/// Description of Interpreter.
	/// </summary>
	public class MudInterpreter
	{
		enum InterpreterState{WaitingForUser,WaitingForPassword,Standard};
		InterpreterState status=InterpreterState.WaitingForUser;
		MudConnection Connection;
		PlayerCharacter player=null;
		Dungeon dungeon;
		string user=null;
		string pass=null;
		public MudInterpreter(MudConnection con,Dungeon dungeon)
		{
			this.dungeon=dungeon;
			Connection=con;
			con.SendString("user name: ");
		}
		
		public void HandleInput(string input)
		{
			input=input.Trim();
			if(status != InterpreterState.Standard)
			{
				Initialize(input);
				return;
			}
			
			string[] inputArgs=input.Split(' ');
				
			if(inputArgs.Length>=1)
			{
				string action=inputArgs[0].ToLower();
				string arg=input.Remove(0,action.Length);
				try
				{
					ActionBuilder builder=player.GetAction(action);
					ActionArgs a=ActionArgs.GetActionArgs(player,arg);
				
					if(builder!=null && a!=null)
					{
						CharacterAction act=builder.BuildAction(a);
						if(act!=null)
						{
							if(act is TargetedAction && ((TargetedAction)act).Target==null)
							{
								player.NotifyPlayer("Targeted actions must have a valid target");
								return;
							}
						player.Room.AddActionToQueue(builder.BuildAction(a));
						}
						
					}
				}catch(ArgumentException ex){
					player.NotifyPlayer(ex.Message);
				}
			}
			if(inputArgs[0].ToLower()=="help")
			{
				string message="Available Actions:\n ";
				foreach(string s in player.GetActionList())
				{
					message=string.Format("{0} {1}",message,s);
				}
				player.NotifyPlayer(message);
					
			}
			if(inputArgs[0].ToLower()=="inventory")
			{
				foreach(string s in player.GetInventory())
				{
					player.NotifyPlayer(s);
				}
			}
			if(inputArgs[0].ToLower()=="status")
			{
				player.NotifyPlayer(player.StatusString());
			}
			
		}
		void Initialize(string input)
		{
			switch(status)
			{
				case InterpreterState.WaitingForUser:
					if(input.Split(' ').Length>1)
					{
						Connection.SendString("Bad user name");
						
					}else{
						user=input;
						Connection.SendString("password: ");
						status=InterpreterState.WaitingForPassword;
					}
					break;
				case InterpreterState.WaitingForPassword:
					System.Security.Cryptography.HashAlgorithm Hash=System.Security.Cryptography.SHA512.Create();
					byte[] hash=Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
					string hashString=Convert.ToBase64String(hash);
					pass=hashString;
					player=new PlayerWarrior(user);
					player.OnNotifyPlayer+=(c,s)=>Connection.SendString(s+"\r\n");
					player.OnNewRoomates+=new Action<PlayerCharacter>(NewRoomates);
					dungeon.AddCharacter(player,dungeon.StartingRoom);
					Logger.Log(string.Format("{0} {1} : {2}",DateTime.Now,user,Connection.ConnectionSocket.RemoteEndPoint.ToString()));
					status=InterpreterState.Standard;
					break;
			}
		}
		void NewRoomates(PlayerCharacter p)
		{
			
		//	currentRoomates=player.Roomates;
			string msg="In the room now |   ";
			for(int x=0;x<player.Roomates.Length;x++)
			{
				msg+=string.Format("{0}:{1}  ",x,player.Roomates[x].StatusString());
			}
			//if(player.Roomates.Length>1)
			player.NotifyPlayer(msg);
			
		}
		
		public void Shutdown()
		{
			if(player!=null){
				player.Room.RemoveCharacter(player);
				player.Room.NotifyPlayers("{0} disconnected",player.Name);
			}
		}
	}
	
}
