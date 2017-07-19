/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/17/2017
 * Time: 9:55 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Actions;
namespace Mud.Characters
{
	/// <summary>
	/// Description of MageCharacter.
	/// </summary>
	public class MageCharacter:PlayerCharacter
	{
	
		public MageCharacter(string name):base(name)
		{
			AddActionToList(MageAttack.GetActionBuilder());
			armorMod=3;
		}
		
		public MageCharacter(string name,int level,int experience):base(name,level,experience)
		{
			AddActionToList(MageAttack.GetActionBuilder());
			armorMod=3;
		}
	}
}
