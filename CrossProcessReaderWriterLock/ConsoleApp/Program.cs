using System;
using Threading.CrossProcess;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ReaderCounter counter = new ReaderCounter("Counter", 5);

            Console.WriteLine(counter.Increase());
            Console.WriteLine(counter.Decrease());
            Console.WriteLine(counter.Decrease());

            Console.WriteLine(counter.Increase());
            Console.WriteLine(counter.Increase());
            Console.WriteLine(counter.Increase());
            Console.WriteLine(counter.Increase());
            Console.WriteLine(counter.Increase());
            Console.WriteLine(counter.Increase());
            Console.WriteLine(counter.Increase());
            Console.WriteLine(counter.Increase());

            Console.WriteLine();

            Console.WriteLine(counter.Decrease());
            Console.WriteLine(counter.Decrease());
            Console.WriteLine(counter.Decrease());
            Console.WriteLine(counter.Decrease());
            Console.WriteLine(counter.Decrease());
            Console.WriteLine(counter.Decrease());
            Console.WriteLine(counter.Decrease());
            Console.WriteLine(counter.Decrease());

            Console.WriteLine();

            Console.WriteLine(counter.Increase());
            Console.WriteLine(counter.Increase());
            Console.WriteLine(counter.Increase());
            Console.WriteLine(counter.Increase());
            Console.WriteLine(counter.Increase());
            Console.WriteLine(counter.Increase());
            Console.WriteLine(counter.Increase());
            Console.WriteLine(counter.Increase());

            Console.WriteLine();

            Console.WriteLine(counter.Decrease());
            Console.WriteLine(counter.Decrease());
            Console.WriteLine(counter.Decrease());
            Console.WriteLine(counter.Decrease());
            Console.WriteLine(counter.Decrease());
            Console.WriteLine(counter.Decrease());
            Console.WriteLine(counter.Decrease());
            Console.WriteLine(counter.Decrease());

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}
