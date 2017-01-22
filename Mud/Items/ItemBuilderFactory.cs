/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/21/2017
 * Time: 12:02 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
namespace Mud.Items
{
	/// <summary>
	/// Description of ItemBuilderFactory.
	/// </summary>
	public class ItemBuilderFactory
	{
		static ItemBuilderFactory instance=null;
		public static ItemBuilderFactory GetInstance()
		{
			if(instance==null)
			{
				instance=new ItemBuilderFactory();
			}
			return instance;
		}
		Dictionary<string,Func<MudItem>> BuilderTable=new Dictionary<string, Func<MudItem>>();
		public ItemBuilderFactory()
		{
			BuilderTable.Add("rusty-spoon",()=>{return new SimpleWeapon("Rusty-Spoon","A ridiculously weak weapon",1);});
			BuilderTable.Add("loincloth",()=>{return new SimpleArmor("Loincloth","Is this really even armor?",2);});
		}
		
		public Func<MudItem> GetBuilder(string name)
		{
			return BuilderTable[name];
		}
	}
}
