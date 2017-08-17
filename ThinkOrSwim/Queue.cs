using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkOrSwim
{
    class Queue : IEnumerator, IEnumerator<Quote>
    {
        BlockingCollection<Quote> queue = new BlockingCollection<Quote>(new ConcurrentQueue<Quote>());
        Quote current;

        internal Queue()
        {
            
        }

        internal void Disconnect()
        {
            try { this.queue.CompleteAdding(); } catch { };
        }

        internal void Push(Quote quote)
        {
            this.queue.Add(quote);
        }

        public Quote Current
        {
            get
            {
                return this.current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.current;
            }
        }

        public void Dispose()
        {
            this.queue.Dispose();
        }

        public bool MoveNext()
        {
            if (this.queue.IsCompleted)
            {
                return false;
            }
            var task = Task.Run(() => this.current = this.queue.Take());
            if (task.Wait(TimeSpan.FromSeconds(60)))
                //return task.Result;
                return true;
            else
                throw new Exception("Timed out");
        }

        public void Reset()
        {
     
        }
    }
}
