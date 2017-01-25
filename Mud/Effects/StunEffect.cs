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
		public StunEffect(MudCharacter creator, MudCharacter target, int duration)
		{
			Creator=creator;
			Target=target;
			Duration=duration;
		}
		
		public string GetName()
		{
			return "Stun";
		}
		
		public void StartTurn()
		{
			if(Duration==0){
				Target.Effects.Remove(this);
				return;
			}
			Duration--;
			Target.Room.AddActionToQueue(new NullAction(Target,"is stunned and can take no action"));
			
		}
	}
}
