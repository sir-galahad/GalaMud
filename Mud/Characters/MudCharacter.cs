/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/13/2017
 * Time: 6:45 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Mud.Actions;

namespace Mud.Characters
{
	/// <summary>
	/// Description of Character.
	/// </summary>
	public class MudCharacter
	{
		protected Dictionary<string,ActionBuilder> ActionList=new Dictionary<string, ActionBuilder>();
		//public event Action<MudCharacter,string> NotifyPlayer;
		public string Name{get;private set;}
		//public virtual bool IsPlayer{get{return false;}set{}}
		public virtual int HitPoints{get;protected set;}
		public virtual int MaxHitPoints{get;protected set;}
		public virtual int Armor{get;protected set;}
		public virtual int Level{get;protected set;}
		public DungeonRoom Room{get;protected set;}
		public Dungeon Dungeon{get;protected set;}
		public MudCharacter(string Name)
		{
			this.Name=Name;
			HitPoints=1;
			Armor=1;
			Level=1;
			MaxHitPoints=Level*Level;
		}
		
		
		internal void JoinDungeon(Dungeon d){
			Dungeon=d;
		}
		public virtual int GetDamage()
		{
			return 1;
		}
		
		
		public virtual int TakeDamage(int damage)
		{
			///<summary>
			/// returns the damage actually taken
			/// </summary>
			
			this.HitPoints-=damage;
			return damage;	
		}
		public string StatusString()
		{
			string name="";
			if(this is PlayerCharacter)name="~";
			name+=Name;
			string output=string.Format("[{0} L:{1} HP:({2}/{3})]",name,Level,HitPoints,MaxHitPoints);
			return output;
		}
		public virtual void GetAction()
		{
		
		}
		public virtual void SetRoom(DungeonRoom room)
		{
			Room=room;
		}
		public virtual void OnDeath()
		{
			
		}
	}
}
