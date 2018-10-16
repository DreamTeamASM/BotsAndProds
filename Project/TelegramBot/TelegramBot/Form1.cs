using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace TelegramBot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static TelegramBotClient Bot;
        private void button1_Click(object sender, EventArgs e)
        {
            Bot = new TelegramBotClient("674361526:AAEH4RVtJK_ydS8LJXy-R8YL-WiD2IMWzHg");
            Bot.OnMessage += BotOnMessageRecieved;
            Bot.StartReceiving(new Telegram.Bot.Types.Enums.UpdateType[] { Telegram.Bot.Types.Enums.UpdateType.Message});

        }

        private static async void BotOnMessageRecieved(object sender,MessageEventArgs messageEventArgs)
        {
            Telegram.Bot.Types.Message msg = messageEventArgs.Message;
            if (msg == null || msg.Type != Telegram.Bot.Types.Enums.MessageType.Text) return;
            string answer = "Heelo!";
            if (msg.Text == "Hi!")
                await Bot.SendTextMessageAsync(msg.Chat.Id, answer);

        }
    }
}
