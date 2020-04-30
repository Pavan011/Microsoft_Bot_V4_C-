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
                DataStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

         private async Task<DialogTurnResult> DataStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var msg = "you are in new dialog";
            var msgs = MessageFactory.Text(msg, msg, InputHints.IgnoringInput);
            return await stepContext.EndDialogAsync(msgs, cancellationToken);
        }

    }
}