using System;
using System.Threading;

namespace TradingTest
{
    class Program
    {
        private static readonly ManualResetEvent ResetEvent = new(false);
        
        static void Main(string[] args)
        {
            BinanceService
            
            ResetEvent.WaitOne(); // Prevents from breaking app on dedicated server
        }
    }
}