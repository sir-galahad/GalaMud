/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/3/2017
 * Time: 1:22 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net.Sockets;
using Mud.Characters;
using System.IO;
using System.Text;
using System.Net.Security;
using System.Collections.Generic;
using Mud;
namespace Mud.Interface
{
	/// <summary>
	/// Description of MudConnection.
	/// </summary>
	enum TelnetStatus{standard,awaitCommand, awaitOption,opNegotiation}
	
	public class MudConnection
	{	object lockObject=new object();
		MudInterpreterXml Interpreter=null;
		public readonly Socket ConnectionSocket;
		public readonly Stream SecureStream;
		StreamReader reader;
		StreamWriter writer;
		byte Command;
		byte option;
		TelnetStatus status;

		public string PlayerName{
			get
			{
				if(Interpreter==null||Interpreter.Player==null)
				{
					return null;
				}
				return Interpreter.Player.Name;
				
			}
		}

		public MudConnection(MudServerXml server,Socket s,Dungeon d)
		{
			ConnectionSocket=s;	
			SecureStream = new NetworkStream (s);

			reader = new StreamReader(SecureStream);
			writer = new StreamWriter (SecureStream);
			writer.AutoFlush = true;
			Interpreter=new MudInterpreterXml(server,this,d);
		}
		
		public void SendString(string output)
		{
			Console.WriteLine ("sending {0}", output);
			byte[] buffer=Encoding.UTF8.GetBytes(output);
			if(!ConnectionSocket.Connected)
			{
				return;		
			}
			lock(lockObject)
			{
				try
				{
					Console.WriteLine("writing {0} bytes",buffer.Length);
					//SecureStream.Write(buffer,0,buffer.Length);
					writer.WriteLine(output);
					//SecureStream.Flush();
				}catch(SocketException){
					ConnectionSocket.Close();
					Interpreter.Shutdown();
				}catch(IOException){
					ConnectionSocket.Close();
					Interpreter.Shutdown();
				}
			}
		}
		
		public void ReadNetwork()
		{
			//byte[] buffer =new byte[4096];
			string xml;

			//StreamReader reader=new StreamReader(SecureStream);
			try
			{
				//received=ConnectionSocket.Receive(buffer,buffer.Length,SocketFlags.None);
				//received = SecureStream.Read(buffer,0,4096);
				xml = reader.ReadLine();

				Console.WriteLine("input{0}",xml);
			}catch(SocketException){
				ConnectionSocket.Close();
				Interpreter.Shutdown();
				return;
			}

			if(xml == null)
			{
				ConnectionSocket.Close();
				Interpreter.Shutdown();
				return;
			}

			//while(!reader.EndOfStream)
			{
				//string line=reader.ReadLine();
				Interpreter.HandleInput(xml);
			}
			
			
		}
		
		/*void AcceptCommand(byte command)
		{
			switch(command)
			{
				case 255: //escaped 255
					stream.WriteByte(command);
					status=TelnetStatus.standard;
					break;
				case 240: //shouldn't get this
					break;
				case 241: //nop
					break;
				case 242:
					break;
				case 243: //break
					break;
					
				case 244: //IP
					break;
					
				case 245://AO
					break;
				case 246://AYT(ARE YOU THERE(PING)
					lock(lockObject)
					{
						try
						{
							ConnectionSocket.Send(new byte[]{255,246});
						}catch(SocketException){
							ConnectionSocket.Close();
							Interpreter.Shutdown();
						}
						status=TelnetStatus.standard;
					}
					break;
				case 247:
					break;
				case 248:
					break;
				case 249:
					break;
				case 250:
					status=TelnetStatus.opNegotiation;
					break;
				case 251:
					this.Command=command;
					status=TelnetStatus.awaitOption;
					break;
				case 252:
					this.Command=command;
					status=TelnetStatus.awaitOption;
					break;
				case 253:
					this.Command=command;
					status=TelnetStatus.awaitOption;
					break;
				case 254:
					this.Command=command;
					status=TelnetStatus.awaitOption;
					break;
			}
		}*/
	}
}
