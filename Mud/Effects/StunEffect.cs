/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/25/2017
 * Time: 11:11 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
using Mud.Actions;
namespace Mud.Effects
{
	/// <summary>
	/// Adds a stunned effect to a character
	/// </summary>
	public class StunEffect : IEffect
	{
		MudCharacter Creator;
		MudCharacter Target;
		int Duration;
		public StunEffect(MudCharacter creator, int duration)
		{
			Creator=creator;
			Duration=duration;
		}
		
		public string GetName()
		{
			return "Stun";
		}
		
		public string GetOwner()
		{
			return Creator.Name;
		}
		
		public void Setup(MudCharacter target)
		{
			if(target==null)
			{
				return;
			}
			
			Target=target;
			target.Effects.Add(this);
			target.OnStartTurn+=StartTurn;
		}
		
		public void Remove()
		{
			Target.Effects.Remove(this);
			Target.OnStartTurn-=StartTurn;
		}
		
		public void StartTurn(MudCharacter t)
		{
			if(Duration==0){
				Remove();
				return;
			}
			Duration--;
			Target.Room.AddActionToQueue(new NullAction(Target,"is stunned and can take no action"));
			
		}
	}
}
