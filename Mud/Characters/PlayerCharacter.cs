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
	/// A character operated by an actual player
	/// special cases like equipable items and inventory added
	/// </summary>
	public class PlayerCharacter:MudCharacter
	{
		public event Action<PlayerCharacter> OnTurnStart;
		public event Action<PlayerCharacter> OnNewRoomates;
		public event Action<PlayerCharacter,MudItem> InventoryChange;
		public event Action<PlayerCharacter,string> ItemEquiped;
		public event Action<PlayerCharacter,int> ExperienceGained;
		Inventory inventory=new Inventory();
		public int Experience{get;private set;}
		public MudCharacter[] Roomates{get;private set;}
		public WeaponItem EquipedWeapon{get;protected set;}
		public ArmorItem EquipedArmor{get;protected set;}
		public override int Power {
			get {
				if(EquipedWeapon==null)return 1;
				return EquipedWeapon.GetDamage(this);
			}
			protected set {
				base.Power = value;
			}
		}
		
		public override int MaxHitPoints {
			get {
				return (int)(Level*2);
			}
			protected set {
				base.MaxHitPoints = value;
			}
		}
		
		public override int Armor {
			get {
				if(EquipedArmor==null)
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
			Experience=0;
			EquipedWeapon=null;
			EquipedArmor=null;
			this.HitPoints=MaxHitPoints;
		}
		
		public PlayerCharacter(string name,int level,int experience):base(name)
		{
			EquipedWeapon=null;
			EquipedArmor=null;
			Level=level;
			Experience=experience;
			this.HitPoints=MaxHitPoints;
		}
		
		public void SetInventory(Inventory inventory)
		{
			this.inventory=inventory;
		}
		
		public void NotifyPlayer(string msg, params object[] args)
		{
			OnNotifyPlayer(this,string.Format(msg, args));
		}
	
		public override void StartTurn()
		{
			base.StartTurn();
			MudCharacter[] tmp=Room.GetCharactersInRoom();
			//if(!TestRoomates(tmp))
			{
				Roomates=tmp;
				if(OnNewRoomates!=null)
				{
					//OnNewRoomates(this);
				}
			}
			if(OnTurnStart!=null){OnTurnStart(this);}
		}
		
		bool TestRoomates(MudCharacter[] newRoomates)
		{	
			if(Roomates==null)return false;
			if(Roomates.Length!=newRoomates.Length)
			{
				return false;
			}
			for(int x=0;x<Roomates.Length;x++)
			{
				if(Roomates[x]!=newRoomates[x])
				{
					return false;
				}
			}
			return true;
		}
		
		public void AddExperience(int exp)
		{
			Experience+=exp;
			NotifyPlayer("\t*You have gained {0} experience points",exp);
			if(Experience>=Level*Level)
			{
				LevelUp();
			}
			if(ExperienceGained!=null)
			{
				ExperienceGained(this,Experience);
			}
		}
		
		void LevelUp()
		{
			Experience=Experience%(Level*Level);
			Level+=1;
			HitPoints=MaxHitPoints;
			
			NotifyPlayer("\t*You increased in skill! you are now level: {0}",Level);
		}
		
		public override void OnDeath()
		{
			Roomates=new MudCharacter[0];
			NotifyPlayer("****You have died and will be sent back to the begining****");
			DungeonRoom r=Dungeon.GetRoom(Dungeon.StartingRoom);
			HitPoints=MaxHitPoints;
			r.AddCharacter(this);
			
		}
		
		public void ReceiveItem(MudItem a)
		{
			int count=0;
			if(a is WeaponItem && a==EquipedWeapon)
			{
				count++;
			}
			if(a is ArmorItem && a==EquipedArmor)
			{
				count++;
			}
			count+=inventory.GetCountItem(a.Name);
			if(count>=a.MaxCount)
			{
				return;
			}
			if(inventory.AddItem(a))
				NotifyPlayer("\t*You looted {0}",a.Name);
			if(InventoryChange!=null)
			{
				InventoryChange(this,a);
			}
		}
		
		public bool InventoryHasItem(string itemName)
		{
			return inventory.HasItem(itemName);
		}
		
		public int GetItemCount(string itemName)
		{
			itemName=itemName.ToLower();
			if(!inventory.HasItem(itemName))return 0;
			return inventory.GetCountItem(itemName);
		}
		
		public bool Equip(string itemname)
		{
			itemname=itemname.ToLower();
			bool result=false;
			MudItem item=inventory.PeekItemByName(itemname);
			if(item==null)
			{
				//inventory.AddItem(item);
				return result;
			}
			if(item is WeaponItem)
			{
				EquipedWeapon=(item as WeaponItem);
				result=true;
			}
			if(item is ArmorItem)
			{
				EquipedArmor=(item as ArmorItem);
				result=true;
			}
			if(result==true && ItemEquiped!=null)
			{
				ItemEquiped(this,itemname);
			}
			return result;
		}
		
		public List<string> GetInventory()
		{
			return inventory.GetItems();
		}
		
		public override void SetRoom(DungeonRoom room)
		{
			base.SetRoom(room);
			Roomates=room.GetCharactersInRoom();
			if(OnNewRoomates!=null)
			{
				//OnNewRoomates(this);
			}
		}
		
		public void NewRoomates()
		{
			Roomates=Room.GetCharactersInRoom();
			if(OnNewRoomates!=null)
			{
				OnNewRoomates(this);
			}
		}
		
		public MudItem GetInventoryItem(string itemName)
		{
			return inventory.PeekItemByName(itemName);
		}
		
		public override string StatusString()
		{
			string name="";
			name+=Name;
			string output=string.Format("[~{0} L:{1} HP:({2}/{3}) Pow:{4} Armor:{5}]",name,Level,HitPoints,MaxHitPoints,Power,Armor);
			return output;
		}
	}
}
