using System.Collections.Generic;

namespace de.mastersign.shell
{
    internal class SynchronizedQueue<T>
    {
        private readonly Queue<T> queue;
        private readonly object lockObject = new object();

        public SynchronizedQueue()
        {
            queue = new Queue<T>();
        }

        public SynchronizedQueue(Queue<T> queue)
        {
            this.queue = queue;
        }

        public int Count
        {
            get
            {
                lock (lockObject)
                {
                    return queue.Count;
                }
            }
        }

        public T Peek()
        {
            lock (lockObject)
            {
                return queue.Peek();
            }
        }

        public void Enqueue(T item)
        {
            lock (lockObject)
            {
                queue.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            lock (lockObject)
            {
                return queue.Dequeue();
            }
        }

        public void Clear()
        {
            lock (lockObject)
            {
                queue.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (lockObject)
            {
                return queue.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (lockObject)
            {
                queue.CopyTo(array, arrayIndex);
            }
        }
    }
}