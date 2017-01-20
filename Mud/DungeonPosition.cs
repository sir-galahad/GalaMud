/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/14/2017
 * Time: 9:17 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Mud
{
	/// <summary>
	/// Description of Position.
	/// </summary>
	public class DungeonPosition
	{	
		public readonly int X;
		public readonly int Y;
		public DungeonPosition(int x,int y)
		{
			X=x;
			Y=y;
		}
	}
}
