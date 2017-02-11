/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/25/2017
 * Time: 12:33 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Actions;
using Mud.Items;
namespace Mud.Characters.NpcCharacters
{
	/// <summary>
	/// Description of BasicMob.
	/// </summary>
	public class BasicMob:NpcCharacter
	{
		public int Damage;
		public BasicMob(string name, int level):base(name)
		{
			MaxHitPoints=(int)(level*1.8);
			Level=level;
			Damage=level;
			Armor=level;
			Power=level;
			HitPoints=MaxHitPoints;
			ItemBuilderFactory factory=ItemBuilderFactory.GetInstance();
			ActionList.Add("attack",AttackAction.GetActionBuilder());
			LootTable.Add(new LootTableElement(100,factory.GetBuilder("healing-potion")));
			if(level==2)
			{
				LootTable.Add(new Mud.Items.LootTableElement(500,factory.GetBuilder("rusty-dagger")));
								
			}
			if(level==3)
			{
				LootTable.Add(new LootTableElement(500,factory.GetBuilder("quilted-armor"))); 
			}
		}
		
		public override void StartTurn()
		{
			base.StartTurn();
			PlayerCharacter[] players=null;
			ActionBuilder[] actions=new ActionBuilder[ActionList.Values.Count];
			ActionList.Values.CopyTo(actions,0);
			Random rand=new Random();
			players=Room.GetPlayersInRoom();
			int index;
			index=rand.Next(0,players.Length);
			ActionBuilder a=actions[rand.Next(0,ActionList.Values.Count)];
			Room.AddActionToQueue(a.BuildAction(new ActionArgs(this,players[index])));
		}
	}
}
