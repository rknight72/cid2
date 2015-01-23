using System.Collections.Generic;
using System.Windows.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class county : DBobject
    {
        public string Name { get; set; }
        public state DefaultState { get; set; }
        public double NorthBound { get; set; }
        public double SouthBound { get; set; }
        public double EastBound { get; set; }
        public double WestBound { get; set; }

        public county()
        {
            Name = "";
            DefaultState = new state();
            NorthBound = SouthBound = EastBound = WestBound = 0;
        }

        public county(int id, string name, state defaultstate, double northbound, double southbound, double eastbound, double westbound)
        {
            ID = id;
            Name = name;
            DefaultState = defaultstate;
            NorthBound = northbound;
            SouthBound = southbound;
            EastBound = eastbound;
            WestBound = westbound;
        }

        public override string ToString()
        { return Name; }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            Name = dr.GetString(1);
            state thestate = new state();
            if (dr[2] != DBNull.Value) MainWindow.GetSingleItem<state>(out thestate, dr.GetInt32(2), MainWindow.States);
            DefaultState = thestate;
            NorthBound = (dr[3] != DBNull.Value) ? dr.GetDouble(3) : 0;
            SouthBound = (dr[4] != DBNull.Value) ? dr.GetDouble(4) : 0;
            EastBound = (dr[5] != DBNull.Value) ? dr.GetDouble(5) : 0;
            WestBound = (dr[6] != DBNull.Value) ? dr.GetDouble(6) : 0;
        }

        public override T CopyItem<T>(T item)
        {
            //Change item to the correct type of object then create a copy of it
            county x = item as county;
            county y = new county(x.ID, x.Name, x.DefaultState, x.NorthBound, x.SouthBound, x.EastBound, x.WestBound);

            //This is necessary because the function doesn't know in advance that T is a county
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            //create an object with no values
            county y = new county(0, "", new state(0, "", ""), 0, 0, 0, 0);

            //This is necessary because the function doesn't know in advance that T is a county
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}
