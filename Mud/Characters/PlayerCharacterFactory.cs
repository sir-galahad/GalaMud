/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/20/2017
 * Time: 11:58 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Mud.Characters
{
	/// <summary>
	/// Description of PlayerCharacterFactory.
	/// </summary>
	public class PlayerCharacterFactory
	{
		public PlayerCharacterFactory()
		{
		}
		
		public PlayerCharacter GetInstanceByClass(string PlayerClass,string name,int level, int exp)//i mean "class" here as in warrior, mage, etc. not as in a C# class
		{
			PlayerClass=PlayerClass.ToLower();
			switch(PlayerClass)
			{
				case "warrior":
					return new WarriorCharacter(name,level,exp);
					
				case "mage":
					return new MageCharacter(name,level,exp);
					
			}
			return null;
		}
		
		public string[] GetAvailableClasses()
		{
			return new string[]{"warrior","mage"};
		}
		
		public static string GetPlayerClassName(PlayerCharacter Player)
		{
			if(Player is WarriorCharacter)return "warrior";
			if(Player is MageCharacter)return "mage";
			return null;
		}
	}
}
