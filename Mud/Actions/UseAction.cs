/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/10/2017
 * Time: 11:23 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
using Mud.Items;
using System.Text.RegularExpressions;
namespace Mud.Actions
{
	/// <summary>
	/// Description of UseAction.
	/// </summary>
	public class UseAction:CharacterAction
	{
		
		public static ActionBuilder GetActionBuilder()
		{
			return new ActionBuilder("use",new Func<ActionArgs,CharacterAction>(BuildAction),
			                         new Func<MudCharacter, string, ActionArgs>(GetArgs),true);
		}
		
		static CharacterAction BuildAction(ActionArgs args)
		{
			if((args.Sender as PlayerCharacter).GetItemCount(args.Argument)<=0)
			{
				throw new ArgumentException(string.Format("you don't have {0}",args.Argument));
			}
			MudItem i=(args.Sender as PlayerCharacter).PeekInventoryItem(args.Argument);
			if(!(i is IUseable)){
				throw new ArgumentException(string.Format("{0} is not usable",i.Name));
			}
			CharacterAction action=new UseAction(args.Sender,args.Argument);
			return action;
		}
		
		public static ActionArgs GetArgs(MudCharacter sender,string input)
		{
			MudCharacter[] targets;
			Regex regex=new Regex("^use (\\S+) on (\\d{1,2})$",RegexOptions.IgnoreCase);
			Regex regex2=new Regex("^use (\\S+)$");
			Match m=regex.Match(input);
			if(!m.Success){
				m=regex2.Match(input);
				if(!m.Success){
					(sender as PlayerCharacter).NotifyPlayer("I'm not sure what item you want to use");
					return null;
				}
			}
			
			string itemName=m.Groups[1].ToString().ToLower();
			if(!(sender as PlayerCharacter).InventoryHasItem(itemName)){
				(sender as PlayerCharacter).NotifyPlayer("Can not use an item you don't have");
				return null;
			}
			MudCharacter target=null;
			if(m.Captures.Count>2)
			{
				int targetnum=int.Parse(m.Groups[2].ToString());
				targets=sender.Room.GetCharactersInRoom();
				if(targetnum>=targets.Length){
					(sender as PlayerCharacter).NotifyPlayer("There is no target present associated with that number");
					return null;
				}
				target=targets[targetnum];
			}
			return new ActionArgs(sender,target,itemName);
			
		}
		string arguments;
		public UseAction(MudCharacter c,string arguments):base(c)
		{
			this.arguments=arguments;
			this.Beneficial=true;
		}
		
		public override string DoAction()
		{
			MudItem i;
			PlayerCharacter player=Character as PlayerCharacter;
			i=player.PeekInventoryItem(arguments);
			if(i is IUseable){
				i=player.PullInventoryItem(arguments);
				return (i as IUseable).Use(Character);
			}
			return string.Format("{0} tried to use an unusable item", Character.Name);
		}
	}
}
