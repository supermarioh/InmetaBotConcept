using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Text;
#pragma warning disable 649

namespace InmetaBotConcept
{
    public enum SizeOptions
    {
        // 0 value in enums is reserved for unknown values.  Either you can supply an explicit one or start enumeration at 1.
        Unknown,
        [Terms(new string[] { "small" })]
        Small,
        [Terms(new string[] { "med", "medium" })]
        Medium,
        [Terms(new string[] { "big", "large" })]
        Large,
        [Terms(new string[] { "family", "extra large" })]
        Family
    };
    public enum PizzaOptions
    {
        [Terms(new string[] { "pepperoni", "peperoni" })]
        Pepperoni = 1,
        [Terms(new string[] { "cheese" })]
        Cheese,
        [Terms(new string[] { "chicken", "kylling" })]
        Chicken
    };

    public enum DrinkOptions
    {
        [Terms(new string[] { "coke", "cocacola", "cola" })]
        CocaCola = 1,
        [Terms(new string[] { "icetea", "tea" })]
        IceTea
    };

    public enum PizzaDressing
    {
        Garlic = 1,
        Aioli,
        Chili
    };


    [Serializable]
    class PizzaOrder
    {
        public string Name;
        public string PhoneNumber;
        public string Address;
        [Prompt("What kind of pizza do you want? {||}")]
        [Template(TemplateUsage.NotUnderstood, "What does \"{0}\" mean???")]
        [Describe("Kind of pizza")]
        public PizzaOptions PizzaName;
        public SizeOptions Size;
        public PizzaDressing Dressing;
        public DrinkOptions Drinks;
        

        //[Optional]
        //public CouponOptions Coupon;

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("We will delivered a {0} {1} pizza with {2} and {3} at {4} in 25 minutes! In case of emergency we will call {5}. Thanks and bye bye.", Size, PizzaName, Dressing, Drinks, Address, PhoneNumber);
            //switch (Kind)
            //{
            //    case PizzaOptions.BYOPizza:
            //        builder.AppendFormat("{0}, {1}, {2}, [", Kind, BYO.Crust, BYO.Sauce);
            //        foreach (var topping in BYO.Toppings)
            //        {
            //            builder.AppendFormat("{0} ", topping);
            //        }
            //        builder.AppendFormat("]");
            //        break;
            //    case PizzaOptions.GourmetDelitePizza:
            //        builder.AppendFormat("{0}, {1}", Kind, GourmetDelite);
            //        break;
            //    case PizzaOptions.SignaturePizza:
            //        builder.AppendFormat("{0}, {1}", Kind, Signature);
            //        break;
            //    case PizzaOptions.StuffedPizza:
            //        builder.AppendFormat("{0}, {1}", Kind, Stuffed);
            //        break;
            //}
            //builder.AppendFormat(", {0}, {1})", Address, PhoneNumber);
            return builder.ToString();
        }
    };
}