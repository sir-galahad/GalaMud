/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/14/2017
 * Time: 8:57 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Mud
{
	/// <summary>
	/// Description of PlayerCharacter.
	/// </summary>
	public class PlayerCharacter:MudCharacter
	{
		int Experience=0;
		public override int MaxHitPoints {
			get {
				return (int)(Level*1.5);
			}
			protected set {
				base.MaxHitPoints = value;
			}
		}
		public event Action<PlayerCharacter,string>OnNotifyPlayer;
		// override bool IsPlayer{get{return true;}set{}}
		public PlayerCharacter(string Name):base(Name)
		{
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
			NotifyPlayer("you have gained {0} experience",exp);
			if(Experience>=Level*Level)
			{
				LevelUp();
			}
		}
		
		void LevelUp()
		{
			Level+=1;
			HitPoints=MaxHitPoints;
		}
		
		public override void OnDeath()
		{
			NotifyPlayer("You have died and will be sent back to the begining");
			DungeonRoom r=Dungeon.GetRoom(Dungeon.StartingRoom);
			HitPoints=MaxHitPoints;
			r.AddCharacter(this);
			
		}
	}
}
