using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class city : DBobject
    {
        public string Name { get; set; }
        public county DefaultCounty { get; set; }
        public township DefaultTownship { get; set; }
        public List<string> ZipCodeList { get; set; }
        public List<int> TownshipIDList { get; set; }

        public city()
        {
            Name = "";
            DefaultCounty = new county();
            DefaultTownship = new township();
            ZipCodeList = new List<string>();
            TownshipIDList = new List<int>();
        }

        public city(int id, string name, county defaultcounty, township defaultownship,
            List<int> townshipidlist, List<string> zipcodelist)
        {
            ID = id;
            Name = name;
            DefaultCounty = defaultcounty;
            DefaultTownship = defaultownship;
            TownshipIDList = townshipidlist;
            ZipCodeList = zipcodelist;
        }

        public override string ToString()
        {
            return Name;
        }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            Name = (dr[1] != DBNull.Value) ? dr.GetString(1) : "";
            county thecounty = null;
            if (dr[2] != DBNull.Value) MainWindow.GetSingleItem<county>(out thecounty, dr.GetInt32(2), MainWindow.Counties);
            DefaultCounty = thecounty;
            township thetownship = null;
            if (dr[3] != DBNull.Value) MainWindow.GetSingleItem<township>(out thetownship, dr.GetInt32(3), MainWindow.Townships);
            DefaultTownship = thetownship;
            if (dr[4] != DBNull.Value)
            {
                string[] tempStringList = dr.GetString(4).Split(',');
                TownshipIDList = new List<int>();
                foreach (string x in tempStringList)
                { TownshipIDList.Add(Convert.ToInt32(x)); }
            }
            else TownshipIDList = null;
            ZipCodeList = (dr[5] != DBNull.Value) ? dr.GetString(5).Split(',').ToList<string>() : null;
        }

        public override T CopyItem<T>(T item)
        {
            //Change item to the correct type of object then create a copy of it
            city x = item as city;
            city y = new city(x.ID, x.Name, x.DefaultCounty, x.DefaultTownship, x.TownshipIDList, x.ZipCodeList);

            //This is necessary because the function doesn't know in advance that T is a city
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            //create an object with no values
            city y = new city(0, "", new county(0,"", null, 0, 0, 0, 0), new township(0,"",false, false), null, null);

            //This is necessary because the function doesn't know in advance that T is a city
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}
