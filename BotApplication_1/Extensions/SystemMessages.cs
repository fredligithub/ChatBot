using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotApplication_1.Extensions
{
    public class SystemMessages
    {
        public async Task Handle(ConnectorClient connector, Activity activity)
        {
            switch(activity.Type)
            {
                case ActivityTypes.ContactRelationUpdate:
                    HandleContactRelation(activity);
                    break;
                case ActivityTypes.ConversationUpdate:
                    await HandleConversationUpdateAsync(connector, activity);
                    break;
                case ActivityTypes.Ping:
                    HandlePing(activity);
                    break;
                case ActivityTypes.Typing:
                    HandleTyping(activity);
                    break;
                default:
                    break;
            }
        }

        private void HandleTyping(Activity activity)
        {
            //user started typing
        }

        void HandlePing(IActivity activity)
        {
            
        }

        /// <summary>
        /// User(s) join or left a conversation
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        async Task HandleConversationUpdateAsync(ConnectorClient connector, IConversationUpdateActivity activity)
        {
            const string welcomeMessage =
                    "Welcome to the Rock, Paper, Scissors game! " +
                    "To begin, type \"rock\", \"paper\", or \"scissors\". ";

            Func<ChannelAccount, bool> isChatbot = channelAcct => channelAcct.Id == activity.Recipient.Id;

            //It works only when user join conversation
            if(activity.MembersAdded.Any(isChatbot))
            {
                Activity reply = (activity as Activity).CreateReply(welcomeMessage);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }

            //It works only when user left conversation
            if(activity.MembersRemoved.Any(isChatbot))
            {
                //To be determined.
            }
        }

        void HandleContactRelation(IContactRelationUpdateActivity activity)
        {
            if(activity.Action == "add")
            {
                //user added chatbot to contact list
            }
            else
            {
                //user removed chatbot from contact list
            }
        }
    }
}