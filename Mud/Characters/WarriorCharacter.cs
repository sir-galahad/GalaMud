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
	public class WarriorCharacter:PlayerCharacter
	{
		public WarriorCharacter(string name):base(name)
		{
			
			AddActionToList(BonkAction.GetActionBuilder());
		
			AddActionToList(AttackAction.GetActionBuilder());
			
		}
		
		public WarriorCharacter(string name,int level,int experience):base(name,level,experience)
		{
			
			AddActionToList(AttackAction.GetActionBuilder());
			
			AddActionToList(BonkAction.GetActionBuilder());
			
		}
	}
}
