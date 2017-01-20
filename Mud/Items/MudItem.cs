/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/18/2017
 * Time: 12:40 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Mud.Items
{
	/// <summary>
	/// Description of MudItem.
	/// </summary>
	public class MudItem
	{
		public virtual string Description{get;protected set;}
		public virtual string Name{get; protected set;}
		public MudItem(string name,string desc)
		{
			Name=name;
			Description=desc;
		}
	}
}
