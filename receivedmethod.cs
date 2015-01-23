using System.Collections.Generic;
using System.Windows.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class receivedmethod : DBobject
    {
        public string Label { get; set; }

        public receivedmethod()
        {
            ID = 0;
            Label = "";
        }

        public receivedmethod(int id, string label)
        {
            ID = id;
            Label = label;
        }

        public override string ToString()
        { return Label; }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            Label = (dr[1] != DBNull.Value) ? dr.GetString(1) : "";
        }

        public override T CopyItem<T>(T item)
        {
            //Change item to the correct type of object then create a copy of it
            receivedmethod x = item as receivedmethod;
            receivedmethod y = new receivedmethod(x.ID, x.Label);

            //This is necessary because the function doesn't know in advance that T is a receivedmethod
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            //create an object with no values
            receivedmethod y = new receivedmethod(0, "");

            //This is necessary because the function doesn't know in advance that T is a receivedmethod
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}