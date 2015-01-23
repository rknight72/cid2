using mshtml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;


namespace CID2
{
    public struct GenericComplaint
    {
        public int ID { get; set; }
        public DateTime DateReceived { get; set; }
        public DateTime? DateInvestigated { get; set; }
        public DateTime? IncidentDate { get; set; }
        public DateTime? DateClosed { get; set; }
        public DateTime? RetentionDate { get; set; }
        public ComplaintType Type { get; set; }
        public int SubID { get; set; }
        public int SiteID { get; set; }
        public string FacName { get; set; }
        public string SiteAddress1 { get; set; }
        public string SortAddress { get; set; }
        public string ComplaintNotes { get; set; }
        public casestatus Status { get; set; }
        public user ReceivedBy { get; set; }
        public user Inspector { get; set; }
        public int Complainant { get; set; }
        public bool Anonymous { get; set; }
        public string InspectionNotes { get; set; }
        public string OtherNotes { get; set; }
        public CETAtype CETA { get; set; }
        public city SiteCity { get; set; }
        public string PlaceID_FacID { get; set; }
        public string Parcel { get; set; }
        public township Township { get; set; }
        public DateTime LastModified { get; set; }
        public receivedmethod MethodReceived { get; set; }
        public string Contact1 { get; set; }
        public string Contact2 { get; set; }
        public string Contact3 { get; set; }
        public string Contact4 { get; set; }
        public string Contact5 { get; set; }
        public string Contact6 { get; set; }
        public string ANTSID { get; set; }
        public bool AppendixA { get; set; }
        public bool Restricted { get; set; }
    } 

    public partial class MainWindow : Window, IDisposable
    {
        public static string strCon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=";
        public static string strCon2 = ";Jet OLEDB:Database Password=";

        public static string settingsDBpath;
        public static OleDbConnection settingsDB;

        /*public static string tasDBpath;
        public static string tasDBpass;
        public static string tasDBconnect;
        public static OleDbConnection tasDB;*/
        public static user thisUser;

        public static string cidDBpath;
        public static string cidDBpass;
        public static string cidDBconnect;
        public static OleDbConnection cidDB;

        public static string cidAttachmentspath;

        public static ListCollectionView Users = null;
        public static ListCollectionView ActiveUsers = null;
        public static ListCollectionView Cities = null;
        public static ListCollectionView AllTasks = null;
        public static ListCollectionView Counties = null;
        public static ListCollectionView FireDepartments = null;
        public static ListCollectionView FacilityContacts = null;
        public static ListCollectionView Facilities = null;
        public static ListCollectionView States = null;
        public static ListCollectionView Statuses = null;
        public static ListCollectionView Townships = null;
        public static ListCollectionView CETATypes = null;
        public static ListCollectionView ComplaintTypes = null;
        public static ListCollectionView OperatingStatuses = null;
        public static ListCollectionView PermittingClassifications = null;
        public static ListCollectionView ReceivedMethods = null;
        public static ListCollectionView ZipCodeList = null;
        public static ListCollectionView ComplaintCategories = null;
        public static ListCollectionView ComplaintLocations = null;

        public static List<Color> colors = new List<Color> { Colors.Blue, Colors.Gold, Colors.Green, Colors.Red, Colors.Orange,
            Colors.Purple, Colors.Aqua, Colors.SpringGreen, Colors.Pink, Colors.Lavender};

        public static List<SearchLocation> SearchList;

        public static MainWindow Main;

        public Chart ChartComp;

        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        static extern bool InternetGetConnectedState(ref ConnectionState lpdwFlags, int dwReserved);
        
        [Flags]
        enum ConnectionState : int
        {
            INTERNET_CONNECTION_MODEM = 0x1,
            INTERNET_CONNECTION_LAN = 0x2,
            INTERNET_CONNECTION_PROXY = 0x4,
            INTERNET_RAS_INSTALLED = 0x10,
            INTERNET_CONNECTION_OFFLINE = 0x20,
            INTERNET_CONNECTION_CONFIGURED = 0x40
        }

        public MainWindow()
        {
            InitializeComponent();

            //set a static reference to the class to allow other windows access to it
            Main = this;

            //open the settings file and get paths to the other databases from it from it
            settingsDBpath = System.AppDomain.CurrentDomain.BaseDirectory + "settings.accdb";
            settingsDB = new OleDbConnection(strCon + settingsDBpath);
            try { settingsDB.Open(); }
            catch { MainWindow.TryAgain(settingsDB, "Settings"); }
            OleDbCommand dbSQL = new OleDbCommand("SELECT * FROM tblDBpath" , settingsDB);
            OleDbDataReader dbReader = dbSQL.ExecuteReader();
            dbReader.Read();

            //open the main db
            if (dbReader[1] != DBNull.Value) cidDBpath = dbReader.GetString(1);
            else { MessageBox.Show("There was a problem opening your CID DB."); MainSettings settings = new MainSettings(); settings.ShowDialog(); }
            cidAttachmentspath = cidDBpath.Substring(0,cidDBpath.LastIndexOf('\\') + 1) + "attachments\\";
            if (dbReader[2] != DBNull.Value) cidDBpass = dbReader.GetString(2);
            else cidDBpass = "";
            cidDBpass = strCon2 + cidDBpass;
            cidDBconnect = strCon + cidDBpath + cidDBpass;
            cidDB = new OleDbConnection(cidDBconnect);
            dbReader.Read();

            
            //all this stuff needs to go, I've linked the users table in the main DB (also because Access is complaining about the number of files I have open during development)

            /*//because I removed the facs table and linked it instead, it's still in the settings.
            //I haven't decided if I want to remove it permanently so we need to skip this result.
            dbReader.Read();

            //open the TAS db to get the user info
            if (dbReader[1] != DBNull.Value) tasDBpath = dbReader.GetString(1);
            else { MessageBox.Show("There was a problem opening your TAS DB."); MainSettings settings = new MainSettings(); settings.ShowDialog(); }
            if (dbReader[2] != DBNull.Value) tasDBpass = dbReader.GetString(2);
            else tasDBpass = "";
            tasDBpass = strCon2 + tasDBpass;
            tasDBconnect = strCon + tasDBpath + tasDBpass;
            tasDB = new OleDbConnection(tasDBconnect);

            dbReader.Close();*/

            dbSQL = new OleDbCommand("SELECT * FROM tblWindowPositions WHERE ID = 1;", settingsDB);
            dbReader = dbSQL.ExecuteReader();
            dbReader.Read();
            Title = dbReader.GetString(1);
            Top = dbReader.GetInt32(2);
            Left = dbReader.GetInt32(3);
            Height = dbReader.GetInt32(4);
            Width = dbReader.GetInt32(5);

            dbReader.Close();

            OpenMainDBConnection();

            //fill login list with only active users
            FillLogin();

            //create lists from all of the necessary tables, the lists are used for filling combo boxes and a variety of other things
            //GetListItems takes <classtype>(database,sqlstatement,classtype,sortby,out variable, addblank)
            
            GetListItems<casestatus>(cidDB, "SELECT ID, Status FROM tbl_case_status;", "Status", new casestatus(), out Statuses, false);
            GetListItems<receivedmethod>(cidDB, "SELECT ID, Label FROM tbl_ReceivedMethods;", "ID", new receivedmethod(), out ReceivedMethods, true);
            GetListItems<state>(cidDB, "SELECT ID, Abbreviation, State FROM tbl_States;", "Abbr", new state(), out States, true);
            GetListItems<county>(cidDB, "SELECT ID, County, State, NorthBound, SouthBound, EastBound, WestBound FROM tbl_Counties;", "Name", new county(), out Counties, true);
            GetListItems<township>(cidDB, "SELECT ID, Township, Restricted, Appendix_A FROM tbl_Townships;", "Name", new township(), out Townships, true);
            GetListItems<city>(cidDB, "SELECT ID, City, County, DefaultTownship, Townships, Zips FROM tbl_Cities;", "Name", new city(), out Cities, true);
            GetListItems<FireDepartment>(cidDB, "SELECT ID, FDName, FDAddress1, FDAddress2, FDCity, FDZip, FDPhone, Email FROM tbl_FireDepartments;", "FDName", new FireDepartment(), out FireDepartments, true);
            GetListItems<TaskName>(cidDB, "SELECT ID, Task_Name, Task_Type FROM tbl_Task_Names;", "Name", new TaskName(), out AllTasks, false);
            GetListItems<CETAtype>(cidDB, "SELECT ID, CETA FROM tbl_CETA_types", "Name", new CETAtype(), out CETATypes, false);
            GetListItems<ComplaintCategory>(cidDB, "SELECT ID, Category FROM tbl_Complaint_Categories", "Category", new ComplaintCategory(), out ComplaintCategories, false);
            GetListItems<ComplaintLocation>(cidDB, "SELECT ID, Location FROM tbl_Complaint_Locations", "Location", new ComplaintLocation(), out ComplaintLocations, false);
            GetListItems<ComplaintType>(cidDB, "SELECT ID, Table_Name, Label, Site_Table, PrimaryContactTable, SecondaryContactTable, TertiaryContactTable, QuaternaryContactTable,"
                + "QuinaryContactTable, SenaryContactTable, SearchTerms, DefaultStarsType, StarsTypes, RetentionMonths, Category, Location FROM tbl_Complaint_Types;", "Label", new ComplaintType(), out ComplaintTypes, false);
            GetListItems<ZipCode>(cidDB, "SELECT ID, ZipCode, DefaultCity FROM tbl_ZipCodes;", "Zip", new ZipCode(), out ZipCodeList, true);
            GetSearchLocs();

            GetListItems<OperatingStatus>(cidDB, "SELECT * FROM tbl_Operating_Status;", "Status", new OperatingStatus(), out OperatingStatuses, true);
            GetListItems<permittingclassification>(cidDB, "SELECT StarsIndex, Permitting_Classification from tbl_Permit_Class;", "Classification", new permittingclassification(), out PermittingClassifications, true);
            GetListItems<Facility>(cidDB, "SELECT * FROM tbl_Facilities;", "FacilityID", new Facility(), out Facilities, true);
            GetListItems<FacilityContact>(cidDB, "SELECT a.ID, a.Fac_ID, b.Contact_Type, a.First_Name, a.Last_Name, a.Phone, a.Email FROM "
                + "tbl_Fac_Contacts as a LEFT JOIN tbl_Contact_Type as b ON a.Contact_type = b.ID;", "LName", new FacilityContact(), out FacilityContacts, true);
            
            cidDB.Close();
        }

        public static bool IsMainDBConnected()
        {
            MainWindow.cidDB.ResetState();
            if (MainWindow.cidDB.State == System.Data.ConnectionState.Broken || MainWindow.cidDB.State == System.Data.ConnectionState.Closed) return false;
            else if (MainWindow.cidDB.State == System.Data.ConnectionState.Broken)
            {
                MainWindow.cidDB.Close();
                return false;
            }

            return true;
        }

        public static void CloseMainDBConnection()
        { if (MainWindow.cidDB.State != System.Data.ConnectionState.Closed) MainWindow.cidDB.Close(); }

        public static void OpenMainDBConnection()
        {
            try { if (!IsMainDBConnected()) MainWindow.cidDB.Open(); }
            catch { MainWindow.TryAgain(MainWindow.cidDB, "Main"); }
        }

        public static bool IsSettingsDBConnected()
        {
            MainWindow.settingsDB.ResetState();
            if (MainWindow.settingsDB.State == System.Data.ConnectionState.Broken || MainWindow.settingsDB.State == System.Data.ConnectionState.Closed) return false;
            else if (MainWindow.settingsDB.State == System.Data.ConnectionState.Broken)
            {
                MainWindow.settingsDB.Close();
                return false;
            }

            return true;
        }

        public static void CloseSettingsDBConnection()
        { if (MainWindow.settingsDB.State != System.Data.ConnectionState.Closed) MainWindow.settingsDB.Close(); }

        public static void OpenSettingsDBConnection()
        {
            try { MainWindow.settingsDB.Open(); }
            catch { MainWindow.TryAgain(MainWindow.settingsDB, "Settings"); }
        }

        private void GetSearchLocs()
        {
            //catalog the columns that combine to make up a complaint so the application knows how to search the fields later
            SearchList = new List<SearchLocation>();

            foreach(ComplaintType x in ComplaintTypes)
            {
                if (x.Label != "temp")
                {
                    string strSQL = "SELECT TOP 1 * FROM " + x.ComplaintTable + ";";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(strSQL, cidDB);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    SearchLocation loc = new SearchLocation(x);
                    foreach (DataColumn y in table.Columns)
                    { loc.AddColumn(y.ColumnName, "a", y.DataType); }
                    //{ loc.AddColumn(y.ColumnName, y.Table.TableName, y.DataType); }

                    strSQL = "SELECT TOP 1 * FROM tbl_Complaints;";
                    adapter = new OleDbDataAdapter(strSQL, cidDB);
                    table = new DataTable();
                    adapter.Fill(table);

                    foreach (DataColumn y in table.Columns)
                    { loc.AddColumn(y.ColumnName, "b", y.DataType); }

                    strSQL = "SELECT TOP 1 * FROM " + x.SiteTable + ";";
                    adapter = new OleDbDataAdapter(strSQL, cidDB);
                    table = new DataTable();
                    adapter.Fill(table);

                    foreach (DataColumn y in table.Columns)
                    { loc.AddColumn(y.ColumnName, "c", y.DataType); }

                    if (x.PrimaryContactTable != "null")
                    {
                        strSQL = "SELECT TOP 1 * FROM " + x.PrimaryContactTable + ";";
                        adapter = new OleDbDataAdapter(strSQL, cidDB);
                        table = new DataTable();
                        adapter.Fill(table);

                        foreach (DataColumn y in table.Columns)
                        { loc.AddColumn(y.ColumnName, "d", y.DataType); }
                    }

                    if (x.SecondaryContactTable != "null")
                    {
                        strSQL = "SELECT TOP 1 * FROM " + x.SecondaryContactTable + ";";
                        adapter = new OleDbDataAdapter(strSQL, cidDB);
                        table = new DataTable();
                        adapter.Fill(table);

                        foreach (DataColumn y in table.Columns)
                        { loc.AddColumn(y.ColumnName, "e", y.DataType); }
                    }

                    if (x.TertiaryContactTable != "null")
                    {
                        strSQL = "SELECT TOP 1 * FROM " + x.TertiaryContactTable + ";";
                        adapter = new OleDbDataAdapter(strSQL, cidDB);
                        table = new DataTable();
                        adapter.Fill(table);

                        foreach (DataColumn y in table.Columns)
                        { loc.AddColumn(y.ColumnName, "f", y.DataType); }
                    }

                    if (x.QuaternaryContactTable != "null")
                    {
                        strSQL = "SELECT TOP 1 * FROM " + x.QuaternaryContactTable + ";";
                        adapter = new OleDbDataAdapter(strSQL, cidDB);
                        table = new DataTable();
                        adapter.Fill(table);

                        foreach (DataColumn y in table.Columns)
                        { loc.AddColumn(y.ColumnName, "g", y.DataType); }
                    }

                    if (x.QuinaryContactTable != "null")
                    {
                        strSQL = "SELECT TOP 1 * FROM " + x.QuinaryContactTable + ";";
                        adapter = new OleDbDataAdapter(strSQL, cidDB);
                        table = new DataTable();
                        adapter.Fill(table);

                        foreach (DataColumn y in table.Columns)
                        { loc.AddColumn(y.ColumnName, "h", y.DataType); }
                    }

                    if (x.SenaryContactTable != "null")
                    {
                        strSQL = "SELECT TOP 1 * FROM " + x.SenaryContactTable + ";";
                        adapter = new OleDbDataAdapter(strSQL, cidDB);
                        table = new DataTable();
                        adapter.Fill(table);

                        foreach (DataColumn y in table.Columns)
                        { loc.AddColumn(y.ColumnName, "i", y.DataType); }
                    }

                    strSQL = "SELECT TOP 1 * FROM tbl_Complainants;";
                    adapter = new OleDbDataAdapter(strSQL, cidDB);
                    table = new DataTable();
                    adapter.Fill(table);

                    foreach (DataColumn y in table.Columns)
                    { loc.AddColumn(y.ColumnName, "j", y.DataType); }

                    SearchList.Add(loc);
                }
            }
        }

        private void btnCompImport_Click(object sender, RoutedEventArgs e)
        {
            CIDImport window = new CIDImport();
            window.ShowDialog();
        }

        public static Address GetAddress(ComplaintType type, out string facname, int subid)
        {
            //Get the address from the database, create an address object and return it. The ComplaintType object stores the table name for the addresses associated with the complaint type
            //this creates a generic address without the specifics that might be associated with each complaint type
            string strSQL = "SELECT a.SiteName, b.Address, b.City, b.Zip FROM " + type.ComplaintTable + " AS a LEFT JOIN "
                + type.SiteTable + " AS b ON b.ID = a.Site WHERE a.ID = " + subid.ToString() + ";";
            OleDbCommand cidCMD = new OleDbCommand(strSQL, cidDB);
            OleDbDataReader cidReader = cidCMD.ExecuteReader();

            facname = "";
            Address thisAddress = new Address();

            while (cidReader.Read())
            {
                facname = (cidReader[0] != DBNull.Value) ? cidReader.GetString(0) : "";
                thisAddress.AddressLine1 = (cidReader[1] != DBNull.Value) ? cidReader.GetString(1) : "";
                if (cidReader[2] != DBNull.Value)
                {
                    if (cidReader[2] is Int32)
                    {
                        city thisCity = new city();
                        MainWindow.GetSingleItem<city>(out thisCity, cidReader.GetInt32(2), MainWindow.Cities);
                        thisAddress.City = thisCity.Name;
                    }
                    else if (cidReader[2] is string) thisAddress.City = cidReader.GetString(2);
                }
                if (cidReader[3] != DBNull.Value)
                {
                    if (cidReader[3] is string) thisAddress.Zip = cidReader.GetString(3);
                    else if (cidReader[3] is Int32) thisAddress.Zip = cidReader.GetInt32(3).ToString();
                }
                state thisState = new state();
                MainWindow.GetSingleItem<state>(out thisState, 41, MainWindow.States);//cidReader.GetInt32(9), MainWindow.States);
                thisAddress.State = thisState;
            }

            cidReader.Close();

            return thisAddress;
        }

        public static void GetListItems<T>(OleDbConnection db, string strSQL, string strSort, T obj, out ListCollectionView lst, bool addblank)
        {
            //create a list of items read from the database and add a blank item if necessary then populate the ListCollectionView
            lst = null;

            OleDbCommand dbSQL = new OleDbCommand(strSQL, db);
            OleDbDataReader dbReader = dbSQL.ExecuteReader();

            //as long as the database returns items then DataReaderMapToList is defined in each type<T> and does the actual work of creating the objects and List<T>
            if (dbReader.HasRows)
            {
                List<T> objList = new List<T>();
                objList = DBobject.DataReaderMapToList<T>(dbReader, obj);
                lst = new ListCollectionView(objList);
                lst.SortDescriptions.Add(new SortDescription(strSort, ListSortDirection.Ascending));
            }

            //add a blank item for fields that are allowed to be null
            if (addblank)
            {
                //the actual creation of null objects is defined as NullItem in the type<T>
                var mi = typeof(T).GetMethod("NullItem");
                var miref = mi.MakeGenericMethod(typeof(T));
                T herp = (T)miref.Invoke(obj, new object[] { obj });
                lst.AddNewItem(herp);
                lst.CommitNew();
            }

            if (dbReader.HasRows) lst.SortDescriptions.Add(new SortDescription(strSort, ListSortDirection.Ascending));
            dbReader.Close();
            dbReader = null;
            dbSQL = null;
        }

        public static void GetSingleItem<T>(out T item, int id, ListCollectionView lst)
        {
            //returns an object from a listcollection based on it's id
            item = default(T);

            var source = lst.Cast<T>();
            PropertyInfo x = typeof(T).GetProperty("ID");
            IEnumerable<T> query = (from A in source where (int)x.GetValue(A) == id select A);
            item = (T)query.FirstOrDefault();
        }

        private void FillLogin()
        {
            //adds only active users to the login combobox

            //user role 9 is for unknown users in the time accounting database
            //not my fault, I didn't create or maintain the original data
            string strSQL = "SELECT ID1, Field1, User_Role, Password, Active, First_Name, Last_Name";
            strSQL += " FROM tbl_Employees WHERE User_Role < 8;";

            OleDbCommand tasSQL = new OleDbCommand(strSQL, cidDB);
            OleDbDataReader tasReader = tasSQL.ExecuteReader();

            //get complete user list
            List<user> userList = new List<user>();
            userList = user.DataReaderMapToList<user>(tasReader);
            Users = new ListCollectionView(userList);
            Users.SortDescriptions.Add(new SortDescription("Active", ListSortDirection.Descending));
            Users.SortDescriptions.Add(new SortDescription("Inits", ListSortDirection.Ascending));
            UsersList.UsersCollection = Users;
            UsersList.UsersCollection.AddNewItem(new user(0, "", 0, "zxdcvcxvz", true, "", ""));
            UsersList.UsersCollection.CommitNew();

            //get active user list
            List<user> ACTIVElist = new List<user>();
            foreach (user USERx in UsersList.UsersCollection)
            { if ((bool)USERx.Active) ACTIVElist.Add(USERx); }
    
            ActiveUsers = new ListCollectionView(ACTIVElist);
            ActiveUsers.SortDescriptions.Add(new SortDescription("Inits", ListSortDirection.Ascending));
            COMBOuserlist.ItemsSource = ActiveUsers;
            COMBOuserlist.SelectedIndex = 0;

            tasReader.Close();
        }

        private void BUTTONlogin_Click(object sender, RoutedEventArgs e)
        {
            //if the passwword in the selected user object matches what is in the box, then login
            string passW = (string)((user)(COMBOuserlist.SelectedItem)).Pass;
            string passBox = BOXpassword.Password;

            if (passW == passBox)
            {
                thisUser = (user)(COMBOuserlist.SelectedItem);
                if (thisUser.Role > 3)
                {
                    foreach(casestatus stat in Statuses)
                    { if (stat.ID == 2) stat.Active = false; }
                }
                LoginGrid.Visibility = System.Windows.Visibility.Hidden;
                LandingGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            { System.Windows.MessageBox.Show("The password is incorrect. This is now case sensitive. Is capslock on?", "Oops", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
        }

        private void BUTTONwoot_Click(object sender, RoutedEventArgs e)
        {
            //this is a test button to get back to the login
            LandingGrid.Visibility = System.Windows.Visibility.Hidden;
            LoginGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //after the window loads then all of the combobox controls are intialized and ready to be populated
            List<casestatus> statusList = new List<casestatus>();
            foreach (casestatus qqq in MainWindow.Statuses) { statusList.Add(qqq); }
            statusList.Add(new casestatus(0, "All Statuses", true));
            ListCollectionView berp = new ListCollectionView(statusList);
            berp.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Ascending));
            cboCompReportsStatus.ItemsSource = berp;
            cboCompReportsStatus.SelectedValue = 1;
            
            List<ComplaintCategory> categoryList = new List<ComplaintCategory>();
            foreach (ComplaintCategory ccc in MainWindow.ComplaintCategories) { categoryList.Add(ccc); }
            categoryList.Add(new ComplaintCategory(0, "All Categories"));
            ListCollectionView herp = new ListCollectionView(categoryList);
            herp.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Ascending));
            cboCompReportsCategory.ItemsSource = herp;
            cboCompReportsCategory.SelectedIndex = 0;

            List<ComplaintLocation> locationList = new List<ComplaintLocation>();
            foreach (ComplaintLocation lll in MainWindow.ComplaintLocations) { locationList.Add(lll); }
            locationList.Add(new ComplaintLocation(0, "All Locations"));
            ListCollectionView derp = new ListCollectionView(locationList);
            derp.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Ascending));
            cboCompReportsLocation.ItemsSource = derp;
            cboCompReportsLocation.SelectedIndex = 0;

            UpdateComplaintChart();

            cboInspReports.ItemsSource = derp;
            cboInspReports.SelectedIndex = 0;

            //Create the Inspection chart
            Chart ChartInsp = new Chart();
            ChartInsp.ChartAreas.Add("Default");
            Series InspSeries = new Series();
            InspSeries.Points.Add(5);
            InspSeries.Points.Add(10);
            ChartInsp.Series.Add(InspSeries);

            // Assign the Chart controls as the host control's child.
            InspHost.Child = ChartInsp;

            COMBOuserlist.Focus();
            Keyboard.Focus(COMBOuserlist);
        }

        private void BUTTONwoot2_Click(object sender, RoutedEventArgs e)
        {
            //another test button, opens the residential open burn window without any data
            OBResCompForm form = new OBResCompForm();
            try { form.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form.Close(); return; }
        }

        private void btnCompNew_Click(object sender, RoutedEventArgs e)
        {
            //opens the complaint wizard to create a new complaint
            ComplaintWizard wiz = new ComplaintWizard();
            try { wiz.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); wiz.Close(); return; }
        }

        private void btnInspNew_Click(object sender, RoutedEventArgs e)
        {
            //opens the inspection wizard to create a new inspection
            InspectionWizard wiz = new InspectionWizard();
            try { wiz.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); wiz.Close(); return; }
        }

        private void btnCompOpen_Click(object sender, RoutedEventArgs e)
        {
            //open the open complaints report, passing (1) tells the form to only show complaints with the status Open
            OpenComplaints comp = new OpenComplaints(1);
            try { comp.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); comp.Close(); return; }
        }

        private void btnCompClosed_Click(object sender, RoutedEventArgs e)
        {
            //open the open complaints report, passing (2) tells the form to only show complaints with the status Closed
            OpenComplaints comp = new OpenComplaints(2);
            try { comp.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); comp.Close(); return; }
        }

        private void btnCompAll_Click(object sender, RoutedEventArgs e)
        {
            //open the open complaints report, passing (0) tells the form to show complaints regardless of the status
            OpenComplaints comp = new OpenComplaints(0);
            try { comp.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); comp.Close(); return; }
        }

        public static bool IsNumeric(string input)
        {
            //function is available globally by calling MainWindow.IsNumeric(string)
            //returns false if the string contains any non-numeric characters
            double num;
            return double.TryParse(input, out num);
        }

        public static bool HasInternet()
        {
            //function is available globally by calling MainWindow.HasInternet()
            //returns false if the PC does not have an active internet connection
            //for some reason it's hard to learn of the existence of this function via google
            ConnectionState Description = 0;
            return InternetGetConnectedState(ref Description, 0);
        }

        public static string[] GetLocFromZip(string zipcode)
        {
            string[] strDoc = new string[2];
            strDoc[0] = strDoc[1] = "";

            //if there's an active internet connection and the zipcode is at least long enough then send the info to google and parse the results it sends back
            //otherwise just send back null strings
            if (HasInternet() && zipcode.Length >= 5)
            {
                string strURL = "http://maps.googleapis.com/maps/api/geocode/json?address=" + zipcode + "&sensor=false";

                WebRequest request = WebRequest.Create(strURL);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string strDocX = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                //parese the city name out of the response
                int pos = strDocX.IndexOf("\"types\" : [ \"locality\", \"political\" ]");
                if (pos > 0)
                {
                    strDoc[0] = strDocX.Substring(0, pos);
                    pos = strDoc[0].LastIndexOf("\",");
                    strDoc[0] = strDoc[0].Substring(0, pos);
                    pos = strDoc[0].LastIndexOf('"');
                    strDoc[0] = strDoc[0].Substring((pos + 1), (strDoc[0].Length - pos - 1));
                }

                //parse the township out of the response
                pos = strDocX.IndexOf("\"types\" : [ \"administrative_area_level_1\", \"political\" ]");
                if (pos > 0)
                {
                    strDoc[1] = strDocX.Substring(0, pos);
                    pos = strDoc[1].LastIndexOf("\"short_name\" : \"") + 16;
                    strDoc[1] = strDoc[1].Substring(pos, (strDoc[1].Length - pos));
                    pos = strDoc[1].LastIndexOf('"');
                    strDoc[1] = strDoc[1].Substring(0, pos);
                }
            }

            return strDoc;
        }
       
        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        // NOTE: Leave out the finalizer altogether if this class doesn't 
        // own unmanaged resources itself, but leave the other methods
        // exactly as they are. 
        ~MainWindow() 
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }
        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) 
            {
                // free managed resources
                if (cidDB != null)
                {
                    cidDB.Dispose();
                    cidDB = null;
                }
                if (settingsDB != null)
                {
                    settingsDB.Dispose();
                    settingsDB = null;
                }
                /*if (tasDB != null)
                {
                    tasDB.Dispose();
                    tasDB = null;
                }*/
                if (ChartComp != null)
                {
                    ChartComp.Dispose();
                    ChartComp = null;
                }
            }
        }
        
        //allow the enter key in the password text box in place of clicking the button
        private void BOXpassword_KeyDown(object sender, KeyEventArgs e)
        { if ((e.Key == Key.Return) || (e.Key == Key.Enter)) BUTTONlogin_Click(sender, e); }

        //if the comboxbox selection changes then call the update function
        private void cboCompReports_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        { UpdateComplaintChart(); }

        private void UpdateComplaintChart()
        {
            if (cboCompReportsCategory.ItemsSource != null && cboCompReportsLocation.ItemsSource != null && cboCompReportsStatus.ItemsSource != null)
            {
                try { cidDB.Open(); }
                catch { MainWindow.TryAgain(cidDB, "Main"); }

                ChartComp = new Chart();
                ChartComp.Customize += ChartComp_Customize;

                ChartComp.ChartAreas.Add("Default");
                Series CompSeries = new Series();
                CompSeries.XValueType = ChartValueType.Int32;
                CompSeries.YValueType = ChartValueType.String;

                string strSQLa, strSQLb;
                strSQLa = strSQLb = "";

                switch ((int)cboCompReportsCategory.SelectedValue)
                {
                    case 0:
                        strSQLb += "";
                        break;
                    default:
                        strSQLb = " WHERE b.Category = " + ((int)cboCompReportsCategory.SelectedValue).ToString();
                        break;
                }

                switch ((int)cboCompReportsLocation.SelectedValue)
                {
                    case 0:
                        strSQLb += "";
                        break;
                    default:
                        if (strSQLb != "") strSQLb += " AND";
                        else strSQLb += " WHERE";
                        strSQLb += " b.Location = " + ((int)cboCompReportsLocation.SelectedValue).ToString();
                        break;
                }

                switch ((int)cboCompReportsStatus.SelectedValue)
                {
                    case 0:
                        strSQLb += "";
                        break;
                    default:
                        if (strSQLb != "") strSQLb += " AND";
                        else strSQLb += " WHERE";
                        strSQLb += " a.Status = " + ((int)cboCompReportsStatus.SelectedValue).ToString();
                        break;
                }

                strSQLa = "SELECT COUNT(*), Complaint_Type FROM tbl_Complaints as a LEFT JOIN tbl_Complaint_Types as b ON a.Complaint_type = b.ID"
                    + strSQLb + " GROUP BY Complaint_Type;";
                OleDbCommand cidCMD = new OleDbCommand(strSQLa, cidDB);
                OleDbDataReader cidReader = cidCMD.ExecuteReader();
                
                while (cidReader.Read())
                {

                    int a = (cidReader[0] != DBNull.Value) ? cidReader.GetInt32(0) : 0;
                    ComplaintType c = new ComplaintType();
                    GetSingleItem<ComplaintType>(out c, cidReader.GetInt32(1), ComplaintTypes);
                    //CompSeries.Points.AddXY(c.Label, a);
                    CompSeries.Points.AddY(a);
                    int b = CompSeries.Points.Count;
                    b = (b > 9) ? b % 10 : b;
                    CompSeries.Points[CompSeries.Points.Count-1].Color = System.Drawing.Color.FromArgb(colors[b].A, colors[b].R, colors[b].G, colors[b].B);
                    CompSeries.Points[CompSeries.Points.Count-1].AxisLabel = c.Label;
                }

                ChartComp.ChartAreas[0].AxisX.Interval = 1;
                cidReader.Close();

                ChartComp.Series.Add(CompSeries);
                CompHost.Child = ChartComp;
            
                cidDB.Close();
            }
        }

        private void ChartComp_Customize(object sender, EventArgs e)
        {
            List<CustomLabel> list = new List<CustomLabel>();
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("^\\d+$");

            foreach (CustomLabel l in ChartComp.ChartAreas[0].AxisY.CustomLabels)
            { if (!r.IsMatch(l.Text)) list.Add(l); }

            if (list.Count > 0)
            {
                foreach (CustomLabel l in list)
                    ChartComp.ChartAreas[0].AxisY.CustomLabels.Remove(l);

                ChartComp.ChartAreas[0].AxisY.Interval = 1;
            }
        }

        public Contact GetComplainant(GenericComplaint comp, bool fdgen)
        {
            if (!comp.Anonymous && !fdgen)
            {
                string strSQL = "SELECT * from tbl_Complainants WHERE ID = " + comp.Complainant.ToString() + ";";
                OleDbCommand cidSQL = new OleDbCommand(strSQL, cidDB);
                OleDbDataReader cidReader = cidSQL.ExecuteReader();
                cidReader.Read();

                state astate = new state();
                if (cidReader[6] != DBNull.Value) MainWindow.GetSingleItem<state>(out astate, cidReader.GetInt32(6), MainWindow.States);

                int cid = (cidReader[0] != DBNull.Value) ? cidReader.GetInt32(0) : 0;
                
                string lname = (cidReader[1] != DBNull.Value) ? cidReader.GetString(1) : "";
                string fname = (cidReader[2] != DBNull.Value) ? cidReader.GetString(2) : "";
                string add1 = (cidReader[3] != DBNull.Value) ? cidReader.GetString(3) : "";
                string add2 = (cidReader[4] != DBNull.Value) ? cidReader.GetString(4) : "";
                string city = (cidReader[5] != DBNull.Value) ? cidReader.GetString(5) : "";
                string zip = (cidReader[7] != DBNull.Value) ? cidReader.GetString(7) : "";
                string email = (cidReader[8] != DBNull.Value) ? cidReader.GetString(8) : "";
                string phone = (cidReader[9] != DBNull.Value) ? cidReader.GetString(9) : "";

                Contact contact = new Contact(cid, fname, lname, add1, add2, city, astate, zip, email, phone, "tbl_Complainants", "Complainant", "tbl_Complaints");
                cidReader.Close();

                return contact;
            }
            else return new Contact();
        }

        public GenericComplaint StartFillingComplaint(int ID)
        {
            //Create some objects
            string strSQL = "SELECT * FROM tbl_Complaints WHERE ID = " + ID.ToString();
            OleDbCommand cidSQL = new OleDbCommand(strSQL, cidDB);
            OleDbDataReader cidReader = cidSQL.ExecuteReader();
            cidReader.Read();
            GenericComplaint comp = CreateGenericComplaint(cidReader);
            cidReader.Close();

            return comp;
        }

        public OBOtherComplaint CreateOBOther(int ID)
        {
            //create and return a source unknown open burning complaint object based on its ID
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            FireDepartment fdinfo = new FireDepartment();

            //Create some objects
            object[] z = FillDBObject(comp);

            bool fdgenerated = Convert.ToBoolean(z[2]);
            //Get fire dept object
            if (z[3] != DBNull.Value) MainWindow.GetSingleItem<FireDepartment>(out fdinfo, Convert.ToInt32(z[3]), MainWindow.FireDepartments);

            OtherLocation complaintaddress = GetOtherSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, fdgenerated));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new OBOtherComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, complainant, comp.Anonymous,
                fdinfo, fdgenerated, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy, comp.Inspector,
                comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type, comp.LastModified,
                comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted);
        }

        public OBNonPermFacComplaint CreateOBNonFac(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);
            //Get Facility name
            string facname = (z[6] != DBNull.Value) ? Convert.ToString(z[6]) : "";

            bool fdgenerated = Convert.ToBoolean(z[4]);
            //Get occupant contact object
            Occupant occupant = (z[2] != DBNull.Value) ? new Occupant(GetContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact", comp.Type.ComplaintTable)) : new Occupant();
            //Get owner contact object
            Owner owner = (z[3] != DBNull.Value) ? new Owner(GetContact(Convert.ToInt32(z[3]), comp.Type.SecondaryContactTable, "SecondaryContact", comp.Type.ComplaintTable)) : new Owner();
            //Get fire dept object
            FireDepartment fdinfo = new FireDepartment();
            if (z[5] != DBNull.Value) MainWindow.GetSingleItem<FireDepartment>(out fdinfo, Convert.ToInt32(z[5]), MainWindow.FireDepartments);

            NonPermittedFacLocation complaintaddress = GetNonPermFacSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, fdgenerated));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new OBNonPermFacComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, occupant, owner, complainant,
                comp.Anonymous, fdinfo, fdgenerated, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy, comp.Inspector,
                comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type, comp.LastModified, comp.MethodReceived,
                dates[3], comp.AppendixA, comp.Restricted, facname);
        }

        public OtherFacilityComplaint CreateOtherFacility(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);

            //Get facility contact object
            FacilityContact contact = new FacilityContact();
            if (z[2] != DBNull.Value) contact = GetFacilityContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact");
            string facilityname = (z[9] != DBNull.Value) ? Convert.ToString(z[9]) : "";

            Facility thisFac = new Facility();
            GetSingleItem<Facility>(out thisFac, Convert.ToInt32(z[1]), Facilities);

            //Get complaint specific portable site location(this preserves the location historically if the portable source moves in the future)
            OtherLocation portableloc = (z[4] != DBNull.Value) ? GetOtherSite(Convert.ToInt32(z[4]), "tbl_Complaint_Portable_Sites") : new OtherLocation();

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new OtherFacilityComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, thisFac, contact, complainant,
                comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy, comp.Inspector,
                comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type, comp.LastModified,
                comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted, facilityname, portableloc);
        }

        public DustFacilityComplaint CreateDustFacility(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);

            //Get facility contact object
            FacilityContact contact = new FacilityContact();
            if (z[2] != DBNull.Value) contact = GetFacilityContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact");
            string facilityname = (z[3] != DBNull.Value) ? Convert.ToString(z[3]) : "";

            Facility thisFac = new Facility();
            GetSingleItem<Facility>(out thisFac, Convert.ToInt32(z[1]), Facilities);

            //Get complaint specific portable site location(this preserves the location historically if the portable source moves in the future)
            OtherLocation portableloc = (z[4] != DBNull.Value) ? GetOtherSite(Convert.ToInt32(z[4]), "tbl_Complaint_Portable_Sites") : new OtherLocation();

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new DustFacilityComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, thisFac, contact, complainant,
                comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy, comp.Inspector,
                comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type, comp.LastModified,
                comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted, facilityname, portableloc);
        }

        public OdorFacilityComplaint CreateOdorFacility(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);

            //Get facility contact object
            FacilityContact contact = new FacilityContact();
            if (z[2] != DBNull.Value) contact = GetFacilityContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact");
            string facilityname = (z[3] != DBNull.Value) ? Convert.ToString(z[3]) : "";

            Facility thisFac = new Facility();
            GetSingleItem<Facility>(out thisFac, Convert.ToInt32(z[1]), Facilities);

            //Get complaint specific portable site location(this preserves the location historically if the portable source moves in the future)
            OtherLocation portableloc = (z[4] != DBNull.Value) ? GetOtherSite(Convert.ToInt32(z[4]), "tbl_Complaint_Portable_Sites") : new OtherLocation();

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new OdorFacilityComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, thisFac, contact, complainant,
                comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy, comp.Inspector,
                comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type, comp.LastModified,
                comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted, facilityname, portableloc);
        }

        public ReleaseFacilityComplaint CreateReleaseFacility(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);

            //Get facility contact object
            FacilityContact contact = new FacilityContact();
            if (z[2] != DBNull.Value) contact = GetFacilityContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact");
            string facilityname = (z[3] != DBNull.Value) ? Convert.ToString(z[3]) : "";

            Facility thisFac = new Facility();
            GetSingleItem<Facility>(out thisFac, Convert.ToInt32(z[1]), Facilities);

            //Get complaint specific portable site location(this preserves the location historically if the portable source moves in the future)
            OtherLocation portableloc = (z[4] != DBNull.Value) ? GetOtherSite(Convert.ToInt32(z[4]), "tbl_Complaint_Portable_Sites") : new OtherLocation();

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new ReleaseFacilityComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, thisFac, contact, complainant,
                comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy, comp.Inspector,
                comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type, comp.LastModified,
                comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted, facilityname, portableloc);
        }

        public OBFacilityComplaint CreateOBFacility(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);

            bool fdgenerated = Convert.ToBoolean(z[3]);
            
            //Get facility contact object
            FacilityContact contact = new FacilityContact();
            if (z[2] != DBNull.Value) contact = GetFacilityContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact");
            
            //Get complaint specific facility name(this preserves the name historically if the facility name changes in the future
            string facilityname = (z[5] != DBNull.Value) ? Convert.ToString(z[5]) : "";

            //Get complaint specific portable site location(this preserves the location historically if the portable source moves in the future)
            OtherLocation portableloc = (z[6] != DBNull.Value) ? GetOtherSite(Convert.ToInt32(z[6]), "tbl_Complaint_Portable_Sites") : new OtherLocation();

            //Get fire dept object
            FireDepartment fdinfo = new FireDepartment();
            if (z[4] != DBNull.Value) MainWindow.GetSingleItem<FireDepartment>(out fdinfo, Convert.ToInt32(z[4]), MainWindow.FireDepartments);

            Facility thisFac = new Facility();
            GetSingleItem<Facility>(out thisFac, Convert.ToInt32(z[1]), Facilities);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, fdgenerated));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new OBFacilityComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, thisFac, contact, complainant,
                comp.Anonymous, fdinfo, fdgenerated, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy,
                comp.Inspector, comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type, comp.LastModified,
                comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted, facilityname, portableloc);
        }

        public DustNonPermFacComplaint CreateDustNonPermFac(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);
            //Get Facility name
            string facname = (z[4] != DBNull.Value) ? Convert.ToString(z[4]) : "";

            //Get occupant contact object
            Occupant occupant = (z[2] != DBNull.Value) ? new Occupant(GetContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact", comp.Type.ComplaintTable)) : new Occupant();
            //Get owner contact object
            Owner owner = (z[3] != DBNull.Value) ? new Owner(GetContact(Convert.ToInt32(z[3]), comp.Type.SecondaryContactTable, "SecondaryContact", comp.Type.ComplaintTable)) : new Owner();

            NonPermittedFacLocation complaintaddress = GetNonPermFacSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new DustNonPermFacComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, occupant, owner,
                complainant, comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy,
                comp.Inspector, comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type, comp.LastModified,
                comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted, facname);
        }

        public AsbestosFacilityComplaint CreateAsbestosFacility(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);

            //Get ANTS ID
            string antsid = (z[8] != DBNull.Value) ? Convert.ToString(z[8]) : "";

            //Get facility contact object
            FacilityContact contact = new FacilityContact();
            if (z[2] != DBNull.Value) contact = GetFacilityContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact");
            DemoContractor demo = (z[3] != DBNull.Value) ? new DemoContractor(GetContact(Convert.ToInt32(z[3]), comp.Type.SecondaryContactTable, "SecondaryContact", comp.Type.ComplaintTable)) : new DemoContractor();
            AbatementContractor abatement = (z[4] != DBNull.Value) ? new AbatementContractor(GetContact(Convert.ToInt32(z[4]), comp.Type.TertiaryContactTable, "TertiaryContact", comp.Type.ComplaintTable)) : new AbatementContractor();
            EntityCoordinator entity = (z[5] != DBNull.Value) ? new EntityCoordinator(GetContact(Convert.ToInt32(z[5]), comp.Type.QuaternaryContactTable, "QuaternaryContact", comp.Type.ComplaintTable)) : new EntityCoordinator();
            Landfill landfill = (z[6] != DBNull.Value) ? new Landfill(GetContact(Convert.ToInt32(z[6]), comp.Type.QuinaryContactTable, "QuinaryContact", comp.Type.ComplaintTable)) : new Landfill();
            OtherOperator otheroperator = (z[7] != DBNull.Value) ? new OtherOperator(GetContact(Convert.ToInt32(z[7]), comp.Type.SenaryContactTable, "SenaryContact", comp.Type.ComplaintTable)) : new OtherOperator();

            Facility thisFac = new Facility();
            GetSingleItem<Facility>(out thisFac, Convert.ToInt32(z[1]), Facilities);
            string facilityname = (z[9] != DBNull.Value) ? Convert.ToString(z[10]) : "";

            //Get complaint specific portable site location(this preserves the location historically if the portable source moves in the future)
            OtherLocation portableloc = (z[10] != DBNull.Value) ? GetOtherSite(Convert.ToInt32(z[6]), "tbl_Complaint_Portable_Sites") : new OtherLocation();

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new AsbestosFacilityComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, thisFac, contact, demo, abatement,
                entity, landfill, otheroperator, complainant, comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes,
                comp.ReceivedBy, comp.Inspector, comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type,
                comp.LastModified, comp.MethodReceived, dates[3], antsid, comp.AppendixA, comp.Restricted, facilityname, portableloc);
        }

        public AsbestosNonPermFacComplaint CreateAsbestosNonPermFac(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);

            //Get ANTS ID
            string antsid = (z[8] != DBNull.Value) ? Convert.ToString(z[8]) : "";
            //Get Facility name
            string facname = (z[9] != DBNull.Value) ? Convert.ToString(z[9]) : "";

            //Get owner contact object
            Owner owner = (z[2] != DBNull.Value) ? new Owner(GetContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact", comp.Type.ComplaintTable)) : new Owner();
            DemoContractor demo = (z[3] != DBNull.Value) ? new DemoContractor(GetContact(Convert.ToInt32(z[3]), comp.Type.SecondaryContactTable, "SecondaryContact", comp.Type.ComplaintTable)) : new DemoContractor();
            AbatementContractor abatement = (z[4] != DBNull.Value) ? new AbatementContractor(GetContact(Convert.ToInt32(z[4]), comp.Type.TertiaryContactTable, "TertiaryContact", comp.Type.ComplaintTable)) : new AbatementContractor();
            EntityCoordinator entity = (z[5] != DBNull.Value) ? new EntityCoordinator(GetContact(Convert.ToInt32(z[5]), comp.Type.QuaternaryContactTable, "QuaternaryContact", comp.Type.ComplaintTable)) : new EntityCoordinator();
            Landfill landfill = (z[6] != DBNull.Value) ? new Landfill(GetContact(Convert.ToInt32(z[6]), comp.Type.QuinaryContactTable, "QuinaryContact", comp.Type.ComplaintTable)) : new Landfill();
            OtherOperator otheroperator = (z[7] != DBNull.Value) ? new OtherOperator(GetContact(Convert.ToInt32(z[7]), comp.Type.SenaryContactTable, "SenaryContact", comp.Type.ComplaintTable)) : new OtherOperator();

            NonPermittedFacLocation complaintaddress = GetNonPermFacSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new AsbestosNonPermFacComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, owner, demo, abatement,
                entity, landfill, otheroperator, complainant, comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes,
                comp.ReceivedBy, comp.Inspector, comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type,
                comp.LastModified, comp.MethodReceived, dates[3], antsid, comp.AppendixA, comp.Restricted, facname);
        }

        public AsbestosOtherComplaint CreateAsbestosOther(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);

            //Get ANTS ID
            string antsid = (z[6] != DBNull.Value) ? Convert.ToString(z[6]) : "";

            OtherLocation complaintaddress = GetOtherSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new AsbestosOtherComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, complainant,
                comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy, comp.Inspector,
                comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type, comp.LastModified,
                comp.MethodReceived, dates[3], antsid, comp.AppendixA, comp.Restricted);
        }

        public OtherNonPermFacComplaint CreateOtherNonPermFac(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);
            //Get Facility name
            string facname = (z[4] != DBNull.Value) ? Convert.ToString(z[4]) : "";

            //Get occupant contact object
            Occupant occupant = (z[2] != DBNull.Value) ? new Occupant(GetContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact", comp.Type.ComplaintTable)) : new Occupant();
            //Get owner contact object
            Owner owner = (z[3] != DBNull.Value) ? new Owner(GetContact(Convert.ToInt32(z[3]), comp.Type.SecondaryContactTable, "SecondaryContact", comp.Type.ComplaintTable)) : new Owner();

            NonPermittedFacLocation complaintaddress = GetNonPermFacSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));
            
            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new OtherNonPermFacComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, occupant, owner,
                complainant, comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy,
                comp.Inspector, comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type,
                comp.LastModified, comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted, facname);
        }

        public ReleaseNonPermFacComplaint CreateReleaseNonPermFac(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);
            //Get Facility name
            string facname = (z[4] != DBNull.Value) ? Convert.ToString(z[4]) : "";

            //Get occupant contact object
            Occupant occupant = (z[2] != DBNull.Value) ? new Occupant(GetContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact", comp.Type.ComplaintTable)) : new Occupant();
            //Get owner contact object
            Owner owner = (z[3] != DBNull.Value) ? new Owner(GetContact(Convert.ToInt32(z[3]), comp.Type.SecondaryContactTable, "SecondaryContact", comp.Type.ComplaintTable)) : new Owner();

            NonPermittedFacLocation complaintaddress = GetNonPermFacSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new ReleaseNonPermFacComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, occupant, owner,
                complainant, comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy,
                comp.Inspector, comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type,
                comp.LastModified, comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted, facname);
        }

        public OdorNonPermFacComplaint CreateOdorNonPermFac(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);
            //Get Facility name
            string facname = (z[4] != DBNull.Value) ? Convert.ToString(z[4]) : "";

            //Get occupant contact object
            Occupant occupant = (z[2] != DBNull.Value) ? new Occupant(GetContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact", comp.Type.ComplaintTable)) : new Occupant();
            //Get owner contact object
            Owner owner = (z[3] != DBNull.Value) ? new Owner(GetContact(Convert.ToInt32(z[3]), comp.Type.SecondaryContactTable, "SecondaryContact", comp.Type.ComplaintTable)) : new Owner();

            NonPermittedFacLocation complaintaddress = GetNonPermFacSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new OdorNonPermFacComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, occupant, owner,
                complainant, comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy,
                comp.Inspector, comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type,
                comp.LastModified, comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted, facname);
        }

        public DustOtherComplaint CreateDustOther(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);

            OtherLocation complaintaddress = GetOtherSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new DustOtherComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, complainant,
                comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy, comp.Inspector,
                comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type, comp.LastModified,
                comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted);
        }

        public OtherOtherComplaint CreateOtherOther(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);

            OtherLocation complaintaddress = GetOtherSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new OtherOtherComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, complainant,
                comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy, comp.Inspector,
                comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type, comp.LastModified,
                comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted);
        }

        public ReleaseOtherComplaint CreateReleaseOther(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);

            OtherLocation complaintaddress = GetOtherSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new ReleaseOtherComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, complainant,
                comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, dates[1], dates[2], comp.CETA, comp.OtherNotes,
                comp.ReceivedBy, comp.Inspector, comp.Status, comp.DateReceived, dates[0], comp.Type, comp.LastModified,
                comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted);
        }

        public OdorOtherComplaint CreateOdorOther(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);

            OtherLocation complaintaddress = GetOtherSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new OdorOtherComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, complainant,
                comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy, comp.Inspector,
                comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type, comp.LastModified,
                comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted);
        }

        public OtherResidentialComplaint CreateOtherResidential(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);
            //Get building description
            string bldgdesc = (z[4] != DBNull.Value) ? Convert.ToString(z[4]) : "";

            //Get occupant contact object
            Occupant occupant = (z[2] != DBNull.Value) ? new Occupant(GetContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact", comp.Type.ComplaintTable)) : new Occupant();
            //Get owner contact object
            Owner owner = (z[3] != DBNull.Value) ? new Owner(GetContact(Convert.ToInt32(z[3]), comp.Type.SecondaryContactTable, "SecondaryContact", comp.Type.ComplaintTable)) : new Owner();

            ResidentialLocation complaintaddress = GetResidentialSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new OtherResidentialComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, occupant, owner,
                complainant, comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy,
                comp.Inspector, comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type, comp.LastModified,
                comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted, bldgdesc);
        }

        public AsbestosResidentialComplaint CreateAsbestosResidential(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);

            //Get ANTS ID
            string antsid = (z[8] != DBNull.Value) ? Convert.ToString(z[8]) : "";
            //Get building description
            string bldgdesc = (z[9] != DBNull.Value) ? Convert.ToString(z[9]) : "";

            //Get owner contact object
            Owner owner = (z[3] != DBNull.Value) ? new Owner(GetContact(Convert.ToInt32(z[3]), comp.Type.PrimaryContactTable, "PrimaryContact", comp.Type.ComplaintTable)) : new Owner();
            DemoContractor demo = (z[3] != DBNull.Value) ? new DemoContractor(GetContact(Convert.ToInt32(z[3]), comp.Type.SecondaryContactTable, "SecondaryContact", comp.Type.ComplaintTable)) : new DemoContractor();
            AbatementContractor abatement = (z[4] != DBNull.Value) ? new AbatementContractor(GetContact(Convert.ToInt32(z[4]), comp.Type.TertiaryContactTable, "TertiaryContact", comp.Type.ComplaintTable)) : new AbatementContractor();
            EntityCoordinator entity = (z[5] != DBNull.Value) ? new EntityCoordinator(GetContact(Convert.ToInt32(z[5]), comp.Type.QuaternaryContactTable, "QuaternaryContact", comp.Type.ComplaintTable)) : new EntityCoordinator();
            Landfill landfill = (z[6] != DBNull.Value) ? new Landfill(GetContact(Convert.ToInt32(z[6]), comp.Type.QuinaryContactTable, "QuinaryContact", comp.Type.ComplaintTable)) : new Landfill();
            OtherOperator otheroperator = (z[7] != DBNull.Value) ? new OtherOperator(GetContact(Convert.ToInt32(z[7]), comp.Type.SenaryContactTable, "SenaryContact", comp.Type.ComplaintTable)) : new OtherOperator();

            ResidentialLocation complaintaddress = GetResidentialSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new AsbestosResidentialComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, owner, demo, abatement,
                entity, landfill, otheroperator, complainant, comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes,
                comp.ReceivedBy, comp.Inspector, comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type,
                comp.LastModified, comp.MethodReceived, dates[3], antsid, comp.AppendixA, comp.Restricted, bldgdesc);
        }

        public DustResidentialComplaint CreateDustResidential(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);
            //Get building description
            string bldgdesc = (z[4] != DBNull.Value) ? Convert.ToString(z[4]) : "";

            //Get occupant contact object
            Occupant occupant = (z[2] != DBNull.Value) ? new Occupant(GetContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact", comp.Type.ComplaintTable)) : new Occupant();
            //Get owner contact object
            Owner owner = (z[3] != DBNull.Value) ? new Owner(GetContact(Convert.ToInt32(z[3]), comp.Type.SecondaryContactTable, "SecondaryContact", comp.Type.ComplaintTable)) : new Owner();

            ResidentialLocation complaintaddress = GetResidentialSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new DustResidentialComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, occupant, owner,
                complainant, comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy,
                comp.Inspector, comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type,
                comp.LastModified, comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted, bldgdesc);
        }

        public ReleaseResidentialComplaint CreateReleaseResidential(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);
            //Get building description
            string bldgdesc = (z[4] != DBNull.Value) ? Convert.ToString(z[4]) : "";

            //Get occupant contact object
            Occupant occupant = (z[2] != DBNull.Value) ? new Occupant(GetContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact", comp.Type.ComplaintTable)) : new Occupant();
            //Get owner contact object
            Owner owner = (z[3] != DBNull.Value) ? new Owner(GetContact(Convert.ToInt32(z[3]), comp.Type.SecondaryContactTable, "SecondaryContact", comp.Type.ComplaintTable)) : new Owner();

            ResidentialLocation complaintaddress = GetResidentialSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new ReleaseResidentialComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, occupant, owner,
                complainant, comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy,
                comp.Inspector, comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type,
                comp.LastModified, comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted, bldgdesc);
        }

        public OdorResidentialComplaint CreateOdorResidential(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);
            //Get building description
            string bldgdesc = (z[4] != DBNull.Value) ? Convert.ToString(z[4]) : "";

            //Get occupant contact object
            Occupant occupant = (z[2] != DBNull.Value) ? new Occupant(GetContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact", comp.Type.ComplaintTable)) : new Occupant();
            //Get owner contact object
            Owner owner = (z[3] != DBNull.Value) ? new Owner(GetContact(Convert.ToInt32(z[3]), comp.Type.SecondaryContactTable, "SecondaryContact", comp.Type.ComplaintTable)) : new Owner();

            ResidentialLocation complaintaddress = GetResidentialSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, false));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new OdorResidentialComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, occupant, owner,
                complainant, comp.Anonymous, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes, comp.ReceivedBy,
                comp.Inspector, comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA, comp.Type,
                comp.LastModified, comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted, bldgdesc);
        }

        public OBResidentialComplaint CreateOBResidential(int ID)
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            GenericComplaint comp = StartFillingComplaint(ID);

            //Create some objects
            object[] z = FillDBObject(comp);
            //Get building description

            bool fdgenerated = Convert.ToBoolean(z[4]);
            //Get occupant contact object
            //Get occupant contact object
            Occupant occupant = (z[2] != DBNull.Value) ? new Occupant(GetContact(Convert.ToInt32(z[2]), comp.Type.PrimaryContactTable, "PrimaryContact", comp.Type.ComplaintTable)) : new Occupant();
            //Get owner contact object
            Owner owner = (z[3] != DBNull.Value) ? new Owner(GetContact(Convert.ToInt32(z[3]), comp.Type.SecondaryContactTable, "SecondaryContact", comp.Type.ComplaintTable)) : new Owner();
            FireDepartment fdinfo = new FireDepartment();
            if (z[5] != DBNull.Value) MainWindow.GetSingleItem<FireDepartment>(out fdinfo, Convert.ToInt32(z[5]), MainWindow.FireDepartments);

            ResidentialLocation complaintaddress = GetResidentialSite(Convert.ToInt32(z[1]), comp.Type.SiteTable);
            string bldgdesc = (z[6] != DBNull.Value) ? Convert.ToString(z[6]) : "";

            //Create complainant object
            Complainant complainant = new Complainant(GetComplainant(comp, fdgenerated));

            cidDB.Close();

            DateTime[] dates = GetDates(comp);

            return new OBResidentialComplaint(comp.ID, comp.SubID, comp.Type.ComplaintTable, complaintaddress, occupant, owner,
                complainant, comp.Anonymous, fdinfo, fdgenerated, comp.ComplaintNotes, comp.InspectionNotes, comp.OtherNotes,
                comp.ReceivedBy, comp.Inspector, comp.Status, comp.DateReceived, dates[0], dates[1], dates[2], comp.CETA,
                comp.Type, comp.LastModified, comp.MethodReceived, dates[3], comp.AppendixA, comp.Restricted, bldgdesc);
        }

        public DateTime[] GetDates(GenericComplaint comp)
        {
            DateTime[] dates = new DateTime[4];
            dates[0] = (comp.IncidentDate != null) ? (DateTime)comp.IncidentDate : DateTime.MinValue;
            dates[1] = (comp.DateClosed != null) ? (DateTime)comp.DateClosed : DateTime.MinValue;
            dates[2] = (comp.RetentionDate != null) ? (DateTime)comp.RetentionDate : DateTime.MinValue;
            dates[3] = (comp.DateInvestigated != null) ? (DateTime)comp.DateInvestigated : DateTime.MinValue;

            return dates;
        }

        public object[] FillDBObject(GenericComplaint comp)
        {
            string strSQL = "SELECT * FROM " + comp.Type.ComplaintTable + " WHERE ID = " + comp.SubID;
            OleDbCommand cidSQL = new OleDbCommand(strSQL, cidDB);
            OleDbDataReader cidReader = cidSQL.ExecuteReader();
            cidReader.Read();

            object[] z = new object[cidReader.FieldCount];
            cidReader.GetValues(z);
            cidReader.Close();

            return z;
        }

        public ResidentialLocation GetResidentialSite(int subid, string table)
        {
            city thecity = new city();
            state thestate = new state();
            county thecounty = new county();
            township thetownship = new township();
            //Create some objects
            string strSQL = "SELECT * FROM " + table + " WHERE ID = " + subid.ToString() + ";";
            OleDbCommand cidSQL = new OleDbCommand(strSQL, cidDB);
            OleDbDataReader cidReader = cidSQL.ExecuteReader();
            cidReader.Read();

            if (cidReader[5] != DBNull.Value) MainWindow.GetSingleItem<city>(out thecity, cidReader.GetInt32(5), MainWindow.Cities);
            if (cidReader[8] != DBNull.Value) MainWindow.GetSingleItem<state>(out thestate, cidReader.GetInt32(8), MainWindow.States);
            if (cidReader[10] != DBNull.Value) MainWindow.GetSingleItem<county>(out thecounty, cidReader.GetInt32(10), MainWindow.Counties);
            if (cidReader[11] != DBNull.Value) MainWindow.GetSingleItem<township>(out thetownship, cidReader.GetInt32(11), MainWindow.Townships);

            double xlat = 0;
            double xlon = 0;
            if (cidReader[7] != DBNull.Value) xlat = cidReader.GetDouble(7);
            if (cidReader[9] != DBNull.Value) xlon = cidReader.GetDouble(9);

            string placeid = (cidReader[1] != DBNull.Value) ? cidReader.GetString(1) : "";
            string add1 = (cidReader[2] != DBNull.Value) ? cidReader.GetString(2) : "";
            string add2 = (cidReader[3] != DBNull.Value) ? cidReader.GetString(3) : "";
            string parcel = (cidReader[4] != DBNull.Value) ? cidReader.GetString(4) : "";
            string zip = (cidReader[6] != DBNull.Value) ? cidReader.GetString(6) : "";

            int xx = cidReader.GetInt32(0);

            ResidentialLocation complaintaddress = new ResidentialLocation(xx, placeid, add1, add2, thecity,
                thestate, zip, thecounty, thetownship, parcel, xlon, xlat);

            cidReader.Close();

            return complaintaddress;
        }

        public OtherLocation GetOtherSite(int subid, string table)
        {
            city thecity = new city();
            state thestate = new state();
            county thecounty = new county();
            township thetownship = new township();
            //Create some objects
            string strSQL = "SELECT * FROM " + table + " WHERE ID = " + subid.ToString() + ";";
            OleDbCommand cidSQL = new OleDbCommand(strSQL, cidDB);
            OleDbDataReader cidReader = cidSQL.ExecuteReader();
            cidReader.Read();

            if (cidReader[4] != DBNull.Value) MainWindow.GetSingleItem<city>(out thecity, cidReader.GetInt32(4), MainWindow.Cities);
            if (cidReader[7] != DBNull.Value) MainWindow.GetSingleItem<state>(out thestate, cidReader.GetInt32(7), MainWindow.States);
            if (cidReader[9] != DBNull.Value) MainWindow.GetSingleItem<county>(out thecounty, cidReader.GetInt32(9), MainWindow.Counties);
            if (cidReader[10] != DBNull.Value) MainWindow.GetSingleItem<township>(out thetownship, cidReader.GetInt32(10), MainWindow.Townships);

            double xlat = (cidReader[6] != DBNull.Value) ? cidReader.GetDouble(6) : 0;
            double xlon = (cidReader[8] != DBNull.Value) ? cidReader.GetDouble(8) : 0;

            string desc = (cidReader[1] != DBNull.Value) ? cidReader.GetString(1) : "";
            string add1 = (cidReader[2] != DBNull.Value) ? cidReader.GetString(2) : "";
            string add2 = (cidReader[3] != DBNull.Value) ? cidReader.GetString(3) : "";
            string zip = (cidReader[5] != DBNull.Value) ? cidReader.GetString(5) : "";

            int xx = (cidReader[0] != DBNull.Value) ? cidReader.GetInt32(0) : 0;

            OtherLocation complaintaddress = new OtherLocation(xx, desc, add1, add2, thecity, thestate, zip, thecounty, thetownship, xlon, xlat);

            cidReader.Close();

            return complaintaddress;
        }

        public NonPermittedFacLocation GetNonPermFacSite(int subid, string table)
        {
            city thecity = new city();
            state thestate = new state();
            county thecounty = new county();
            township thetownship = new township();
            //Create some objects
            string strSQL = "SELECT * FROM " + table + " WHERE ID = " + subid.ToString() + ";";
            OleDbCommand cidSQL = new OleDbCommand(strSQL, cidDB);
            OleDbDataReader cidReader = cidSQL.ExecuteReader();
            cidReader.Read();

            if (cidReader[5] != DBNull.Value) MainWindow.GetSingleItem<city>(out thecity, cidReader.GetInt32(5), MainWindow.Cities);
            if (cidReader[8] != DBNull.Value) MainWindow.GetSingleItem<state>(out thestate, cidReader.GetInt32(8), MainWindow.States);
            if (cidReader[10] != DBNull.Value) MainWindow.GetSingleItem<county>(out thecounty, cidReader.GetInt32(10), MainWindow.Counties);
            if (cidReader[11] != DBNull.Value) MainWindow.GetSingleItem<township>(out thetownship, cidReader.GetInt32(11), MainWindow.Townships);

            double xlat = (cidReader[7] != DBNull.Value) ? cidReader.GetDouble(7) : 0;
            double xlon = (cidReader[9] != DBNull.Value) ? cidReader.GetDouble(9) : 0;

            string placeid = (cidReader[1] != DBNull.Value) ? cidReader.GetString(1) : "";
            string add1 = (cidReader[2] != DBNull.Value) ? cidReader.GetString(2) : "";
            string add2 = (cidReader[3] != DBNull.Value) ? cidReader.GetString(3) : "";
            string parcel = (cidReader[4] != DBNull.Value) ? cidReader.GetString(4) : "";
            string zip = (cidReader[6] != DBNull.Value) ? cidReader.GetString(6) : "";

            int xx = (cidReader[0] != DBNull.Value) ? cidReader.GetInt32(0) : 0;

            NonPermittedFacLocation complaintaddress = new NonPermittedFacLocation(xx, placeid, add1, add2, thecity, thestate,
                zip, thecounty, thetownship, parcel, xlon, xlat);

            cidReader.Close();

            return complaintaddress;
        }

        public FacilityContact GetFacilityContact(int id, string label, string parenttable)
        {
            string strSQL = "SELECT * FROM tbl_Complaint_Facility_Contacts WHERE ID = " + id.ToString() + ";";
            OleDbCommand cidSQL = new OleDbCommand(strSQL, cidDB);
            OleDbDataReader cidReader = cidSQL.ExecuteReader();
            cidReader.Read();

            int cid = (cidReader[0] != DBNull.Value) ? cidReader.GetInt32(0) : 0;

            string fname = (cidReader[1] != DBNull.Value) ? cidReader.GetString(1) : "";
            string lname = (cidReader[2] != DBNull.Value) ? cidReader.GetString(2) : "";
            string phone = (cidReader[3] != DBNull.Value) ? cidReader.GetString(3) : "";
            string email = (cidReader[4] != DBNull.Value) ? cidReader.GetString(4) : "";

            FacilityContact contact = new FacilityContact(cid, "", "", fname, lname, phone, email, label, parenttable);

            cidReader.Close();

            return contact;
        }

        public Contact GetContact(int subid, string table, string label, string parenttable)
        {
            string strSQL = "SELECT * FROM " + table + " WHERE ID = " + subid + ";";
            OleDbCommand cidSQL = new OleDbCommand(strSQL, cidDB);
            OleDbDataReader cidReader = cidSQL.ExecuteReader();
            cidReader.Read();
            object[] a = new object[cidReader.FieldCount];
            
            state thestate = new state();
            if (a[6] != DBNull.Value) MainWindow.GetSingleItem<state>(out thestate, Convert.ToInt32(a[6]), MainWindow.States);

            string lname = (cidReader[1] != DBNull.Value) ? cidReader.GetString(1) : "";
            string fname = (cidReader[2] != DBNull.Value) ? cidReader.GetString(2) : "";
            string add1 = (cidReader[3] != DBNull.Value) ? cidReader.GetString(3) : "";
            string add2 = (cidReader[4] != DBNull.Value) ? cidReader.GetString(4) : "";
            string city = (cidReader[5] != DBNull.Value) ? cidReader.GetString(5) : "";
            string zip = (cidReader[7] != DBNull.Value) ? cidReader.GetString(7) : "";
            string email = (cidReader[8] != DBNull.Value) ? cidReader.GetString(8) : "";
            string phone = (cidReader[9] != DBNull.Value) ? cidReader.GetString(9) : "";

            Contact contact = new Contact(subid, fname, lname, add1, add2, city, thestate, zip, email, phone, table, label, parenttable);
            cidReader.Close();

            return contact;
        }

        public GenericComplaint CreateGenericComplaint(OleDbDataReader cidReader)
        {
            GenericComplaint comp = new GenericComplaint();
            ComplaintType theType = new ComplaintType();

            comp.ID = cidReader.GetInt32(0);
            if (cidReader[1] != DBNull.Value) MainWindow.GetSingleItem<ComplaintType>(out theType, cidReader.GetInt32(1), MainWindow.ComplaintTypes);
            comp.Type = theType;
            comp.SubID = cidReader.GetInt32(2);
            if (cidReader[3] != DBNull.Value)
            {
                casestatus s = new casestatus();
                GetSingleItem<casestatus>(out s, cidReader.GetInt32(3), Statuses);
                comp.Status = s;
            }
            if (cidReader[4] != DBNull.Value)
            {
                user u = new user();
                GetSingleItem<user>(out u, cidReader.GetInt32(4), Users);
                comp.Inspector = u;
            }
            if (cidReader[5] != DBNull.Value)
            {
                user u = new user();
                GetSingleItem<user>(out u, cidReader.GetInt32(5), Users);
                comp.ReceivedBy = u;
            }
            if (cidReader[6] != DBNull.Value) comp.DateReceived = cidReader.GetDateTime(6);
            if (cidReader[7] != DBNull.Value) comp.DateClosed = cidReader.GetDateTime(7);
            else comp.DateClosed = null;
            if (cidReader[8] != DBNull.Value) comp.IncidentDate = cidReader.GetDateTime(8);
            else comp.IncidentDate = null;
            if (cidReader[9] != DBNull.Value) comp.DateInvestigated = cidReader.GetDateTime(9);
            else comp.DateInvestigated = null;
            comp.Anonymous = cidReader.GetBoolean(10);
            comp.Complainant = (cidReader[11] != DBNull.Value) ? cidReader.GetInt32(11) : 0;
            comp.ComplaintNotes = (cidReader[12] != DBNull.Value) ? cidReader.GetString(12) : "";
            comp.InspectionNotes = (cidReader[13] != DBNull.Value) ? cidReader.GetString(13) : "";
            comp.OtherNotes = (cidReader[14] != DBNull.Value) ? cidReader.GetString(14) : "";
            if (cidReader[15] != DBNull.Value) comp.RetentionDate = cidReader.GetDateTime(15);
            else comp.RetentionDate = null;
            if (cidReader[16] != DBNull.Value)
            {
                CETAtype c = new CETAtype();
                GetSingleItem<CETAtype>(out c, cidReader.GetInt32(16), CETATypes);
                comp.CETA = c;
            }
            if (cidReader[20] != DBNull.Value) comp.LastModified = cidReader.GetDateTime(20);
            if (cidReader[21] != DBNull.Value)
            {
                receivedmethod r = new receivedmethod();
                GetSingleItem<receivedmethod>(out r, cidReader.GetInt32(21), ReceivedMethods);
                comp.MethodReceived = r;
            }
            comp.AppendixA = cidReader.GetBoolean(26);
            comp.Restricted = cidReader.GetBoolean(27);

            return comp;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            MainSettings win = new MainSettings();
            try { win.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); win.Close(); return; }
        }

        public static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        public bool FilterComplaintTasks(object item)
        {
            TaskName task = item as TaskName;

            if (task == null) return false;
            else if (task.Type == 1 || task.Type == 3) return true;

            return false;
        }

        public bool FilterInspectionTasks(object item)
        {
            TaskName task = item as TaskName;

            if (task == null) return false;
            else if (task.Type == 2 || task.Type == 3) return true;

            return false;
        }

        public bool FilterRecordsTasks(object item)
        {
            TaskName task = item as TaskName;

            if (task == null) return false;
            else if (task.Type == 4) return true;

            return false;
        }

        public static string GetAuditorURL(string addline1)
        {
            try { cidDB.Open(); }
            catch { if (!MainWindow.TryAgain(cidDB, "Main")) return null; }

            //Create some objects
            OleDbCommand cidSQL = new OleDbCommand("SELECT * FROM tbl_AuditorURL", cidDB);
            OleDbDataReader cidReader = cidSQL.ExecuteReader();
            cidReader.Read();

            string strURL = (cidReader[1] != DBNull.Value) ? cidReader.GetString(1) : "";
            string strURL2 = (cidReader[2] != DBNull.Value) ? cidReader.GetString(2) : "";
            cidReader.Close();
            cidDB.Close();
            
            string num;
            string str;
            SplitAdd(addline1, out(num), out(str));

            strURL += num + strURL2 + str;
            return strURL;
        }

        public static void SplitAdd(string addline1, out string number, out string street)
        {
            number = "";
            street = "";
            int first = addline1.IndexOf(" ");

            if (first > 0)
            {
                number = addline1.Substring(0, first);
                if (MainWindow.IsNumeric(number))
                    street = addline1.Substring(first, (addline1.Length - first)).TrimStart(' ')  + " " + number.TrimEnd(' ');
                else
                {
                    number = "";
                    street = addline1.TrimStart(' ');
                }
            }
        }

        public static string ParceParcel(string html)
        {
            int intPos = html.IndexOf("NumberOfResults") + 16;
            string strResults = html.Substring(intPos, (html.IndexOf("</SPAN>", intPos) - intPos));
            int intPos2 = strResults.IndexOf("\">");
            if (intPos2 > -1) intPos2 += 2;
            else intPos2 = 0;
            strResults = strResults.Substring(intPos2, (strResults.LastIndexOf(" ") - intPos2));
            return strResults;
        }

        public static void ParceResults(IHTMLTable tbl, DataGrid grid)
        {
            List<ParcelResult> listResults = new List<ParcelResult>();

            IHTMLElementCollection rows = tbl.rows;
            foreach (IHTMLTableRow row in rows)
            {
                if ((row.rowIndex > 0) & (row.rowIndex < 21))
                {
                    ParcelResult tempResult = new ParcelResult();

                    IHTMLElementCollection cells = row.cells;
                    foreach (HTMLTableCell cell in cells)
                    {
                        switch (cell.cellIndex)
                        {
                            case 0:
                                tempResult.strParcel = cell.innerText;
                                break;
                            case 1:
                                tempResult.strOwner = cell.innerText;
                                break;
                            case 2:
                                tempResult.strAdd = cell.innerText;
                                break;
                            default:
                                break;
                        }
                    }
                    listResults.Add(tempResult);
                }
            }
            ListCollectionView parcels = new ListCollectionView(listResults);
            parcels.SortDescriptions.Add(new SortDescription("lngParcel", ListSortDirection.Ascending));
            grid.ItemsSource = parcels;
        }

        public static void GetCoords(Address loc, out string lat, out string lon)
        { lat = lon = "0"; }

        public static void GetCoords(ComplaintAddress loc, out string lat, out string lon)
        {
            string strCity = (loc.City != null) ? loc.City.Name : "";
            string strState = (loc.State != null) ? loc.State.Name : "";
            string strURL = "http://maps.googleapis.com/maps/api/geocode/xml?address=";
            strURL += loc.AddressLine1 + " " + strCity + " " + strState + " " + loc.Zip + "&sensor=true";
            lat = "";
            lon = "";

            XmlUrlResolver resolve = new XmlUrlResolver();
            XmlDocument doc = new XmlDocument();
            doc.XmlResolver = resolve;
            doc.Load(strURL);
            XmlNodeList nodelist = doc.GetElementsByTagName("location");
            if (nodelist.Count > 0)
            {
                nodelist = nodelist[0].ChildNodes;
                foreach (XmlNode node in nodelist)
                {
                    if (node.Name == "lat")
                    {
                        string templat = node.InnerText;
                        if (MainWindow.IsNumeric(templat) && ((Convert.ToDecimal(templat) > 39) && (Convert.ToDecimal(templat) < 42))) lat = templat;
                    }
                    else if (node.Name == "lng")
                    {
                        string templon = node.InnerText;
                        if (MainWindow.IsNumeric(templon) && ((Convert.ToDecimal(templon) > -82) && (Convert.ToDecimal(templon) < -80))) lon = templon;
                    }
                }
                if ((lat == "") || (lon == ""))
                {
                    lat = "0";
                    lon = "0";
                }
            }
            else MessageBox.Show("There were no coordinates found for this address.");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!MainWindow.IsSettingsDBConnected()) MainWindow.settingsDB.Open();

            OleDbCommand cmd = new OleDbCommand("UPDATE tblWindowPositions SET winTop = @xpos, winLeft = @ypos, winHeight = @height, winWidth = @width WHERE ID = 1;", settingsDB);
            cmd.Parameters.AddWithValue("@xpos", Top);
            cmd.Parameters.AddWithValue("@ypos", Left);
            cmd.Parameters.AddWithValue("@height", Height);
            cmd.Parameters.AddWithValue("@width", Width);
            cmd.ExecuteNonQuery();
            settingsDB.Close();
        }

        private void btnCompSearch_Click(object sender, RoutedEventArgs e)
        {
            RelatedSearch search = new RelatedSearch();
            try { search.Show(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); search.Close(); return; }
        }

        private void btnCompManage_Click(object sender, RoutedEventArgs e)
        {
            RecordManager man = new RecordManager();
            try { man.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); man.Close(); return; }
        }

        public static bool TryAgain(OleDbConnection db, string dbname)
        {
            Exception except = new Exception();
            bool failed = false;
            for(int i=0; i<5; i++)
            {
                failed = false;
                try { db.Open(); }
                catch (Exception Exception)
                {
                    except = Exception;
                    failed = true;
                }
                if (!failed || db.State == System.Data.ConnectionState.Open) break;
                else Task.Delay(1000);
            }
            if (failed) MessageBox.Show("Could not open a connection to the " + dbname + " database. " + System.Windows.MessageBox.Show(except.Message));

            return failed;
        }

        public static void Button_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Button x = sender as Button;
            bool enabled = ((bool)e.NewValue);

            ButtonStateChanged(x, enabled);
        }

        public static void ButtonStateChanged(Button button, bool enabled)
        {
            if (enabled) button.Foreground = System.Windows.Media.Brushes.DarkRed;
            else button.Foreground = System.Windows.Media.Brushes.DarkGray;
        }
    }
}
