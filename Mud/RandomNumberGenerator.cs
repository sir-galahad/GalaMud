/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/19/2017
 * Time: 11:11 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Mud
{
	/// <summary>
	/// a static random number generator, because this being a game tends to call the random number generator a lot
	/// and creating 2 or more instances at the same time using time based seeds means 2 Random objects will generate the same value
	/// so i've centralized it so calls on our RNG are sequential;
	/// </summary>
	public static class RandomNumberGenerator
	{
		static Random rand=new Random();
		static RandomNumberGenerator()
		{
		}
		
		public static int GetRand(int min, int max)
		{
			return rand.Next(min,max);
		}
	}
}
