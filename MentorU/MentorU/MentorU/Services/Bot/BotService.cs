using System;
using Microsoft.Bot.Connector.DirectLine;
using System.Threading.Tasks;
using System.Collections.Generic;
using MentorU.Models;
using System.Threading;


namespace MentorU.Services.Bot
{
    public class BotService
    {
        public static DirectLineClient Client { get; set; }
        public static Conversation BotConversation { get; set; }
        public const string BOT_HANDLE = "AssistU";
        public event Action<List<BotMessage>> BotMessageReceived;

        private static string WaterMark { get; set; }
        private const string _directLineSecret = "20LzcCzdXBg.IrynKUwXT74ePL49AUQhCjtdS4IH9XX1XJmvGjJaYjc";
        private static bool _started { get; set; }

        public BotService()
        {
            Client = new DirectLineClient(_directLineSecret); 
            _started = false;
        }

        public async Task SetUpAsync()
        {
            if (!_started)
            {
                BotConversation = await Client.Conversations.StartConversationAsync()
                    .ConfigureAwait(false);
                _started = true;
            }
        }


        public async Task ReceiveMessageAsync()
        {
            var response = await Client.Conversations.GetActivitiesAsync(
                BotConversation.ConversationId, WaterMark).ConfigureAwait(false);
            
            WaterMark = response.Watermark;
            var activities = new List<Activity>();
            foreach(var act in response.Activities)
            {
                if (act.From.Id == BOT_HANDLE)
                    activities.Add(act);
            }
            BotMessageReceived?.Invoke(CreateBotMessages(activities));
        }


        private List<BotMessage> CreateBotMessages(IEnumerable<Activity> activities)
        {
            var botMessages = new List<BotMessage>();
            foreach(var act in activities)
            {
                botMessages.Add(new BotMessage() { ActivityId = act.Id, Content = act.Text, ISent = false });
            }
            return botMessages;
        }


        public async Task SendMessageAsync(string message, string userName="")
        {
            var userMessage = new Activity()
            {
                From = new ChannelAccount(userName),
                Text = message,
                Type = ActivityTypes.Message
            };
            await Client.Conversations.PostActivityAsync(BotConversation.ConversationId, userMessage)
                .ConfigureAwait(false);

            await ReceiveMessageAsync();
        }

    }
}
