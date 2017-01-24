/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/23/2017
 * Time: 10:17 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
namespace Mud.Characters.NpcCharacters
{
	/// <summary>
	/// Description of NpcFactory.
	/// </summary>
	public class NpcFactory
	{
		static NpcFactory instance=null;
		public static NpcFactory GetInstance()
		{
			if(instance==null)
				instance = new NpcFactory();
			return instance;
		}
		
		
		Dictionary<string,Func<NpcCharacter>> NpcGenList=new Dictionary<string, Func<NpcCharacter>>();
		NpcFactory()
		{
			NpcGenList.Add("skeleton",()=>new Weakling("Skeleton"));
		}
		
		public NpcCharacter GetCharacter(string name)
		{
			return NpcGenList[name]();
		}
		
	}
}
