using System.Collections.Generic;
using System.Windows.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class ComplaintCategory : DBobject
    {
        public string Label { get; set; }

        public ComplaintCategory()
        { Label = ""; }

        public ComplaintCategory(int id, string label)
        {
            ID = id;
            Label = label;
        }

        public override string ToString()
        { return Label; }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            Label = (dr[1] != DBNull.Value) ? dr.GetString(1) : "";
        }

        public override T CopyItem<T>(T item)
        {
            ComplaintCategory x = item as ComplaintCategory;
            ComplaintCategory y = new ComplaintCategory(x.ID, x.Label);
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            ComplaintCategory y = new ComplaintCategory(0, "");
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}
