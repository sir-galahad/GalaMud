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
			PlayerCharacter player=new PlayerCharacter("*test*");
			PlayerCharacter player2=new PlayerCharacter("*test2*");
			MudCharacter mob=new Weakling("Skeleton");
			player2.OnNotifyPlayer+=(P,S)=>Console.WriteLine(S);
			player.OnNotifyPlayer+=(P,S)=>{Console.WriteLine(S); Console.Out.Flush();};
			d.AddCharacter(player,d.StartingRoom);
			d.AddCharacter(player2,d.StartingRoom);
			d.AddCharacter(mob,new DungeonPosition(1,1));
			while(true){
				string input=Console.ReadLine();
				string[] inputArgs=input.Split(' ');
				
				if(inputArgs.Length>=2){
					string action=inputArgs[0].ToLower();
					string direction=inputArgs[1].ToLower();
					if(action=="m" || action=="move"){
						int x;
						for(x=0;x<MoveAction.DirectionString.Length;x++){
							if(direction==MoveAction.DirectionString[x]) break;
						}
					
						CharacterAction Action=new MoveAction(player,(MapDirection)x);
						player.Room.AddActionToQueue(Action);
					}
				}
			}
		}
	}
}