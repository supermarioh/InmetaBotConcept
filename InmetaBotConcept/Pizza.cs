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
        [Terms(new string[] { "KVESS", "kvess", "1" })]
        KVESS = 1,
        [Terms(new string[] { "DRØMMEN", "2", "drommen" })]
        DRØMMEN = 2,
        [Terms(new string[] { "3", "PIZZABAKEREN", "pizzabakeren" })]
        PIZZABAKEREN = 3,
        [Terms(new string[] { "4", "SNADDER", "snadder" })]
        SNADDER = 4,
        [Terms(new string[] { "5", "MIX", "mix" })]
        MIX = 5,
        [Terms(new string[] { "6", "MEKSIKANEREN","meksikaneren" })]
        MEKSIKANEREN = 6,
        [Terms(new string[] { "7", "BIFFEN", "biffen" })]
        BIFFEN = 7
    };

    public enum DrinkOptions
    {

        [Terms(new string[] { "coke", "cocacola", "cola" })]
        CocaCola = 1,
        [Terms(new string[] { "icetea", "tea" })]
        IceTea = 2,
        [Terms(new string[] { "none", "no" })]
        None 
    };

    public enum PizzaDressing
    {
        Garlic =1,
        Aioli,
        Chili,
        [Terms(new string[] { "none", "no" })]
        None
    };

    [Serializable]
    class PizzaOrder
    {

        [Prompt("What kind of pizza do you want? {||}")]
        [Template(TemplateUsage.NotUnderstood, "What does \"{0}\" mean???")]
        [Describe("Kind of pizza")]
        public PizzaOptions PizzaName;
        public SizeOptions Size;
        public PizzaDressing Dressing;
        public DrinkOptions Drinks;
        //Customer information
        public string PhoneNumber;
        public string Name;  
        public string Address;

        //[Optional]
        //public CouponOptions Coupon;

        public override string ToString()
        {
            var builder = new StringBuilder();
            //builder.AppendFormat("We will delivered a {0} {1} pizza with {2} and {3} at {4} in 25 minutes! In case of emergency we will call {5}. Thanks and bye bye.", Size, PizzaName, Dressing, Drinks, Address, PhoneNumber);
            builder.AppendFormat("We will delivered a {0} {1} pizza with {2} dressing and {3} to drink at {4} in 25 minutes to you {5}! In case we need to reach you, we will call {6}. We hope our pizza will meet your expectations. Thank you, and hope to be of service to you soon again.", Size, PizzaName, Dressing, Drinks, Address, Name, PhoneNumber);
            return builder.ToString();
        }
    };
}