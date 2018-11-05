/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/20/2017
 * Time: 11:58 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Mud.Interface;
namespace Mud.Characters
{
	/// <summary>
	/// Description of PlayerCharacterFactory.
	/// </summary>
	public class PlayerCharacterFactory
	{
		public static Dictionary<string,PlayerCharacter> CharacterClasses = new Dictionary<string, PlayerCharacter>();

		static PlayerCharacterFactory(){
			CharacterClasses.Add("warrior", new WarriorCharacter("notAPlayer",null));
			CharacterClasses.Add("mage", new MageCharacter("notAPlayer",null));
		}

		public PlayerCharacterFactory()
		{
		}
		
		public PlayerCharacter GetInstanceByClass(string PlayerClass,string name,int level, int exp,MudConnection conn)//i mean "class" here as in warrior, mage, etc. not as in a C# class
		{
			PlayerClass=PlayerClass.ToLower();
			switch(PlayerClass)
			{
				case "warrior":
					return new WarriorCharacter(name,level,exp,conn);
					
				case "mage":
					return new MageCharacter(name,level,exp,conn);
					
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
