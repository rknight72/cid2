using System.Collections.Generic;
using System.Windows.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class FireDepartment : DBobject
    {
        public string FDName { get; set; }

        public Contact FDinfo = new Contact();

        public FireDepartment()
        {
            FDName = FDinfo.FName = FDinfo.LName = FDinfo.Email = FDinfo.Phone = FDinfo.AddressLine1 = FDinfo.AddressLine2 = FDinfo.City = FDinfo.Zip = "";
        }

        public FireDepartment(int id, string firedeptname, string addressline1, string addressline2, string city, string zip, string phone, string email)
        {
            ID = id;
            FDName = firedeptname;
            FDinfo.FName = "";
            FDinfo.LName = "";
            FDinfo.AddressLine1 = addressline1;
            FDinfo.AddressLine2 = addressline2;
            FDinfo.City = city;
            FDinfo.Zip = zip;
            FDinfo.Email = email;
            FDinfo.Phone = phone;
            FDinfo.State = new state(41, "Ohio", "OH");
        }

        public override string ToString()
        {  return FDName; }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            FDName = (dr[1] != DBNull.Value) ? dr.GetString(1) : "";
            FDinfo.AddressLine1 = (dr[2] != DBNull.Value) ? dr.GetString(2) : "";
            FDinfo.AddressLine2 = (dr[3] != DBNull.Value) ? dr.GetString(3) : "";
            FDinfo.City = (dr[4] != DBNull.Value) ? dr.GetString(4) : "";
            FDinfo.Zip = (dr[5] != DBNull.Value) ? dr.GetString(5) : "";
            FDinfo.Phone = (dr[6] != DBNull.Value) ? dr.GetString(6) : "";
            FDinfo.FName = "";
            FDinfo.LName = "";
            FDinfo.Email = (dr[7] != DBNull.Value) ? dr.GetString(9) : "";
        }

        public override T CopyItem<T>(T item)
        {
            //Change item to the correct type of object then create a copy of it
            FireDepartment x = item as FireDepartment;
            FireDepartment y = new FireDepartment(x.ID, x.FDName, x.FDinfo.AddressLine1, x.FDinfo.AddressLine2,
                x.FDinfo.City, x.FDinfo.Zip, x.FDinfo.Phone, x.FDinfo.Email);

            //This is necessary because the function doesn't know in advance that T is a FireDepartment
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            //create an object with no values
            FireDepartment y = new FireDepartment(0, "", "", "", "", "", "", "");

            //This is necessary because the function doesn't know in advance that T is a FireDepartment
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}