/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/20/2017
 * Time: 10:44 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Actions;
namespace Mud.Characters
{
	/// <summary>
	/// Description of PlayerWarrior.
	/// </summary>
	public class PlayerWarrior:PlayerCharacter
	{
		public PlayerWarrior(string name):base(name)
		{
			AddActionToList(MoveAction.GetActionBuilder());
			AddActionToList(AttackAction.GetActionBuilder());
			AddActionToList(EquipAction.GetActionBuilder());
			AddActionToList(BonkAction.GetActionBuilder());
		}
	}
}
