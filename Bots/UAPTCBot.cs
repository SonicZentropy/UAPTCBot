// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio UAPTCBot v4.9.2

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using UAPTCBot.Data;

namespace UAPTCBot.Bots
{
    public class CovidSymptomChoices
    {
        public const string TEMPERATURE = "Temperature of 100F or higher";
        public const string COUGH = "Cough";
        public const string BREATHING = "Hard Time Breathing";
        public const string SORE_THROAT = "Sore Throat";
        public const string BODY_ACHES = "Body Aches";
        public const string TASTE_SMELL_LOSS = "Loss of taste or smell.";
        public const string NONE = "None!";
    }

    public class UAPTCBot : ActivityHandler
    {
        private readonly UAPTCBotAccessors _accessors;
        private DialogSet _dialogs;

        private const string CovidDialogID = "covid";
        private const string CovidDetailsID = "covid_details";

        public UAPTCBot(UAPTCBotAccessors accessors)
        {
            // Set the _accessors 
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));

            // The DialogSet needs a DialogState accessor, it will call it when it has a turn context.
            _dialogs = new DialogSet(accessors.ConversationDialogState);

            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                NameStepAsync,
                NameConfirmStepAsync,
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            _dialogs.Add(new WaterfallDialog(CovidDetailsID, waterfallSteps));
            _dialogs.Add(new ChoicePrompt(CovidDialogID));
        }

        public override async Task OnTurnAsync(ITurnContext turnContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Get the conversation state from the turn context.
                //var profile_state = await _accessors.UserProfileAccessor.GetAsync(turnContext, () => new UserProfile());
                //var user_state = _accessors.UserState;

                // Get the state properties from the turn context.
                UserProfile userProfile =
                    await _accessors.UserProfileAccessor.GetAsync(turnContext, () => new UserProfile());
                ConversationData conversationData =
                    await _accessors.ConversationDataAccessor.GetAsync(turnContext, () => new ConversationData());


                if (!conversationData.SaidHello)
                {
                    // Handle the Greeting
                    string strMessage =
                        $"Hello!  Would you like to self-report 'covid' related issues, or ask something else? {System.Environment.NewLine}";

                    await turnContext.SendActivityAsync(strMessage);

                    // Set SaidHello
                    conversationData.SaidHello = true;
                }
                else
                {
                    // Get the user state from the turn context.

                    var humantxt = turnContext.Activity.Text.ToLower();

                    if (humantxt.Contains("covid") || conversationData.IsInCovidDialog)
                    {
                        if (userProfile.LastReportedSymptom == null ||
                            userProfile.LastReportedSymptom.Equals(CovidSymptomChoices.NONE))
                        {
                            // Run the DialogSet - let the framework identify the current state of the dialog from
                            // the dialog stack and figure out what (if any) is the active dialog.
                            var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                            var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                            // If the DialogTurnStatus is Empty we should start a new dialog.
                            if (results.Status == DialogTurnStatus.Empty)
                            {
                                conversationData.IsInCovidDialog = true;
                                await dialogContext.BeginDialogAsync(CovidDetailsID, null, cancellationToken);
                            }
                        }
                        else
                        {
                            // Echo back to the user whatever they typed.
                            var responseMessage =
                                $"You have already self-reported Covid status!  Can we help you with anything else?'\n";
                            await turnContext.SendActivityAsync(responseMessage);
                        }
                    }


                    if (!conversationData.IsInCovidDialog)
                    {
                        var replyText = "What else can I help you with?";
                        if (humantxt.Contains("bot"))
                        {
                            replyText = "I am absolutely a configurable bot.";
                        }
                        else if (humantxt.Contains("reset"))
                            replyText = "To reset your password, please visit https://uaptc.edu/reset";
                        else if (humantxt.Contains("blackboard"))
                            replyText = "To log directly into Blackboard, please visit https://elearning.uaptc.edu/";
                        else if (humantxt.Contains("advising"))
                            replyText =
                                "To contact your student advisors, please call  (501) 812-2220 or email advising@uaptc.edu";

                        await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText),
                            cancellationToken);
                    }
                }

                // Save the new turn count into the conversation state.
                await _accessors.ConversationDataAccessor.SetAsync(turnContext, conversationData);
                await _accessors.ConversationState.SaveChangesAsync(turnContext);

                // Save the user profile updates into the user state.
                await _accessors.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
            }

            await base.OnTurnAsync(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome to UAPTC DrBot!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText),
                        cancellationToken);
                }
            }
        }

        private static async Task<DialogTurnResult> NameStepAsync(
            WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            return await stepContext.PromptAsync(CovidDialogID,
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Do you have ANY of the following symptoms?"),
                    Choices = ChoiceFactory.ToChoices(new List<string>
                    {
                        CovidSymptomChoices.TEMPERATURE,
                        CovidSymptomChoices.COUGH,
                        CovidSymptomChoices.BREATHING,
                        CovidSymptomChoices.SORE_THROAT,
                        CovidSymptomChoices.BODY_ACHES,
                        CovidSymptomChoices.TASTE_SMELL_LOSS,
                        CovidSymptomChoices.NONE
                    }),
                    Style = ListStyle.HeroCard,
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> NameConfirmStepAsync(
            WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choicestr = ((FoundChoice) stepContext.Result).Value;
            UserProfile userProfile =
                await _accessors.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            // We can send messages to the user at any point in the WaterfallStep.
            if (choicestr.Equals(CovidSymptomChoices.NONE))
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text($"You're cleared for work!"), cancellationToken);
                userProfile.LastReportedSymptom = CovidSymptomChoices.NONE;
            }
            else
            {
                switch (choicestr)
                {
                    case CovidSymptomChoices.BODY_ACHES:
                        userProfile.LastReportedSymptom = CovidSymptomChoices.BODY_ACHES;
                        break;
                    case CovidSymptomChoices.BREATHING:
                        userProfile.LastReportedSymptom = CovidSymptomChoices.BREATHING;
                        break;
                    case CovidSymptomChoices.TEMPERATURE:
                        userProfile.LastReportedSymptom = CovidSymptomChoices.TEMPERATURE;
                        break;
                    case CovidSymptomChoices.COUGH:
                        userProfile.LastReportedSymptom = CovidSymptomChoices.COUGH;
                        break;
                    case CovidSymptomChoices.SORE_THROAT:
                        userProfile.LastReportedSymptom = CovidSymptomChoices.SORE_THROAT;
                        break;
                    case CovidSymptomChoices.TASTE_SMELL_LOSS:
                        userProfile.LastReportedSymptom = CovidSymptomChoices.TASTE_SMELL_LOSS;
                        break;
                }


                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text($"You self reported {choicestr}. PLEASE DO NOT COME TO WORK!  Call your supervisor to advise them of your illness."),
                    cancellationToken);
            }


            ConversationData conversationData =
                await _accessors.ConversationDataAccessor.GetAsync(stepContext.Context, () => new ConversationData());

            conversationData.IsInCovidDialog = false;

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, 
            // here it is the end.
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}