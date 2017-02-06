/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/4/2017
 * Time: 2:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Characters;
using Mud.Actions;
using Mud;
using Mud.Items;
using System.Text;
using System.Collections.Generic;
namespace TestClient
{
	/// <summary>
	/// Description of Interpreter.
	/// </summary>
	public class MudInterpreter
	{
		enum InterpreterState{WaitingForUser,WaitingForPassword,Standard};
		InterpreterState status=InterpreterState.WaitingForUser;
		MudConnection Connection;
		PlayerCharacter player=null;
		Dungeon dungeon;
		Dictionary<string,Action<string>> Commands=new Dictionary<string, Action<string>>();
		string user=null;
		string pass=null;
		public MudInterpreter(MudConnection con,Dungeon dungeon)
		{
			this.dungeon=dungeon;
			Connection=con;
			con.SendString("user name: ");
			Commands.Add("help",new Action<string>(HelpCommand));
			Commands.Add("status",new Action<string>(StatusCommand));
			Commands.Add("inventory",new Action<string>(InventoryCommand));
			Commands.Add("examine",new Action<string>(ExamineCommand));
		}
		
		public void HandleInput(string input)
		{
			input=input.Trim();
			if(status != InterpreterState.Standard)
			{
				Initialize(input);
				return;
			}
			
			string[] inputArgs=input.Split(' ');
			string command=inputArgs[0].ToLower();
			string commandargs=input.Substring(command.Length).Trim();;
			if(inputArgs.Length>=1)
			{
				string arg=input.Remove(0,command.Length);
				try
				{
					ActionBuilder builder=player.GetAction(command);
					ActionArgs a=ActionArgs.GetActionArgs(player,arg);
				
					if(builder!=null && a!=null)
					{
						CharacterAction act=builder.BuildAction(a);
						if(act!=null)
						{
							if(act is TargetedAction && ((TargetedAction)act).Target==null)
							{
								player.NotifyPlayer("Targeted actions must have a valid target");
								return;
							}
						player.Room.AddActionToQueue(builder.BuildAction(a));
						}
						
					}
				}catch(ArgumentException ex){
					player.NotifyPlayer(ex.Message);
				}
			}
			if(Commands.ContainsKey(command))
			{
				Commands[command](commandargs);
			}
			
		}
		
		void Initialize(string input)
		{
			switch(status)
			{
				case InterpreterState.WaitingForUser:
					if(input.Split(' ').Length>1)
					{
						Connection.SendString("Bad user name");
						
					}else{
						user=input;
						Connection.SendString("password: ");
						status=InterpreterState.WaitingForPassword;
					}
					break;
				case InterpreterState.WaitingForPassword:
					System.Security.Cryptography.HashAlgorithm Hash=System.Security.Cryptography.SHA512.Create();
					byte[] hash=Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
					string hashString=Convert.ToBase64String(hash);
					pass=hashString;
					player=new PlayerWarrior(user);
					player.OnNotifyPlayer+=(c,s)=>Connection.SendString(s+"\r\n");
					player.OnNewRoomates+=new Action<PlayerCharacter>(NewRoomates);
					dungeon.AddCharacter(player,dungeon.StartingRoom);
					Logger.Log(string.Format("{0} {1} : {2}",DateTime.Now,user,Connection.ConnectionSocket.RemoteEndPoint.ToString()));
					status=InterpreterState.Standard;
					break;
			}
		}
		void NewRoomates(PlayerCharacter p)
		{
			
		//	currentRoomates=player.Roomates;
			string msg="In the room now |   ";
			for(int x=0;x<player.Roomates.Length;x++)
			{
				msg+=string.Format("{0}:{1}  ",x,player.Roomates[x].StatusString());
			}
			//if(player.Roomates.Length>1)
			player.NotifyPlayer(msg);
			
		}
		
		public void Shutdown()
		{
			if(player!=null){
				
				player.Room.NotifyPlayers("{0} disconnected",player.Name);
				player.Room.RemoveCharacter(player);
			}
		}
		
		void HelpCommand(string args)
		{
			string message="Available Actions:\n\t ";
			foreach(string s in player.GetActionList())
			{
				message=string.Format("{0} {1}",message,s);
			}
			
			message+="\nAvailable commands:\n\t";
			foreach(string s in Commands.Keys)
			{
				message=string.Format("{0} {1}",message,s);
			}
			player.NotifyPlayer(message);
					
		}
		
		void InventoryCommand(string args)
		{
			string message="Inventory:\n";
			string[] inventory=player.GetInventory().ToArray();
			if(inventory.Length==0)
			{
				message+="<empty>";
				player.NotifyPlayer(message);
				return;
			}
			foreach(string s in inventory)
			{
				message=string.Format("{0}   {1}",message,s);
			}
			player.NotifyPlayer(message);
		}
		void StatusCommand(string args)
		{
			string message=player.StatusString();
			string weapon="No weapon";
			if(player.EquipedWeapon!=null)
			{
				weapon=player.EquipedWeapon.Name;
			}
			message+=string.Format("\nWeapon: {0}\n",weapon);
			string armor="No armor";
			if(player.EquipedArmor!=null)
			{
				armor=player.EquipedArmor.Name;
			}
			message+=string.Format("Armor: {0}\n",armor);
			player.NotifyPlayer(message);
		}
		
		void ExamineCommand(string args)
		{
			string[] words=args.Split(' ');
			string itemName=args.Split(' ')[0].ToLower();
			MudItem item;
			item=player.GetInventoryItem(itemName);
			if(item==null)
			{
				player.NotifyPlayer("You can't examine an item you don't have");
			}
			string message=string.Format("Examining {0}: ",item.Name);
			message+=item.Description;
			player.NotifyPlayer(message);
		}
	}
	
}
