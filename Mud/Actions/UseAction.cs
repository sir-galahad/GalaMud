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
namespace Mud.Actions
{
	/// <summary>
	/// Description of UseAction.
	/// </summary>
	public class UseAction:CharacterAction
	{
		
		public static ActionBuilder GetActionBuilder()
		{
			return new ActionBuilder("use",new Func<ActionArgs,CharacterAction>(BuildAction),true);
		}
		
		static CharacterAction BuildAction(ActionArgs args)
		{
			if(string.IsNullOrWhiteSpace(args.Argument))
			{
				throw new ArgumentException("use must be used in conjunction with name of the item to use");
			}
			
			if((args.Sender as PlayerCharacter).GetItemCount(args.Argument)<=0)
			{
				throw new ArgumentException(string.Format("you don't have {0}",args.Argument));
			}
			MudItem i=(args.Sender as PlayerCharacter).PeekInventoryItem(args.Argument);
			if(!(i is IUseable)){
				throw new ArgumentException(string.Format("{0} is not usable",i.Name));
			}
			return new UseAction(args.Sender,args.Argument);
		}
		string arguments;
		public UseAction(MudCharacter c,string arguments):base(c)
		{
			this.arguments=arguments;
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
