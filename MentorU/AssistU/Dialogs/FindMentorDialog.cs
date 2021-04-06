using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;


namespace CoreBot.Dialogs
{
    public class FindMentorDialog : CancelAndHelpDialog
    {
        private const string FieldMsgText = "What type career would you like your mentor to have?";
        private const string SkillsMsgText = "What would you like your mentor to be skilled at?";

        public FindMentorDialog() : base(nameof(FindMentorDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                FieldStepAsync,
                SkillsStepAsync,
                PresentMentor,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        /// <summary>
        /// First step to get the desired field
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> FieldStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var mentorDesire = (MentorDesire)stepContext.Options;
            if (mentorDesire.Field == null)
            {
                var promptMessage = MessageFactory.Text(FieldMsgText, FieldMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }
            return await stepContext.NextAsync(mentorDesire.Field, cancellationToken);
        }

        /// <summary>
        /// Second step to get the desired skills, currently only gets one skill.
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> SkillsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var mentorDesire = (MentorDesire)stepContext.Options;
            mentorDesire.Field = (string)stepContext.Result;

            if (mentorDesire.Skills == null)
            {
                var promptMessage = MessageFactory.Text(SkillsMsgText, SkillsMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }
            return await stepContext.NextAsync(mentorDesire.Skills, cancellationToken);
        }


        /// <summary>
        /// Closing dialog to confirm the request and pull from the DB
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> PresentMentor(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var mentorDesire = (MentorDesire)stepContext.Options;
            mentorDesire.Skills = (string)stepContext.Result;
            //string dbMsg = $"<QUERY> {mentorDesire.Field} {mentorDesire.Skills}";
            string finalMsg = $"You want {mentorDesire.Field}, and {mentorDesire.Skills}. \n I would recommend Steve as your mentor";
            var promptMessage = MessageFactory.Text(finalMsg, finalMsg, InputHints.IgnoringInput);
            await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
