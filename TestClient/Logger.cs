/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/4/2017
 * Time: 8:44 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
namespace TestClient
{
	/// <summary>
	/// Description of Logger.
	/// </summary>
	public static class Logger
	{
		static string filename="log.txt";
		static Logger()
		{
			
		}
		public static void Log(string toLog)
		{
			FileStream log=new FileStream(filename,FileMode.Append);
			StreamWriter writer=new StreamWriter(log);
			writer.Write(toLog+"\r\n");
			Console.Write(toLog+"\r\n");
			writer.Close();
		}
	}
}
