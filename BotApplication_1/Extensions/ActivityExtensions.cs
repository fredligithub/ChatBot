using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotApplication_1.Extensions
{
    /// <summary>
    /// This class is used to build a custom message activity.
    /// Similar to the build-in activity.CreateReply function,
    /// But you can implement your custmization code here.
    /// </summary>
    public static class ActivityExtensions
    {
        //The userActivity parameter is the Activity that this message is being build to reply to.
        public static Activity BuildMessageActivity(this Activity userActivity, string message, string locale= "en-US")
        {
            IMessageActivity replyActivity = new Activity(ActivityTypes.Message)
            {
                From = new ChannelAccount
                {
                    Id = userActivity.Recipient.Id,
                    Name = userActivity.Recipient.Name
                },
                Recipient = new ChannelAccount
                {
                    Id = userActivity.From.Id,
                    Name = userActivity.From.Name
                },
                Conversation = new ConversationAccount
                {
                    Id = userActivity.Conversation.Id,
                    Name = userActivity.Conversation.Name,
                    IsGroup = userActivity.Conversation.IsGroup
                },

                ReplyToId = userActivity.Id,
                Text = message,
                Locale = locale
            };

            return (Activity)replyActivity;
        }
    }
}