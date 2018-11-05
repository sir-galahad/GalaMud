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
using Mud.Interface;
namespace Mud.Characters
{
	/// <summary>
	/// Description of PlayerWarrior.
	/// </summary>
	public class WarriorCharacter:PlayerCharacter
	{
		public WarriorCharacter(string name,MudConnection conn ):base(name,conn)
		{

			this.Description = "Masters of physical combat with many tricks to subdue their enemies";
			AddActionToList(BonkAction.GetActionBuilder());
		
			AddActionToList(AttackAction.GetActionBuilder());
			
		}
		
		public WarriorCharacter(string name,int level,int experience, MudConnection conn):base(name,level,experience,conn)
		{
			this.Description = "Masters of physical combat with many tricks to subdue their enemies";
			AddActionToList(AttackAction.GetActionBuilder());
			
			AddActionToList(BonkAction.GetActionBuilder());
			
		}
	}
}
