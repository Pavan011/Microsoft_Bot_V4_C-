// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        // private readonly FlightBookingRecognizer _luisRecognizer;
        private readonly IBotServices _botServices;
        private readonly IBotQnA _botQnAService;
        protected readonly ILogger Logger;

        public MainDialog(IBotServices botServices,IBotQnA botQnA, MyNewDialog myNewDialog, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _botQnAService = botQnA;
            _botServices = botServices;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(myNewDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Use the text provided in FinalStepAsync or the default if it is the first time.
            var messageText = stepContext.Options?.ToString() ?? "Hi welcome to my sample bot";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }
        
        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Call LUIS and gather any potential booking details. (Note the TurnContext has the response to the prompt.)
            //var luisResult = await _luisRecognizer.RecognizeAsync(stepContext.Context, cancellationToken);
             var recognizerResult = await _botServices.Dispatch.RecognizeAsync(stepContext.Context, cancellationToken);
            var topIntent = recognizerResult.GetTopScoringIntent();
            
            switch(topIntent.intent)
            {
                case "com":
                var msg = "hi welcome to bot";
                var msgs = MessageFactory.Text(msg, msg, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(msgs, cancellationToken);
                break;

                case "qna":
                
                var options = new QnAMakerOptions { Top = 1 };
                var response = await _botQnAService.SampleQnA.GetAnswersAsync(stepContext.Context, options);
                if (response != null && response.Length > 0)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(response[0].Answer), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("No QnA Maker answers were found."), cancellationToken);
            }
            break;
                // var liscall = topIntent;
                // return await stepContext.BeginDialogAsync(nameof(MyNewDialog),liscall, cancellationToken);
                

                // case "":
                // await ProcessSampleQnAAsync(turnContext, cancellationToken);
                // break;
                

                default: 

                var didntUnderstandMessageText = $"Sorry, I didn't get that. Please try asking in a different way (intent was {topIntent})";
                var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                break;
            }
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Restart the main dialog with a different message the second time around
            var promptMessage = "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}
