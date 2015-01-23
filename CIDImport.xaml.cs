using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Microsoft.Win32;

namespace CID2
{
    /// <summary>
    /// Interaction logic for CIDImport.xaml
    /// </summary>
    public partial class CIDImport : Window
    {
        public CIDImport()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SortedDictionary<string, Int32> dictTypes = new SortedDictionary<string, int>();
            dictTypes.Add("", 0);
            dictTypes.Add("Open Burning Complaint - Residential", 1);
            dictTypes.Add("Open Burning Complaint - Facility", 2);
            dictTypes.Add("Open Burning Complaint - Other", 3);
            dictTypes.Add("Fugitive Dust Complaint - Facility", 4);
            dictTypes.Add("Fugitive Dust Complaint - Other", 5);
            dictTypes.Add("Odor Complaint - Facility", 6);
            dictTypes.Add("Odor Complaint - Other", 7);
            dictTypes.Add("Facility Air Release Complaint", 8);
            dictTypes.Add("Asbestos Complaint", 9);
            dictTypes.Add("Other Complaint", 10);

            cboImportType.ItemsSource = dictTypes;
            cboImportType.SelectedIndex = 0;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        { this.Close(); }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (txtCIDPath.Text != "" && (int)cboImportType.SelectedValue > 0)
            {
                OleDbConnection db = new OleDbConnection(MainWindow.strCon + txtCIDPath.Text + MainWindow.strCon2 + txtCIDPass.Password);
                try { db.Open(); }
                catch (Exception Exception)
                {
                    MessageBox.Show("There was a problem opening your CID DB. Check the path and password.");
                    System.Windows.MessageBox.Show(Exception.Message);
                }

                OleDbCommand dbSQL = new OleDbCommand("SELECT * FROM oldtable where TYPE2 = " + ((int)cboImportType.SelectedValue).ToString() + ";" , db);
                OleDbDataReader dbReader = dbSQL.ExecuteReader();

                switch ((int)cboImportType.SelectedValue)
                {
                    case 1://OB Residential
                        { }
                        break;
                    case 2://OB Facility
                        CreateOBFac(dbReader, db);
                        break;
                    case 3://OB Other
                        { }
                        break;
                    case 4://Fugitive Facility
                        CreateDustFac(dbReader, db);
                        break;
                    case 5://Fugitive Other
                        { }
                        break;
                    case 6://Odor Facility
                        CreateOdorFac(dbReader, db);
                        break;
                    case 7://Odor Other
                        { }
                        break;
                    case 8://Release Facility
                        CreateReleaseFac(dbReader, db);
                        break;
                    case 9://Asbestos complaint
                        { }
                        break;
                    case 10://Other complaint
                        { }
                        break;
                    default:
                        { }
                        break;
                }
                dbReader.Close();
                db.Close();
            }
        }

        private void CreateReleaseFac(OleDbDataReader old, OleDbConnection olddb)
        {
            MainWindow.OpenMainDBConnection();

            while (old.Read())
            {
                GenericComplaint comp = CreateGenericComplaint(old, 21);//Set new complaint type here

                Facility thisFac = new Facility();
                if (comp.PlaceID_FacID != "") thisFac = FindFacility(comp.PlaceID_FacID);
                if (thisFac.ID != 0)
                {
                    //Get facility contact object
                    FacilityContact contact = CreateFacContact(comp, old);

                    OtherLocation portableloc = new OtherLocation();
                    if (thisFac.Portable) portableloc = CreatePortable(old);

                    //Create complainant object
                    Complainant complainant = CreateComplainant(comp, old);

                    DateTime retentiondate = DateTime.MinValue;

                    //int a = 0;
                    //if (thisFac.ID == 810) a = 1;

                    ReleaseFacilityComplaint newcomp = new ReleaseFacilityComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, thisFac, contact,
                        complainant, comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes,
                        comp.ReceivedBy, comp.Inspector, comp.Status, comp.DateReceived, (DateTime)comp.IncidentDate, (DateTime)comp.DateClosed,
                        retentiondate, comp.CETA, comp.Type, comp.LastModified, comp.MethodReceived, (DateTime)comp.DateInvestigated, comp.AppendixA,
                        comp.Restricted, comp.FacName, portableloc);
                    newcomp.NewComplaint = true;

                    ReleaseFacCompForm form = new ReleaseFacCompForm(newcomp, true);
                    form.Show();
                    form.IsLocked = false;
                    form.aComplaint.SaveComplaint(olddb);
                    form.Close();
                }
            }

            MainWindow.CloseMainDBConnection();
        }

        private void CreateOdorFac(OleDbDataReader old, OleDbConnection olddb)
        {
            MainWindow.OpenMainDBConnection();

            while (old.Read())
            {
                GenericComplaint comp = CreateGenericComplaint(old, 9);//Set new complaint type here

                Facility thisFac = new Facility();
                if (comp.PlaceID_FacID != "") thisFac = FindFacility(comp.PlaceID_FacID);
                if (thisFac.ID != 0)
                {
                    //Get facility contact object
                    FacilityContact contact = CreateFacContact(comp, old);

                    OtherLocation portableloc = new OtherLocation();
                    if (thisFac.Portable) portableloc = CreatePortable(old);

                    //Create complainant object
                    Complainant complainant = CreateComplainant(comp, old);

                    DateTime retentiondate = DateTime.MinValue;

                    //int a = 0;
                    //if (thisFac.ID == 810) a = 1;

                    OdorFacilityComplaint newcomp = new OdorFacilityComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, thisFac, contact,
                        complainant, comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes,
                        comp.ReceivedBy, comp.Inspector, comp.Status, comp.DateReceived, (DateTime)comp.IncidentDate, (DateTime)comp.DateClosed,
                        retentiondate, comp.CETA, comp.Type, comp.LastModified, comp.MethodReceived, (DateTime)comp.DateInvestigated, comp.AppendixA,
                        comp.Restricted, comp.FacName, portableloc);
                    newcomp.NewComplaint = true;

                    OdorFacCompForm form = new OdorFacCompForm(newcomp, true);
                    form.Show();
                    form.IsLocked = false;
                    form.aComplaint.SaveComplaint(olddb);
                    form.Close();
                }
            }

            MainWindow.CloseMainDBConnection();
        }

        private void CreateDustFac(OleDbDataReader old, OleDbConnection olddb)
        {
            MainWindow.OpenMainDBConnection();

            while (old.Read())
            {
                GenericComplaint comp = CreateGenericComplaint(old, 17);//Set new complaint type here

                Facility thisFac = new Facility();
                if (comp.PlaceID_FacID != "") thisFac = FindFacility(comp.PlaceID_FacID);
                if (thisFac.ID != 0)
                {
                    //Get facility contact object
                    FacilityContact contact = CreateFacContact(comp, old);

                    OtherLocation portableloc = new OtherLocation();
                    if (thisFac.Portable) portableloc = CreatePortable(old);

                    //Create complainant object
                    Complainant complainant = CreateComplainant(comp, old);

                    DateTime retentiondate = DateTime.MinValue;

                    //int a = 0;
                    //if (thisFac.ID == 810) a = 1;

                    DustFacilityComplaint newcomp = new DustFacilityComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, thisFac, contact,
                        complainant, comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes,
                        comp.ReceivedBy, comp.Inspector, comp.Status, comp.DateReceived, (DateTime)comp.IncidentDate, (DateTime)comp.DateClosed,
                        retentiondate, comp.CETA, comp.Type, comp.LastModified, comp.MethodReceived, (DateTime)comp.DateInvestigated, comp.AppendixA,
                        comp.Restricted, comp.FacName, portableloc);
                    newcomp.NewComplaint = true;

                    DustFacilityCompForm form = new DustFacilityCompForm(newcomp, true);
                    form.Show();
                    form.IsLocked = false;
                    form.aComplaint.SaveComplaint(olddb);
                    form.Close();
                }
            }

            MainWindow.CloseMainDBConnection();
        }

        private void CreateOBFac(OleDbDataReader old, OleDbConnection olddb)
        {
            MainWindow.OpenMainDBConnection();

            while (old.Read())
            {
                GenericComplaint comp = CreateGenericComplaint(old, 5);

                Facility thisFac = new Facility();
                if (comp.PlaceID_FacID != "") thisFac = FindFacility(comp.PlaceID_FacID);
                if (thisFac.ID != 0)
                {
                    //Get facility contact object
                    FacilityContact contact = CreateFacContact(comp, old);

                    //Get fire dept object
                    FireDepartment fdinfo = new FireDepartment();
                    if (old[34] != DBNull.Value) MainWindow.GetSingleItem<FireDepartment>(out fdinfo, Convert.ToInt32(old[34]), MainWindow.FireDepartments);
                    bool fdgenerated = old.GetBoolean(35);

                    OtherLocation portableloc = new OtherLocation();
                    if (thisFac.Portable) portableloc = CreatePortable(old);

                    //Create complainant object
                    Complainant complainant = CreateComplainant(comp, old);

                    DateTime retentiondate = DateTime.MinValue;

                    //int a = 0;
                    //if (thisFac.ID == 810) a = 1;

                    OBFacilityComplaint newcomp = new OBFacilityComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, thisFac, contact,
                        complainant, comp.Anonymous, fdinfo, fdgenerated, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes,
                        comp.ReceivedBy, comp.Inspector, comp.Status, comp.DateReceived, (DateTime)comp.IncidentDate, (DateTime)comp.DateClosed,
                        retentiondate, comp.CETA, comp.Type, comp.LastModified, comp.MethodReceived, (DateTime)comp.DateInvestigated, comp.AppendixA,
                        comp.Restricted, comp.FacName, portableloc);
                    newcomp.NewComplaint = true;

                    OBFacCompForm form = new OBFacCompForm(newcomp, true);
                    form.Show();
                    form.IsLocked = false;
                    form.aComplaint.SaveComplaint(olddb);
                    form.Close();
                }
            }

            MainWindow.CloseMainDBConnection();
        }

        public FacilityContact CreateFacContact(GenericComplaint comp, OleDbDataReader old)
        {
            FacilityContact contact = new FacilityContact();
            contact.FName = (old[13] != DBNull.Value) ? old.GetString(13) : "";
            contact.LName = (old[12] != DBNull.Value) ? old.GetString(12) : "";
            contact.Phone = (old[20] != DBNull.Value) ? old.GetString(20) : "";
            if (contact.FName == "" && contact.LName == "") contact = new FacilityContact();

            return contact;
        }

        public Complainant CreateComplainant(GenericComplaint comp, OleDbDataReader old)
        {
            Complainant complainer = new Complainant();

            if (!comp.Anonymous)
            {
                complainer.ID = 0;
                complainer.FName = (old[6] != DBNull.Value) ? old.GetString(6) : "";
                complainer.LName = (old[5] != DBNull.Value) ? old.GetString(5) : "";
                complainer.AddressLine1 = (old[7] != DBNull.Value) ? Convert.ToInt32(old.GetDouble(7)).ToString() : "";
                complainer.AddressLine1 += (old[8] != DBNull.Value) ? (" " + old.GetString(8)) : "";
                complainer.AddressLine2 = "";
                complainer.City = (old[44] != DBNull.Value) ? old.GetString(44) : "";
                complainer.Zip = (old[24] != DBNull.Value) ? Convert.ToInt32(old.GetDouble(24)).ToString() : "";
                complainer.Phone = (old[9] != DBNull.Value) ? old.GetString(9) : "";
                complainer.Email = "";
            }
            
            return complainer;
        }

        public Facility FindFacility(string facid)
        {
            Facility retfac = new Facility();

            var source = MainWindow.Facilities.Cast<Facility>();
            PropertyInfo x = typeof(Facility).GetProperty("FacilityID");
            IEnumerable<Facility> query = (from A in source where (string)x.GetValue(A) == facid select A);
            retfac = (Facility)query.FirstOrDefault();
            
            if (retfac == null) retfac = new Facility();

            return retfac;
        }

        public OtherLocation CreatePortable(OleDbDataReader old)
        {
            OtherLocation portableloc = new OtherLocation();

            portableloc.AddressLine1 = (old[15] != DBNull.Value) ? old.GetString(15) : "";
            portableloc.AddressLine1 += (old[16] != DBNull.Value) ? (" " + old.GetString(16)) : "";

            if (old[21] != DBNull.Value) portableloc.City = ConvertCity(old.GetInt32(21));
            if (old[17] != DBNull.Value) portableloc.Township = ConvertTownship(old.GetInt32(17));

            portableloc.Zip = (old[22] != DBNull.Value) ? old.GetString(22) : "";
            portableloc.Latitude = (old[38] != DBNull.Value) ? old.GetDouble(38) : 0;
            portableloc.Longitude = (old[39] != DBNull.Value) ? old.GetDouble(39) : 0;

            return portableloc;
        }

        public township ConvertTownship(int town)
        {
            switch (town)
            {
                case 1:
                    town = 0;
                    break;
                case 2:
                    town = 1;
                    break;
                case 3:
                    town = 2;
                    break;
                case 4:
                    town = 3;
                    break;
                case 5:
                    town = 4;
                    break;
                case 6:
                    town = 5;
                    break;
                case 7:
                    town = 6;
                    break;
                case 8:
                    town = 0;//there is no 8
                    break;
                case 9:
                    town = 7;
                    break;
                case 10:
                    town = 8;
                    break;
                case 11:
                    town = 9;
                    break;
                case 12:
                    town = 10;
                    break;
                case 13:
                    town = 11;//is this right? Hills & Dales = Canton? I think so
                    break;
                case 14:
                    town = 12;
                    break;
                case 15:
                    town = 13;
                    break;
                case 16:
                    town = 14;
                    break;
                case 18:
                    town = 16;
                    break;
                case 19:
                    town = 17;
                    break;
                case 20:
                    town = 18;
                    break;
                case 21:
                    town = 19;
                    break;
                case 22:
                    town = 20;
                    break;
                case 23:
                    town = 6;
                    break;
                case 24:
                    town = 21;
                    break;
                case 25:
                    town = 22;
                    break;
                default:
                    town = 0;
                    break;
            }

            township rettown = new township();
            if (town > 0) MainWindow.GetSingleItem<township>(out rettown, town, MainWindow.Townships);
            return rettown;
        }

        public city ConvertCity(int cityid)
        {
            switch (cityid)
            {
                case 1:
                    cityid = 5;
                    break;
                case 2:
                    cityid = 14;
                    break;
                case 3:
                    cityid = 1;
                    break;
                case 4:
                    cityid = 20;
                    break;
                case 5:
                    cityid = 8;
                    break;
                case 6:
                    cityid = 12;
                    break;
                case 7:
                    cityid = 17;
                    break;
                case 8:
                    cityid = 9;
                    break;
                case 9:
                    cityid = 4;
                    break;
                case 10:
                    cityid = 2;
                    break;
                case 11:
                    cityid = 7;
                    break;
                case 12:
                    cityid = 6;
                    break;
                case 13:
                    cityid = 5;//is this right? Hills & Dales = Canton? I think so
                    break;
                case 14:
                    cityid = 13;
                    break;
                case 15:
                    cityid = 26;
                    break;
                case 16:
                    cityid = 27;
                    break;
                case 18://there's no 17
                    cityid = 16;
                    break;
                case 19:
                    cityid = 18;
                    break;
                case 20:
                    cityid = 3;
                    break;
                case 21:
                    cityid = 0;
                    break;
                case 22:
                    cityid = 23;
                    break;
                case 23:
                    cityid = 25;
                    break;
                case 24:
                    cityid = 22;
                    break;
                case 25:
                    cityid = 24;
                    break;
                default:
                    cityid = 0;
                    break;
            }

            city retcity = new city();
            if (cityid > 0) MainWindow.GetSingleItem<city>(out retcity, cityid, MainWindow.Cities);
            return retcity;
        }

        public GenericComplaint CreateGenericComplaint(OleDbDataReader old, int type)
        {
            GenericComplaint comp = new GenericComplaint();
            
            comp.ID = 0;
            ComplaintType theType = new ComplaintType();
            MainWindow.GetSingleItem<ComplaintType>(out theType, type, MainWindow.ComplaintTypes);
            comp.Type = theType;
            comp.OtherNotes = (old[1] != DBNull.Value) ? ("Old CID ID - " + old.GetString(1)) : "";
            comp.Anonymous = old.GetBoolean(4);
            comp.ComplaintNotes = (old[10] != DBNull.Value) ? old.GetString(10) : "";
            comp.IncidentDate = (old[11] != DBNull.Value) ? old.GetDateTime(11) : DateTime.MinValue;
            user inspector = new user();
            if (old[18] != DBNull.Value) MainWindow.GetSingleItem<user>(out inspector, old.GetInt32(18), MainWindow.Users);
            comp.Inspector = inspector;
            comp.InspectionNotes = (old[19] != DBNull.Value) ? old.GetString(19) : "";
            comp.FacName = (old[14] != DBNull.Value) ? old.GetString(14) : "";
            comp.PlaceID_FacID = (old[26] != DBNull.Value) ? old.GetInt32(26).ToString() : "";
            comp.Restricted = old.GetBoolean(27);
            comp.AppendixA = old.GetBoolean(28);
            comp.DateClosed = (old[30] != DBNull.Value) ? old.GetDateTime(30) : DateTime.MinValue;
            comp.DateReceived = (old[31] != DBNull.Value) ? old.GetDateTime(31) : (old[2] != DBNull.Value) ? old.GetDateTime(2) : DateTime.MinValue;

            if (old[32] != DBNull.Value)
            {
                string inits = old.GetString(32);
                var source = MainWindow.Users.Cast<user>();
                PropertyInfo x = typeof(user).GetProperty("Inits");
                IEnumerable<user> query = (from A in source where (string)x.GetValue(A) == inits select A);
                user rcvdby = (user)query.FirstOrDefault();
                if (rcvdby != null) comp.ReceivedBy = rcvdby;
            }

            if(old[33] != DBNull.Value)
            {
                string label = old.GetString(33);
                var source = MainWindow.ReceivedMethods.Cast<receivedmethod>();
                PropertyInfo x = typeof(receivedmethod).GetProperty("Label");
                IEnumerable<receivedmethod> query = (from A in source where (string)x.GetValue(A) == label select A);
                receivedmethod method = (receivedmethod)query.FirstOrDefault();
                if (method != null) comp.MethodReceived = method;
            }

            CETAtype ceta = new CETAtype();
            if (old[36] != DBNull.Value)
            {
                int id = old.GetInt32(36);
                MainWindow.GetSingleItem<CETAtype>(out ceta, id, MainWindow.CETATypes);
            }
            comp.CETA = ceta;

            casestatus status = new casestatus();
            if (old[42] != DBNull.Value)
            {
                int id = old.GetInt32(42);
                MainWindow.GetSingleItem<casestatus>(out status, id, MainWindow.Statuses);
            }
            comp.Status = status;

            comp.DateInvestigated = (old[43] != DBNull.Value) ? old.GetDateTime(43) : DateTime.MinValue;

            return comp;
        }

        private void btnCID_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".accdb";
            dlg.Filter = "Access database (.accdb)|*.accdb";

            if (dlg.ShowDialog() == true) txtCIDPath.Text = dlg.FileName;
        }
    }
}
