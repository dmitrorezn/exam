using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Queues
{
    class Program
    {
        public static Object theLocker = new object();

        static void Main(string[] args)
        {
            int theNumber = 0;

            var queue = new FIFOQueue(10000);

            // uncomment below to observe concurrency issues even while all methods of FIFOQueue using locks inside
            //for (int i = 0; i < 10000; i++) {
            //    queue.Put(i);
            //}
            //ThreadPool.QueueUserWorkItem((o) => DequeueWhileExists(queue));
            //ThreadPool.QueueUserWorkItem((o) => DequeueWhileExists(queue));

            var thread1 = new Thread((o) => PutThenPick(queue));
            var thread2 = new Thread((o) => PutThenPick(queue));
            var thread3 = new Thread((o) => PutThenPick(queue));

            thread1.Start();
            thread2.Start();
            thread3.Start();

            var allThreadsAreDone = false;
            while (!allThreadsAreDone)
            {
                //Console.WriteLine(theNumber);
                allThreadsAreDone = thread1.ThreadState == System.Threading.ThreadState.Stopped &&
                    thread2.ThreadState == System.Threading.ThreadState.Stopped &&
                    thread3.ThreadState == System.Threading.ThreadState.Stopped;
            }

            //Console.WriteLine("result:" + theNumber);
            Console.ReadLine();

            //ThreadPool.QueueUserWorkItem((o) => PutThenPick(queue));
            //ThreadPool.QueueUserWorkItem((o) => PutThenPick(queue));
            //ThreadPool.QueueUserWorkItem((o) => PutThenPick(queue));


            //Thread.Sleep(2000);

            //Console.ReadLine();
        }

        static void PutThenPick(FIFOQueue queue)
        {
            for (int i = 0; i < 5; i++)
            {
                queue.Put(i);
                queue.Pick();
            }
            Console.WriteLine("--------------ready-------------");
        }

        static void DequeueWhileExists(FIFOQueue queue)
        {
            while (true)
            {
                if (queue.Count() > 0)
                {
                    queue.Pick();
                }
            }
        }
    }

    interface IQueue
    {
        // берем из очереди
        int Pick();
        // ставим в очередь
        void Put(int putIt);
    }

    // first in first out Queue
    class FIFOQueue : IQueue
    {
        // реализация хранилища на массиве
        private int[] array;
        private int length;

        public FIFOQueue(int capacity)
        {
            array = new int[capacity];
        }

        public int Count()
        {
            return this.length;
        }

        public int Pick()
        {
            lock (array)
            {
                var retIt = array[length - 1];
                length--;
                Console.WriteLine("\tpick " + retIt + " length " + length + " treadID: " + Thread.CurrentThread.ManagedThreadId);
                return retIt;
            }
        }

        public void Put(int putIt)
        {
            lock (array)
            {
                if (length == array.Length)
                {
                    return;
                }

                for (int i = length - 1; i >= 0; i--)
                {
                    array[i + 1] = array[i];
                }
                array[0] = putIt;
                length++;
                Console.WriteLine("\tpuck " + putIt + " length " + length + " treadID: " + Thread.CurrentThread.ManagedThreadId);

            }
        }
    }
}



