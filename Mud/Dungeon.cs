/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/14/2017
 * Time: 8:56 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
namespace Mud
{
	/// <summary>
	/// Description of Dungeon.
	/// </summary>
	public class Dungeon
	{
		Dictionary<string,PlayerCharacter> Players=new Dictionary<string, PlayerCharacter>();
		DungeonRoom[,] Map;
		int mapX;
		int mapY;
		public DungeonPosition StartingRoom{get;private set;}
		public Dungeon(int mapx,int mapy,DungeonPosition startingRoom)
		{
			mapX=mapx;
			mapY=mapy;
			StartingRoom=startingRoom;
			Map=new DungeonRoom[mapx,mapy];
			for(int x=0;x<mapx;x++){
				for(int y=0;y<mapy;y++){
					Map[x,y]=new DungeonRoom(this,new DungeonPosition(x,y));
				}
			}
			
		}
		public void AddCharacter(MudCharacter character,DungeonPosition pos)
		{
			Map[pos.X,pos.Y].AddCharacter(character);
			character.JoinDungeon(this);
			character.SetRoom(Map[pos.X,pos.Y]);
		}
		public DungeonRoom GetRoom(int xloc,int yloc)
		{	if(xloc<0||xloc>=mapX) return null;
			if(yloc<0||yloc>=mapY) return null;
			return Map[xloc,yloc];
		}
		public DungeonRoom GetRoom(DungeonPosition pos)
		{
			return GetRoom(pos.X,pos.Y);
		}
	}
}
