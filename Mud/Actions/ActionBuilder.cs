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
	public enum ArgumentType {none,direction,character,item}


	/// <summary>
	/// Description of ActionBuilder.
	/// </summary>
	public class ActionBuilder
	{
		public readonly string Name;
		public readonly Func<ActionArgs,CharacterAction> BuildAction;
		public readonly Func<MudCharacter,string,ActionArgs>TranslateArgs;
		public readonly Boolean IsBeneficial;
		public static readonly string[] ArgumentTypes = {"None","Direction","Character","Item"};
		public readonly ArgumentType[] Arguments;
		public readonly string Description;
		public ActionBuilder(
							 string name,
							 string description,
							 ArgumentType[] args,
		                     Func<ActionArgs, CharacterAction> builder,
							 Func<MudCharacter, string,ActionArgs> translator,
							 bool beneficial
							)
		{
			Name=name.ToLower();
			Description = description;
			Arguments=args;
			BuildAction=builder;
			IsBeneficial=beneficial;
			TranslateArgs=translator;
		}
	}
}
