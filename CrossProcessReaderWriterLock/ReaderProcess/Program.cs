using System;
using Threading.CrossProcess;

namespace ReaderProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();
            ReaderWriterSynchronizer02 synchronizer = new ReaderWriterSynchronizer02("Synchronizer", 5, TimeSpan.FromSeconds(3));

            for (int i = 0; i < 30; i++)
            {
                if (synchronizer.TryEnterReadLock())
                {
                    Console.WriteLine("Read {0} Critical Section", i);

                    synchronizer.ExitReadLock();
                }
            }

            Console.ReadLine();
        }
    }
}
