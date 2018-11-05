/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/19/2017
 * Time: 11:21 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
namespace Mud.Effects
{
	/// <summary>
	/// Description of Burn.
	/// </summary>
	public class BurnEffect:IEffect
	{
		MudCharacter owner;
		MudCharacter target=null;
		int damage;
		int duration=3;
		public BurnEffect(MudCharacter owner,int damage)
		{
			this.owner=owner;
			this.damage=damage;
		}
		
		public string GetName()
		{
			return "burn";
		}
		
		public string GetOwner()
		{
			return owner.Name;
		}
		
		public void Setup(MudCharacter Target)
		{
			if(Target==null)
			{
				return;
			}
			target=Target;
			target.Effects.Add(this);
			target.OnEndTurn+=BurnTarget;
		}
		
		public void Remove()
		{
			if(target==null){
				return;
			}
			target.Effects.Remove(this);
			target.OnEndTurn-=BurnTarget;
		}
		
		void BurnTarget(MudCharacter t)
		{
			int d;
			duration--;
			d=target.TakeDamage(owner,damage);
			target.Room.NotifyPlayers("\t{0} took {1} damage from {2}'s burn",target.Name,d.ToString(),owner.Name);
			if(duration<=0){
				Remove();
			}
		}
		
		public void AddDamage(int d)
		{
			damage+=d;
			Console.WriteLine ("new damage {0}", damage);
			duration=3;
			
		}
	}
}
