// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bot.Builder.Community.Dialogs.Prompts;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Microsoft.BotBuilderSamples
{
    public class PromptDialog : ComponentDialog
    {
        public PromptDialog()
            : base("PromptDialog")
        {
            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                PromptStepAsync,
                EchoResultStepAsync,
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new FuzzyChoicePrompt(nameof(ChoicePrompt), threshold: 0.2D));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> PromptStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please enter your choice."),
                    Choices = ChoiceFactory.ToChoices(new List<string>
                    {
                        "12 Station Road, Banks, Lancashire",
                        "5 St Pauls Square, Liverpool",
                        "5th Avenue, New York, New York",
                        "8th Avenue, New York"
                    }),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> EchoResultStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = (FoundChoice)stepContext.Result;

            await stepContext.Context.SendActivityAsync(result.Value);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}