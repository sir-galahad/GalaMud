/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/25/2017
 * Time: 11:26 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
using Mud.Effects;
using System.Text.RegularExpressions;
namespace Mud.Actions
{
	/// <summary>
	/// Description of BonkAction.
	/// </summary>
	public class BonkAction : TargetedAction
	{
		public static ActionBuilder GetActionBuilder()
		{
			return new ActionBuilder("Bonk",
			                         (a)=>{return new BonkAction(a.Sender,a.Target);},
			                         new Func<MudCharacter, string, ActionArgs>(GetArgs),
			                         false);
		}
		
		public static ActionArgs GetArgs(MudCharacter sender,string input)
		{
			MudCharacter[] targets;
			Regex regex=new Regex("^bonk (\\d{1,2})$",RegexOptions.IgnoreCase);
			Match m=regex.Match(input);
			if(!m.Success){
				return null;
			}
			int targetnum=int.Parse(m.Groups[1].ToString());
			targets=sender.Room.GetCharactersInRoom();
			if(targetnum>=targets.Length){
				return null;
			}
			return new ActionArgs(sender,targets[targetnum]);
			
		}
		public BonkAction(MudCharacter character,MudCharacter target):base(character,target)
		{
			Beneficial=false;
		}
		
		public override string DoAction()
		{
			foreach(IEffect e in Target.Effects)
			{
				if(e.GetName().ToLower()=="stun")
					return string.Format("\t{0} attempted to stun {1}, but {1} is already stunned",Character.StatusString(),Target.StatusString());
			}
			Target.Effects.Add(new StunEffect(Character,Target,3));
			return string.Format("\t{0} BONKS {1} over the head doing no damage, but stunning them",Character.StatusString(),Target.StatusString());
		}
	}
}
