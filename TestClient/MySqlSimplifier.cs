/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/7/2017
 * Time: 8:57 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using MySql;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.Data;
using Mud.Characters;
using Mud.Items;
namespace TestClient
{
	/// <summary>
	/// Description of MySqlSimplifier.
	/// </summary>
	public class MySqlSimplifier
	{
		
		static string Hostname=null;
		static string User=null;
		static string Password=null;
		static MySqlSimplifier conn=null;
		object lockObject=new object();
		public static void Setparams(string host,string user, string password)
		{
			Hostname=host;
			User=user;
			Password=password;
		}
		
		public static MySqlSimplifier GetInstance()
		{
			if(Hostname==null)
			{
				throw new ArgumentException("params not set");
			}
			if(MySqlSimplifier.conn==null)
			{
				conn=new MySqlSimplifier(Hostname,User,Password);
			}
			return conn;
		}
		
		MySqlConnection connection;
		
		public MySqlSimplifier(string hostname,string user,string password)
		{
		
		}
		
		MySqlDataReader Command(string command)
		{
			if((connection.State==ConnectionState.Closed))
			{
				CreateDataBase();
			}
			MySqlCommand cmd= connection.CreateCommand();
			cmd.CommandText=command;
			cmd.CommandType=System.Data.CommandType.Text;
			return cmd.ExecuteReader();
		}
		
		public void CreateDataBase()
		{
			lock(lockObject)
			{MySqlConnectionStringBuilder sb=new MySqlConnectionStringBuilder();
				sb.Server=Hostname;
				sb.UserID=User;
				sb.Password=Password;
				connection=new MySqlConnection(sb.ToString());
				connection.Open();
				Command("CREATE DATABASE IF NOT EXISTS Mud;").Close();
				Command("USE Mud;").Close();
				Command("CREATE TABLE IF NOT EXISTS authentication (lowerName VARCHAR(20),salt VARCHAR(20),pass VARCHAR(88),PRIMARY KEY(lowerName));").Close();
				Command("CREATE TABLE IF NOT EXISTS inventory (lowerName VARCHAR(20),item VARCHAR(30),count INT,UNIQUE KEY(lowerName,item));").Close();;
				Command("CREATE TABLE IF NOT EXISTS players (lowerName VARCHAR(20),name VARCHAR(20),class VARCHAR(20),level INT,experience INT," +
				        "weapon VARCHAR(30),armor VARCHAR(30),PRIMARY KEY(lowerName));").Close();
			}
		}
		
		bool AddPlayer(PlayerCharacter p,string pass,string salt)
		{
			lock(lockObject)
			{
				MySqlDataReader c=Command(string.Format("SELECT (name) FROM players WHERE lcasename='{0}';",p.Name.ToLower()));
				if(c.HasRows)return false;
				c.Close();
				
				Command(string.Format("INSERT INTO players(lowerName,name,class,level,experience,weapon,armor) VALUE ('{0}','{1}','{2}',{3},{4},'none','none');",p.Name.ToLower(),
				                      p.Name,"warrior",p.Level,p.Experience)).Close();;
				Command(string.Format("INSERT INTO authentication (lowerName,salt,pass) VALUE ('{0}','{1}','{2}');",p.Name.ToLower(),salt,pass)).Close();
			}
			return true;
			
		}
		
		public void StorePlayer(PlayerCharacter p)
		{
			string armor="none";
			string weapon="none";
			
			if(p.EquipedWeapon!=null)
			{
				weapon=p.EquipedWeapon.Name;
			}
			
			if(p.EquipedArmor!=null)
			{
				armor=p.EquipedArmor.Name;
			}
			
			lock(lockObject){
				Command(string.Format("REPLACE INTO players(lowerName,name,class,level,experience,weapon,armor) VALUE ('{0}','{1}','{2}',{3},{4}," +
				                      "'{5}','{6}');",p.Name.ToLower(),p.Name,"warrior",p.Level,p.Experience,weapon,armor)).Close();
			}
		}
		
		public void StoreUserandPass(string user,string salt, string pass)
		{
			Command(string.Format("REPLACE INTO authentication (lowerName,salt,pass) VALUE ('{0}','{1}','{2}');",user.ToLower(),salt,pass)).Close();
		}
		
		public PlayerCharacter GetPlayer(string name)
		{
			string realName;
			string playerClass;
			int level;
			int experience;
			string weapon;
			string armor;
			string itemname;
			int itemCount;
			PlayerCharacter player;
			MySqlDataReader reader;
			Inventory inventory=new Inventory();
			lock(lockObject)
			{
				reader=Command(string.Format("SELECT * FROM players WHERE lowerName='{0}';",name.ToLower()));
				reader.Read();
				realName=reader.GetString("name");
				playerClass=reader.GetString("class");
				level=reader.GetInt32("level");
				experience=reader.GetInt32("experience");
				weapon=reader.GetString("weapon");
				armor=reader.GetString("armor");
				reader.Close();
				player=new PlayerWarrior(realName,level,experience);
				reader=Command(string.Format("SELECT * FROM inventory WHERE lowerName='{0}';",name.ToLower()));
				while(reader.Read())
				{
					//reader.Read();
					itemname=reader.GetString("item");
					itemCount=reader.GetInt32("count");
					ItemBuilderFactory factory=ItemBuilderFactory.GetInstance();
					for(int x=0;x<itemCount;x++)
					{
						inventory.AddItem(factory.GetBuilder(itemname.ToLower()).Invoke());
					}
				}
				reader.Close();
				player.SetInventory(inventory);
				player.Equip(weapon);
				player.Equip(armor);
				return player;
			}
			
			
		}
		
		public void ChangeItemCount(string playername,string itemName,int count)
		{
			lock(lockObject)
			{
				Command(string.Format("REPLACE INTO inventory (lowerName,item,count) VALUE ('{0}','{1}',{2});",playername.ToLower(),itemName,count)).Close();
			}
		}
		
		public bool PlayerExists(string playername)
		{
			MySqlDataReader reader;
			lock(lockObject)
			{
				reader=Command(string.Format("SELECT (lowerName) FROM authentication WHERE lowerName='{0}';",playername.ToLower()));
				if(reader.HasRows)
				{
					reader.Close();
					return true;
				}
				reader.Close();
			}
			return false;
		}
		
		public string GetPlayerSalt(string playername)
		{
			MySqlDataReader reader;
			string salt;
			lock(lockObject)
			{
				reader=Command(string.Format("SELECT (salt) FROM authentication WHERE lowerName='{0}';",playername.ToLower()));
				if(!reader.HasRows)
				{
					reader.Close();
					return null;
				}
				reader.Read();
				salt=reader.GetString("salt");
				reader.Close();
			}
			return salt;
		}
		
		public string GetPlayerPassword(string playername)
		{
			MySqlDataReader reader;
			string password;
			lock(lockObject)
			{
				reader=Command(string.Format("SELECT (pass) FROM authentication WHERE lowerName='{0}';",playername.ToLower()));
				if(!reader.HasRows)
				{
					reader.Close();
					return null;
				}
				reader.Read();
				password=reader.GetString("pass");
				reader.Close();
			}
			return password;
		}
		
		public void Close()
		{
			connection.Close();
		}
	}
}


