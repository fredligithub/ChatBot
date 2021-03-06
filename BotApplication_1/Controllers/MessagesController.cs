﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using BotApplication_1.Extensions;

namespace BotApplication_1
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            //New Bot Connector
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            if (activity.GetActivityType() == ActivityTypes.Message)
            {
                //Bot Template Build-in Function
                //await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());

                //Your Customization Logical Code
                string message = string.Empty;
                var state = new Game.GameState();

                if (activity.Text.ToLower().Contains("score"))
                {
                    message = await state.GetScoresAsync(activity);
                }
                else if (activity.Text.ToLower().Contains("delete"))
                {
                    message = await state.DeleteScoreAsync(activity);
                }
                else
                {
                    var game = new Game.RPSGame();
                    message = game.Play(activity.Text);

                    if(message.ToLower().Contains("tie"))
                    {
                        //Update "Tie" result
                        await state.AddTieAsync(activity);
                    }
                    else if(message.ToLower().Contains("type"))
                    {
                        //Invalid input, do nothing
                    }
                    else
                    {
                        //User win or Bot win
                        bool userWin = message.ToLower().Contains("win");
                        await state.UpdateScoreAsync(activity, userWin);
                    }
                }

                //Create Reply to Bot
                //Activity reply = activity.CreateReply(message, locale: "en-US");
                Activity reply = activity.BuildMessageActivity(message, locale: "en-US");
                await connector.Conversations.ReplyToActivityAsync(reply);           
            }
            else
            {
                //Bot Template Build-in Function
                //HandleSystemMessage(activity);

                //Customizaiton Message Handling
                await new SystemMessages().Handle(connector, activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            string messageType = message.GetActivityType();
            if (messageType == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (messageType == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (messageType == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
                if(message.Action == "add")
                {
                    //Send a notification mail
                }
                else if(message.Action == "remove")
                {
                    //Clean out any cached, or un-used data associated with that user

                    //Send a notification mail to Admin to review logs, learn why user remove bot and improve it.

                }
            }
            else if (messageType == ActivityTypes.Typing)
            {
                // Handle knowing that the user is typing
            }
            else if (messageType == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}