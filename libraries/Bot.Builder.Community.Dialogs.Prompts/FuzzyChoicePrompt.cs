using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bot.Builder.Community.Recognizers.Fuzzy;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Bot.Builder.Community.Dialogs.Prompts
{
    public class FuzzyChoicePrompt : ChoicePrompt
    {
        private double _threshold;

        public FuzzyChoicePrompt(string dialogId, PromptValidator<FoundChoice> validator = null, string defaultLocale = null, double threshold = 0.6D) : base(dialogId, validator, defaultLocale)
        {
            _threshold = threshold;
        }

        protected override async Task<PromptRecognizerResult<FoundChoice>> OnRecognizeAsync(ITurnContext turnContext, IDictionary<string, object> state, PromptOptions options,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var baseResult = await base.OnRecognizeAsync(turnContext, state, options, cancellationToken);

            if (baseResult.Succeeded)
            {
                return baseResult;
            }

            var fuzzyRecognizer = new FuzzyRecognizer(new FuzzyRecognizerOptions(_threshold));
            var fuzzyResult = await fuzzyRecognizer.Recognize(options.Choices.Select(c => c.Value), turnContext.Activity.Text);

            var foundChoice = new PromptRecognizerResult<FoundChoice>();

            if (fuzzyResult.Matches != null && fuzzyResult.Matches.Any())
            {
                var topMatch = options.Choices.First(c => c.Value == fuzzyResult.Matches.First().Choice);

                foundChoice = new PromptRecognizerResult<FoundChoice>
                {
                    Value = new FoundChoice() {Value = topMatch.Value}, Succeeded = true
                };
            }

            return foundChoice;
        }
    }
}
