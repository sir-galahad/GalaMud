/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/10/2017
 * Time: 10:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
namespace Mud.Items
{
	/// <summary>
	/// Description of IUseable.
	/// </summary>
	public interface IUseable
	{
		string Use(MudCharacter user);
	}
}
