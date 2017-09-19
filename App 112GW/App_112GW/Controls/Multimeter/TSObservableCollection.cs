using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace rMultiplatform
{
    public class TSObservableCollection<T>
    {
        private Mutex Mutex;
        public ObservableCollection<T> Data;
        public int Count => Data.Count;
        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                Data.CollectionChanged += value;
            }
            remove
            {
                Data.CollectionChanged -= value;
            }
        }

        public TSObservableCollection()
        {
            Data = new ObservableCollection<T>();
            Mutex = new Mutex();
        }

        public void RemoveAt(int Index)
        {
            Mutex.WaitOne();
            Data.RemoveAt(Index);
            Mutex.ReleaseMutex();
        }
        public void Clear()
        {
            Mutex.WaitOne();
            Data.Clear();
            Mutex.ReleaseMutex();
        }
        public void Add(T Item)
        {
            Mutex.WaitOne();
            Data.Add(Item);
            Mutex.ReleaseMutex();
        }

        public T this[int key]
        {
            get
            {
                Mutex.WaitOne();
                var otuput = Data[key];
                Mutex.ReleaseMutex();
                return otuput;
            }
        }

        public List<T> ToList()
        {
            Mutex.WaitOne();
            var output = Data.ToList();
            Mutex.ReleaseMutex();
            return output;
        }
    }
}
