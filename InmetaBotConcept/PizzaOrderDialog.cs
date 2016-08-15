using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Luis.Models;

using InmetaBotConcept.Properties;

namespace InmetaBotConcept
{
    [LuisModel("39d3d817-ef26-423a-8f01-2e0639acc3a8", "8e15977e64d64c4884260b3d70d442ae")]
    [Serializable]
    class PizzaOrderDialog : LuisDialog<PizzaOrder>
    {
        private readonly BuildFormDelegate<PizzaOrder> MakePizzaForm;

        internal PizzaOrderDialog(BuildFormDelegate<PizzaOrder> makePizzaForm)
        {
            this.MakePizzaForm = makePizzaForm;
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry. I didn't understand you.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Login")]
        public async Task ProcessLoginWithPhoneNumber(IDialogContext context, LuisResult result)
        {
            try
            {
                var entities = new List<EntityRecommendation>(result.Entities);
                var UserNumber = entities.FirstOrDefault(i => i.Type.Equals("PhoneNumber")).Entity;
                int number;
                var ValidUsers = new List<Customer>();

                string[] lines = Resources.Users.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (string line in lines)
                {
                    string[] vals = line.Split(new char[] { ';' });
                    ValidUsers.Add(new Customer(vals[0].ToString(), vals[1].ToString(), vals[2].ToString()));
                }

                if (int.TryParse(UserNumber, out number))
                {

                    var existsUser = ValidUsers.Exists(x => x.PhoneNumber.Equals(UserNumber));

                    if (existsUser)
                    {
                        var currentUser = ValidUsers.First(x => x.PhoneNumber.Equals(UserNumber));

                        entities.Add(new EntityRecommendation(type: "Name") { Entity = currentUser.Name });
                        entities.Add(new EntityRecommendation(type: "PhoneNumber") { Entity = currentUser.PhoneNumber });
                        entities.Add(new EntityRecommendation(type: "Address") { Entity = currentUser.Address });
                        await context.PostAsync(string.Format("Welcome back {0}!", ValidUsers.First(x => x.PhoneNumber.Equals(UserNumber)).Name));

                    }
                    else
                        await context.PostAsync("New user! You need to register first!");

                }
                else
                {
                    await context.PostAsync(string.Format("Your Phone Number is: {0}", result.Entities[0].Entity));
                }
            }
            catch (Exception exp)
            {
                await context.PostAsync("Ohhh noooo!!! I'm stuck here. " + exp.Message);
            }
        }

        [LuisIntent("Cancel")]
        public async Task ProcessCancel(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Order canceled!");
        }

        [LuisIntent("Menu")]
        public async Task ProcessMenuForm(IDialogContext context, LuisResult result)
        {
            var entities = new List<EntityRecommendation>(result.Entities);
            switch (entities.FirstOrDefault(i => i.Type.Equals("MenuAndHelp")).Entity)
            {
                case "hi": await context.PostAsync("Hello there! Ready for some pizza? Possible commands: "); ShowPossibleCommands(context); break;
                case "menu": await context.PostAsync("you can choose"); ShowMenu(context); break;
                case "help": await context.PostAsync("these are examples of commands you can use:"); ShowPossibleCommands(context); break;
                default: await context.PostAsync("Sorry, I'm having an hard time to understand you. This is what you can choose:"); break;
            }

            context.Wait(MessageReceived);
        }

        private async void ShowMenu(IDialogContext context)
        {
            await context.PostAsync("1. Pepperoni pizza 2. Cheese pizza 3. chicken pizza");
        }

        private async void ShowPossibleCommands(IDialogContext context)
        {
            await context.PostAsync("Example 1: menu Example 2: large pepperoni pizza Example 3: medium cheese with garlic dressing and coke");
        }

        [LuisIntent("Order")]
        public async Task ProcessPizzaForm(IDialogContext context, LuisResult result)
        {
            var entities = new List<EntityRecommendation>(result.Entities);
            if (entities.Exists(e => e.Type.Equals("PizzaName")))
            {
                var PizzaName = entities.FirstOrDefault(e => e.Type.Equals("PizzaName")).Entity;
                entities.Add(new EntityRecommendation(type: "PizzaName") { Entity = PizzaName });
            }
            if (entities.Exists(e => e.Type.Equals("PizzaSize")))
            {
                string PizzaSize = entities.FirstOrDefault(e => e.Type.Equals("PizzaSize")).Entity;
                entities.Add(new EntityRecommendation(type: "Size") { Entity = PizzaSize });
            }

            if (entities.Exists(e => e.Type.Equals("PizzaDressing")))
            {
                string PizzaSize = entities.FirstOrDefault(e => e.Type.Equals("PizzaDressing")).Entity;
                entities.Add(new EntityRecommendation(type: "Dressing") { Entity = PizzaSize });
            }

            if (entities.Exists(e => e.Type.Equals("DrinkOptions")))
            {
                string Drinks = entities.FirstOrDefault(e => e.Type.Equals("DrinkOptions")).Entity;
                entities.Add(new EntityRecommendation(type: "Drinks") { Entity = Drinks });
            }

            //string PizzaDressing = entities.FirstOrDefault(e => e.Type.Equals("PizzaDressing")).Entity;
            //string DrinkOptions = entities.FirstOrDefault(e => e.Type.Equals("DrinkOptions")).Entity;



            // Infer kind
            //foreach (var entity in result.Entities)
            //{
            //    string kind = null;
            //    switch (entity.Type)
            //    {
            //        case "Signature": kind = "Signature"; break;
            //        //case "GourmetDelite": kind = "Gourmet delite"; break;
            //        //case "Stuffed": kind = "stuffed"; break;
            //        default:
            //            if (entity.Type.StartsWith("BYO")) kind = "byo";
            //            break;
            //    }
            //    if (kind != null)
            //    {
            //        entities.Add(new EntityRecommendation(type: "Kind") { Entity = kind });
            //        break;
            //    }
            //}

            var pizzaForm = new FormDialog<PizzaOrder>(new PizzaOrder(), this.MakePizzaForm, FormOptions.PromptInStart, entities);
            context.Call<PizzaOrder>(pizzaForm, PizzaFormComplete);
        }

        private async Task PizzaFormComplete(IDialogContext context, IAwaitable<PizzaOrder> result)
        {
            PizzaOrder order = null;
            try
            {
                order = await result;
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("You canceled the form!");
                return;
            }

            if (order != null)
            {
                await context.PostAsync("Your Pizza Order: " + order.ToString());
            }
            else
            {
                await context.PostAsync("Form returned empty response!");
            }

            context.Wait(MessageReceived);
        }


    }

    public class Customer
    {
        public Customer(string pn, string n, string a)
        {
            PhoneNumber = pn;
            Name = n;
            Address = a;
        }

        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

    }
}