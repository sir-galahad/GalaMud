/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/19/2017
 * Time: 9:53 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
namespace Mud.Actions
{
	/// <summary>
	/// Description of ActionBuilder.
	/// </summary>
	public class ActionBuilder
	{
		public readonly string Name;
		public readonly Func<ActionArgs,CharacterAction> BuildAction;
		public readonly Func<MudCharacter,string,ActionArgs>TranslateArgs;
		public readonly Boolean IsBeneficial;
		public ActionBuilder(string name,Func<ActionArgs,CharacterAction>builder,Func<MudCharacter,string,ActionArgs>translator,bool beneficial)
		{
			Name=name.ToLower();
			BuildAction=builder;
			IsBeneficial=beneficial;
			TranslateArgs=translator;
		}
		
		
		
	}
}
