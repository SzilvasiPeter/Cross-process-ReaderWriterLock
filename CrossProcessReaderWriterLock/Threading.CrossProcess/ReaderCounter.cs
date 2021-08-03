using System;
using System.Threading;

namespace Threading.CrossProcess
{
    /// <summary>
    /// Counter to track the number of processes or threads that can read access to the repository.
    /// </summary>
    internal class ReaderCounter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterProcessReadCounter"/> class.
        /// </summary>
        /// <param name="name">The synchronization object name. The name is case-sensitive.</param
        /// <param name="maxConcurrentRead">The maximum number of reader requests that can be granted concurrently.</param>
        internal ReaderCounter(string name, int maxConcurrentRead)
        {
            MaximumCount = maxConcurrentRead + InitialCount;
            myReadCounterSemaphore = new Semaphore(InitialCount, MaximumCount, name);
            myIncomingOperation = new Semaphore(1, 1, name + ".Incoming");
        }

        /// <summary>
        /// Increases the read counter by 1 across processes.
        /// </summary>
        /// <returns></returns>
        internal int Increase()
        {
            // Make sure for atomic increase
            myIncomingOperation.WaitOne();

            int counter = RetrieveCurrentCount();

            // Not allowing to exceed maximum count
            if (counter != MaximumCount - 1)
            {
                counter = myReadCounterSemaphore.Release();
            }
            else
            {
                counter++;
            }

            myIncomingOperation.Release();

            return counter;
        }

        /// <summary>
        /// Decreases the read counter by 1 across processes.
        /// </summary>
        /// <returns></returns>
        internal int Decrease()
        {
            // Make sure for atomic decrease
            myIncomingOperation.WaitOne();

            int counter = RetrieveCurrentCount() - 1;
            myReadCounterSemaphore.WaitOne(0);

            myIncomingOperation.Release();

            return counter;
        }

        /// <summary>
        /// Releases all resources used by the current instance.
        /// </summary>
        public void Dispose()
        {
            myReadCounterSemaphore?.Dispose();
            myIncomingOperation?.Dispose();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Maximum number of concurrent read count.
        /// </summary>
        internal int MaximumCount { get; private set; }

        /// <summary>
        /// Initial count in order to safely exceed or deceed semaphore.
        /// </summary>
        private const int InitialCount = 1;

        /// <summary>
        /// Semaphore to keep track of number of reader.
        /// </summary>
        private readonly Semaphore myReadCounterSemaphore;

        /// <summary>
        /// Semaphore to ensure atomic increase and decrease.
        /// </summary>
        private readonly Semaphore myIncomingOperation;

        /// <summary>
        /// Retrieves the current number of readers.
        /// </summary>
        /// <returns>The active reader counts.</returns>
        private int RetrieveCurrentCount()
        {
            myReadCounterSemaphore.WaitOne(0);
            int counter = myReadCounterSemaphore.Release();
            return counter;
        }
    }
}
