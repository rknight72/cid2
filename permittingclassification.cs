using System.Collections.Generic;
using System.Windows.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class permittingclassification : DBobject
    {
        public string Classification { get; set; }

        public permittingclassification()
        { Classification = ""; }

        public permittingclassification(int id, string status)
        {
            ID = id;
            Classification = status;
        }

        public override string ToString()
        { return Classification; }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            Classification = (dr[1] != DBNull.Value) ? dr.GetString(1) : "";
        }

        public override T CopyItem<T>(T item)
        {
            //Change item to the correct type of object then create a copy of it
            permittingclassification x = item as permittingclassification;
            permittingclassification y = new permittingclassification(x.ID, x.Classification);

            //This is necessary because the function doesn't know in advance that T is a permittingclassification
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            //create an object with no values
            permittingclassification y = new permittingclassification(9999, "");

            //This is necessary because the function doesn't know in advance that T is a permittingclassification
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}

