using System;
using Microsoft.Bot.Connector.DirectLine;
using System.Threading.Tasks;
using System.Collections.Generic;
using MentorU.Models;


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
        private string _userName { get; set; }

        public BotService()
        {
            Client = new DirectLineClient(_directLineSecret); 
            _started = false;
        }

        /// <summary>
        /// Initialize the converstaion and start the registration
        /// to prompt the bot to send the welcome message
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task SetUpAsync(string userName)
        {
            if (!_started)
            {
                _userName = userName;
                BotConversation = await Client.Conversations.StartConversationAsync()
                    .ConfigureAwait(false);
                _started = true;
                await RegisterAsync();
            }
        }

        /// <summary>
        /// Received activites from the bot
        /// </summary>
        /// <returns></returns>
        public async Task ReceiveMessageAsync()
        {
            var response = await Client.Conversations.GetActivitiesAsync(
                BotConversation.ConversationId, WaterMark).ConfigureAwait(false);
            
            WaterMark = response.Watermark;
            var activities = new List<Activity>();
            foreach(var act in response.Activities)
            {
                if (act.From.Id == BOT_HANDLE)
                {
                    activities.Add(act);
                }
            }
            BotMessageReceived?.Invoke(CreateBotMessages(activities));
        }

        /// <summary>
        /// Format the message to allow extraction of content
        /// </summary>
        /// <param name="activities"></param>
        /// <returns></returns>
        private List<BotMessage> CreateBotMessages(IEnumerable<Activity> activities)
        {
            var botMessages = new List<BotMessage>();
            foreach(var act in activities)
            {
                botMessages.Add(new BotMessage() { ActivityId = act.Id, Content = act.Text, ISent = false });
            }
            return botMessages;
        }


        /// <summary>
        /// Build an activity with the specified message to send to the bot
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(string message)
        {
            var userMessage = new Activity()
            {
                From = new ChannelAccount(_userName),
                Text = message,
                Type = ActivityTypes.Message
            };
            await Client.Conversations.PostActivityAsync(BotConversation.ConversationId, userMessage)
                .ConfigureAwait(false);

            await ReceiveMessageAsync();
        }


        /// <summary>
        /// Notify the bot that it should start the conversation.
        /// </summary>
        /// <returns></returns>
        public async Task RegisterAsync()
        {
            var userMessage = new Activity()
            {
                From = new ChannelAccount(_userName),
                Type = ActivityTypes.ConversationUpdate
            };
            await Client.Conversations.PostActivityAsync(BotConversation.ConversationId, userMessage)
                .ConfigureAwait(false);
            await ReceiveMessageAsync();
        }
    }
}
