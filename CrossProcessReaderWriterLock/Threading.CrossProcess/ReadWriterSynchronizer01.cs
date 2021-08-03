using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Threading.CrossProcess
{
    public class ReadWriterSynchronizer01
    {
        public ReadWriterSynchronizer01(string name, int maxReaderCount)
        {
            myIncomingOperation = new Semaphore(1, 1, name + ".Incoming");
            myReadOperation = new Semaphore(1, 1, name + ".Reader");
            myWriteOperation = new Semaphore(1, 1, name + ".Writer");
            myInterprocessCounter = new ReaderCounter(name + ".Counter", maxReaderCount);
        }

        public void EnterReadLock()
        {
            myIncomingOperation.WaitOne();
            myReadOperation.WaitOne();

            int currentCount = myInterprocessCounter.Increase();
            if (currentCount == 1)
            {
                myWriteOperation.WaitOne();
            }

            myReadOperation.Release();
            myIncomingOperation.Release();
        }

        public void ExitReadLock()
        {
            myReadOperation.WaitOne();

            int currentCount = myInterprocessCounter.Decrease();
            if (currentCount == 0)
            {
                myWriteOperation.Release();
            }

            myReadOperation.Release();
        }

        public void TryEnterWriteLock()
        {
            myIncomingOperation.WaitOne();
            myWriteOperation.WaitOne();
        }

        public void ExitWriteLock()
        {
            myWriteOperation.Release();
            myIncomingOperation.Release();
        }

        private readonly ReaderCounter myInterprocessCounter;
        private readonly Semaphore myIncomingOperation;
        private readonly Semaphore myReadOperation;
        private readonly Semaphore myWriteOperation;
    }
}