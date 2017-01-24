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
namespace Mud.Actions
{
	/// <summary>
	/// Description of NullAction.
	/// </summary>
	public class NullAction:CharacterAction
	{
		public static ActionBuilder GetActionBuilder()
		{
			return new ActionBuilder("wait",
			                         O=>{return new NullAction(O.Sender);},
			                         true);
			                         
		}
		public NullAction(MudCharacter c):base(c)
		{
			Beneficial=true;
		}
		public override string DoAction()
		{
			return string.Format("{0} does nothing",Character.Name);
		}
	}
}
