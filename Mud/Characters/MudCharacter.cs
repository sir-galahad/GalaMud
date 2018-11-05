﻿/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/13/2017
 * Time: 6:45 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Xml;
using Mud.Actions;
using Mud.Effects;
namespace Mud.Characters
{
	/// <summary>
	/// Description of Character.
	/// </summary>
	public class MudCharacter
	{
		protected Dictionary<string,ActionBuilder> ActionList=new Dictionary<string, ActionBuilder>();
		public event Action<MudCharacter> OnStartTurn;
		public event Action<MudCharacter> OnEndTurn;
		public string Name{get;private set;}
		public List<IEffect> Effects=new List<IEffect>();
		protected int armorMod=2;
		public virtual int HitPoints{get;protected set;}
		public virtual int MaxHitPoints{get;protected set;}
		public virtual int Armor{get;protected set;}
		public virtual int Power{get;protected set;}
		public virtual int Level{get;protected set;}
		public DungeonRoom Room{get;protected set;}
		public Dungeon Dungeon{get;protected set;}
		public MudCharacter(string Name)
		{
			this.Name=Name;
			HitPoints=1;
			Armor=1;
			Power=1;
			Level=1;
			MaxHitPoints=Level*Level;
		}
		
		
		internal void JoinDungeon(Dungeon d){
			Dungeon=d;
		}
		
		
		public virtual int TakeDamage(MudCharacter attacker,int damage)
		{
			///<summary>
			/// returns the damage actually taken
			/// </summary>
			int totaldamage=damage;
			int damageAbsorbed=0;
			int threshhold=(int)(damage*0.9);
			if(damage==0)return 0;
			damageAbsorbed=damage;
			damage-=Armor/armorMod;
			damageAbsorbed-=damage;
			if(damageAbsorbed>threshhold)
			{
				damage=totaldamage-threshhold;
			}
			if(damage<0)
				damage=0;
			this.HitPoints-=damage;
			if(HitPoints<0)HitPoints=0;
			return damage;	
		}
		
		public virtual string StatusString()
		{
			string name="";
			name+=Name;
			string output=string.Format("[{0} L:{1} HP:({2}/{3})]",name,Level,HitPoints,MaxHitPoints);
			return output;
		}

		public virtual XmlNode GetStatus (XmlDocument tmpdoc)
		{
			XmlNode character = tmpdoc.CreateElement("character");
			XmlNode name = tmpdoc.CreateElement("name");
			XmlNode level = tmpdoc.CreateElement("level");
			XmlNode hp = tmpdoc.CreateElement("hitPoints");
			XmlNode maxHp = tmpdoc.CreateElement("maxHitPoints");
			XmlNode valueNode;

			valueNode = tmpdoc.CreateTextNode(Name);
			name.AppendChild(valueNode);
			character.AppendChild(name);

			valueNode = tmpdoc.CreateTextNode(Level.ToString());
			level.AppendChild(valueNode);
			character.AppendChild(level);

			valueNode = tmpdoc.CreateTextNode(HitPoints.ToString());
			hp.AppendChild(valueNode);
			character.AppendChild(hp);

			valueNode = tmpdoc.CreateTextNode(MaxHitPoints.ToString());
			maxHp.AppendChild(valueNode);
			character.AppendChild(maxHp);

			return character;

		}
		
		public virtual void StartTurn()
		{
			if(OnStartTurn!=null)
			{
				OnStartTurn(this);
			}
		}
		
		public virtual void EndTurn()
		{
			if(OnEndTurn!=null)
			{
				OnEndTurn(this);
			}
		}
	
		public virtual void SetRoom(DungeonRoom room)
		{
			Room=room;
		}
		
		public virtual void OnDeath()
		{
		
		}
		
		protected void AddActionToList(ActionBuilder b)
		{
			ActionList.Add(b.Name,b);
		}
		
		public ActionBuilder GetAction(string name)
		{
			try{
				return ActionList[name];
			}catch(KeyNotFoundException){
				return null;
			}
		}
		
		public string[] GetActionList()
		{
			string[] actions=new string[ActionList.Keys.Count];
			ActionList.Keys.CopyTo(actions,0);
			return actions;
		}
		
		public void Heal(int amount)
		{
			HitPoints+=amount;
			if(HitPoints>MaxHitPoints)HitPoints=MaxHitPoints;
		}
	}
}
