using System.Collections.Generic;
using System.Windows.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class ComplaintTask : DBobject
    {
        public int ParentID { get; set; }
        public TaskName Task { get; set; }
        public DateTime TaskDate { get; set; }
        public user EnteredBy { get; set; }
        public string Details { get; set; }
        public string InitialCaret { get; set; }

        public ComplaintTask()
        {
            ParentID = 0;
            Task = new TaskName();
            TaskDate = DateTime.MinValue;
            EnteredBy = new user();
            Details = "";
            InitialCaret = "";
        }

        public ComplaintTask(int id, int parentid, TaskName task, DateTime taskdate, user enteredby, string details)
        {
            ID = id;
            ParentID = parentid;
            Task = task;
            TaskDate = taskdate;
            EnteredBy = enteredby;
            Details = details;
            if (Details != null && Details != "") InitialCaret = "-";
        }

        public override string ToString()
        { return Task.Name; }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            ParentID = dr.GetInt32(1);
            TaskName t = null;
            if (dr[2] != DBNull.Value) MainWindow.GetSingleItem<TaskName>(out t, dr.GetInt32(2), MainWindow.AllTasks);
            Task = t;
            TaskDate = (dr[3] != DBNull.Value) ? dr.GetDateTime(3) : DateTime.MinValue;
            user u = null;
            if (dr[4] != DBNull.Value) MainWindow.GetSingleItem<user>(out u, dr.GetInt32(4), MainWindow.Users);
            EnteredBy = u;
            Details = (dr[5] != DBNull.Value) ? dr.GetString(5) : "";
        }

        public override T CopyItem<T>(T item)
        {
            ComplaintTask x = item as ComplaintTask;
            ComplaintTask y = new ComplaintTask(x.ID, x.ParentID, x.Task, x.TaskDate, x.EnteredBy, x.Details);
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}
