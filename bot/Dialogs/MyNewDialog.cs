using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class MyNewDialog : CancelAndHelpDialog
    {
        public MyNewDialog()
            : base(nameof(MyNewDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                DataStepAsync,
                sampleStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

         private async Task<DialogTurnResult> DataStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var msg = "please tell me your name";
            var msgs = MessageFactory.Text(msg, msg, InputHints.IgnoringInput);
            var promptMessage = MessageFactory.Text(msg, msg, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

         private async Task<DialogTurnResult> sampleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = stepContext.Result;
            var msg = $"thats great end of the dialog yourname is {result}";
            var msgs = MessageFactory.Text(msg, msg, InputHints.IgnoringInput);
            await stepContext.Context.SendActivityAsync(msgs, cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}