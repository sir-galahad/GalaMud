/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/20/2017
 * Time: 12:39 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
namespace Mud.Actions
{
	/// <summary>
	/// Description of ActionArgs.
	/// </summary>
	public class ActionArgs
	{
		public readonly MudCharacter Sender;
		public readonly MudCharacter Target;
		public readonly string Argument;
		public static ActionArgs GetActionArgs(PlayerCharacter sender, string toParse)
		{
			MudCharacter target=null;
			string arguments=null;
			
			string[] words=toParse.Split(' ');
			if(words.Length>2)
			{
				return null;
			}
			DungeonRoom room=sender.Room;
			MudCharacter[] characters=room.GetCharactersInRoom();
			foreach(string word in words)
			{
				if(target!=null && arguments==null)
				{
					arguments=word;
					break;
				}
				foreach(MudCharacter c in characters){
					if(c.Name==word)
					{
						target=c;
						break;
					}
				}
				arguments=word;
			}
			return new ActionArgs(sender,target,arguments);
		}
		public ActionArgs(MudCharacter sender, MudCharacter target)
		{
			Sender=sender;
			Target=target;
			Argument=null;
		}
		public ActionArgs(MudCharacter sender)
		{
			Sender=sender;
			Target=null;
			Argument=null;
		}
		public ActionArgs(MudCharacter sender,MudCharacter target,string argument)
		{
			Sender=sender;
			Target=target;
			Argument=argument;
		}
	}
}
