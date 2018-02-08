/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/2/2017
 * Time: 11:06 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud;
using Mud.Characters;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Text;
namespace Mud.Interface
{
	/// <summary>
	/// Description of Server.
	/// </summary>
	public class MudServer
	{
		Dungeon MudDungeon;
		Socket Listener;
		Dictionary<Socket,MudConnection> Sockets=new Dictionary<Socket, MudConnection>();
		bool isStarted=false;
		
		public MudServer(Dungeon dungeon)
		{
			MudDungeon=dungeon;
		
		}
		public string[] Players{
			
			get
			{
				List<string> names=new List<string>();
				foreach(MudConnection c in Sockets.Values)
				{
					if(c!=null && c.PlayerName!=null)
					{
						names.Add(c.PlayerName);
					}
				}
				return names.ToArray();
			}
		}
		
		public void Start()
		{
			
			if(isStarted)
				return;
			Sockets.Clear();
			Listener=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			
			Listener.Bind(new IPEndPoint(IPAddress.Any,23));
			Listener.Listen(5);
			Sockets.Add(Listener,null);
			int x=0;
			while(true)
			{
				List<Socket> readList=new List<Socket>();
				readList.AddRange(Sockets.Keys);
				Socket.Select(readList,null,null,10000);
				
				foreach(Socket s in readList)
				{
					if(s==Listener)
					{
						//Console.WriteLine("listener");
						NewConnection();
					}else{
						x++;
						//Console.WriteLine(x.ToString());
						
						//Console.WriteLine("other");
						Sockets[s].ReadNetwork();
						if(!s.Connected)
						{
							Sockets.Remove(s);
						}
						
						
					}
				}
				
			}
		}
		
		void NewConnection()
		{
			Socket s=Listener.Accept();
			Sockets.Add(s,new MudConnection(this,s,MudDungeon));
			
		}
		
		void RemoveConnection(MudConnection con)
		{
			Sockets.Remove(con.ConnectionSocket);
		}
	}
}
