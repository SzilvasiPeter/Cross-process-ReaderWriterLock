using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Threading.CrossProcess
{
    public class ReadWriterSynchronizer01 : IDisposable
    {
        public ReadWriterSynchronizer01(string name, int maxReaderCount)
        {
            myIncomingOperation = new Semaphore(1, 1, name + ".Incoming");
            myReadOperation = new Semaphore(1, 1, name + ".Reader");
            myWriteOperation = new Semaphore(1, 1, name + ".Writer");
            myCrossprocessCounter = new ReaderCounter(name + ".Counter", maxReaderCount);
        }

        public void EnterReadLock()
        {
            myIncomingOperation.WaitOne();
            myReadOperation.WaitOne();

            // Local variable is necessary, because of compiler optimalization
            int currentCount = myCrossprocessCounter.Increase();
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

            // Local variable is necessary, because of compiler optimalization
            int currentCount = myCrossprocessCounter.Decrease();
            if (currentCount == 0)
            {
                myWriteOperation.Release();
            }

            myReadOperation.Release();
        }

        public void EnterWriteLock()
        {
            myIncomingOperation.WaitOne();
            myWriteOperation.WaitOne();
        }

        public void ExitWriteLock()
        {
            myWriteOperation.Release();
            myIncomingOperation.Release();
        }

        public void Dispose()
        {
            myIncomingOperation?.Dispose();
            myReadOperation?.Dispose();
            myWriteOperation?.Dispose();
            myCrossprocessCounter?.Dispose();

            GC.SuppressFinalize(this);
        }

        private readonly ReaderCounter myCrossprocessCounter;
        private readonly Semaphore myIncomingOperation;
        private readonly Semaphore myReadOperation;
        private readonly Semaphore myWriteOperation;
    }
}