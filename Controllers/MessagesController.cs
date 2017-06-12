﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using InsuranceBoT;
using InsuranceBOT;
using Autofac;
using Microsoft.Bot.Builder.Autofac.Base;
using Microsoft.Bot.Builder.Dialogs.Internals;

namespace InsuranceBOTDemo
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>

        internal static IDialog<LossForm> BuildInsuranceDialog()
        {
            return Chain.From(() => FormDialog.FromForm(LossForm.BuildForm));
        }

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            //supress the exception msg, "sorry my bot code is having issue"
            var builder = new ContainerBuilder();
            builder.RegisterAdapterChain<IPostToBot>
            (
            typeof(EventLoopDialogTask),
            typeof(SetAmbientThreadCulture),
            typeof(PersistentDialogTask),
            typeof(ExceptionTranslationDialogTask),
            typeof(SerializeByConversation),
            typeof(PostUnhandledExceptionToUser),
            typeof(LogPostToBot)
            ).InstancePerLifetimeScope();

            if (activity.Type == ActivityTypes.Message)
            {
                /// without LUIS 
                /// await Conversation.SendAsync(activity, BuildInsuranceDialog);
                /// 
                /// with LUIS
                await Conversation.SendAsync(activity, () => new BuildInsuranceLUISDialog());
                return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}