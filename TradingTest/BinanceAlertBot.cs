using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Skender.Stock.Indicators;

namespace TradingTest
{
    public class BinanceAlertBotService
    {
        private static readonly BinanceAlertBotService Instance = new();
        public static BinanceAlertBotService GetInstance() => Instance;
        
        private Timer _timer;

        private long _accountId = YOUR_TELEGRAM_ID;

        private List<string> _watchlist = new();
        
        public void Start()
        {
            _timer = new Timer(async state => await Update(state), null, 0, 45000);
        }

        private async Task Update(object state)
        {
            foreach (var symbol in _watchlist)
            {
                var candles = await BinanceService.GetInstance().GetMinuteCandles(symbol, 1500);
                if (candles == null) continue;
                
                var quotes = candles.Select(candle => new Quote
                {
                    Close = candle.ClosePrice,
                    Date = candle.CloseTime,
                    High = candle.HighPrice,
                    Low = candle.LowPrice,
                    Open = candle.OpenPrice,
                    Volume = candle.Volume
                });
            
                // Measure two SMMA's for current and previous candles
                var smmaShortLast = quotes.GetSmma(21).Last().Smma;
                var smmaShortPrevious = quotes.GetSmma(21).SkipLast(1).Last().Smma;
                var smmaLongLast = quotes.GetSmma(200).Last().Smma;
                var smmaLongPrevious = quotes.GetSmma(200).SkipLast(1).Last().Smma;
            
                // Detect reversal
                var isCurrentLong = smmaShortLast > smmaLongLast;
                var isPreviousLong = smmaShortPrevious > smmaLongPrevious;
                if (isCurrentLong != isPreviousLong)
                {
                    await TelegramService.GetInstance().SendMessage(_accountId,
                        $"Reversal for {symbol}. **{(isCurrentLong ? "Long" : "Short")}**");
                }
            }
        }

        public async void AddTicker(string ticker)
        {
            _watchlist.Add(ticker);
            await TelegramService.GetInstance().SendMessage(_accountId, "Added new ticker");
        }

        public async void RemoveTicker(string ticker)
        {
            _watchlist.Remove(ticker);
            await TelegramService.GetInstance().SendMessage(_accountId, "Removed ticker");
        }

        public async void ShowTickers()
        {
            await TelegramService.GetInstance().SendMessage(_accountId, $"Tickers: \n{string.Join('\n', _watchlist)}");
        }
    }
}