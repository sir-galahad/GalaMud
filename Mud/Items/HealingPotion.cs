/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/10/2017
 * Time: 11:13 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
namespace Mud.Items
{
	/// <summary>
	/// Description of HealingPotion.
	/// </summary>
	public class HealingPotion : MudItem, IUseable
	{
		public HealingPotion():base("Healing-potion","A magical potion that will restore upto 1/2(+1) of a characters maximum HP")
		{
			this.MaxCount=5;
		}
		
		public string Use(MudCharacter user)
		{
			int initHp=user.HitPoints;
			int toHeal=user.MaxHitPoints/2;
			toHeal+=1;
			user.Heal(toHeal);
			int hp=user.HitPoints;
			hp=hp-initHp;
			return string.Format("{0} used a healing-potion to restore {1} HP",user.Name,hp);
		}
	}
}
