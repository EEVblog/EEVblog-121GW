using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;

namespace rMultiplatform
{
	public interface IObservableList<T>
	{
		int Count { get; }
		event NotifyCollectionChangedEventHandler CollectionChanged;

		T this[int key] { get; }
		void RemoveAt(int Index);
		void Clear();
		void Add(T Item);

		List<T> ToList();
	}

	public class TriggerList<T> : IObservableList<T>
	{
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		private List<T> Data = new List<T>();

		#region IOBSERVABLE_FUNCTIONS
		private T at(int key)
		{
			T output;
			lock(Data) output = Data[key];
			return output;
		}
		public T this[int key] => at(key);
		public int  Count => Data.Count;
		public void RemoveAt(int Index)
		{
			lock (Data) Data.RemoveAt(Index);
		}
		public void Clear()
		{
			lock (Data) Data.Clear();
		}
		public void Add(T Item)
		{
			lock (Data) Data.Add(Item);
		}
		public List<T> ToList()
		{
			List<T> output;
			lock (Data) output = new List<T>(Data);
			return output;
		}
		#endregion

		public void Trigger()
		{
			CollectionChanged?.Invoke(this, null);
		}
	}

	public class ObservableList<T> : IObservableList<T>
	{
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		private List<T> Data = new List<T>();

		#region IOBSERVABLE_FUNCTIONS
		private T at(int key)
		{
			T output;
			lock (Data) output = Data[key];
			return output;
		}
		T IObservableList<T>.this[int key] => at(key);
		public int  Count => Data.Count;
		public void RemoveAt(int Index)
		{
			lock(Data) Data.RemoveAt(Index);
			CollectionChanged?.Invoke(this, null);
		}
		public void Clear()
		{
			lock (Data) Data.Clear();
			CollectionChanged?.Invoke(this, null);
		}
		public void Add(T Item)
		{
			lock(Data) Data.Add(Item);
			CollectionChanged?.Invoke(this, null);
		}
		public List<T> ToList()
		{
			List<T> output;
			lock (Data) output = new List<T>(Data);
			return output;
		}
		#endregion
	}
}
