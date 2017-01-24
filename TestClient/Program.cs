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
using Mud;
namespace TestClient
{
	class Program
	{
		public static void Main(string[] args)
		{
			Dungeon d=new Dungeon(3,3,new DungeonPosition(2,2));
			PlayerCharacter player=new PlayerWarrior("galahad");
			PlayerCharacter player2=new PlayerCharacter("*test2*");
			MudCharacter mob=new Weakling("Skeleton");
			player2.OnNotifyPlayer+=(P,S)=>Console.WriteLine(S);
			player.OnNotifyPlayer+=(P,S)=>{Console.WriteLine(S); Console.Out.Flush();};
			d.AddCharacter(player,d.StartingRoom);
			d.AddCharacter(player2,d.StartingRoom);
			d.AddNpcsToRoom((new string[]{"skeleton"}),new DungeonPosition(1,1));
			d.AddNpcsToRoom((new string[]{"skeleton"}),new DungeonPosition(2,1));
			d.AddNpcsToRoom((new string[]{"skeleton"}),new DungeonPosition(0,1));
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
								player.Room.AddActionToQueue(builder.BuildAction(a));
							
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
				
			}
		}
	}
}