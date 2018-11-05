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
using Mud.Interface;
namespace Mud.Characters
{
	/// <summary>
	/// Description of MageCharacter.
	/// </summary>
	public class MageCharacter:PlayerCharacter
	{
	
		public MageCharacter(string name, MudConnection conn):base(name,conn)
		{
			this.Description = "Mages are capable of doing the most overall damage, but are also the most susceptable to damage as well";
			AddActionToList(MageAttack.GetActionBuilder());
			armorMod=3;
		}
		
		public MageCharacter(string name,int level,int experience, MudConnection conn):base(name,level,experience, conn)
		{
			this.Description = "Mages are capable of doing the most overall damage, but are also the most susceptable to damage as well";
			AddActionToList(MageAttack.GetActionBuilder());
			armorMod=3;
		}
	}
}
