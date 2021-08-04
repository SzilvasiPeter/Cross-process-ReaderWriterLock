using System;
using System.Threading;
using Threading.CrossProcess;

namespace WriterProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();
            ReaderWriterSynchronizer02 synchronizer = new ReaderWriterSynchronizer02("Synchronizer", 5, TimeSpan.FromSeconds(3));

            for (int i = 0; i < 10; i++)
            {
                if (synchronizer.TryEnterWriteLock())
                {
                    Thread.Sleep(300);
                    Console.WriteLine("Write {0} Critical Section", i);

                    synchronizer.ExitWriteLock();
                }
            }

            Console.ReadLine();
        }
    }
}
