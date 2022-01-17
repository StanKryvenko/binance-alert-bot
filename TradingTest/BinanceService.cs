using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;

namespace TradingTest
{
    public class BinanceService
    {
        private static readonly BinanceService Instance = new();
        public static BinanceService GetInstance() => Instance;
        
        private readonly BinanceClient _binanceClient = new();

        public void Initialize(string key, string secret)
        {
            BinanceClient.SetDefaultOptions(new BinanceClientOptions
                {ApiCredentials = new ApiCredentials(key, secret)});
            BinanceSocketClient.SetDefaultOptions(new BinanceSocketClientOptions
                {ApiCredentials = new ApiCredentials(key, secret)});
        }

        public async Task<IEnumerable<IBinanceKline>> GetMinuteCandles(string ticker, int candlesAmount)
        {
            var klines = await _binanceClient.UsdFuturesApi.ExchangeData.GetKlinesAsync(
                ticker,
                KlineInterval.OneMinute,
                limit: candlesAmount);
            return klines.Data;
        }
    }
}