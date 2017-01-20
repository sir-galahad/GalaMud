/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/20/2017
 * Time: 1:22 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
namespace Mud.Characters.NpcCharacters
{
	/// <summary>
	/// Just a place holder in case we want to make sweeping npc changes later
	/// </summary>
	public class NpcCharacter:MudCharacter
	{
		List<PlayerCharacter> Attackers=new List<PlayerCharacter>();
		public NpcCharacter(string name):base(name)
		{
		}
		
		public override int TakeDamage(MudCharacter attacker, int damage)
		{
			
			damage= base.TakeDamage(attacker,damage);
			if(attacker==null || ! (attacker is PlayerCharacter))return damage;
			if(damage<=0)return damage;
			Attackers.Add(attacker as PlayerCharacter);
			return damage;
		}
		
		public override void OnDeath()
		{
			base.OnDeath();
			foreach(PlayerCharacter c in Attackers)
			{
				c.AddExperience(Level);
			}
		}
	}
}
