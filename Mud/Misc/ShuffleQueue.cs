using System;
using System.Collections;
using System.Collections.Generic;

/**************************************************************
 *the purpose of this class is to allow items to be Enqueued
 *and then Deququed in random order
 **************************************************************/

namespace Mud.Misc
{
	public class ShuffleQueue<T> : IEnumerable<T>
	{
		List<T> queue = new List<T>();
		Random randgen = new Random();

		public int Count{
			get{return queue.Count;}
			private set{}
		}

		public ShuffleQueue ()
		{
		
		}

		public void Enqueue(T item)
		{
			queue.Add(item);
		}

		public T Dequeue()
		{
			T retval;
			int index = randgen.Next ();
			index %= queue.Count;
			retval = queue [index];

			//remove the item from the queue
			queue[index]=queue[queue.Count-1];
			queue.RemoveAt(queue.Count - 1);
			return retval;
		}

		public void Clear()
		{
			queue.Clear ();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return ((IEnumerable<T>)queue).GetEnumerator ();
		}


		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)queue).GetEnumerator ();
		}
	}
}

