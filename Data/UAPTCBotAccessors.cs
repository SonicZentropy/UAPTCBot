using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace UAPTCBot.Data
{
    public class UAPTCBotAccessors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EchoBotAccessors"/> class.
        /// Contains the <see cref="ConversationState"/> and associated <see cref="IStatePropertyAccessor{T}"/>.
        /// </summary>
        /// <param name="conversationState">The state object that stores the counter.</param>
        /// /// <param name="userState">The state object that stores the user state.</param>
        public UAPTCBotAccessors(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }
        

        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }

        /// <summary>
        /// Gets the accessor name for the user profile property accessor.
        /// </summary>
        /// <value>The accessor name for the user profile property accessor.</value>
        /// <remarks>Accessors require a unique name.</remarks>
        public static string UserProfileName { get; } = "UserProfile";

        /// <summary>
        /// Gets the accessor name for the conversation data property accessor.
        /// </summary>
        /// <value>The accessor name for the conversation data property accessor.</value>
        /// <remarks>Accessors require a unique name.</remarks>
        public static string ConversationDataName { get; } = "ConversationData";

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for the user profile property.
        /// </summary>
        /// <value>
        /// The accessor for the user profile property.
        /// </value>
        public IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for the conversation data property.
        /// </summary>
        /// <value>
        /// The accessor for the conversation data property.
        /// </value>
        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }

        /// <summary>
        /// Gets the <see cref="ConversationState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="ConversationState"/> object.</value>
        public ConversationState ConversationState { get; }

        /// <summary>
        /// Gets the <see cref="UserState"/> object for the bot.
        /// </summary>
        /// <value>The <see cref="UserState"/> object.</value>
        public UserState UserState { get; }
    }
}
