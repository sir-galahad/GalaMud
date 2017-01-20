/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/18/2017
 * Time: 1:38 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
namespace Mud.Items
{
	/// <summary>
	/// Description of WeaponItem.
	/// </summary>
	public class WeaponItem:MudItem
	{

		public WeaponItem(string name, string desc ):base(name,desc)
		{
			
		}
		
		public virtual int GetDamage(PlayerCharacter player)
		{
			return 1;
		}
	}
}
