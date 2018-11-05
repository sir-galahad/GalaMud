/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/16/2017
 * Time: 5:52 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
using System.Text.RegularExpressions;
namespace Mud.Actions
{
	/// <summary>
	/// Description of NullAction.
	/// </summary>
	public class NullAction:CharacterAction
	{
		string Message;
		public static ActionBuilder GetActionBuilder()
		{
			return new ActionBuilder(
				"wait",
				"Wait a turn",
				new ArgumentType[]{},
				O=>{return new NullAction(O.Sender,"waits a turn");},
				new Func<MudCharacter, string, ActionArgs>(GetArgs),
				true
			);
			                         
		}
		public static ActionArgs GetArgs(MudCharacter sender,string input)
		{
			Regex regex=new Regex("^wait$",RegexOptions.IgnoreCase);
			Match m=regex.Match(input);
			if(!m.Success){
				return null;
			}
			
			return new ActionArgs(sender);
			
		}
		public NullAction(MudCharacter c,string message):base(c)
		{
			Beneficial=true;
			Message=message;
		}
		public override string DoAction()
		{
			return string.Format("\t{0} {1}",Character.StatusString(),Message);
		}
	}
}
