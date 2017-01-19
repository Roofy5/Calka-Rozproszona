using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Library
{
    public class PoolOfThreads
    {
        private static PoolOfThreads instance = new PoolOfThreads();

        public static PoolOfThreads Instance
        {
            get { return instance; }
        }

        private List<Thread> listOfThreads;
        private int maxThreads;

        public int MaxThreads
        {
            get { return maxThreads; }
            set { maxThreads = value; }
        }

        private PoolOfThreads()
        {
            listOfThreads = new List<Thread>();
        }


        /// <exception cref="Exception"></exception>
        public Thread NewThread(ParameterizedThreadStart threadMethod)
        {
            FreeThreads();

            if (listOfThreads.Count < maxThreads)
            {
                Thread thread = new Thread(threadMethod);
                listOfThreads.Add(thread);
                return thread;
            }
            else
                throw new Exception("No free threads...");
        }

        private void FreeThreads()
        {
            /*var freeThreads = listOfThreads.Select(thread => thread.ThreadState == ThreadState.Stopped) as List<Thread>;
            foreach (Thread thread in freeThreads)
                listOfThreads.Remove(thread);
            */
            listOfThreads.RemoveAll(thread => thread.ThreadState == ThreadState.Stopped || thread.ThreadState == ThreadState.Aborted);
        }
    }
}
