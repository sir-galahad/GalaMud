/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/23/2017
 * Time: 10:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
namespace Mud
{
	/// <summary>
	/// Description of HealingRoom.
	/// </summary>
	public class HealingRoom:DungeonRoom
	{
		
		public HealingRoom(Dungeon parent,DungeonPosition pos):base(parent,pos)
		{
			Message="You have a pleasently calm feeling, your wounds are healed, and you can't bring yourself to perform even the slightest aggressive actions";
		}
		
		public override void AddCharacter(Mud.Characters.MudCharacter character)
		{
			base.AddCharacter(character);
			character.Heal(character.MaxHitPoints);
		}
		public override void AddActionToQueue(Mud.Actions.CharacterAction action)
		{
			if(!action.Beneficial)
			{
				if(action.Character is PlayerCharacter)
				{
					(action.Character as PlayerCharacter).NotifyPlayer("Harmful actions may not be taken in this sacred place");
				}
				return;
			}
			base.AddActionToQueue(action);
		}
		public override void SpawnNpcs()
		{
			//ATTEMPT NO SPAWNINGS HERE;
		}
	}
}
