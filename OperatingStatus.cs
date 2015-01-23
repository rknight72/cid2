using System.Collections.Generic;
using System.Windows.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class OperatingStatus : DBobject
    {
        public string Status { get; set; }

        public OperatingStatus()
        { Status = ""; }

        public OperatingStatus(int id, string status)
        {
            ID = id;
            Status = status;
        }

        public override string ToString()
        { return Status; }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            Status = (dr[1] != DBNull.Value) ? dr.GetString(1) : "";
        }

        public override T CopyItem<T>(T item)
        {
            //Change item to the correct type of object then create a copy of it
            OperatingStatus x = item as OperatingStatus;
            OperatingStatus y = new OperatingStatus(x.ID, x.Status);

            //This is necessary because the function doesn't know in advance that T is a permittingclassification
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            //create an object with no values
            OperatingStatus y = new OperatingStatus(0, "");

            //This is necessary because the function doesn't know in advance that T is a permittingclassification
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}
