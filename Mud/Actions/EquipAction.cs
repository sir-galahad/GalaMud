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
namespace Mud.Actions
{
	/// <summary>
	/// Description of EquipAction.
	/// </summary>
	public class EquipAction:CharacterAction
	{
		public static ActionBuilder GetActionBuilder()
		{
			return new ActionBuilder("equip",O=>{
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
			                         },true);
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
				return string.Format("\t*{0} equipped",ItemName);
			return "couldn't equip item";
		}
	}
}
