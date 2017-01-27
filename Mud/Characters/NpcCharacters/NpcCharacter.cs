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
using Mud.Items;
namespace Mud.Characters.NpcCharacters
{
	/// <summary>
	/// Just a place holder in case we want to make sweeping npc changes later
	/// </summary>
	public class NpcCharacter:MudCharacter
	{	
		Random randGen=new Random();
		protected List<LootTableElement> LootTable=new List<LootTableElement>();
		List<MudCharacter> Attackers=new List<MudCharacter>();
		public NpcCharacter(string name):base(name)
		{
		}
		
		public override int TakeDamage(MudCharacter attacker, int damage)
		{
			
			damage= base.TakeDamage(attacker,damage);
			if(attacker==null || ! (attacker is PlayerCharacter))return damage;
			if(damage<=0)return damage;
			if(!Attackers.Contains(attacker))
			{
				Attackers.Add(attacker as PlayerCharacter);
			}
			return damage;
		}
	
		public override void OnDeath()
		{
			base.OnDeath();
			foreach(MudCharacter c in Attackers)
			{
				if(c is PlayerCharacter)
				{
					PlayerCharacter p=(c as PlayerCharacter);
					p.AddExperience(Level);
					List<MudItem> loot=GetLoot();
					foreach(MudItem item in loot)
						p.ReceiveItem(item);
				}
			}
		}
		
		public List<MudItem> GetLoot()
		{
			List<MudItem> loot=new List<MudItem>();
			foreach(LootTableElement e in LootTable)
			{
				int x=randGen.Next(0,1000);
				if(x<e.Chance)loot.Add(e.Builder());
			}
			return loot;
		}
		
	}
}
