/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/17/2017
 * Time: 8:30 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
namespace Mud.Actions
{
	/// <summary>
	/// Description of TargetedAction.
	/// </summary>
	public class TargetedAction:CharacterAction
	{
		public MudCharacter Target{get;private set;}
		public TargetedAction(MudCharacter character,MudCharacter target):base(character)
		{
			Target=target;
		}
		public override string DoAction()
		{	
			Random rand=new Random();
			MudCharacter[] characters;
			if(!Character.Room.HasCharacter(Target))
			{
				//attempt to pick a new target
				if(Target is PlayerCharacter)
				{
					characters=Character.Room.GetPlayersInRoom();
				}else{
					characters=Character.Room.GetNonPlayersInRoom();
				}
				if(characters.Length==0){Target=null; return"";}
				Target=characters[rand.Next(0,characters.Length)];
				
				
			}
			return "";
		}
	}
}
