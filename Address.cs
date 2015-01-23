using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CID2
{
    public class Address
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public state State { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }

        public Complaint Owner { get; set; }

        public Address()
        {
            AddressLine1 = AddressLine2 = Zip = City = "";
            State = new state();
        }

        public Address(string addressline1, string addressline2, string city, state state, string zip)
        {
            AddressLine1 = addressline1;
            AddressLine2 = addressline2;
            if (city != null)
                City = city;
            State = state;
            Zip = zip;
        }

        public virtual List<Control> GetRequiredControls() { return new List<Control>(); }

        public void SetOwner(Complaint owner) { Owner = owner; }

        public virtual void BlankAddress()
        {
            AddressLine1 = AddressLine2 = Zip = City = "";
            State = new state();
        }
    }
}
