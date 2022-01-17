using System;
using System.Threading;

namespace TradingTest
{
    public class Program
    {
        private const string BinanceKey = "YOUR_BINANCE_KEY";
        private const string BinanceSecret = "YOUR_BINANCE_SECRET";
        private const string TelegramKey = "TELEGRAM_BOT_KEY";

        public static readonly ManualResetEvent ResetEvent = new(false);

        static void Main(string[] args)
        {
            TelegramService.GetInstance().Initialize(TelegramKey);
            BinanceService.GetInstance().Initialize(BinanceKey, BinanceSecret);
            BinanceAlertBotService.GetInstance().Start();

            ResetEvent.WaitOne(); // Prevents from breaking app on dedicated server
        }
    }
}