using System.Collections.Generic;
using System.Windows.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class user
    {
        public int ID { get; set; }
        public string Inits { get; set; }
        public int Role { get; set; }
        public string Pass { get; set; }
        public bool Active { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }

        public user()
        {
            ID = Role = 0;
            Inits = Pass = FName = LName = "";
            Active = false;
        }

        public user(int id, string inits, int role, string pass, bool active, string fname, string lname)
        {
            ID = id;
            Inits = inits;
            Role = role;
            Pass = pass;
            Active = active;
            FName = fname;
            LName = lname;
        }

        public override string ToString()
        {
            if ((FName != "") || (LName != "")) return LName + ", " + FName;
            else return "";
        }

        public T CopyItem<T>(T item)
        {
            user x = item as user;
            user y = new user(x.ID, x.Inits, x.Role, x.Pass, x.Active, x.FName, x.LName);
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public T NullItem<T>(T item)
        {
            user y = new user(0, "", 0, "", false, "", "");
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public static List<user> DataReaderMapToList<T>(OleDbDataReader dr)
        {
            List<user> list = new List<user>();
            while (dr.Read())
            {
                user obj = new user(dr.GetInt32(0), dr.GetString(1), dr.GetInt32(2), dr.GetString(3), dr.GetBoolean(4),
                    dr.GetString(5), dr.GetString(6));
                if (obj.LName != "") list.Add(obj);
            }
            return list;
        }
    }

    public class UsersList
    {
        private static List<user> UserList = new List<user>();
        public static ListCollectionView UsersCollection = new ListCollectionView(UserList);
    }
}
