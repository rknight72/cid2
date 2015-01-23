using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;
using System.Data.OleDb;
using System;

namespace CID2
{
    class ZipCode : DBobject
    {
        public string Zip { get; set; }
        city DefaultCity { get; set; }

        public ZipCode()
        {
            Zip = "";
            DefaultCity = null;
        }

        public ZipCode(int id, string zip, city defaultcity)
        {
            ID = id;
            Zip = zip;
            DefaultCity = defaultcity;
        }

        public override string ToString()
        {
            return Zip;
        }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            Zip = dr.GetString(1);
            city c = null;
            if (dr[2] != DBNull.Value) MainWindow.GetSingleItem<city>(out c, dr.GetInt32(2), MainWindow.Cities);
            DefaultCity = c;
        }

        public override T CopyItem<T>(T item)
        {
            //Change item to the correct type of object then create a copy of it
            ZipCode x = item as ZipCode;
            ZipCode y = new ZipCode(x.ID, x.Zip, x.DefaultCity);//, x.CityIDList);
            
            //This is necessary because the function doesn't know in advance that T is a ZipCode
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            //create an object with no values
            ZipCode y = new ZipCode(0, "", null);

            //This is necessary because the function doesn't know in advance that T is a ZipCode
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}
