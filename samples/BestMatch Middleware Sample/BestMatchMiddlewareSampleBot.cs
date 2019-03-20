


using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.Logging;

namespace BestMatchMiddleware_Sample
{
    public class BestMatchMiddlewareSampleBot : IBot
    {
        private readonly ILogger logger;
        private ConversationState _conversationState;
        private DialogSet Dialogs { get; set; }

        public BestMatchMiddlewareSampleBot(ILoggerFactory loggerFactory, ConversationState conversationState)
        {
            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }

            _conversationState = conversationState;

            Dialogs = new DialogSet(_conversationState.CreateProperty<DialogState>(nameof(BestMatchMiddlewareSampleBot)));
            Dialogs.Add(new PromptDialog());
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dc = await Dialogs.CreateContextAsync(turnContext); 

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    var dialogResult = await dc.ContinueDialogAsync();

                    if (!dc.Context.Responded)
                    {
                        switch (dialogResult.Status)
                        {
                            case DialogTurnStatus.Empty:
                                await dc.BeginDialogAsync(nameof(PromptDialog));
                                break;

                            case DialogTurnStatus.Waiting:
                                break;

                            case DialogTurnStatus.Complete:
                                await dc.EndDialogAsync();
                                break;

                            default:
                                await dc.CancelAllDialogsAsync();
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
