using System.Collections.Generic;
using System.Windows.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    class LogEntry : DBobject
    {
        public DateTime EntryDate { get; set; }
        public string strEntry { get; set; }
        public user EnteredBy { get; set; }

        public LogEntry()
        {
            EntryDate = new DateTime();
            strEntry = "";
            ID = 0;
            EnteredBy = new user();
        }
        
        public LogEntry(int id, DateTime entrydate, string strentry, user enteredby)
        {
            ID = id;
            EntryDate = entrydate;
            strEntry = strentry;
            EnteredBy = enteredby;
        }

        public override string ToString()
        { return strEntry; }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            strEntry = (dr[1] != DBNull.Value) ? dr.GetString(1) : "";
            user u = null;
            if (dr[3] != DBNull.Value) MainWindow.GetSingleItem<user>(out u, dr.GetInt32(3), MainWindow.Users);
            EnteredBy = u;
            EntryDate = (dr[4] != DBNull.Value) ? dr.GetDateTime(4) : DateTime.MinValue;
        }

        public override T CopyItem<T>(T item)
        {
            LogEntry x = item as LogEntry;
            LogEntry y = new LogEntry(x.ID, x.EntryDate, x.strEntry, x.EnteredBy);
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}