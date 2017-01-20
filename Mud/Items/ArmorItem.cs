/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/18/2017
 * Time: 12:47 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
namespace Mud.Items
{
	/// <summary>
	/// Description of ArmorItem.
	/// </summary>
	public class ArmorItem:MudItem
	{
		public ArmorItem(string name,string desc):base(name,desc)
		{
		}
		public virtual int GetArmor(PlayerCharacter player)
		{
			return 0;
		}
	}
}
