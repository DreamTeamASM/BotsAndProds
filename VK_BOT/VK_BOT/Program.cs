using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using VkNet;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.RequestParams;
namespace VK_BOT
{
    class Program
    {
        static VkApi vkapi = new VkApi();
        delegate void MessagesRecievedDelegate(VkApi owner, ReadOnlyCollection<Message> messages);
        static long userID = 0;
        static event MessagesRecievedDelegate NewMessages;

        static ulong? Ts;
        static ulong? Pts;
        static bool IsActive;
        static Timer WatchTimer = null;
        static byte CurrentSleepSteps = 1;
        static byte MaxSleepSteps = 3;
        static int StepSleepTime = 333;
        static void Main(string[] args)
        {
            SQLiteConnection con = new SQLiteConnection("Data source = DB.db;version=3");
            con.Open();
            string loginUser;
            string passwordUser;
            ulong appid;
            SQLiteCommand com;
            try
            {
                //com = new SQLiteCommand("select Login from Users where id ==1", con);
                //loginUser = com.ExecuteScalar().ToString();

                //com = new SQLiteCommand("select Password from Users where id ==1", con);
                //passwordUser = com.ExecuteScalar().ToString();

                //com = new SQLiteCommand("select AppID from Users where id ==1", con);
                //appid = ulong.Parse(com.ExecuteScalar().ToString());

                string KEY = string.Empty;
                KEY = "278b13af3bc3b709a06ecb08e7b507565c190b4bcd7669b1155659d03f0e5d40ff3057233674e4777b24a";
                if (Auth(KEY))
                {
                    Console.WriteLine("Авторизация успешно завершена!", ConsoleColor.Green);
                    Eye();
                    Console.WriteLine("Нажмите ENTER чтобы выйти...");
                    Console.ReadLine();
                    Thread.Sleep(10000);
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Thread.Sleep(10000);
            }

        }
       
        static bool Auth(string GroupID) 
        {
            try
            {
                vkapi.Authorize(new ApiAuthParams { AccessToken = GroupID });
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        static void Watcher_NewMessages(VkApi owner, ReadOnlyCollection<Message> messages)
        {
            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i].Type != MessageType.Sended)
                {
                    User Sender = vkapi.Users.Get(messages[i].UserId.Value);
                    Console.WriteLine("Новое сообщение: {0} {1}: {2}", Sender.FirstName, Sender.LastName, messages[i].Body);
                    userID = messages[i].UserId.Value;
                    Console.Beep();
                    //Command(messages[i].Body);
                }
            }
        }
        static void Eye()
        {
            LongPollServerResponse Pool = vkapi.Messages.GetLongPollServer(true);
            StartAsync(Pool.Ts, Pool.Pts);
            NewMessages += Watcher_NewMessages;
        }
        static async void StartAsync(ulong? lastTs = null, ulong? lastPts = null)
        {
            if (IsActive) ColorMessage("Messages for {0} already watching", ConsoleColor.Red);
            IsActive = true;
            await GetLongPoolServerAsync(lastPts);
            WatchTimer = new Timer(new TimerCallback(WatchAsync), null, 0, Timeout.Infinite);
        }
        static LongPollHistoryResponse GetLongPoolHistory()
        {
            if (!Ts.HasValue) GetLongPoolServer(null);
            MessagesGetLongPollHistoryParams rp = new MessagesGetLongPollHistoryParams();
            rp.Ts = Ts.Value;
            rp.Pts = Pts;
            int i = 0;
            LongPollHistoryResponse history = null;
            string errorLog = "";

            while (i < 5 && history == null)
            {
                i++;
                try
                {
                    history = vkapi.Messages.GetLongPollHistory(rp);
                }
                catch (TooManyRequestsException)
                {
                    Thread.Sleep(150);
                    i--;
                }
                catch (Exception ex)
                {
                    errorLog += string.Format("{0} - {1}{2}", i, ex.Message, Environment.NewLine);
                }
            }

            if (history != null)
            {
                Pts = history.NewPts;
                foreach (var m in history.Messages)
                {
                    m.FromId = m.Type == MessageType.Sended ? vkapi.UserId : m.UserId;
                }
            }
            else ColorMessage(errorLog, ConsoleColor.Red);
            return history;
        }
        static Task<LongPollHistoryResponse> GetLongPoolHistoryAsync()
        {
            return Task.Run(() => { return GetLongPoolHistory(); });
        }
        static async void WatchAsync(object state)
        {
            LongPollHistoryResponse history = await GetLongPoolHistoryAsync();
            if (history.Messages.Count > 0)
            {
                CurrentSleepSteps = 1;
                NewMessages?.Invoke(vkapi, history.Messages);
            }
            else if (CurrentSleepSteps < MaxSleepSteps) CurrentSleepSteps++;
            WatchTimer.Change(CurrentSleepSteps * StepSleepTime, Timeout.Infinite);
        }
        static void ColorMessage(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        static Task<LongPollServerResponse> GetLongPoolServerAsync(ulong? lastPts = null)
        {
            return Task.Run(() =>
            {
                return GetLongPoolServer(lastPts);
            });
        }
        static LongPollServerResponse GetLongPoolServer(ulong? lastPts = null)
        {
            LongPollServerResponse response = vkapi.Messages.GetLongPollServer(false, lastPts == null);
            Ts = response.Ts;
            Pts = Pts == null ? response.Pts : lastPts;
            return response;
        }
    }
}
