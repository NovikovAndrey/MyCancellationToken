using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyCancellationToken
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            new Task(() =>
            {
                Thread.Sleep(400);
                cts.Cancel();
            }).Start();
            try
            {
                int[] numbers = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9 };
                var factorials = from n in numbers.AsParallel().WithCancellation(cts.Token)
                                select Factorial(n);
                foreach (var n in factorials)
                {
                    Console.WriteLine(n);
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions != null)
                {
                    foreach(Exception exception in ex.InnerExceptions)
                    {
                        Console.WriteLine(exception.Message);
                    }
                }
            }
            finally
            {
                cts.Dispose();
            }
            Console.ReadKey();
        }

        private static object Factorial(int n)
        {
            int result = 1;
            for (int i=1; i<=n; i++)
            {
                result *= i;
            }
            Console.WriteLine($"result = {result}");
            Thread.Sleep(1000);
            return result;
        }
    }
}
