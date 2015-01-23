using System.Collections.Generic;
using System.Windows.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class casestatus : DBobject
    {
        public string Status { get; set; }
        public bool Active { get; set; }

        public casestatus()
        {
            ID = 0;
            Status = "";
            Active = true;
        }

        public casestatus(int id, string status, bool active)
        {
            ID = id;
            Status = status;
            Active = active;
        }

        public override string ToString()
        { return Status; }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            Status = (dr[1] != DBNull.Value) ? dr.GetString(1) : "";
            Active = true;
        }

        public override T CopyItem<T>(T item)
        {
            casestatus x = item as casestatus;
            casestatus y = new casestatus(x.ID, x.Status, x.Active);
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            casestatus y = new casestatus(0, "", true);
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}
