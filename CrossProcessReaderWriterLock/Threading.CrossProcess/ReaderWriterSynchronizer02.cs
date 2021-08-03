using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Threading.CrossProcess
{
    /// <summary>
    /// API:YES Reader writer synchronizer. It limits the number of threads that can access a resource concurrently for read and write operations.
    /// </summary>
    public class ReaderWriterSynchronizer02
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderWriterSynchronizer02"/> class.
        /// </summary>
        /// <param name="name">The synchronization object name. The name is case-sensitive.</param>
        /// <param name="maxReaderCount">The counter number for controlling the maximum number of concurrent read operation.</param>
        /// <param name="timeout">Reader and writer TimeSpan that represents the number of milliseconds to wait, or a TimeSpan that represents -1 milliseconds to wait indefinitely.</param>
        public ReaderWriterSynchronizer02(string name, int maxReaderCount, TimeSpan timeout)
        {
            CheckSemaphoreName(name);

            myInterProcessReadCounter = new ReaderCounter(name + ".Counter", maxReaderCount);
            myIncomingOperation = new Semaphore(1, 1, name + ".Incoming");
            myReadOperation = new Semaphore(1, 1, name + ".Reader");
            myWriteOperation = new Semaphore(1, 1, name + ".Writer");

            myTimeout = timeout;
        }

        /// <summary>
        /// Tries to enter the read semaphore lock. Blocks the current thread until the current instance receives a signal in the specified TimeSpan time interval.
        /// </summary>
        /// <returns>true if the current instance receives a signal; otherwise, false.</returns>
        public bool TryEnterReadLock()
        {
            if (!myIncomingOperation.WaitOne(myTimeout))
            {
                return false;
            }

            if (!myReadOperation.WaitOne(myTimeout))
            {
                myIncomingOperation.Release();
                return false;
            }

            // Local variable is necessary, because of compiler optimalization
            int currentCount = myInterProcessReadCounter.Increase();
            if (currentCount == 1)
            {
                if (!myWriteOperation.WaitOne(myTimeout))
                {
                    myIncomingOperation.Release();
                    myReadOperation.Release();
                    return false;
                }
            }

            myReadOperation.Release();
            myIncomingOperation.Release();

            if (currentCount == myInterProcessReadCounter.MaximumCount)
            {
                return false;
            }

            myIsReadLockEntered = true;

            return myIsReadLockEntered.Value;
        }

        /// <summary>
        /// Exits the read semaphore.
        /// </summary>
        public void ExitReadLock()
        {
            if (!myIsReadLockEntered.HasValue || myIsReadLockEntered.Value == false)
            {
                throw new InvalidOperationException();
            }

            if (!myReadOperation.WaitOne(myTimeout))
            {
                return;
            }

            // Local variable is necessary, because of compiler optimalization
            int currentCount = myInterProcessReadCounter.Decrease();
            if (currentCount == 0)
            {
                myWriteOperation.Release();
            }

            myReadOperation.Release();
            myIsReadLockEntered = false;
        }

        /// <summary>
        /// Tries to enter the write semaphore lock. Blocks the current thread until the current instance receives a signal in the specified TimeSpan time interval.
        /// </summary>
        public bool TryEnterWriteLock()
        {
            if (!myIncomingOperation.WaitOne(myTimeout))
            {
                return false;
            }

            if (!myWriteOperation.WaitOne(myTimeout))
            {
                myIncomingOperation.Release();
                return false;
            }

            myIsWriteLockEntered = true;

            return myIsWriteLockEntered.Value;
        }

        /// <summary>
        /// Exits the write semaphore.
        /// </summary>
        public void ExitWriteLock()
        {
            if (!myIsWriteLockEntered.HasValue || myIsWriteLockEntered.Value == false)
            {
                throw new InvalidOperationException();
            }

            myWriteOperation.Release();
            myIncomingOperation.Release();

            myIsWriteLockEntered = false;
        }

        /// <summary>
        /// Releases all resources used by the current instance.
        /// </summary>
        public void Dispose()
        {
            myIncomingOperation?.Dispose();
            myReadOperation?.Dispose();
            myWriteOperation?.Dispose();
            myInterProcessReadCounter?.Dispose();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Limits and count the maximum number of concurrent read operation.
        /// </summary>
        private readonly ReaderCounter myInterProcessReadCounter;

        /// <summary>
        /// Semaphore synchronizer to ensure no writer starvation.
        /// </summary>
        private readonly Semaphore myIncomingOperation;

        /// <summary>
        /// Semaphore synchronizer for read operations.
        /// </summary>
        private readonly Semaphore myReadOperation;

        /// <summary>
        /// Semaphore synchronizer to ensure write exclusivity.
        /// </summary>
        private readonly Semaphore myWriteOperation;

        /// <summary>
        /// Time interval to wait for semaphore signal.
        /// </summary>
        private readonly TimeSpan myTimeout;

        /// <summary>
        /// Reader entering flag to ensure one enter per exit.
        /// </summary>
        private bool? myIsReadLockEntered = null;

        /// <summary>
        /// Writer entering flag to ensure one enter per exit.
        /// </summary>
        private bool? myIsWriteLockEntered = null;

        /// <summary>
        /// Checks synchronization object name validity.
        /// </summary>
        /// <param name="name">The synchronization object name. The name is case-sensitive.</param>
        private static void CheckSemaphoreName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Empty synchronization object name could not be used for synchronizating between processes!", nameof(name));
            }
        }
    }
}
