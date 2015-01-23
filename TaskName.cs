using System.Collections.Generic;
using System.Windows.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class TaskName : DBobject
    {
        public string Name { get; set; }
        public int Type { get; set; }

        public TaskName()
        {
            Name = "";
            Type = 0;
        }

        public TaskName(int id, string name, int type)
        {
            ID = id;
            Name = name;
            Type = type;
        }

        public override string ToString()
        { return Name; }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            Name = (dr[1] != DBNull.Value) ? dr.GetString(1) : "";
            Type = (dr[2] != DBNull.Value) ? dr.GetInt32(2) : 0;
        }

        public override T CopyItem<T>(T item)
        {
            //Change item to the correct type of object then create a copy of it
            TaskName x = item as TaskName;
            TaskName y = new TaskName(x.ID, x.Name, x.Type);

            //This is necessary because the function doesn't know in advance that T is a TaskName
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            //create an object with no values
            TaskName y = new TaskName(0, "", 0);

            //This is necessary because the function doesn't know in advance that T is a TaskName
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}
