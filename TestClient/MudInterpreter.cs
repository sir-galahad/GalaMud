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
		enum InterpreterState{NeworContinue,WaitingForUser,WaitingForPassword,Standard};
		InterpreterState status=InterpreterState.WaitingForUser;
		MudConnection Connection;
		PlayerCharacter player=null;
		Dungeon dungeon;
		
		bool Continue;
		Dictionary<string,Action<string>> Commands=new Dictionary<string, Action<string>>();
		string user=null;
		string pass=null;
		string salt=null;
		MySqlSimplifier database=MySqlSimplifier.GetInstance();
		public MudInterpreter(MudConnection con,Dungeon dungeon)
		{
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
						Connection.SendString("password: ");
						if(Continue&&exists){
							salt=database.GetPlayerSalt(user);
						}
						status=InterpreterState.WaitingForPassword;
					}
					
					break;
					
				case InterpreterState.WaitingForPassword:
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
						player=new PlayerWarrior(user);
						database.StorePlayer(player);
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
						player=database.GetPlayer(user);
						status=InterpreterState.Standard;
					}
					
					player.OnNotifyPlayer+=(c,s)=>Connection.SendString(s+"\r\n");
					player.OnNewRoomates+=new Action<PlayerCharacter>(NewRoomates);
					dungeon.AddCharacter(player,dungeon.StartingRoom);
					Logger.Log(string.Format("{0} {1} : {2}",DateTime.Now,user,Connection.ConnectionSocket.RemoteEndPoint.ToString()));
					status=InterpreterState.Standard;
					player.ItemEquiped+=(c,i)=>{database.StorePlayer(player);};
					player.ExperienceGained+=(c,e)=>{database.StorePlayer(player);};
					player.InventoryChange+=(c,i)=>{database.ChangeItemCount(player.Name,i.Name,player.GetItemCount(i.Name));};
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
			item=player.PeekInventoryItem(itemName);
			if(item==null)
			{
				player.NotifyPlayer("You can't examine an item you don't have");
			}
			string message=string.Format("Examining {0}: ",item.Name);
			message+=item.Description;
			player.NotifyPlayer(message);
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
		
	}
	
}
