using System.Collections.Generic;
using System.Windows.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class township : DBobject
    {
        public string Name { get; set; }
        public bool Restricted { get; set; }
        public bool AppendixA { get; set; }

        public township()
        {
            Name = "";
            Restricted = AppendixA = false;
        }

        public township(int id, string name, bool restricted, bool appendixa)
        {
            ID = id;
            Name = name;
            Restricted = restricted;
            AppendixA = appendixa;
        }

        public override string ToString()
        { return Name; }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            Name = (dr[1] != DBNull.Value) ? dr.GetString(1) : "";
            Restricted = (dr[2] != DBNull.Value) ? dr.GetBoolean(2) : false;
            AppendixA = (dr[3] != DBNull.Value) ? dr.GetBoolean(3) : false;
        }

        public override T CopyItem<T>(T item)
        {
            township x = item as township;
            township y = new township(x.ID, x.Name, x.Restricted, x.AppendixA);
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            township y = new township(0, "", false, false);
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}
