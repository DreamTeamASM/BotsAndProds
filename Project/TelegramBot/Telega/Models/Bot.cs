using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegramBot
{
    class Bot
    {
        private static TelegramBotClient client;
        public static List<Comands> comandsList;
       // public static IReadOnlyList<Comands> commands { get=> comandsList.AsReadOnly(); }
        public static async Task<TelegramBotClient> Get()
        {
            if (client != null)
                return client;
            comandsList = new List<Comands>();
            comandsList.Add(new HelloCommand());
          
            client = new TelegramBotClient(AppSettings.Key);
            var hook = string.Format(AppSettings.Url,"api/message/update");
            await  client.SetWebhookAsync("");
            return client;
        }
    }
}
