/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/21/2017
 * Time: 11:50 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Mud.Items
{
	/// <summary>
	/// Description of Inventory.
	/// </summary>
	public class Inventory
	{
		Dictionary<string,Stack<MudItem>> Items=new Dictionary<string, Stack<MudItem>>();
		public Inventory()
		{
		}
		
		public bool AddItem(MudItem item)
		{
			int itemCount=0;
			try{
				itemCount=Items[item.Name].Count;
			}catch(KeyNotFoundException){
				Items.Add(item.Name,new Stack<MudItem>());
			}
			if(itemCount>=item.MaxCount)
				return false;
			
			Items[item.Name].Push(item);
			return true;
		}
		
		public int GetCountItem(MudItem item)
		{
			int itemCount=0;
			try{
				itemCount=Items[item.Name].Count;
			}catch(KeyNotFoundException){
				//itemCount definitely 0
			}
			return itemCount;
		}
			
		public void RemoveItem(MudItem item)
		{
			try{
			if(GetCountItem(item)==0) return;
			Items[item.Name].Pop();
			}catch(KeyNotFoundException){}
		}
		
		public MudItem PullItemByName(string name)
		{
			MudItem item=null;
			try{
				item=Items[name].Pop();
			}catch(KeyNotFoundException){}
			return item;
		}
	}
}
