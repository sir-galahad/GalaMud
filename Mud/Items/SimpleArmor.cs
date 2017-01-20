/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/18/2017
 * Time: 12:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
namespace Mud.Items
{
	/// <summary>
	/// Description of SimpleArmor.
	/// </summary>
	public class SimpleArmor:ArmorItem
	{
		int Armor;
		public SimpleArmor(string name, string desc,int armor):base(name,desc)
		{
			Armor=armor;
		}
		public override int GetArmor(PlayerCharacter player)
		{
			return Armor;
		}
	}
}
