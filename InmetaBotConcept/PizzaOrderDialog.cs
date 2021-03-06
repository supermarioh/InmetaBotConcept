﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Luis.Models;

using InmetaBotConcept.Properties;
using System.Resources;

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

        [LuisIntent("ZlatanKing")]
        public async Task ProcessDiscount(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Congratulations! You have found the easter egg! Zlatan is the true King! You will get 20% discount!");
        }

        [LuisIntent("Login")]
        public async Task ProcessLoginWithPhoneNumber(IDialogContext context, LuisResult result)
        {
            var entities = new List<EntityRecommendation>(result.Entities);
            try
            {
                
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
                        await context.PostAsync(string.Format("Welcome back {0}! What are you going to have today?", currentUser.Name));

                    }
                    else {

                        entities.Add(new EntityRecommendation(type: "PhoneNumber") { Entity = UserNumber });
                        await context.PostAsync("Hello there! Looks like it's your first order with us. Please type 'menu' or 'help' to start your order.");
                    }
                }
                else
                {
                    await context.PostAsync(string.Format("Your Phone Number is: {0}", result.Entities[0].Entity));
                }
            }
            catch (Exception exp)
            {
                await context.PostAsync("Ohhh noooo!!! An error occurred. Please try again later." + exp.Message);
            }


            var pizzaForm = new FormDialog<PizzaOrder>(new PizzaOrder(), this.MakePizzaForm, FormOptions.PromptInStart, entities);
            context.Call<PizzaOrder>(pizzaForm, PizzaFormComplete);
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
                case "hi": await context.PostAsync("Hello there! Please enter your phone number or type 'menu'");break;
                case "hei": await context.PostAsync("Hello there! Please enter your phone number or type 'menu'"); break;
                case "hello": await context.PostAsync("Hello there! Please enter your phone number or type 'menu'"); break;
                case "menu": await context.PostAsync("Pizza Menu:"); ShowMenu(context); break;
                case "help": await context.PostAsync(""); ShowPossibleCommands(context); break;
                default: await context.PostAsync("Sorry, I'm having an hard time understanding you. This is what you can choose:"); ShowPossibleCommands(context); break;
            }

            context.Wait(MessageReceived);
        }

        private async void ShowMenu(IDialogContext context)
        {
            await context.PostAsync("1. KVESS  \n2. DRØMMEN  \n3. PIZZABAKEREN  \n4. SNADDER  \n5. MIX  \n6. MEKSIKANEREN  \n7. BIFFEN ");
        }

        private async void ShowPossibleCommands(IDialogContext context)
        {
            await context.PostAsync("Example 1: menu  \nExample 2: large KVESS pizza  \n3: medium DRØMMEN with garlic dressing and coke");

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
                //Save new customer 
                await context.PostAsync("Your order: " + order.ToString());
                //Save order to DB
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