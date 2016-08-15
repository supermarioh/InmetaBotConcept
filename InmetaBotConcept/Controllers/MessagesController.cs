using System.Web.Http;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Http.Description;
using System.Net.Http;
using System.Diagnostics;
using System;

namespace InmetaBotConcept
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private static IForm<PizzaOrder> BuildForm()
        {
            var builder = new FormBuilder<PizzaOrder>();

            //ActiveDelegate<PizzaOrder> isBYO = (pizza) => pizza.Kind == PizzaOptions.BYOPizza;
            //ActiveDelegate<PizzaOrder> isSignature = (pizza) => pizza.Kind == PizzaOptions.SignaturePizza;
            //ActiveDelegate<PizzaOrder> isGourmet = (pizza) => pizza.Kind == PizzaOptions.GourmetDelitePizza;
            
            return builder
                // .Field(nameof(PizzaOrder.Choice))
                .Field(nameof(PizzaOrder.Size))
                .Field(nameof(PizzaOrder.PizzaName))
                .Field(nameof(PizzaOrder.Dressing))
                .Field(nameof(PizzaOrder.Drinks))
                //.Field(nameof(PizzaOrder.Address))
                //.Field(nameof(PizzaOrder.GourmetDelite), isGourmet)
                //.Field(nameof(PizzaOrder.Signature), isSignature)
                //.Field(nameof(PizzaOrder.Stuffed), isStuffed)
                .AddRemainingFields()
                .Confirm("Would you like a {Size} {PizzaName} pizza? Please confirm with 'yes' or to change options 'no'")
                //.Confirm("Would you like a {Size}, {BYO.Crust} crust, {BYO.Sauce}, {BYO.Toppings} pizza?", isBYO)
                //.Confirm("Would you like a {Size}, {&Signature} {Signature} pizza?", isSignature, dependencies: new string[] { "Size", "Kind", "Signature" })
                //.Confirm("Would you like a {Size}, {&GourmetDelite} {GourmetDelite} pizza?", isGourmet)
                //.Confirm("Would you like a {Size}, {&Stuffed} {Stuffed} pizza?", isStuffed)
                .Build()
                ;
        }

        internal static IDialog<PizzaOrder> MakeRoot()
        {
            return Chain.From(() => new PizzaOrderDialog(BuildForm)).DefaultIfException<PizzaOrder>();
        }

        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            try
            {
                if (activity != null)
                {
                    // one of these will have an interface and process it
                    switch (activity.GetActivityType())
                    {
                        case ActivityTypes.Message:
                            await Conversation.SendAsync(activity, MakeRoot);
                            break;

                        case ActivityTypes.ConversationUpdate:
                        case ActivityTypes.ContactRelationUpdate:
                        case ActivityTypes.Typing:
                        case ActivityTypes.DeleteUserData:
                        default:
                            Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
                            break;
                    }
                }
            }
            catch (Exception exp)
            {

            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }
    }
}