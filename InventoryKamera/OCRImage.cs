using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryKamera
{
    public class OCRImage 
    {
        public List <Bitmap> bm  { get; set; }
        public string type { get; set; }
        public int id { get; set; }

        public OCRImage(List <Bitmap> _bm, string _type, int _id)
        {
            bm = _bm;
            type = _type;
            id = _id;
        }

    }

    public class Queue<T>
    {
        /// <summary>Used as a lock target to ensure thread safety.</summary>
        private readonly object _Locker = new object();

        private readonly System.Collections.Generic.Queue<T> _Queue = new System.Collections.Generic.Queue<T>();

        /// <summary></summary>
        public void Enqueue(T item)
        {
            lock (_Locker)
            {
                _Queue.Enqueue(item);
            }
        }

        /// <summary>Enqueues a collection of items into this queue.</summary>
        public virtual void EnqueueRange(IEnumerable<T> items)
        {
            lock (_Locker)
            {
                if (items == null)
                {
                    return;
                }

                foreach (T item in items)
                {
                    _Queue.Enqueue(item);
                }
            }
        }

        /// <summary></summary>
        public T Dequeue()
        {
            lock (_Locker)
            {
                return _Queue.Dequeue();
            }
        }

        /// <summary></summary>
        public void Clear()
        {
            lock (_Locker)
            {
                _Queue.Clear();
            }
        }

        /// <summary></summary>
        public Int32 Count
        {
            get
            {
                lock (_Locker)
                {
                    return _Queue.Count;
                }
            }
        }

        /// <summary></summary>
        public Boolean TryDequeue(out T item)
        {
            lock (_Locker)
            {
                if (_Queue.Count > 0)
                {
                    item = _Queue.Dequeue();
                    return true;
                }
                else
                {
                    item = default(T);
                    return false;
                }
            }
        }
    }
}
