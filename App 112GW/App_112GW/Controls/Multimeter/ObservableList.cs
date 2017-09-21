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
        event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;

        void RemoveAt(int Index);
        void Clear();
        void Add(T Item);

        T this[int key] { get; }
        List<T> ToList();
    }

    public class TriggerList<T> : IObservableList<T>
    {
        private Mutex Mutex = new Mutex();
        public List<T> Data = new List<T>();

        public T this[int key] => Data[key];

        public int Count => Data.Count;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Add(T Item)         => Data.Add(Item);
        public void Clear()             => Data.Clear();
        public void RemoveAt(int Index) => Data.RemoveAt(Index);
        public void Modify() => Mutex.WaitOne();
        public void Trigger()
        {
            Mutex.ReleaseMutex();
            CollectionChanged?.Invoke(this, null);
        }

        public List<T> ToList()
        {
            Mutex.WaitOne();
            var output = new List<T>(Data);
            Mutex.ReleaseMutex();
            return output;
        }
    }

    public class ObservableList<T>: IObservableList<T>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private Mutex   Mutex;
        public List<T>  Data;
        public int      Count => Data.Count;

        public void RemoveAt(int Index)
        {
            Mutex.WaitOne();
            Data.RemoveAt(Index);
            Mutex.ReleaseMutex();
            CollectionChanged?.Invoke(this, null);
        }
        public void Clear()
        {
            Mutex.WaitOne();
            Data.Clear();
            Mutex.ReleaseMutex();
            CollectionChanged?.Invoke(this, null);
        }
        public void Add(T Item)
        {
            Mutex.WaitOne();
            Data.Add(Item);
            Mutex.ReleaseMutex();
            CollectionChanged?.Invoke(this, null);
        }
        T IObservableList<T>.this[int key]
        {
            get
            {
                Mutex.WaitOne();
                var output = Data[key];
                Mutex.ReleaseMutex();
                return output;
            }
        }
        public List<T> ToList()
        {
            Mutex.WaitOne();
            var output = new List<T>(Data);
            Mutex.ReleaseMutex();
            return output;
        }

        public ObservableList()
        {
            Data = new List<T>();
            Mutex = new Mutex();
        }
    }
}
