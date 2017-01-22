/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/14/2017
 * Time: 8:57 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Mud.Items;
namespace Mud.Characters
{
	/// <summary>
	/// Description of PlayerCharacter.
	/// </summary>
	public class PlayerCharacter:MudCharacter
	{
		
		Inventory inventory=new Inventory();
		int Experience=0;
		public WeaponItem EquipedWeapon{get;protected set;}
		public ArmorItem EquipedArmor{get;protected set;}
		public override int MaxHitPoints {
			get {
				return (int)(Level*1.5);
			}
			protected set {
				base.MaxHitPoints = value;
			}
		}
		public override int Armor {
			get {
				if(EquipedWeapon==null)
					return base.Armor;
				return EquipedArmor.GetArmor(this);
			}
			protected set {
				base.Armor = value;
			}
		}
		public event Action<PlayerCharacter,string>OnNotifyPlayer;
		// override bool IsPlayer{get{return true;}set{}}
		public PlayerCharacter(string Name):base(Name)
		{
		}
		
		public override int GetDamage()
		{
			if(EquipedWeapon==null)return 1;
			return EquipedWeapon.GetDamage(this);
		}
		public void NotifyPlayer(string msg, params object[] args)
		{
			OnNotifyPlayer(this,string.Format(msg, args));
		}
		public override void GetAction()
		{
			
		}
		public void AddExperience(int exp)
		{
			Experience+=exp;
			NotifyPlayer("You have gained {0} experience points",exp);
			if(Experience>=Level*Level)
			{
				LevelUp();
			}
		}
		
		void LevelUp()
		{
			Level+=1;
			HitPoints=MaxHitPoints;
			NotifyPlayer("You increased in skill! you are now level: {0}",Level);
		}
		
		public override void OnDeath()
		{
			NotifyPlayer("You have died and will be sent back to the begining");
			DungeonRoom r=Dungeon.GetRoom(Dungeon.StartingRoom);
			HitPoints=MaxHitPoints;
			r.AddCharacter(this);
			
		}
		
		public void ReceiveItem(MudItem a)
		{
			inventory.AddItem(a);
			NotifyPlayer("You looted {0}",a.Name);
		}
		
		public bool Equip(string itemname)
		{
			bool result=false;
			MudItem tmp;
			MudItem item=inventory.PullItemByName(itemname);
			if(item==null)
			{
				inventory.AddItem(item);
				return result;
			}
			if(item is WeaponItem)
			{
				tmp=EquipedWeapon as WeaponItem;
				if(tmp!=null)
				{
					inventory.AddItem(tmp);
				}
			}
			if(item is ArmorItem)
			{
				tmp=EquipedArmor as ArmorItem;
				if(tmp!=null)
				{
					inventory.AddItem(tmp);
				}
			}
			return false;
		}
	}
}
