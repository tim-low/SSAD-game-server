using Game_Server.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Game_Server.Util
{
    public class SynchronizeList<T> : ICollection where T : GameObject
    {
        private List<T> List = new List<T>();
        private Mutex Mutex = new Mutex(false);

        public int Count
        {
            get 
            {
                int a = 0;
                Mutex.WaitOne();
                a = List.Count;
                Mutex.ReleaseMutex();
                return a;
            }
        }

        public bool IsReadOnly => false;

        public bool IsSynchronized => true;

        public object SyncRoot => new object();

        public void Add(T item)
        {
            Mutex.WaitOne();
            List.Add(item);
            Mutex.ReleaseMutex();
        }

        public void Clear()
        {
            Mutex.WaitOne();
            List.Clear();
            Mutex.ReleaseMutex();
        }

        public bool Contains(T item)
        {
            bool value;
            Mutex.WaitOne();
            value = List.Contains(item);
            Mutex.ReleaseMutex();
            return value;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Mutex.WaitOne();
                List.CopyTo(array, arrayIndex);
            Mutex.ReleaseMutex();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            Mutex.WaitOne();
            var value = this.GetEnumerator();
            Mutex.ReleaseMutex();
            return value;
        }

        public bool Remove(T item)
        {
            Mutex.WaitOne();
            var value = List.Remove(item);
            Mutex.ReleaseMutex();
            return value;
        }

        public T Get(string id)
        {
            Mutex.WaitOne();
            var value = List.SingleOrDefault(item => item.GetIdentifier() == id);
            Mutex.ReleaseMutex();
            return value;
        }

        public List<T> GetAll()
        {
            Mutex.WaitOne();
            var value = List;
            Mutex.ReleaseMutex();
            return value;
        }

        public List<T> Randomize()
        {
            Mutex.WaitOne();
            List.Shuffle();
            Mutex.ReleaseMutex();
            return List;
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            Mutex.WaitOne();
            var value = List.GetEnumerator();
            Mutex.ReleaseMutex();
            return value;
        }
    }
}
