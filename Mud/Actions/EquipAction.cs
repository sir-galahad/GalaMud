/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/21/2017
 * Time: 11:36 PM
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
	/// Description of EquipAction.
	/// </summary>
	public class EquipAction:CharacterAction
	{
		public static ActionBuilder GetActionBuilder()
		{
			return new ActionBuilder("equip",
			                         "Equip an item",
			                         new ArgumentType[]{ArgumentType.item},
			                         O=>{
			                         	if((O.Sender is PlayerCharacter))
			                         	{
			                         		PlayerCharacter P= O.Sender as PlayerCharacter;
			                         		if(!P.InventoryHasItem(O.Argument))
			                         		{
			                         			P.NotifyPlayer("You can't equip an item you don't have.");
			                         			return null;
			                         		}
			                         		return new EquipAction(O.Sender,O.Argument);
			                         	}
			                  
			                         	return null;
			                         },
			                         new Func<MudCharacter, string, ActionArgs>(GetArgs),
			                         true);
		}
		
		public static ActionArgs GetArgs(MudCharacter sender,string input)
		{
			
			Regex regex=new Regex("^equip (\\S+)$",RegexOptions.IgnoreCase);
			Match m=regex.Match(input);
			if(!m.Success){
				(sender as PlayerCharacter).NotifyPlayer("i don't quite understand");
				return null;
			}
			string itemName=m.Groups[1].ToString().ToLower();
			if(!(sender as PlayerCharacter).InventoryHasItem(itemName)){
				(sender as PlayerCharacter).NotifyPlayer("Can not equip an item you don't have");
				return null;
			}
			return new ActionArgs(sender,null,itemName);
		}
		
		string ItemName;
		public EquipAction(MudCharacter character,string name):base(character)
		{ 	
			Beneficial=true;
			ItemName=name;
		}
		
		public override string DoAction()
		{
			if((Character as PlayerCharacter).Equip(ItemName))
				return string.Format("\t*{0} equipped {1}",Character.Name,ItemName);
			return "couldn't equip item";
		}
	}
}
