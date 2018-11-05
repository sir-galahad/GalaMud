/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/13/2017
 * Time: 7:19 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;

namespace Mud.Actions
{
	/// <summary>
	/// Description of ChracterAction.
	/// </summary>


	public abstract class CharacterAction
	{


		public bool Beneficial{get;protected set;}
		public MudCharacter Character{get;private set;}
		public CharacterAction(MudCharacter c )
		{
			Character=c;
			Beneficial=false;

		}
		
		
		public abstract string DoAction();
	}
}
