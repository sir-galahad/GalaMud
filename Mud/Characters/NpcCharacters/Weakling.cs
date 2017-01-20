/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/20/2017
 * Time: 1:26 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Mud.Actions;
namespace Mud.Characters.NpcCharacters
{
	/// <summary>
	/// Description of Weakling.
	/// </summary>
	public class Weakling : NpcCharacter
	{
		public Weakling(string name):base(name)
		{
			ActionList.Add("null1",NullAction.GetActionBuilder());
			ActionList.Add("null2",NullAction.GetActionBuilder());
			ActionList.Add("attack",AttackAction.GetActionBuilder());
		}
		
		public override void GetAction()
		{
			//Room.AddActionToQueue(new NullAction(this));
			ActionBuilder[] actions=new ActionBuilder[ActionList.Values.Count];
			ActionList.Values.CopyTo(actions,0);
			Random rand=new Random();
			PlayerCharacter[] players=Room.GetPlayersInRoom();
			int index;
			index=rand.Next(0,players.Length);
			ActionBuilder a=actions[rand.Next(0,ActionList.Values.Count)];
			Room.AddActionToQueue(a.BuildAction(new ActionArgs(this,players[index])));
		}
	}
}
