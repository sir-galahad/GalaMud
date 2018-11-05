/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/17/2017
 * Time: 10:04 PM
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
	/// Description of MageAttack.
	/// </summary>
	public class MageAttack:TargetedAction
	{
		public static ActionBuilder GetActionBuilder()
		{
			return new ActionBuilder("attack",
			                         "Attack a character",
			                         new ArgumentType[]{ArgumentType.character},
			                         (o)=>{return new MageAttack(o.Sender,o.Target);},
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
		
		public MageAttack(MudCharacter character,MudCharacter target):base(character,target)
		{
			Beneficial=false;
		}
		
		public override string DoAction()
		{
			base.DoAction();
			bool found=false;
			if(Target==null)
			{
				return string.Format("No available target for {0}'s attack",Character.Name);
			}
			double dmg=Character.Power*0.75;
			double modifier=((double) RandomNumberGenerator.GetRand(40,61))/50.0;
			dmg=(modifier*dmg);
			int dmgdone=Target.TakeDamage(Character,(int)dmg);
			foreach(IEffect e in Target.Effects)
			{
				Console.WriteLine ("checking for for burn");
				if(e.GetName()=="burn" && e.GetOwner()==Character.Name)
				{
					Console.WriteLine ("found it");
					(e as BurnEffect).AddDamage(((int)(dmg/3.0))+1);
					found=true;	
				}
			}
			
			if(!found){
				IEffect burn=new BurnEffect(Character,((int)(dmg/3.0))+1);
				burn.Setup(Target);
			}
			return string.Format("\t{0} attacked {1} doing {2} damage ({3} damage absorbed by armor)",Character.StatusString(),Target.StatusString(),dmgdone,(int)dmg-dmgdone);
			
			
		}
	}
}
