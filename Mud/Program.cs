/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/13/2017
 * Time: 7:08 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Actions;
using Mud.Characters;
using Mud.Characters.NpcCharacters;
using Mud.Interface;
using Mud;
namespace TestClient
{
	class Program
	{
		
		public static void Main(string[] args)
		{
			Program prog=new Program();
			Dungeon d=new Dungeon(3,3,new DungeonPosition(2,2));
			//MudCharacter mob=new Weakling("Skeleton");
			d.AddNpcsToRoom((new string[]{"goblin"}),new DungeonPosition(1,2));
			d.AddNpcsToRoom((new string[]{"goblin"}),new DungeonPosition(0,2));
			d.AddNpcsToRoom((new string[]{"skeleton"}),new DungeonPosition(2,1));
			d.AddNpcsToRoom((new string[]{"skeleton"}),new DungeonPosition(1,1));
			d.AddNpcsToRoom((new string[]{"orc"}),new DungeonPosition(0,0));
			d.AddNpcsToRoom((new string[]{"orc"}),new DungeonPosition(1,0));
			d.AddNpcsToRoom((new string[]{"orc","orc"}),new DungeonPosition(2,0));
			d.SetRoom(new DungeonPosition(0,1),new HealingRoom(d,new DungeonPosition(0,1)));
			d.SetRoom(new DungeonPosition(2,2),new HealingRoom(d,new DungeonPosition(2,2)));
			MudServer server=new MudServer(d);
			Console.WriteLine("password for mud database");
			string pass=Console.ReadLine().Trim();
			SqlSimplifier.Setparams("127.0.0.1","mud",pass);
			SqlSimplifier.GetInstance().CreateDataBase();
			server.Start();
			//prog.RunGame();
		}
		
		PlayerCharacter player=null;
		MudCharacter[] currentRoomates=new MudCharacter[0];
			
		
		public void RunGame()
		{
			
			Dungeon d=new Dungeon(3,3,new DungeonPosition(2,2));
			MudCharacter mob=new Weakling("Skeleton");
			d.AddNpcsToRoom((new string[]{"goblin"}),new DungeonPosition(1,2));
			d.AddNpcsToRoom((new string[]{"goblin"}),new DungeonPosition(0,2));
			d.AddNpcsToRoom((new string[]{"skeleton"}),new DungeonPosition(2,1));
			d.AddNpcsToRoom((new string[]{"skeleton"}),new DungeonPosition(1,1));
			d.AddNpcsToRoom((new string[]{"orc"}),new DungeonPosition(0,0));
			d.AddNpcsToRoom((new string[]{"orc"}),new DungeonPosition(1,0));
			d.AddNpcsToRoom((new string[]{"orc","orc"}),new DungeonPosition(2,0));
			d.SetRoom(new DungeonPosition(0,1),new HealingRoom(d,new DungeonPosition(0,1)));
			d.SetRoom(new DungeonPosition(2,2),new HealingRoom(d,new DungeonPosition(2,2)));
			while(true){
				string input=Console.ReadLine();
				string[] inputArgs=input.Split(' ');
				
				if(inputArgs.Length>=1){
					string action=inputArgs[0].ToLower();
					string arg=input.Remove(0,action.Length);
					try{
						ActionBuilder builder=player.GetAction(action);
						ActionArgs a=ActionArgs.GetActionArgs(player,arg);
					
						if(builder!=null && a!=null){
							CharacterAction act=builder.BuildAction(a);
							if(act!=null)
							{
								if(act is TargetedAction && ((TargetedAction)act).Target==null)
								{
									player.NotifyPlayer("Targeted actions must have a valid target");
									continue;
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
					Console.Write("available actions: ");
					foreach(string s in player.GetActionList())
					{
						Console.Write("{0} ",s);
					}
					Console.WriteLine("");
					Console.Out.Flush();
					
				}
				if(inputArgs[0].ToLower()=="inventory")
				{
					foreach(string s in player.GetInventory())
					{
						Console.WriteLine(s);
					}
				}
				if(inputArgs[0].ToLower()=="status")
				{
					Console.WriteLine(player.StatusString());
					Console.Out.Flush();
				}
				
			}
		}
	}
}