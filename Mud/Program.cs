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
			MudServerXml server=new MudServerXml(d);
			string pass = "";
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

				
			}
		}
	}
}