using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InmetaBotConcept
{
    [Serializable]
    public class Customer
    {
        public Customer(string pn, string n, string a, string lo)
        {
            PhoneNumber = pn;
            Name = n;
            Address = a;
            LastOrder = lo;
        }

        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string LastOrder { get; set; }

    }
}