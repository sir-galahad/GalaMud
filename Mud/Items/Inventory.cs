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
		
		public List<String> GetItems()
		{
			List<string> items=new List<string>();
			foreach(string s in Items.Keys)
			{
				if(GetCountItem(s)>0)
					items.Add(s);
			}
			return items;
		}
		public bool AddItem(MudItem item)
		{
			int itemCount=0;
			try{
				itemCount=Items[item.Name.ToLower()].Count;
			}catch(KeyNotFoundException){
				Items.Add(item.Name.ToLower(),new Stack<MudItem>());
			}
			if(itemCount>=item.MaxCount)
				return false;
			
			Items[item.Name.ToLower()].Push(item);
			return true;
		}
		
		public int GetCountItem(string item)
		{
			item=item.ToLower();
			int itemCount=0;
			try{
				itemCount=Items[item].Count;
			}catch(KeyNotFoundException){
				//itemCount definitely 0
			}
			return itemCount;
		}
			
		public void RemoveItem(MudItem item)
		{
			try{
			if(GetCountItem(item.Name)==0) return;
			Items[item.Name].Pop();
			}catch(KeyNotFoundException){}
		}
		
		public MudItem PullItemByName(string name)//note this removes the item from inventory
		{
			MudItem item=null;
			try{
				item=Items[name].Pop();
			}catch(KeyNotFoundException){}
			return item;
		}
		public MudItem PeekItemByName(string name)
		{
			MudItem item =null;
			try{
				if(Items[name].Count==0)
				{
					return null;
				}
				item=Items[name].Peek();
			}catch(KeyNotFoundException){}
			return item;
		}
		public bool HasItem(string name)
		{
			bool result=false;
			try{
				if(Items[name].Count>0)
				{
					result=true;
				}
			}catch(KeyNotFoundException){}
			return result;
		}
	}
}
