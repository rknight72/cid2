using System.Collections.Generic;
using System.Windows.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class CETAtype : DBobject
    {
        public string Name { get; set; }

        public CETAtype()
        { Name = ""; }

        public CETAtype(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public override string ToString()
        {  return Name; }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            Name = (dr[1] != DBNull.Value) ? dr.GetString(1) : "";
        }

        public override T CopyItem<T>(T item)
        {
            CETAtype x = item as CETAtype;
            CETAtype y = new CETAtype(x.ID, x.Name);
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            CETAtype y = new CETAtype(0, "");
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}