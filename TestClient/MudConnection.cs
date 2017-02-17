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
using System.Collections.Generic;
using Mud;
namespace TestClient
{
	/// <summary>
	/// Description of MudConnection.
	/// </summary>
	enum TelnetStatus{standard,awaitCommand, awaitOption,opNegotiation}
	
	public class MudConnection
	{	object lockObject=new object();
		MudInterpreter Interpreter=null;
		public readonly Socket ConnectionSocket;
		MemoryStream stream=new MemoryStream();
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
		public MudConnection(MudServer server,Socket s,Dungeon d)
		{
			ConnectionSocket=s;	
			Interpreter=new MudInterpreter(server,this,d);
		}
		
		public void SendString(string output)
		{
			byte[] buffer=Encoding.UTF8.GetBytes(output);
			if(!ConnectionSocket.Connected)
			{
				return;
				
			}
			lock(lockObject)
			{
				try
				{
					ConnectionSocket.Send(buffer);
				}catch(SocketException){
					ConnectionSocket.Close();
					Interpreter.Shutdown();
				}
			}
		}
		
		public void ReadNetwork()
		{
		
			byte[] buffer=new byte[10];
			
			
			int received=0;
			StreamReader reader=new StreamReader(stream);

			while(ConnectionSocket.Poll(0,SelectMode.SelectRead))
			{
				try
				{
					received=ConnectionSocket.Receive(buffer,buffer.Length,SocketFlags.None);
				}catch(SocketException){
					ConnectionSocket.Close();
					Interpreter.Shutdown();
					break;
				}
				if(received==0)
				{
					ConnectionSocket.Close();
					Interpreter.Shutdown();
					return;
				}
				for(int x=0;x<received;x++)
				{
					byte b=buffer[x];
					switch(status)
					{
						case TelnetStatus.standard:
							if(b==255){
								status=TelnetStatus.awaitCommand;
							}else{
								stream.WriteByte(b);
							}
							break;
						
						case TelnetStatus.awaitCommand:
							AcceptCommand(b);
							break;
						case TelnetStatus.opNegotiation:
							if(b==240)
							{
								status=TelnetStatus.standard;
							}
							break;
						case TelnetStatus.awaitOption:
							option=b;
							//deal with option here
							status=TelnetStatus.standard;
							break;
					}
				}
			}
			stream.Seek(0,SeekOrigin.Begin);
			while(!reader.EndOfStream)
			{
				string line=reader.ReadLine();
				Interpreter.HandleInput(line);
			}
			stream.SetLength(0);
			
			
		}
		
		void AcceptCommand(byte command)
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
		}
	}
}
