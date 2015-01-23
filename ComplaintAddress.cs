using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CID2
{
    public class ComplaintAddress : Address
    {
        public int ID { get; set; }
        public county County { get; set; }
        public township Township { get; set; }
        public string Parcel { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        new public city City { get; set; }

        public ComplaintAddress()
        {
            ID = 0;
            County = new county();
            Township = new township();
            Parcel = "";
            Longitude = Latitude = 0;
            City = new city();
        }

        public ComplaintAddress(string addressline1, string addressline2, city city, state state, string zip, county county,
            township township, string parcel, double longitude, double latitude)
        {
            AddressLine1 = addressline1;
            AddressLine2 = addressline2;
            City = city;
            State = state;
            Zip = zip;
            County = county;
            Township = township;
            Parcel = parcel;
            Longitude = longitude;
            Latitude = latitude;
        }

        public virtual void UpdateContentFromSiteControl() { }
        public virtual void UpdateSiteControlContent() { }
        public virtual void LoadNewAddress(int id) { }

        public override void BlankAddress()
        {
            base.BlankAddress();

            ID = 0;
            County = new county();
            Township = new township();
            Parcel = "";
            Longitude = Latitude = 0;
            City = new city();            
        }
    }
}
