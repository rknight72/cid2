using System.Collections.Generic;
using System.Windows.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class state : DBobject
    {
        public string Name { get; set; }
        public string Abbr { get; set; }

        public state()
        { Name = Abbr = ""; }

        public state(int id, string abbreviation, string name)
        {
            ID = id;
            Name = name;
            Abbr = abbreviation;
        }

        public override string ToString()
        { return Abbr; }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            Name = (dr[1] != DBNull.Value) ? dr.GetString(1) : "";
            Abbr = (dr[2] != DBNull.Value) ? dr.GetString(2) : "";
        }

        public override T CopyItem<T>(T item)
        {
            //Change item to the correct type of object then create a copy of it
            state x = item as state;
            state y = new state(x.ID, x.Name, x.Abbr);

            //This is necessary because the function doesn't know in advance that T is a state
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            //create an object with no values
            state y = new state(0, "", "");

            //This is necessary because the function doesn't know in advance that T is a state
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}
