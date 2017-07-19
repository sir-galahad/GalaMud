/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/13/2017
 * Time: 7:32 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
using System.Text.RegularExpressions;
namespace Mud.Actions
{
	/// <summary>
	/// Description of AttackAction.
	/// </summary>
	public class AttackAction : TargetedAction
	{
		static Random RandGen=new Random();
		public static ActionBuilder GetActionBuilder()
		{
			return new ActionBuilder("attack",
			                         (o)=>{return new AttackAction(o.Sender,o.Target);},
			                         new Func<MudCharacter, string, ActionArgs>(GetArgs),
			                         false);
		}
		
		public static ActionArgs GetArgs(MudCharacter sender,string input)
		{
			MudCharacter[] targets;
			Regex regex=new Regex("^attack (\\d{1,2})$",RegexOptions.IgnoreCase);
			Match m=regex.Match(input);
			if(!m.Success){
				(sender as PlayerCharacter).NotifyPlayer("attack what? (target enemies by number)");
				return null;
			}
			int targetnum=int.Parse(m.Groups[1].ToString());
			targets=sender.Room.GetCharactersInRoom();
			if(targetnum>=targets.Length){
				(sender as PlayerCharacter).NotifyPlayer("no character present associated with that number");
				return null;
			}
			return new ActionArgs(sender,targets[targetnum]);
			
		}
		public AttackAction(MudCharacter Character,MudCharacter target):base(Character,target)
		{
			Beneficial=false;
		}
		public override string DoAction()
		{
			base.DoAction();
			if(Target==null){return string.Format("No available targets for {0}'s attack",Character.Name);}
			double dmg=Character.Power;
			double modifier=((double) RandomNumberGenerator.GetRand(40,61))/50.0;
			dmg=(modifier*dmg);
			int dmgdone=Target.TakeDamage(Character,(int)dmg);
			return string.Format("\t{0} attacked {1} doing {2} damage ({3} damage absorbed by armor)",Character.StatusString(),Target.StatusString(),dmgdone,(int)dmg-dmgdone);
		}
		
	
	}
}
