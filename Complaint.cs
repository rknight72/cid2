using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;


namespace CID2
{
    public class Complaint
    {
        public int ID { get; set; }
        public int SubID { get; set; }
        public string TypeTable { get; set; }
        public string ComplaintText { get; set; }
        public string InspectionNotes { get; set; }
        public string OtherNotes { get; set; }
        public DateTime DateReceived { get; set; }
        public DateTime IncidentDate { get; set; }
        public DateTime DateClosed { get; set; }
        public DateTime RetentionDate { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime InvestigatedDate { get; set; }
        public bool Anonymous { get; set; }
        public bool AppendixA { get; set; }
        public bool Restricted { get; set; }
        public bool NewComplaint { get; set; }

        public ComplaintType CompType = new ComplaintType();
        public casestatus Status = new casestatus();
        public Complainant ComplainantInfo = new Complainant();
        public user ReceivedBy = new user();
        public user Inspector = new user();
        public CETAtype CETA = new CETAtype();
        public receivedmethod MethodReceived = new receivedmethod();

        public TextBox ComplaintTextBox = new TextBox();
        public TextBox InspectionNotesBox = new TextBox();
        public TextBox OtherNotesBox = new TextBox();
        public TextBlock IDBlock = new TextBlock();
        public ComboBox ReceivedByBox = new ComboBox();
        public ComboBox InspectorBox = new ComboBox();
        public ComboBox StatusBox = new ComboBox();
        public ComboBox CETABox = new ComboBox();
        public ComboBox MethodReceivedBox = new ComboBox();
        public DatePicker DateReceivedBox = new DatePicker();
        public DatePicker IncidentDateBox = new DatePicker();
        public DatePicker DateClosedBox = new DatePicker();
        public DatePicker RetentionBox = new DatePicker();
        public DatePicker InvestigatedBox = new DatePicker();
        public bool ControlsSet = false;

        public ComplaintForm Form { get; set; }

        public Complaint()
        {
            ID = SubID = 0;
            TypeTable = ComplaintText = InspectionNotes = OtherNotes = "";
            DateReceived = IncidentDate = DateClosed = DateTime.MinValue;
            Anonymous = NewComplaint = false;
        }

        public Complaint(int id, string complainttext, string inspectionnotes, string othernotes, bool anonymous, Complainant complainant, CETAtype ceta)
        {
            ID = id;
            ComplaintText = complainttext;
            InspectionNotes = inspectionnotes;
            OtherNotes = othernotes;
            ComplainantInfo = complainant;
            ComplainantInfo.SetOwner(this);
            Anonymous = anonymous;
            NewComplaint = true;
            CETA = ceta;
        }

        public Complaint(int id, int subid, string typetable, string complainttext, string inspectionnotes, string othernotes,
            bool anonymous, Complainant complainant, user receivedby, user inspector, casestatus status, DateTime datereceived,
            DateTime incidentdate, DateTime dateclosed, DateTime retentiondate, CETAtype ceta, ComplaintType type,
            receivedmethod methodreceived, DateTime investigateddate, bool appendixa, bool restricted)
        {
            ID = id;
            SubID = subid;
            TypeTable = typetable;
            ComplaintText = complainttext;
            InspectionNotes = inspectionnotes;
            OtherNotes = othernotes;
            ComplainantInfo = complainant;
            ComplainantInfo.SetOwner(this);
            ReceivedBy = receivedby;
            Inspector = inspector;
            Status = status;
            DateReceived = datereceived;
            IncidentDate = incidentdate;
            DateClosed = dateclosed;
            RetentionDate = retentiondate;
            Anonymous = anonymous;
            CETA = ceta;
            CompType = type;
            NewComplaint = false;
            MethodReceived = methodreceived;
            InvestigatedDate = investigateddate;
            AppendixA = appendixa;
            Restricted = restricted;
        }

        public void SetForm(ComplaintForm form)
        { Form = form; }

        public virtual void SetComplaintSpecificAddressInfo(string specificstring) { }
        public virtual string GetComplaintSpecificAddressInfo() { return ""; }
        public virtual void SetParcelInfo(string parcel, string owner, string zip) { }
        public virtual ComplaintAddress GetComplaintAddress() { return new ComplaintAddress(); }
        public virtual Contact GetOccupantAddress() { return new Contact(); }
        public virtual void SetContactFilter() { }

        public virtual Object MyClone()
        {
            Complaint obj = new Complaint();

            obj.ComplaintText = new string(ComplaintText.ToCharArray());
            obj.InspectionNotes = new string(InspectionNotes.ToCharArray());
            obj.OtherNotes = new string(OtherNotes.ToCharArray());
            obj.DateReceived = new DateTime(DateReceived.Ticks);
            obj.IncidentDate = new DateTime(IncidentDate.Ticks);
            obj.DateClosed = new DateTime(DateClosed.Ticks);
            obj.InvestigatedDate = new DateTime(InvestigatedDate.Ticks);
            obj.Anonymous = (Anonymous == true);
            if (Status != null && Status.ID != 0) MainWindow.GetSingleItem<casestatus>(out obj.Status, Status.ID, MainWindow.Statuses);
            else obj.Status = new casestatus();
            if (ReceivedBy != null && ReceivedBy.ID != 0) MainWindow.GetSingleItem<user>(out obj.ReceivedBy, ReceivedBy.ID, MainWindow.Users);
            else obj.ReceivedBy = new user();
            if (Inspector != null && Inspector.ID != 0) MainWindow.GetSingleItem<user>(out obj.Inspector, Inspector.ID, MainWindow.Users);
            else obj.Inspector = new user();
            if (MethodReceived != null && MethodReceived.ID != 0) MainWindow.GetSingleItem<receivedmethod>(out obj.MethodReceived, MethodReceived.ID, MainWindow.ReceivedMethods);
            if (CETA != null) MainWindow.GetSingleItem<CETAtype>(out obj.CETA, CETA.ID, MainWindow.CETATypes);
            if (ComplainantInfo != null)// && ComplainantInfo.ID != 0)
            {
                obj.ComplainantInfo.ID = ComplainantInfo.ID;
                obj.ComplainantInfo.FName = new string(ComplainantInfo.FName.ToCharArray());
                obj.ComplainantInfo.LName = new string(ComplainantInfo.LName.ToCharArray());
                obj.ComplainantInfo.AddressLine1 = new string(ComplainantInfo.AddressLine1.ToCharArray());
                obj.ComplainantInfo.AddressLine2 = new string(ComplainantInfo.AddressLine2.ToCharArray());
                obj.ComplainantInfo.City = new string(ComplainantInfo.City.ToCharArray());
                if (ComplainantInfo.State != null && ComplainantInfo.State.ID != 0)
                {
                    state s = new state();
                    MainWindow.GetSingleItem<state>(out s, ComplainantInfo.State.ID, MainWindow.States);
                    obj.ComplainantInfo.State = s;
                }
                else obj.ComplainantInfo.State = new state();
                obj.ComplainantInfo.Zip = new string(ComplainantInfo.Zip.ToCharArray());
                obj.ComplainantInfo.Phone = new string(ComplainantInfo.Phone.ToCharArray());
                obj.ComplainantInfo.Email = new string(ComplainantInfo.Email.ToCharArray());
                obj.ComplainantInfo.thisControl = ComplainantInfo.thisControl;
                obj.ComplainantInfo.SetOwner(this);
            }
            else obj.ComplainantInfo = new Complainant(ComplainantInfo.thisControl, this);
            obj.AppendixA = (AppendixA == true);
            obj.Restricted = (Restricted == true);

            return obj;
        }

        public void SetCompType(string strCompName)
        {
            foreach(ComplaintType x in MainWindow.ComplaintTypes)
            {
                if (x.Label == strCompName)
                {
                    CompType = x;
                    TypeTable = x.ComplaintTable;
                    CETA = x.defaultType;
                }
            }
        }

        public void SetAsNew(bool isnew)
        { NewComplaint = isnew; }

        public override string ToString()
        { return Convert.ToString(ID); }

        public virtual List<Control> GetRequiredControls()
        {
            List<Control> controls = ComplainantInfo.GetRequiredControls();
            controls.Add(ComplaintTextBox);
            controls.Add(DateReceivedBox);
            controls.Add(ReceivedByBox);
            controls.Add(StatusBox);
            controls.Add(CETABox);
            return controls;
        }

        public virtual void SetControls(TextBlock idblock, TextBox complainttextbox, TextBox inspectionnotesbox, TextBox othernotesbox,
            ComboBox receivedbybox, ComboBox inspectorbox, ComboBox statusbox, DatePicker datereceivedbox,
            DatePicker incidentdatebox, DatePicker dateclosedbox, DatePicker retentionbox, ComboBox cetabox, ComboBox methodreceivedbox,
            DatePicker investigatedbox)
        {
            ControlsSet = true;
            IDBlock = idblock;
            ComplaintTextBox = complainttextbox;
            InspectionNotesBox = inspectionnotesbox;
            OtherNotesBox = othernotesbox;
            ReceivedByBox = receivedbybox;
            InspectorBox = inspectorbox;
            StatusBox = statusbox;
            DateReceivedBox = datereceivedbox;
            IncidentDateBox = incidentdatebox;
            DateClosedBox = dateclosedbox;
            RetentionBox = retentionbox;
            CETABox = cetabox;
            MethodReceivedBox = methodreceivedbox;
            InvestigatedBox = investigatedbox;
        }

        public virtual void UpdateContentFromControls()
        {
            if (ControlsSet)
            {
                ComplaintText = ComplaintTextBox.Text;
                InspectionNotes = InspectionNotesBox.Text;
                OtherNotes = OtherNotesBox.Text;
                ReceivedBy = (user)(ReceivedByBox.SelectedItem);
                Inspector = (user)(InspectorBox.SelectedItem);
                Status = (casestatus)(StatusBox.SelectedItem);
                MethodReceived = (receivedmethod)MethodReceivedBox.SelectedItem;
                if (DateReceivedBox.SelectedDate != null) DateReceived = (DateTime)DateReceivedBox.SelectedDate;
                if (IncidentDateBox.SelectedDate != null) IncidentDate = (DateTime)IncidentDateBox.SelectedDate;
                if (DateClosedBox.SelectedDate != null) DateClosed = (DateTime)DateClosedBox.SelectedDate;
                if (RetentionBox.SelectedDate != null) RetentionDate = (DateTime)RetentionBox.SelectedDate;
                if (InvestigatedBox.SelectedDate != null) InvestigatedDate = (DateTime)InvestigatedBox.SelectedDate;
                CETA = (CETAtype)CETABox.SelectedItem;
            }

            ComplainantInfo.UpdateContentFromControls();
        }

        public virtual void UpdateControlContent()
        {
            if (ControlsSet)
            {
                IDBlock.Text = "ID: C" + ID.ToString();
                ComplaintTextBox.Text = ComplaintText; ;
                InspectionNotesBox.Text = InspectionNotes;
                OtherNotesBox.Text = OtherNotes;
                if (ReceivedBy != null) ReceivedByBox.SelectedValue = ReceivedBy.ID;
                if (Inspector != null) InspectorBox.SelectedValue = Inspector.ID;
                if (Status != null) StatusBox.SelectedValue = Status.ID;
                if (CETA != null) CETABox.SelectedValue = CETA.ID;
                if (MethodReceived != null) MethodReceivedBox.SelectedValue = MethodReceived.ID;
                if (DateReceived.Ticks != 0) DateReceivedBox.SelectedDate = DateReceived;
                if (IncidentDate.Ticks != 0) IncidentDateBox.SelectedDate = IncidentDate;
                if (DateClosed.Ticks != 0) DateClosedBox.SelectedDate = DateClosed;
                if (RetentionDate.Ticks != 0) RetentionBox.SelectedDate = RetentionDate;
                if (InvestigatedDate.Ticks != 0) InvestigatedBox.SelectedDate = InvestigatedDate;
            }

            ComplainantInfo.UpdateControlContent();
        }

        public void RestoreComplaintDataMembers(Complaint comp)
        {
            ComplaintText = comp.ComplaintText;
            InspectionNotes = comp.InspectionNotes;
            OtherNotes = comp.OtherNotes;
            DateReceived = comp.DateReceived;
            DateClosed = comp.DateClosed;
            IncidentDate = comp.IncidentDate;
            RetentionDate = comp.RetentionDate;
            InvestigatedDate = comp.InvestigatedDate;
            MethodReceived = comp.MethodReceived;
            Anonymous = comp.Anonymous;
            AppendixA = comp.AppendixA;
            Restricted = comp.Restricted;
            if (comp.Status != null && comp.Status.ID != 0)
            {
                casestatus s = new casestatus();
                MainWindow.GetSingleItem<casestatus>(out s, comp.Status.ID, MainWindow.Statuses);
                Status = s;
            }
            else Status = new casestatus();
            if (comp.ReceivedBy != null && comp.ReceivedBy.ID != 0)
            {
                user u = new user();
                MainWindow.GetSingleItem<user>(out u, comp.ReceivedBy.ID, MainWindow.Users);
                ReceivedBy = u;
            }
            else ReceivedBy = new user();
            if (comp.Inspector != null && comp.Inspector.ID != 0)
            {
                user u = new user();
                MainWindow.GetSingleItem<user>(out u, comp.Inspector.ID, MainWindow.Users);
                Inspector = u;
            }
            else Inspector = new user();
            if (comp.CETA != null && comp.CETA.ID != 0)
            {
                CETAtype c = new CETAtype();
                MainWindow.GetSingleItem<CETAtype>(out c, comp.CETA.ID, MainWindow.CETATypes);
                CETA = c;
            }
            else CETA = new CETAtype();
            ComplainantInfo.AddressLine1 = comp.ComplainantInfo.AddressLine1;
            ComplainantInfo.AddressLine2 = comp.ComplainantInfo.AddressLine2;
            ComplainantInfo.City = comp.ComplainantInfo.City;
            if (comp.ComplainantInfo.State != null && comp.ComplainantInfo.State.ID != 0)
            {
                state s = new state();
                MainWindow.GetSingleItem<state>(out s, comp.ComplainantInfo.State.ID, MainWindow.States);
                ComplainantInfo.State = s;
            }
            else ComplainantInfo.State = new state();
            ComplainantInfo.Zip = comp.ComplainantInfo.Zip;
            ComplainantInfo.Phone = comp.ComplainantInfo.Phone;
            ComplainantInfo.Email = comp.ComplainantInfo.Email;

            return;
        }

        public List<string> CompareDataMembers(Complaint comp, List<string> differenceList)
        {
            List<String> returnList = new List<string>();
            returnList.AddRange(differenceList);

            if (ComplaintText != comp.ComplaintText) returnList.Add("Complaint Notes");
            if (InspectionNotes != comp.InspectionNotes) returnList.Add("Inspection Notes");
            if (OtherNotes != comp.OtherNotes) returnList.Add("Other Notes");
            if (DateReceived != comp.DateReceived) returnList.Add("Date Received to " + DateReceived.ToShortDateString());
            if (DateClosed != comp.DateClosed) returnList.Add("Date Closed to " + DateClosed.ToShortDateString());
            if (IncidentDate != comp.IncidentDate) returnList.Add("Incident Date to " + IncidentDate.ToShortDateString());
            if (RetentionDate != comp.RetentionDate) returnList.Add("Retention Date to " + RetentionDate.ToShortDateString());
            if (InvestigatedDate != comp.InvestigatedDate) returnList.Add("Investigation started Date to " + InvestigatedDate.ToShortDateString());
            if (Anonymous != comp.Anonymous) returnList.Add("Anonymous to " + Anonymous.ToString());
            if (Anonymous != comp.AppendixA) returnList.Add("Appendix A to " + AppendixA.ToString());
            if (Anonymous != comp.Restricted) returnList.Add("Restricted to " + Restricted.ToString());
            if (Status.ID != comp.Status.ID) returnList.Add("Case Status to " + Status.Status);
            if (ReceivedBy.ID != comp.ReceivedBy.ID) returnList.Add("Received by to " + ReceivedBy.Inits);
            if (Inspector.ID != comp.Inspector.ID) returnList.Add("Inspector to " + Inspector.Inits);
            if (CETA.ID != comp.CETA.ID) returnList.Add("STARS2 type set to " + CETA.Name);
            if (MethodReceived.ID != comp.MethodReceived.ID) returnList.Add("Method received changed to " + MethodReceived.Label);
            if (ComplainantInfo.FName != comp.ComplainantInfo.FName || ComplainantInfo.LName != comp.ComplainantInfo.LName) returnList.Add("Complainant name");
            if (ComplainantInfo.AddressLine1 != comp.ComplainantInfo.AddressLine1 || ComplainantInfo.AddressLine2 != comp.ComplainantInfo.AddressLine2
                || ComplainantInfo.City != comp.ComplainantInfo.City || ComplainantInfo.State.ID != comp.ComplainantInfo.State.ID ||
                ComplainantInfo.Zip != comp.ComplainantInfo.Zip) returnList.Add("Complainant address");
            if (ComplainantInfo.Phone != comp.ComplainantInfo.Phone || ComplainantInfo.Email != comp.ComplainantInfo.Email) returnList.Add("Complainant contact info");

            return returnList;
        }

        public void SaveLog(string log)
        {
            MainWindow.OpenMainDBConnection();
            
            string strSQL = "INSERT INTO tbl_AuditTrail (Notes, pID, uID, dtSaved) VALUES (@notes, @pid, @uid, @dt);";
            OleDbCommand cmd = new OleDbCommand(strSQL, MainWindow.cidDB);
            cmd.Parameters.AddWithValue("@notes", log);
            cmd.Parameters.AddWithValue("@pid", ID);
            cmd.Parameters.AddWithValue("uid", MainWindow.thisUser.ID);
            cmd.Parameters.Add("@dt", OleDbType.Date).Value = DateTime.Now;
            //cmd.Parameters.AddWithValue("@dt", dt);
            cmd.ExecuteNonQuery();

            MainWindow.CloseMainDBConnection();
        }

        public void SaveComplaint(OleDbConnection olddb = null)
        {
            UpdateContentFromControls();

            MainWindow.OpenMainDBConnection();
            OleDbCommand cidCMD = new OleDbCommand();
            string strSQL;

            bool bCont = false;

            if (!NewComplaint)
            {
                strSQL = "UPDATE tbl_Complaints SET Complaint_Type = @type, Status = @status, Inspector = @inspector, ReceivedBy = @rcvdby, "
                + "Date_Received = @dtrcvd, Date_Closed = @dtclosed, Incident_Date = @incidentdt, Date_Investigated = @investdt, "
                + "Anonymous = @anon, Complaint = @complaint, Complaint_Inspection_Notes = @inspnotes, Other_Notes = @othernotes, "
                + "Retention_Date = @retentiondt, CETA_Type = @ceta, LastModified = @mod, MethodReceived = @methrcvd, AppendixA = @appa, "
                + "Restricted = @restricted WHERE ID = " + ID.ToString() + ";";
                bCont = ExecuteComplaintSQL(out cidCMD, strSQL, olddb);
            }
            else
            {
                strSQL = "INSERT INTO tbl_Complaints (Complaint_Type, Status, Inspector, ReceivedBy, Date_Received, Date_Closed, Incident_Date, "
                + "Date_Investigated, Anonymous, Complaint, Complaint_Inspection_Notes, Other_Notes, Retention_Date, CETA_Type, LastModified, "
                + "MethodReceived, AppendixA, Restricted) VALUES (@type, @status, @inspector, @rcvdby, @dtrcvd, @dtclosed, @incidentdt, "
                + "@investdt, @anon, @complaint, @inspnotes, @othernotes, @publicnotes, @ceta, @mod, @methrcvd, @appa, @restricted);";
                bCont = ExecuteComplaintSQL(out cidCMD, strSQL, olddb);

                if (bCont)
                {
                    cidCMD.CommandText = "SELECT @@Identity";
                    ID = (int)cidCMD.ExecuteScalar();
                    IDBlock.Text = "ID: C" + ID.ToString();
                }
            }

            if (bCont)
            {
                SaveSubTables(cidCMD);
                NewComplaint = false;
                if (olddb == null) MainWindow.CloseMainDBConnection();
            }
        }

        public bool ExecuteComplaintSQL(out OleDbCommand cidCMD, string strSQL, OleDbConnection olddb = null)
        {
            DateTime lowerlimit = new DateTime(1990,1,1);

            cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
            cidCMD.Parameters.Add("@type", OleDbType.Integer).Value = ((CompType != null && CompType.ID > 0) ? CompType.ID : (object)DBNull.Value);
            cidCMD.Parameters.Add("@status", OleDbType.Integer).Value = ((Status != null && Status.ID > 0) ? Status.ID : (object)DBNull.Value);
            cidCMD.Parameters.Add("@inspector", OleDbType.Integer).Value = (( Inspector != null && Inspector.ID > 0) ? Inspector.ID : (object)DBNull.Value);
            cidCMD.Parameters.Add("@rcvdby", OleDbType.Integer).Value = ((ReceivedBy != null && ReceivedBy.ID > 0) ? ReceivedBy.ID : (object)DBNull.Value);
            cidCMD.Parameters.Add("@dtrcvd", OleDbType.Date).Value = (DateReceived > lowerlimit ? DateReceived : (object)DBNull.Value);
            cidCMD.Parameters.Add("@dtclosed", OleDbType.Date).Value = (DateClosed > lowerlimit ? DateClosed : (object)DBNull.Value);
            cidCMD.Parameters.Add("@incidentdt", OleDbType.Date).Value = (IncidentDate > lowerlimit ? IncidentDate : (object)DBNull.Value);
            cidCMD.Parameters.Add("@investdt", OleDbType.Date).Value = (IncidentDate > lowerlimit ? IncidentDate : (object)DBNull.Value);
            cidCMD.Parameters.AddWithValue("@anon", Anonymous);
            cidCMD.Parameters.AddWithValue("@complaint", ComplaintText);
            cidCMD.Parameters.AddWithValue("@inspnotes", InspectionNotes);
            cidCMD.Parameters.AddWithValue("@othernotes", OtherNotes);
            cidCMD.Parameters.Add("@retentiondt", OleDbType.Date).Value = (RetentionDate > lowerlimit ? RetentionDate : (object)DBNull.Value);
            cidCMD.Parameters.Add("@ceta", OleDbType.Integer).Value = ((CETA != null && CETA.ID > 0) ? CETA.ID : (object)DBNull.Value);
            LastModified = DateTime.Now;
            cidCMD.Parameters.Add("@mod", OleDbType.Date).Value = LastModified;
            cidCMD.Parameters.Add("@methrcvd", OleDbType.Integer).Value = ((MethodReceived != null && MethodReceived.ID > 0) ? MethodReceived.ID : (object)DBNull.Value);
            cidCMD.Parameters.AddWithValue("@appa", AppendixA);
            cidCMD.Parameters.AddWithValue("@restricted", Restricted);

            try
            {
                int i = cidCMD.ExecuteNonQuery();

                if (i > 0)
                {
                    
                    if (olddb != null)
                    {
                        string imported = "Old CID ID - ";
                        string oldid = OtherNotes.Remove(0, imported.Length);

                        string deleteSQL = "DELETE * FROM oldtable WHERE ID = " + oldid + ";";
                        OleDbCommand oldCMD = new OleDbCommand(deleteSQL, olddb);
                        oldCMD.ExecuteNonQuery();
                    }

                    return true;
                }
                return false;
            }
            catch
            {
                MainWindow.CloseMainDBConnection();
                return false;
            }
        }

        public void SaveSubTables(OleDbCommand cidCMD)
        {
            SaveSubComplaint(cidCMD);
            ComplainantInfo.SaveContact(cidCMD, ID);
            SaveContacts(cidCMD);
            SaveComplaintSpecific(cidCMD);
        }

        public virtual void SaveSubComplaint(OleDbCommand cidCMD) { }
        public virtual void SaveComplaintSpecific(OleDbCommand cidCMD) { }
        public virtual void SaveContacts(OleDbCommand cidCMD) { }
        public virtual void ExecuteSubTypeSQL(out OleDbCommand cidCMD, string strSQL) { cidCMD = null; }
    }        
}
