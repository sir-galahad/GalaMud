﻿/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/18/2017
 * Time: 1:42 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
namespace Mud.Items
{
	/// <summary>
	/// Description of SimpleWeapon.
	/// </summary>
	public class SimpleWeapon:WeaponItem
	{
		int Power;
		public SimpleWeapon(string name, string desc, int power):base(name,desc)
		{
			
			Power=power;
			MaxCount=1;
			Description=string.Format("{0} (+{1} Power)",Description,Power);
		}
		
		public override int GetDamage(PlayerCharacter player)
		{
			return Power;
		}
	}
}
