/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/14/2017
 * Time: 12:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
namespace Mud.Actions
{
	/// <summary>
	/// Description of MoveAction.
	/// </summary>
	public enum MapDirection{north,south,east,west}
	public class MoveAction:CharacterAction
	{
		public static ActionBuilder GetActionBuilder()
		{
			return new ActionBuilder("move",
			                  (o)=>{
			                  	bool validArg=false;
			                  	foreach(string a in DirectionString)
			                  	{
			                  		if(a==o.Argument){validArg=true;break;}
			                  	}
			                  	if(validArg==false){throw new ArgumentException("move can only use one of the four cardinal directions");}
			                  	return new MoveAction(o.Sender,o.Argument);
			                  },true);
		}
		public static string[] DirectionString={"north","south","east","west"};
		MapDirection direction;
		public MoveAction(MudCharacter character,string direction):base(character)
		{
			Beneficial=true;
			MapDirection dir=MapDirection.north;
			for(int x=1;x<DirectionString.Length;x++){
				if(DirectionString[x]==direction)
				{
					dir=(MapDirection)x;
				}
			}
			this.direction=dir;
		}
		public MoveAction(MudCharacter character,MapDirection dir):base(character)
		{
			direction=dir;
		}
		
		public override string DoAction()
		{
			DungeonPosition pos=((PlayerCharacter)(Character)).Room.Position;
			int x=pos.X;
			int y=pos.Y;
			DungeonRoom room=Character.Dungeon.GetRoom(pos.X,pos.Y);
			DungeonRoom NewRoom;
			//room.RemoveCharacter(Character);
			switch(direction){
				case MapDirection.north:
					y-=1;
					break;
				case MapDirection.south:
					y+=1;
					break;
				case MapDirection.east:
					x+=1;
					break;
				case MapDirection.west:
					x-=1;
					break;
			
			}
			foreach(MudCharacter c in Character.Room.GetCharactersInRoom())
			{
				if(!(c is PlayerCharacter))
				{
					return "Some sort of magic barrier prevents you from leaving a room with live evil minions";
				}
				
			}
			NewRoom=Character.Dungeon.GetRoom(x,y);
			if(NewRoom==null) return String.Format("{0} ran into the wall",Character.Name);
			room.RemoveCharacter(Character);
			room.NotifyPlayers(Character.Name+" has left the room heading " + MoveAction.DirectionString[(int)direction]);
			NewRoom.AddCharacter(Character);
			return "";
		
		}
	}
}
