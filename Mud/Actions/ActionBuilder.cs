/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/19/2017
 * Time: 9:53 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Mud.Actions
{
	/// <summary>
	/// Description of ActionBuilder.
	/// </summary>
	public class ActionBuilder
	{
		public readonly string Name;
		public readonly Func<MudCharacter,MudCharacter,CharacterAction> BuildAction;
		public readonly Boolean IsBeneficial;
		public ActionBuilder(string name,Func<MudCharacter,MudCharacter,CharacterAction>builder,bool beneficial)
		{
			Name=name;
			BuildAction=builder;
			IsBeneficial=beneficial;
		}
	}
}
