/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/21/2017
 * Time: 1:18 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Mud.Items
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class LootTableElement
	{
		public readonly int Chance;//Given a random number between 0 and 1000 must be less than Chance to be rewarded with said item
		public readonly Func<MudItem>Builder;
		public LootTableElement(int chance,Func<MudItem> builder)
		{
			Chance=chance;
			Builder=builder;
		}
	}
}
