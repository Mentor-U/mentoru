using System.Threading;
using System.Threading.Tasks;
using CoreBot.CognitiveModels;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;


namespace CoreBot.Dialogs
{
    public class FindMentorDialog : CancelAndHelpDialog
    {
        private const string FieldMsgText = "What type career would you like your mentor to have?";
        private const string SkillsMsgText = "What would you like your mentor to be skilled at?";
        private readonly EntityRecognizer _luisRecognizer;

        public FindMentorDialog(EntityRecognizer luisRecognizer) : base(nameof(FindMentorDialog))
        {
            _luisRecognizer = luisRecognizer;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                FieldStepAsync,
                SkillsStepAsync,
                StateMentorSearch,
                FinalStep,
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

            var luisResult = await _luisRecognizer.RecognizeAsync<MentorFinder>(stepContext.Context, cancellationToken);
            mentorDesire.Field = luisResult.ExtractField;

            if (string.IsNullOrEmpty(mentorDesire.Field) && !string.IsNullOrEmpty((string)stepContext.Result))
            {
                mentorDesire.Field = (string)stepContext.Result;
            }

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
        private async Task<DialogTurnResult> StateMentorSearch(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var mentorDesire = (MentorDesire)stepContext.Options;

            var luisResult = await _luisRecognizer.RecognizeAsync<MentorFinder>(stepContext.Context, cancellationToken);
            mentorDesire.Skills = luisResult.ExtractSkill;

            if (string.IsNullOrEmpty(mentorDesire.Skills) && !string.IsNullOrEmpty((string)stepContext.Result))
            {
                mentorDesire.Skills = (string)stepContext.Result;
            }

            string dbMsg = $"<QUERY>:{mentorDesire.Field}:{mentorDesire.Skills}#Alright, I will find a mentor working in {mentorDesire.Field} and skilled in {mentorDesire.Skills}";
            var promptMessage = MessageFactory.Text(dbMsg, dbMsg, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }


        /// <summary>
        /// Return to the main dialog
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> FinalStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
