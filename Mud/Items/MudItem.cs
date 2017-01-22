/*
 * Created by SharpDevelop.
 * User: Aaron2
 * Date: 1/18/2017
 * Time: 12:40 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Mud.Items
{
	/// <summary>
	/// Description of MudItem.
	/// </summary>
	public class MudItem
	{
		public virtual string Description{get;protected set;}
		public virtual string Name{get; protected set;}
		public virtual int MaxCount{get;protected set;}
		public MudItem(string name,string desc)
		{
			Name=name;
			Description=desc;
			MaxCount=0;
		}

		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
		MudItem other = obj as MudItem;
			if (other == null)
				return false;
				return this.Description == other.Description && this.Name == other.Name && this.MaxCount==other.MaxCount;
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (Description != null)
					hashCode += 1000000007 * Description.GetHashCode();
				if (Name != null)
					hashCode += 1000000009 * Name.GetHashCode();
				hashCode += 1000000011 * MaxCount.GetHashCode();
				
			}
			return hashCode;
		}

		public static bool operator ==(MudItem lhs, MudItem rhs) 
		{
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(MudItem lhs, MudItem rhs) 
		{
			return !(lhs == rhs);
		}

		#endregion
	}
}
