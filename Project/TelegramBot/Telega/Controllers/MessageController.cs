using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Telegram.Bot.Types;
using TelegramBot;

namespace Telega.Controllers
{
    public class MessageController : ApiController
    {
        [Route(@"api/message/update")]
        public async Task<OkResult> Update([FromBody]Update update)
        {
            var commands = Bot.comandsList;
            var message = update.Message;
            var client = Bot.Get();
            foreach(var command in commands)
            {
                if (command.Contains(message.Text))
                {
                    command.Execute(message, client.Result);
                }

            }
            return Ok();
        }
    }
}
