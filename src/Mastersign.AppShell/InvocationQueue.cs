using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace de.mastersign.shell
{
    internal class InvocationQueue
    {
        public event EventHandler ProcessingStarts;
        public event EventHandler ProcessingEnds;

        private readonly Queue<CommandInfo> queue = new Queue<CommandInfo>();
        private volatile bool isWorking;
        private volatile bool isProcessing;
        private volatile bool canceling;

        private volatile CommandInfo currentWork;

        private readonly InvocationCallback invoker;

        public InvocationQueue(InvocationCallback invoker)
        {
            this.invoker = invoker;
        }

        public void Enqueue(CommandInfo info)
        {
            if (canceling)
            {
                return;
            }
            lock (queue)
            {
                queue.Enqueue(info);
            }
            ProcessTail();
        }

        private void ProcessTail()
        {
            CommandInfo cmd = null;
            lock (queue)
            {
                if (isWorking)
                {
                    return;
                }
                if (queue.Count > 0)
                {
                    cmd = queue.Dequeue();
                    currentWork = cmd;
                    isWorking = true;
                }
            }
            if (cmd != null)
            {
                OnProcessingStarts();
                ThreadPool.QueueUserWorkItem(WorkItem, cmd);
            }
        }

        private void WorkItem(object state)
        {
            var info = (CommandInfo)state;
            invoker(info);

            bool endsProcessing;
            
            lock (queue)
            {
                isWorking = false;
                currentWork = null;
                endsProcessing = queue.Count == 0;
            }
            
            if (endsProcessing)
            {
                OnProcessingEnds();
            }
            
            ProcessTail();
        }

        public bool IsWorking
        {
            get
            {
                lock (queue)
                {
                    return isWorking || queue.Count > 0;
                }
            }
        }

        public void Cancel()
        {
            canceling = true;
            lock (queue)
            {
                queue.Clear();
            }
            if (currentWork != null)
            {
                while (currentWork.Pipe == null)
                {
                    Thread.Sleep(10);
                }
                currentWork.Pipe.StopAsync();
            }
            canceling = false;
        }

        private void OnProcessingStarts()
        {
            if (isProcessing)
            {
                return;
            }
            isProcessing = true;
            if (ProcessingStarts != null)
            {
                ProcessingStarts(this, EventArgs.Empty);
            }
        }

        private void OnProcessingEnds()
        {
            isProcessing = false;
            if (ProcessingEnds != null)
            {
                ProcessingEnds(this, EventArgs.Empty);
            }
        }
    }
}
