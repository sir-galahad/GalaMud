/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/25/2017
 * Time: 11:08 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Mud.Effects
{
	/// <summary>
	/// Description of IEffect.
	/// </summary>
	public interface IEffect
	{
		string GetName();
		void StartTurn();
		
	}
}
