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

namespace Mud.Interface
{
	/// <summary>
	/// Description of Interpreter.
	/// </summary>
	public class MudInterpreter
	{
		enum InterpreterState{NeworContinue,WaitingForUser,WaitingForPassword,WaitingForClass,Standard};
		InterpreterState status=InterpreterState.WaitingForUser;
		MudConnection Connection;
		public PlayerCharacter Player{get;private set;}
		Dungeon dungeon;
		MudServerXml Server;
		bool Continue;
		Dictionary<string,Action<string>> Commands=new Dictionary<string, Action<string>>();
		string user=null;
		string userClass=null;
		string pass=null;
		string salt=null;
		SqlSimplifier database=SqlSimplifier.GetInstance();
		
		public MudInterpreter(MudServerXml server,MudConnection con,Dungeon dungeon)
		{
			Server=server;
			this.dungeon=dungeon;
			Connection=con;
			status=InterpreterState.NeworContinue;
			con.SendString("Create a new character or continue (N/C): ");
			Commands.Add("help",new Action<string>(HelpCommand));
			Commands.Add("status",new Action<string>(StatusCommand));
			Commands.Add("inventory",new Action<string>(InventoryCommand));
			Commands.Add("examine",new Action<string>(ExamineCommand));
			salt=GenerateSalt();
		}
		
		public void HandleInput(string input)
		{
			input=input.Trim();
			if(status != InterpreterState.Standard)
			{
				Initialize(input);
				return;
			}
			input=input.ToLower();
			string[] inputArgs=input.Split(' ');
			string command=inputArgs[0].ToLower();
			string commandargs=input.Substring(command.Length).Trim();;
			if(inputArgs.Length>=1)
			{
				ActionArgs a=null;
				string arg=input.Remove(0,command.Length);
				try
				{
					ActionBuilder builder=Player.GetAction(command);
					if(builder!=null)
					{
						a=builder.TranslateArgs(Player,input);
					}
				
					if(builder!=null && a!=null)
					{
						CharacterAction act=builder.BuildAction(a);
						if(act!=null)
						{
							if(act is TargetedAction && ((TargetedAction)act).Target==null)
							{
								Player.NotifyPlayer("Targeted actions must have a valid target");
								return;
							}
						Player.Room.AddActionToQueue(builder.BuildAction(a));
						}
						
					}
				}catch(ArgumentException ex){
					Player.NotifyPlayer(ex.Message);
				}
			}
			if(Commands.ContainsKey(command))
			{
				Commands[command](commandargs);
			}
			
		}
		
		void Initialize(string input)
		{
			PlayerCharacterFactory f=new PlayerCharacterFactory();
			if(input.Length==0)
			{
				return;
			}
			switch(status)
			{
				case InterpreterState.NeworContinue:
					if(input.ToLower()[0]!='n'&&input.ToLower()[0]!='c')
					{
						Connection.SendString("Create a new character or continue (N/C): ");
						return;
					}
					Continue=false;
					
					if(input.ToLower()[0]=='c')
					{
						Continue=true;
					}
					
					Connection.SendString("username: ");
					status=InterpreterState.WaitingForUser;
					break;
					
				case InterpreterState.WaitingForUser:
					bool exists=database.PlayerExists(input);
					if(exists && !Continue)
					{
						Connection.SendString("user already exists\nusername: ");
						return;
					}
					string result=ValidatePlayerName(input);
					if(result!=null){
						Connection.SendString(result+"\n");
						Connection.SendString("username: ");
					}else{
						user=input;
						if(Continue)
						{
							Connection.SendString("password: ");
							if(exists){
								salt=database.GetPlayerSalt(user);
							}
							status=InterpreterState.WaitingForPassword;
							
						}else{
							Connection.SendString("Choose a class:\r\n");
							foreach(string s in f.GetAvailableClasses())
							{
								Connection.SendString(s+"\r\n");
							}
							status=InterpreterState.WaitingForClass;
						}
					}
					
					break;
				case InterpreterState.WaitingForClass:
					input=input.ToLower().Trim();
					bool classExists=false;
					foreach(string s in f.GetAvailableClasses())
					{
						if(s==input)
						{
							classExists=true;
							break;
						}
					}
					if(!classExists)
					{
						Connection.SendString("No such class");
						Connection.SendString("Choose a class:\r\n");
						foreach(string s in f.GetAvailableClasses())
						{
							Connection.SendString(s+"\r\n");
						}
					}else{
						userClass=input;
						status=InterpreterState.WaitingForPassword;
						Connection.SendString("Password: ");
					}
					break;
					
				case InterpreterState.WaitingForPassword:
					//System.Security.Cryptography.HashAlgorithm Hash=System.Security.Cryptography.RNGCryptoServiceProvider.Create();
					System.Security.Cryptography.HashAlgorithm Hash=System.Security.Cryptography.SHA512.Create();
					byte[] hash=Hash.ComputeHash(Encoding.UTF8.GetBytes(salt+input));
					string hashString=Convert.ToBase64String(hash);
					if(pass==null)
					{
						pass=hashString;
						if(!Continue)
						{
							Connection.SendString("re-enter password: ");
							return;
						}
					}else if(hashString!=pass && !Continue)
					{
						Connection.SendString("passwords do not match\npassword: ");
						return;
					}
					
					if(!Continue &&hashString==pass)
					{
						Console.WriteLine("{0} ({1})",pass,pass.Length);
						Player=f.GetInstanceByClass(userClass,user,1,0,Connection);
						database.StorePlayer(Player);
						database.StoreUserandPass(user,salt,pass);
					
					}
					if(Continue)
					{
						string storedpass=database.GetPlayerPassword(user);
						if(storedpass!=pass)
						{
							Connection.SendString("bad username or password\nCreate a new character or continue (N/C): ");
							status=InterpreterState.NeworContinue;
							user=null;
							pass=null;
							return;
						}
						if(AlreadyOnline(user))
						{
							Connection.SendString("Player already logged in\r\n");
							Connection.ConnectionSocket.Close();
							return;
						}
						Player=database.GetPlayer(user);
						
						status=InterpreterState.Standard;
					}
					
					Player.OnNotifyPlayer+=(c,s)=>Connection.SendString(s+"\r\n");
					Player.OnNewRoomates+=new Action<PlayerCharacter>(NewRoomates);
					dungeon.AddCharacter(Player,dungeon.StartingRoom);
					Logger.Log(string.Format("{0} {1} : {2}",DateTime.Now,user,Connection.ConnectionSocket.RemoteEndPoint.ToString()));
					status=InterpreterState.Standard;
					Player.ItemEquiped+=(c,i)=>{database.StorePlayer(Player);};
					Player.ExperienceGained+=(c,e)=>{database.StorePlayer(Player);};
					Player.InventoryChange+=(c,i)=>{database.ChangeItemCount(Player.Name,i.Name,Player.GetItemCount(i.Name));};
					status=InterpreterState.Standard;
					break;
			}
		}
		void NewRoomates(PlayerCharacter p)
		{
			
		//	currentRoomates=player.Roomates;
			string msg="In the room now |   ";
			for(int x=0;x<Player.Roomates.Length;x++)
			{
				msg+=string.Format("{0}:{1}  ",x,Player.Roomates[x].StatusString());
			}
			//if(player.Roomates.Length>1)
			Player.NotifyPlayer(msg);
			
		}
		
		public void Shutdown()
		{
			if(Player!=null){
				
				Player.Room.NotifyPlayers("{0} disconnected",Player.Name);
				Player.Room.RemoveCharacter(Player);
			}
		}
		
		void HelpCommand(string args)
		{
			string message="Available Actions:\r\n\t ";
			foreach(string s in Player.GetActionList())
			{
				message=string.Format("{0} {1}",message,s);
			}
			
			message+="\r\nAvailable commands:\r\n\t";
			foreach(string s in Commands.Keys)
			{
				message=string.Format("{0} {1}",message,s);
			}
			Player.NotifyPlayer(message);
					
		}
		
		void InventoryCommand(string args)
		{
			string message="Inventory:\r\n";
			string[] inventory=Player.GetInventory().ToArray();
			if(inventory.Length==0)
			{
				message+="<empty>";
				Player.NotifyPlayer(message);
				return;
			}
			foreach(string s in inventory)
			{
				message=string.Format("{0}  {1} x{2}\r\n",message,s,Player.GetItemCount(s));
			}
			Player.NotifyPlayer(message);
		}
		void StatusCommand(string args)
		{
			string message=Player.StatusString();
			string weapon="No weapon";
			if(Player.EquipedWeapon!=null)
			{
				weapon=Player.EquipedWeapon.Name;
			}
			message+=string.Format("\nWeapon: {0}\n",weapon);
			string armor="No armor";
			if(Player.EquipedArmor!=null)
			{
				armor=Player.EquipedArmor.Name;
			}
			message+=string.Format("Armor: {0}\n",armor);
			Player.NotifyPlayer(message);
		}
		
		void ExamineCommand(string args)
		{
			string[] words=args.Split(' ');
			string itemName=args.Split(' ')[0].ToLower();
			MudItem item;
			item=Player.PeekInventoryItem(itemName);
			if(item==null)
			{
				Player.NotifyPlayer("You can't examine an item you don't have");
				return;
			}
			string message=string.Format("Examining {0}: ",item.Name);
			message+=item.Description;
			Player.NotifyPlayer(message);
		}
		
		public string ValidatePlayerName(string name)
		{
			if(name.Length>20) return "too many characters used";
			if(name.Length<3) return "names must use at least 3 characters";
			foreach(char c in name)
			{
				if(!Char.IsLetterOrDigit(c))
				{
					return "names can only include letters and numbers";
				}
			}
			return null;
		}
		
		public string GenerateSalt()
		{
			Random rand=new Random();
			byte[] buffer=new byte[4];
			rand.NextBytes(buffer);
			return Convert.ToBase64String(buffer);				
		}
		
		public bool AlreadyOnline(string user)
		{
			foreach(string name in Server.Players)
			{
				if(name.ToLower()==user.ToLower())
				{
					return true;
				}
			}
			return false;
		}
		
	}
	
}
