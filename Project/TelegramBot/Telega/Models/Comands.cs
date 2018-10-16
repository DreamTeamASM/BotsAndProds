using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
   public abstract class Comands
    {
        public abstract string Name { get; }
        public abstract void Execute(Message message,TelegramBotClient client);
        public bool Contains(string comand)
        {
            return comand.Contains(this.Name) && comand.Contains(AppSettings.Name);
        }

    }
}
