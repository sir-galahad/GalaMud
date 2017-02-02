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
			                         false);
		}
		public AttackAction(MudCharacter Character,MudCharacter target):base(Character,target)
		{
			Beneficial=false;
		}
		public override string DoAction()
		{
			base.DoAction();
			if(Target==null){return string.Format("No available targets for {0}'s",Character.Name);}
			double dmg=Character.Power;
			double modifier=((double) RandGen.Next(40,61))/50.0;
			dmg=(modifier*dmg);
			Console.WriteLine(dmg.ToString());
			int dmgdone=Target.TakeDamage(Character,(int)dmg);
			return string.Format("\t{0} attacked {1} doing {2} damage ({3} damage absorbed by armor)",Character.StatusString(),Target.StatusString(),dmgdone,(int)dmg-dmgdone);
		}
		
	
	}
}
