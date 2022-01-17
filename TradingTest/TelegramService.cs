using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TradingTest
{
    public class TelegramService
    {
        private static readonly TelegramService Instance = new();
        public static TelegramService GetInstance() => Instance;
        
        public TelegramBotClient BotClient { get; set; }
        
        public void Initialize(string key)
        {
            BotClient = new TelegramBotClient(key);
            
            BotClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), null);
        }
        
        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException apiRequestException)
                Console.WriteLine("BOT ERROR: " + apiRequestException);
            return Task.CompletedTask;
        }

        private Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.ChannelPost ?? update.Message;
            if (message != null && !string.IsNullOrEmpty(message.Text))
            {
                var messageText = message.Text;
                if (messageText.ToLower().StartsWith("add "))
                    BinanceAlertBotService.GetInstance().AddTicker(messageText.Replace("add ", ""));
                else if (messageText.ToLower().StartsWith("remove "))
                    BinanceAlertBotService.GetInstance().RemoveTicker(messageText.Replace("remove ", ""));
                else if (messageText.ToLower() == "tickers")
                    BinanceAlertBotService.GetInstance().ShowTickers();
            }
            return Task.CompletedTask;
        }
        
        public async Task SendMessage(long accountId, string message)
        {
            await BotClient.SendTextMessageAsync(accountId, message, ParseMode.Markdown);
        }
    }
}