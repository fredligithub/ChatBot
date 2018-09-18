using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotApplication_1.Game
{
    public class GameState
    {
        [Serializable]
        class PlaySore
        {
            public DateTime Date { get; set; } = DateTime.Now;
            public bool UserWin { get; set; }
        }

        public async Task<string> GetScoresAsync(Activity activity)
        {
            using (StateClient stateClient = activity.GetStateClient())
            {
                //Get user state data base on Channel Id and From Id. 
                IBotState chatbotState = stateClient.BotState;
                BotData chatbotData = await chatbotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

                //Get state data(property), it was set/created in function: UpdateScoreAsync() as below
                Queue<PlaySore> scoreQueue = chatbotData.GetProperty<Queue<PlaySore>>(property: "scores");

                if (scoreQueue == null)
                {
                    return "Try typing Rock, Paper, or Scissors to Play first.";
                }

                int plays = scoreQueue.Count;
                int userWins = scoreQueue.Where(q => q.UserWin).Count();
                int chatbotWins = scoreQueue.Where(q => !q.UserWin).Count();
                int ties = chatbotData.GetProperty<int>(property: "ties");

                return $"Out of the last {plays} contests, you scored {userWins} and chatbot scored {chatbotWins}. "
                    + $"You have also {ties} ties since playing.";
            }
        }

        public async Task UpdateScoreAsync(Activity activity, bool userWin)
        {
            using (StateClient stateClient = activity.GetStateClient())
            {
                IBotState chatbotState = stateClient.BotState;
                BotData chatbotData = await chatbotState.GetUserDataAsync(activity.Id, activity.From.Id);
                Queue<PlaySore> scoreQueue = chatbotData.GetProperty<Queue<PlaySore>>(property: "scores");

                if (scoreQueue == null)
                {
                    scoreQueue = new Queue<PlaySore>();
                }

                if (scoreQueue.Count >= 10)
                {
                    scoreQueue.Dequeue();
                }

                scoreQueue.Enqueue(new PlaySore { UserWin = userWin });

                chatbotData.SetProperty<Queue<PlaySore>>(property: "scores", data: scoreQueue);
                await chatbotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, chatbotData);
            }
        }

        public async Task<string> DeleteScoreAsync(Activity activity)
        {
            using (StateClient stateClient = activity.GetStateClient())
            {
                IBotState chatbotState = stateClient.BotState;
                await chatbotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);

                return "All Scores Deleted.";
            }
        }

        public async Task AddTieAsync(Activity activity)
        {
            using (StateClient stateClient = activity.GetStateClient())
            {
                IBotState chatbotState = stateClient.BotState;
                BotData chatbotData = await chatbotState.GetUserDataAsync(activity.Id, activity.From.Id);

                int ties = chatbotData.GetProperty<int>(property: "ties");

                chatbotData.SetProperty<int>(property: "ties", data: ++ties);

                await chatbotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, chatbotData);
            }
        }
    }
}