/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 2/7/2017
 * Time: 8:57 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using Mud.Characters;
using Mud.Items;
namespace Mud.Interface
{
	/// <summary>
	/// Description of MySqlSimplifier.
	/// </summary>
	public class SqlSimplifier
	{
		
		static string Hostname=null;
		static string User=null;
		static string Password=null;
		static SqlSimplifier conn=null;
		object lockObject=new object();
		public static void Setparams(string host,string user, string password)
		{
			Hostname=host;
			User=user;
			//Password=password;
		}
		
		public static SqlSimplifier GetInstance()
		{
			if(Hostname==null)
			{
				throw new ArgumentException("params not set");
			}
			if(SqlSimplifier.conn==null)
			{
				conn=new SqlSimplifier(Hostname,User,Password);
			}
			return conn;
		}
		
		NpgsqlConnection connection;
		
		public SqlSimplifier(string hostname,string user,string password)
		{
		
		}
		
		NpgsqlDataReader Command(string command)
		{
			
			if((connection.State==ConnectionState.Closed))
			{
				CreateDataBase();
			}
			NpgsqlCommand cmd= connection.CreateCommand();
			cmd.CommandText=command;
			cmd.CommandType=System.Data.CommandType.Text;
			return cmd.ExecuteReader();
		}
		
		public void CreateDataBase()
		{
			lock(lockObject)
			{NpgsqlConnectionStringBuilder sb=new NpgsqlConnectionStringBuilder();
				sb.Host=Hostname;
				sb.UserName=User;
				sb.Database = "GalaMUD";
				sb.Password=Password;
				connection=new NpgsqlConnection(sb.ToString());
				connection.Open();
				Command("CREATE SCHEMA IF NOT EXISTS GalaMUD;").Close();
				Command("CREATE TABLE IF NOT EXISTS GalaMUD.authentication (lowerName VARCHAR(20),salt VARCHAR(20),pass VARCHAR(88),PRIMARY KEY(lowerName));").Close();
				Command("CREATE TABLE IF NOT EXISTS GalaMUD.inventory (lowerName VARCHAR(20),item VARCHAR(30),count INT,PRIMARY KEY(lowerName,item));").Close();;
				Command("CREATE TABLE IF NOT EXISTS GalaMUD.players (lowerName VARCHAR(20),name VARCHAR(20),class VARCHAR(20),level INT,experience INT," +
				        "weapon VARCHAR(30),armor VARCHAR(30),PRIMARY KEY(lowerName));").Close();
			}
		}
		
		bool AddPlayer(PlayerCharacter p,string pass,string salt)
		{
			lock(lockObject)
			{
				NpgsqlDataReader c=Command(string.Format("SELECT (name) FROM GalaMUD.players WHERE lcasename='{0}';",p.Name.ToLower()));
				if(c.HasRows)return false;
				c.Close();
				
				Command(string.Format("INSERT INTO GalaMUD.players(lowerName,name,class,level,experience,weapon,armor) VALUE ('{0}','{1}','{2}',{3},{4},'none','none');",p.Name.ToLower(),
				                      p.Name,"warrior",p.Level,p.Experience)).Close();;
				Command(string.Format("INSERT INTO GalaMUD.authentication (lowerName,salt,pass) VALUE ('{0}','{1}','{2}');",p.Name.ToLower(),salt,pass)).Close();
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
			string playerclass=PlayerCharacterFactory.GetPlayerClassName(p);
			lock(lockObject){
				Command(string.Format(
					@"INSERT INTO GalaMUD.players(lowerName,name,class,level,experience,weapon,armor)
						VALUES ('{0}','{1}','{2}',{3},{4},'{5}','{6}')
						ON CONFLICT(lowerName) DO UPDATE SET
							level=EXCLUDED.level,
							experience=EXCLUDED.experience,
							weapon=EXCLUDED.weapon,
							armor=EXCLUDED.armor;",
					p.Name.ToLower(),p.Name,playerclass,p.Level,p.Experience,weapon,armor)).Close();
			}
		}
		
		public void StoreUserandPass(string user,string salt, string pass)
		{
			Command(string.Format(
				@"INSERT INTO GalaMUD.authentication (lowerName,salt,pass) 
					VALUES ('{0}','{1}','{2}')
					ON CONFLICT(lowerName) DO UPDATE SET
						salt=EXCLUDED.salt,
						pass=EXCLUDED.pass;",
				user.ToLower(),salt,pass)).Close();
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
			NpgsqlDataReader reader;
			Inventory inventory=new Inventory();
			lock(lockObject)
			{
				PlayerCharacterFactory f=new PlayerCharacterFactory();
				reader=Command(string.Format("SELECT name, class, level, experience, weapon, armor FROM GalaMUD.players WHERE lowerName='{0}';",name.ToLower()));
				reader.Read();
				realName=reader.GetString(0);
				playerClass=reader.GetString(1);
				level=reader.GetInt32(2);
				experience=reader.GetInt32(3);
				weapon=reader.GetString(4);
				armor=reader.GetString(5);
				reader.Close();
				player=f.GetInstanceByClass(playerClass,realName,level,experience);
				reader=Command(string.Format("SELECT item, count FROM GalaMUD.inventory WHERE lowerName='{0}';",name.ToLower()));
				while(reader.Read())
				{
					//reader.Read();
					itemname=reader.GetString(0);
					itemCount=reader.GetInt32(1);
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
				Command(string.Format(
					@"INSERT INTO GalaMUD.inventory (lowerName,item,count)
						VALUES ('{0}','{1}',{2})
						ON CONFLICT(lowerName,item) DO UPDATE SET
							count=EXCLUDED.count;",
					playername.ToLower(),itemName,count)
				).Close();
			}
		}
		
		public bool PlayerExists(string playername)
		{
			NpgsqlDataReader reader;
			lock(lockObject)
			{
				reader=Command(string.Format("SELECT (lowerName) FROM GalaMUD.authentication WHERE lowerName='{0}';",playername.ToLower()));
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
			NpgsqlDataReader reader;
			string salt;
			lock(lockObject)
			{
				reader=Command(string.Format("SELECT (salt) FROM GalaMUD.authentication WHERE lowerName='{0}';",playername.ToLower()));
				if(!reader.HasRows)
				{
					reader.Close();
					return null;
				}
				reader.Read();
				salt=reader.GetString(0);
				reader.Close();
			}
			return salt;
		}
		
		public string GetPlayerPassword(string playername)
		{
			NpgsqlDataReader reader;
			string password;
			lock(lockObject)
			{
				reader=Command(string.Format("SELECT (pass) FROM GalaMUD.authentication WHERE lowerName='{0}';",playername.ToLower()));
				if(!reader.HasRows)
				{
					reader.Close();
					return null;
				}
				reader.Read();
				password=reader.GetString(0);
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


