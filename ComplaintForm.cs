using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Controls;
using mshtml;

namespace CID2
{
    public struct RelatedComplaint
    {
        public int ID { get; set; }
        public int pID { get; set; }
        public DateTime DateEntered { get; set; }
        public string ComplaintNotes { get; set; }
    }

    public struct MapLoc
    {
        public int ID { get; set; }
        public string Label { get; set; }
        public string Links { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

    public struct PrintStruct
    {
        public int index { get; set; }
        public RichTextBox textbox { get; set; }
        public string tbirdtext { get; set; }
        public string outlooktext { get; set; }
    }

    public struct PrintTableStruct
    {
        public string label { get; set; }
        public string tbirdtext { get; set; }
        public TableRowGroup printgroup { get; set; }
    }

    public struct ContactPrintStruct
    {
        public Contact contact;
        public string label;
        public string shapelabel;

        public ContactPrintStruct(Contact cont, string lab, string shapelab)
        {
            contact = cont;
            label = lab;
            shapelabel = shapelab;
        }
    }

    public class ComplaintForm : Window
    {
        private static TimeSpan LockSpan = new TimeSpan(0, 15, 0);
        private static TimeSpan LockWarning = new TimeSpan(0, 10, 0);
        public TimeSpan TimeLocked { get; set; }
        public DispatcherTimer LockTimer = new DispatcherTimer();
        public bool IsLocked { get; set; }

        public Complaint aComplaint;
        public Complaint thisComplaint { get { return aComplaint; } set { aComplaint = value;} }

        public Complaint oComplaint;
        public Complaint originalComplaint { get { return oComplaint; } set { oComplaint = value; } }

        public bool RecordLocked { get; set; }

        //Items and controls common to all records
        public Grid ParentGrid { get; set; }
        public WebBrowser ParcelBrowser;
        public DataGrid ParcelGrid;
        public TabItem ParcelTab;

        //File explorer items
        public static FileBrowserControl ExplorerControl;
        public static Button btnExplBack;
        public static Button btnExplFwd;

        //Change Log
        public DataGrid ChangeGrid;

        //associated
        public DataGrid AssociatedGrid;
        List<RelatedComplaint> lstRelated;
        ListCollectionView RelatedView;

        //additional complaints
        public DataGrid AdditionalGrid;
        List<RelatedComplaint> lstAdditional;
        ListCollectionView AdditionalView;

        //Google map
        public WebBrowser MapBrowser;
        public string strMapHTML;
        public string strMapPath = System.AppDomain.CurrentDomain.BaseDirectory + "map.html";
        public ListCollectionView listMapCompTypes;
        public ComboBox cboRadius;
        public ComboBox cboMapTypes;
        public TextBlock txtBlankCoordinates;
        
        //Tasks
        public DataGrid TaskGrid;
        List<ComplaintTask> listTasks;
        public DataGrid PublicRecordsTaskGrid;
        List<ComplaintTask> listPRTasks;

        //Record ID and controls
        public bool NewRecord = false;
        public TextBlock IDBlock;
        public Button UnlockRecord;
        public Button SaveRecord;
        public TextBlock RetentionBlock;
        public DatePicker RetentionDate;

        //Cursor for the Parcel Tab
        public Cursor parcelCursor { get; set; }

        public void RegisterEvents(Microsoft.WindowsAPICodePack.Controls.WindowsPresentationFoundation.ExplorerBrowser ExpBrowser)
        {
            EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyUpEvent, new KeyEventHandler(ResetTimer), true);
            EventManager.RegisterClassHandler(typeof(Window), Mouse.MouseUpEvent, new MouseButtonEventHandler(ResetTimer), true);
            ExpBrowser.NavigationLog.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(FileNavComplete);
            EventManager.RegisterClassHandler(typeof(Hyperlink), Hyperlink.ClickEvent, new RoutedEventHandler(Hyperlink_Click));
        }

        public bool OutOfBounds(county thisCounty, double latitude, double longitude)
        { return (latitude < thisCounty.SouthBound || latitude > thisCounty.NorthBound || longitude < thisCounty.WestBound || longitude > thisCounty.EastBound); }

        public void FillMapCombos(ComboBox cboMap, ComboBox cboRad)
        {
            List<ComplaintType> comboList = new List<ComplaintType>();
            foreach (ComplaintType q in MainWindow.ComplaintTypes)
            { comboList.Add(q); }
            comboList.Add(new ComplaintType(0, " All complaint types", "", "", "", "", "", "", "", "", null, null, null, 0, null, null, null));
            comboList.Add(new ComplaintType(9999, "Only related complaints", "", "", "", "", "", "", "", "", null, null, null, 0, null, null, null));
            ListCollectionView maplist = new ListCollectionView(comboList);
            maplist.SortDescriptions.Add(new SortDescription("Label", ListSortDirection.Ascending));
            FillCombo(cboMap, maplist);
            cboMap.SelectedIndex = 0;

            Dictionary<double, string> dictRad = new Dictionary<double, string>();
            for (double i = 1; i <= 10; i++)
            { dictRad.Add((0.804672 * i), (i / 2).ToString() + " miles"); }
            cboRad.ItemsSource = dictRad;
            cboRad.SelectedIndex = 1;
        }

        public void FillTaskCombos(ComboBox cboTasks, ComboBox cboPRTasks)
        {
            List<TaskName> list = new List<TaskName>();
            foreach (object x in MainWindow.AllTasks.SourceCollection)
            { list.Add((TaskName)x); }

            ListCollectionView lst = new ListCollectionView(list);
            lst.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            lst.Filter = (y) => ((TaskName)y).Type == 1 || ((TaskName)y).Type == 3;
            FillCombo(cboTasks, lst);

            ListCollectionView alst = new ListCollectionView(list);
            alst.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            alst.Filter = (y) => ((TaskName)y).Type == 4;
            FillCombo(cboPRTasks, alst);
        }

        public void LockRecord(bool locked)
        {
            MainWindow.OpenMainDBConnection();

            string strSQL = "SELECT IsLocked, LockedBY, LockedStamp, LastModified FROM tbl_Complaints WHERE ID = " + thisComplaint.ID.ToString();
            OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
            OleDbDataReader cidReader = cidCMD.ExecuteReader();
            cidReader.Read();

            bool islocked = cidReader.GetBoolean(0);
            user aUser = new user();
            DateTime lockedTime = DateTime.Now;
            if (cidReader[2] != DBNull.Value) lockedTime = cidReader.GetDateTime(2);
            if (cidReader[1] != DBNull.Value) MainWindow.GetSingleItem<user>(out aUser, cidReader.GetInt32(1), MainWindow.ActiveUsers);
            DateTime lastMod = new DateTime(1, 1, 1);
            if (cidReader[3] != DBNull.Value) lastMod = cidReader.GetDateTime(3);
            cidReader.Close();

            TimeLocked = (DateTime.Now - lockedTime);

            if ((locked) && (islocked) && (aUser.ID != MainWindow.thisUser.ID) && (TimeLocked < LockSpan))
            {
                TimeSpan aTime = LockSpan - (DateTime.Now - lockedTime);

                string message = "This record is already locked by " + aUser.Inits + ". If it is not being actively edited, the lock will expire in " + aTime.ToString((@"mm\:ss"));
                MessageBox.Show(message);
            }
            else
            {
                strSQL = "UPDATE tbl_Complaints SET IsLocked = @locked, LockedStamp = @dttime, LockedBy = @userid WHERE ID = @theID;";
                cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                cidCMD.Parameters.AddWithValue("@locked", locked);

                if (((locked) && (!islocked)) || ((locked) && (islocked) && (aUser.ID == MainWindow.thisUser.ID)) || ((locked) && (islocked) && (aUser.ID != MainWindow.thisUser.ID) && (TimeLocked >= LockSpan)))
                {
                    bool bMod = false;
                    if (lastMod != new DateTime(1, 1, 1) && lastMod > aComplaint.LastModified)
                    {
                        MessageBoxResult rslt = MessageBox.Show("This record has been edited since you opened it. You must refresh the data in order to edit. Refresh the data now?"
                            , "Modified", MessageBoxButton.YesNo);
                        if (rslt == MessageBoxResult.Yes)
                        {
                            MainWindow.CloseMainDBConnection();

                            bMod = false;
                            CallRestoreDataMembers();
                            aComplaint.UpdateControlContent();
                            MainWindow.OpenMainDBConnection();
                        }
                        else bMod = true;
                    }

                    if (!bMod)
                    {
                        IsLocked = true;
                        TimeLocked = new TimeSpan(0);
                        LockTimer.Interval = new TimeSpan(150000000);
                        LockTimer.Tick += new EventHandler(LockTimerEvent);
                        LockTimer.Start();
                        UnlockRecord.Content = "CANCEL EDITS";

                        cidCMD.Parameters.Add("@dttime", OleDbType.Date).Value = DateTime.Now;
                        cidCMD.Parameters.AddWithValue("@userid", MainWindow.thisUser.ID);
                    }
                }
                else if ((!locked) && (islocked) && (aUser.ID == MainWindow.thisUser.ID))
                {
                    cidCMD.Parameters.Add("@dttime", OleDbType.Date).Value = (object)DBNull.Value;
                    cidCMD.Parameters.AddWithValue("@userid", (object)DBNull.Value);

                    LockTimer.Stop();
                    TimeLocked = new TimeSpan(0);
                    UnlockRecord.Content = "EDIT RECORD";
                    UnlockRecord.Foreground = System.Windows.Media.Brushes.DarkRed;
                    IsLocked = false;
                }

                cidCMD.Parameters.AddWithValue("@theID", thisComplaint.ID);
                cidCMD.ExecuteNonQuery();

                bool closed = ((aComplaint.Status.ID == 2) && (MainWindow.thisUser.Role > 3));
                foreach (object x in LogicalTreeHelper.GetChildren(ParentGrid))
                { if (x is Visual) GetAllControls((Visual)x, locked, closed); }
                aComplaint.ComplainantInfo.thisControl.CheckControls();
            }

            MainWindow.CloseMainDBConnection();
            CheckStatus(true);
        }

        public virtual void CallRestoreDataMembers()
        {
            oComplaint = new Complaint();
            aComplaint.RestoreComplaintDataMembers(oComplaint);
        }

        public void FileNavComplete(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            btnExplBack.IsEnabled = ExplorerControl.Control.ExplorerBrowserControl.NavigationLog.CanNavigateBackward;
            btnExplFwd.IsEnabled = ExplorerControl.Control.ExplorerBrowserControl.NavigationLog.CanNavigateForward;
        }

        public void LockTimerEvent(object source, EventArgs e)
        {
            if (TimeLocked < LockWarning)
            {
                TimeLocked += new TimeSpan(150000000);
                LockTimer.Interval = new TimeSpan(150000000);
                UnlockRecord.Content = "CANCEL EDITS";
            }
            else 
                if (TimeLocked < LockSpan)
                {

                    TimeLocked += new TimeSpan(10000000);
                    UnlockRecord.Content = "CANCEL - " + (LockSpan - TimeLocked).ToString(@"mm\:ss");
                    if (UnlockRecord.Foreground == System.Windows.Media.Brushes.DarkRed) UnlockRecord.Foreground = System.Windows.Media.Brushes.DarkGray;
                    else UnlockRecord.Foreground = System.Windows.Media.Brushes.DarkRed;
                }
                else
                {
                    MainWindow.OpenMainDBConnection();

                    string strSQL = "UPDATE tbl_Complaints SET IsLocked = @locked, LockedStamp = @dttime, LockedBy = @userid WHERE ID = @theID;";
                    OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                    cidCMD.Parameters.AddWithValue("@locked", false);
                    cidCMD.Parameters.Add("@dttime", OleDbType.Date).Value = (object)DBNull.Value;
                    cidCMD.Parameters.AddWithValue("@userid", (object)DBNull.Value);
                    cidCMD.Parameters.AddWithValue("@theID", thisComplaint.ID);
                    cidCMD.ExecuteNonQuery();

                    TimeLocked = new TimeSpan(0);
                    UnlockRecord.Content = "EDIT RECORD";
                    IsLocked = false;
                    LockTimer.Stop();

                    MainWindow.CloseMainDBConnection();
                    MessageBox.Show("You have been inactive for 15 minutes. This record has been automatically unlocked and your changes have not been saved.");
                }
        }

        public static void ResetTimer(object sender, EventArgs e)
        {
            if ((sender != null) && ((sender is ComplaintForm) || (sender is SiteControlBase) || sender is ContactControlBase))
            {
                ComplaintForm x = sender as ComplaintForm;
                x.RequiredInfoChanged();
                if (x.IsLocked)
                {
                    x.LockTimer.Interval = new TimeSpan(15);
                    x.TimeLocked = new TimeSpan(0);
                }
            }
        }

        public void UnloadRecord()
        {
            if (IsLocked)
            {
                MainWindow.OpenMainDBConnection();

                string strSQL = "UPDATE tbl_Complaints SET IsLocked = @locked, LockedStamp = @dttime, LockedBy = @userid WHERE ID = @theID;";
                OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                cidCMD.Parameters.AddWithValue("@locked", false);
                cidCMD.Parameters.Add("@dttime", OleDbType.Date).Value = (object)DBNull.Value;
                cidCMD.Parameters.AddWithValue("@userid", (object)DBNull.Value);
                cidCMD.Parameters.AddWithValue("@theID", thisComplaint.ID);
                cidCMD.ExecuteNonQuery();

                TimeLocked = new TimeSpan(0);
                UnlockRecord.Content = "EDIT RECORD";
                IsLocked = false;
                LockTimer.Stop();

                MainWindow.CloseMainDBConnection();
            }
        }

        public void GetAllControls(Visual con, bool locked, bool closed)
        {
            foreach (object x in LogicalTreeHelper.GetChildren(con))
            {
                if (x is Visual)
                {
                    if ((((x is TextBox) || (x is ComboBox) || (x is Button)) || (x is DatePicker) || (x is CheckBox)) && CheckButtonName(((Control)x).Name, closed))
                    { ((Control)x).IsEnabled = locked; }
                    GetAllControls((Visual)x, locked, closed);
                }
            }
        }

        public bool CheckButtonName(string name, bool closed)
        {
            if ((name.EndsWith("_nolock") && !closed) || name.EndsWith("_neverlock")) return false;

            return true;
        }

        public void SetFormControls(Grid parentgrid, WebBrowser parcelbrowser, WebBrowser mapbrowser, DataGrid associatedgrid, DataGrid taskgrid,
            DataGrid parcelgrid, TabItem parceltab, FileBrowserControl filebrowse, ComboBox cbomapradius, ComboBox cbomaptype, Button btnexplback,
            Button btnexplfwd, DataGrid publicrecordstaskgrid, DataGrid additionalgrid, DataGrid changegrid, TextBlock txtblankcoordinates)
        {
            ParentGrid = parentgrid;
            ParcelBrowser = parcelbrowser;
            MapBrowser = mapbrowser;
            cboRadius = cbomapradius;
            cboMapTypes = cbomaptype;
            AssociatedGrid = associatedgrid;
            TaskGrid = taskgrid;
            ParcelGrid = parcelgrid;
            ParcelTab = parceltab;
            ExplorerControl = filebrowse;
            btnExplBack = btnexplback;
            btnExplFwd = btnexplfwd;
            PublicRecordsTaskGrid = publicrecordstaskgrid;
            AdditionalGrid = additionalgrid;
            ChangeGrid = changegrid;
            txtBlankCoordinates = txtblankcoordinates;
        }

        public void SetRecordControlButtons(bool isnew, TextBlock idblock, Button unlockrecord, Button saverecord, TextBlock retentionblock, DatePicker retentiondate)
        {
            NewRecord = isnew;
            IDBlock = idblock;
            UnlockRecord = unlockrecord;
            SaveRecord = saverecord;
            RetentionBlock = retentionblock;
            RetentionDate = retentiondate;

            SaveRecord.IsEnabled = true;
            
            if (!NewRecord) { UnlockRecord.IsEnabled = true; }
        }

        public void ButtonStateChanged(Button button, bool enabled)
        {
            if (enabled) button.Foreground = System.Windows.Media.Brushes.DarkRed;
            else button.Foreground = System.Windows.Media.Brushes.DarkGray;
        }

        public void SetComplaint(Complaint complaint)
        { thisComplaint = complaint; }

        public void FillCombo(ComboBox combo, ListCollectionView list)
        {
            combo.ItemsSource = list;
            combo.SelectedItem = null;
        }

        public void ParcelPageLoaded()
        {
            parcelCursor = null;

            string strResults = "";
            HTMLDocument doc = (HTMLDocument)ParcelBrowser.Document;

            if (doc != null)
            {
                if (doc.url.Contains("http://maps"))
                {
                    MessageBox.Show(doc.url);
                }
                else
                {
                    if (doc.body.innerHTML != null)
                    {
                        strResults = MainWindow.ParceParcel(doc.body.innerHTML);
                        if (MainWindow.IsNumeric(strResults))
                            MainWindow.ParceResults((IHTMLTable)doc.getElementById("ctl00_ContentPlaceHolder1_gvSearchResults"), ParcelGrid);
                        else
                        {
                            MessageBox.Show("There were no matches.");
                            if (MessageBox.Show("Show the auditors page? You may need to accept the Terms of Service and try again.", "No matches", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            { ParcelBrowser.Visibility = System.Windows.Visibility.Visible; }
                        }
                    }
                }
            }
        }

        public void LoadParcelPage(string address)
        {
            parcelCursor = Cursors.Wait;

            //Hide the browser control, the user doesn't get to see it unless something doesn't work
            ParcelBrowser.Visibility = System.Windows.Visibility.Hidden;

            string strURL = "";
            ParcelTab.IsEnabled = false;

            if (address != null)
            {
                strURL = MainWindow.GetAuditorURL(address);

                if (strURL != null)
                {
                    ParcelBrowser.Navigate(strURL);
                    ParcelTab.IsEnabled = true;
                    ParcelTab.Focus();
                }
            }
        }

        public void OpenAuditor(string address)
        {
            string strURL = "";

            if (address != null)
                strURL = MainWindow.GetAuditorURL(address);

            if (strURL != null)
                Process.Start(strURL);
        }

        public void DoubleClickParcel(object item)
        {
            ParcelResult result = (ParcelResult)item;
            ParcelConfirm confirm = new ParcelConfirm(result, (Complaint)aComplaint);
            try { confirm.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); confirm.Close(); return; }
        }

        public void DoubleClickParcel(TextBox parcelbox, ComboBox zipbox, TextBox ownerbox, object item)
        {
            ParcelResult parcel = (ParcelResult)item;
            int pos = parcel.strAdd.LastIndexOf(" ") + 1;
            string zip = parcel.strAdd.Substring(pos, 5);

            ParcelConfirm confirm = new ParcelConfirm(parcel.strParcel, zip, parcel.strOwner, parcelbox, zipbox, ownerbox);
            try { confirm.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); confirm.Close(); return; }
        }

        public void CatchMapLink(string mID)
        {
            MainWindow.OpenMainDBConnection();

            int thisid = Convert.ToInt32(mID.Substring(17));
            string strSQL = "SELECT Type_SubID, Date_Received, Status, Complaint_Type, Complaint FROM tbl_Complaints WHERE ID = " + mID.Substring(17);

            OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
            OleDbDataReader cidReader = cidCMD.ExecuteReader();

            int subid;
            string strcomp = "";
            casestatus stat = new casestatus();
            DateTime dtrcvd = new DateTime();
            ComplaintType type = new ComplaintType();
            string facname = "";
            Address add = new Address();
            while(cidReader.Read())
            {
                subid = cidReader.GetInt32(0);
                if (cidReader[1] != DBNull.Value) dtrcvd = cidReader.GetDateTime(1);
                MainWindow.GetSingleItem<casestatus>(out stat, cidReader.GetInt32(2), MainWindow.Statuses);
                MainWindow.GetSingleItem<ComplaintType>(out type, cidReader.GetInt32(3), MainWindow.ComplaintTypes);
                strcomp = cidReader.GetString(4);
                add = MainWindow.GetAddress(type, out facname, subid);
            }
            cidReader.Close();
            MainWindow.CloseMainDBConnection();

            int pid = thisComplaint.ID;
            if (pid != 0 && lstRelated.Exists(z => z.ID == thisid)) pid = ((RelatedComplaint)lstRelated.Find(z => z.ID == thisid)).pID;
            MiniComplaint minicomp = new MiniComplaint(pid, thisid, dtrcvd, stat, type, strcomp, facname, add, true);
            try { minicomp.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); minicomp.Close(); return; }

            FindRelated(thisComplaint.ID);
        }

        public void LoadMap(string lat, string lon, bool refreshifnull)
        {
            string path = strMapPath;

            txtBlankCoordinates.Visibility = System.Windows.Visibility.Hidden;
            MapBrowser.Visibility = System.Windows.Visibility.Visible;

            if ( ((lat != null) && (lon != null)) && ((lat != "") && (lon != "")) && (MainWindow.IsNumeric(lat) && MainWindow.IsNumeric(lon)))
            {
                MainWindow.OpenMainDBConnection();

                List<ComplaintType> types = new List<ComplaintType>();

                switch((int)cboMapTypes.SelectedValue)
                {
                    case 0:
                    case 9999:
                        foreach(ComplaintType x in MainWindow.ComplaintTypes)
                        { if(x.Label != "temp") types.Add(x); }
                        break;
                    default:
                        ComplaintType y = new ComplaintType();
                        MainWindow.GetSingleItem<ComplaintType>(out y, (int)cboMapTypes.SelectedValue, MainWindow.ComplaintTypes);
                        types.Add(y);
                        break;
                }

                List<MapLoc> listLocs = new List<MapLoc>();
                MapLoc aLoc = new MapLoc();
                aLoc.ID = thisComplaint.ID;
                aLoc.Label = "This ";
                aLoc.Lat = Convert.ToDouble(lat);
                aLoc.Lon = Convert.ToDouble(lon);
                aLoc.Lat = Math.Truncate(100000 * aLoc.Lat) / 100000;
                aLoc.Lon = Math.Truncate(100000 * aLoc.Lon) / 100000;
                aLoc.Links = "'<a href=\"http://127.0.0.1/" + thisComplaint.ID.ToString() + "\">complaint</a>'";
                listLocs.Add(aLoc);

                foreach (ComplaintType type in types)
                {
                    if (aComplaint != null && type.ComplaintTable != "temp")
                    {
                        double dblRad = (double)cboRadius.SelectedValue * 0.01;
                        dblRad *= dblRad;
                        string strSQL = "SELECT a.ID, a.Complaint_Type, c.Latitude, c.Longitude FROM (tbl_Complaints AS a LEFT JOIN " + type.ComplaintTable
                            + " AS b ON b.ID = a.Type_SubID) LEFT JOIN " + type.SiteTable + " AS c ON c.ID = b.Site WHERE (((" + lon + " - c.Longitude) * ("
                            + lon + " - c.Longitude) / 1.3) + ((" + lat + " - c.Latitude) * (" + lat + " - c.Latitude))) <= " + dblRad.ToString()
                            + " AND a.Complaint_Type = " + type.ID.ToString() + ";";
                        OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                        OleDbDataReader cidReader = cidCMD.ExecuteReader();

                        while (cidReader.Read())
                        {
                            int xx = cidReader.GetInt32(0);
                            if (xx != thisComplaint.ID)
                            {
                                MapLoc tempLoc = new MapLoc();
                                tempLoc.ID = xx;
                                tempLoc.Label = "Comp ID - ";
                                tempLoc.Lat = Math.Truncate(100000 * cidReader.GetDouble(2)) / 100000;
                                tempLoc.Lon = Math.Truncate(100000 * cidReader.GetDouble(3)) / 100000;
                                tempLoc.Links = "'<a href=\"http://127.0.0.1/" + xx.ToString() + "\">" + xx.ToString() + "</a>'";

                                if ((((int)cboMapTypes.SelectedValue == 9999) && (lstRelated.FindIndex(x => (x.ID == tempLoc.ID)) != -1)) || ((int)cboMapTypes.SelectedValue != 9999))
                                {
                                    if (!listLocs.Contains(tempLoc))
                                    {
                                        if (listLocs.Exists(x => (x.Lat == tempLoc.Lat && x.Lon == tempLoc.Lon)))
                                        {
                                            int yy = listLocs.FindLastIndex(x => (x.Lat == tempLoc.Lat && x.Lon == tempLoc.Lon));
                                            tempLoc.Links = "'" + listLocs[yy].Links.Trim('\'') + ", " + tempLoc.Links.Trim('\'') + "'";
                                            tempLoc.Label = listLocs[yy].Label;
                                            listLocs[yy] = tempLoc;
                                        }
                                        else listLocs.Add(tempLoc);
                                    }
                                }
                            }
                        }
                        cidReader.Close();
                    }
                }
                MainWindow.CloseMainDBConnection();

                int zzz = (int)Math.Round((16 - (int)(Math.Round((double)cboRadius.SelectedValue, MidpointRounding.ToEven)) / 1.75), MidpointRounding.ToEven);

                strMapHTML = File.ReadAllText(path);
                string c = "zoom: ";
                int pos = strMapHTML.IndexOf(c) + 6;
                strMapHTML = strMapHTML.Substring(0, pos) + zzz.ToString() + strMapHTML.Substring(pos, (strMapHTML.Length - pos));
                
                c = "locations = [];";
                pos = strMapHTML.IndexOf(c) + 13;
                string strLocs = "";
                int aa = 1;
                foreach(MapLoc loc in listLocs)
                {
                    strLocs += "['" + loc.Label + "', " + loc.Lat.ToString() + ", " + loc.Lon.ToString() + ", " + loc.Links + ", " + aa.ToString() + "],";
                    aa++;
                }
                strLocs = strLocs.Trim(',');
                strMapHTML = strMapHTML.Substring(0, pos) + strLocs + strMapHTML.Substring(pos, (strMapHTML.Length - pos));
                pos = strMapHTML.IndexOf("maps.LatLng(") + 12;
                strMapHTML = strMapHTML.Substring(0, pos) + lat + ", " + lon + strMapHTML.Substring((pos + 20), (strMapHTML.Length - (pos + 20)));
                MapBrowser.NavigateToString(strMapHTML);
            }
            else if (refreshifnull)
            {
                strMapHTML = File.ReadAllText(path);
                MapBrowser.NavigateToString(strMapHTML);
            }            
        }

        public void RequiredInfoChanged()
        {
            if (!IsLocked) SaveRecord.IsEnabled = IsLocked;
            else
            {
                List<Control> controls = aComplaint.GetRequiredControls();
                bool hasvalue = true;

                foreach (Control control in controls)
                {
                    string strName = control.GetType().FullName;
                    if (strName == "System.Windows.Controls.TextBox")
                    {
                        if ((((TextBox)control).Text == null) || (((TextBox)control).Text == "") && ((Control)control).Name != "")
                        {
                            if (control.Name == "txtCompFName" || control.Name == "txtCompLName")
                            {
                                if ((bool)aComplaint.ComplainantInfo.thisControl.chkCompAnon.IsChecked && hasvalue) hasvalue = true;
                                else if ((bool)aComplaint.ComplainantInfo.thisControl.chkCompFD.IsChecked && hasvalue) hasvalue = true;
                                else hasvalue = false;
                            }
                            else hasvalue = false;
                        }
                    }
                    else if (strName == "System.Windows.Controls.ComboBox")
                    {
                        if (((ComboBox)control).SelectedItem == null)
                            hasvalue = false;
                    }
                }

                if (SaveRecord != null) SaveRecord.IsEnabled = hasvalue;
            }
        }

        public void SetAttachmentsFolder()
        {
            string strPath = MainWindow.cidAttachmentspath + thisComplaint.ID.ToString() + "\\";
            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);
            ShellObject folder = ShellObject.FromParsingName(strPath);

            ExplorerControl.Control.IsEnabled = true;
            ExplorerControl.Control.ExplorerBrowserControl.Navigate(folder);
        }

        public void FillRelated(OleDbConnection cidDB, List<int[]> lst)
        {
            lstRelated = new List<RelatedComplaint>();

            foreach(int[] x in lst)
            {
                string strSQL = "SELECT Date_Received, Complaint FROM tbl_Complaints WHERE ID = " + x[0].ToString() + ";";
                OleDbCommand cidSQL = new OleDbCommand(strSQL, cidDB);
                OleDbDataReader cidReader = cidSQL.ExecuteReader();
                cidReader.Read();
                if(cidReader.HasRows)
                {
                    RelatedComplaint y = new RelatedComplaint();
                    y.ID = x[0];
                    y.pID = x[1];
                    if (cidReader[0] != DBNull.Value) y.DateEntered = cidReader.GetDateTime(0);
                    y.ComplaintNotes = cidReader.GetString(1);
                    lstRelated.Add(y);
                }
                cidReader.Close();
            }
            if (lstRelated.Count > 0)
            {
                RelatedView = new ListCollectionView(lstRelated);
                RelatedView.SortDescriptions.Add(new SortDescription("DateEntered", ListSortDirection.Ascending));
                AssociatedGrid.ItemsSource = RelatedView;
            }
        }

        public void FindRelated(int ID)
        {
            MainWindow.OpenMainDBConnection();

            int[] x = new int[] { ID, 0 };
            List<int[]> lst = new List<int[]>();
            RecursiveRelated(MainWindow.cidDB, ID, lst);
            FillRelated(MainWindow.cidDB, lst);

            MainWindow.CloseMainDBConnection();
        }

        public void FindMultiples(int siteID)
        {
            if (AdditionalGrid != null)
            {
                MainWindow.OpenMainDBConnection();

                List<int> lst = new List<int>();
                foreach (ComplaintType type in MainWindow.ComplaintTypes)
                {
                    if (aComplaint.CompType.SiteTable == type.SiteTable && type.ComplaintTable != "temp")
                    {
                        string strSQL = "SELECT a.ID FROM (tbl_Complaints AS a LEFT JOIN " + type.ComplaintTable +
                            " AS b ON b.ID = a.Type_SubID) WHERE b.SITE = " + siteID.ToString() + " AND Complaint_Type = " + type.ID.ToString() + ";";
                        OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                        OleDbDataReader cidReader = cidCMD.ExecuteReader();

                        while (cidReader.Read())
                        { if (cidReader.GetInt32(0) != aComplaint.ID) lst.Add(cidReader.GetInt32(0)); }
                        cidReader.Close();
                    }
                }

                lstAdditional = new List<RelatedComplaint>();
                foreach (int i in lst)
                {
                    string strSQL = "SELECT Date_Received, Complaint FROM tbl_Complaints WHERE ID = " + i.ToString() + ";";
                    OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                    OleDbDataReader cidReader = cidCMD.ExecuteReader();
                    cidReader.Read();

                    RelatedComplaint comp = new RelatedComplaint();
                    comp.ID = i;
                    comp.DateEntered = (cidReader[0] != DBNull.Value) ? cidReader.GetDateTime(0) : DateTime.MinValue;
                    comp.ComplaintNotes = (cidReader[1] != DBNull.Value) ? cidReader.GetString(1) : "";
                    lstAdditional.Add(comp);
                    cidReader.Close();
                }

                MainWindow.CloseMainDBConnection();

                AdditionalView = new ListCollectionView(lstAdditional);
                AdditionalView.SortDescriptions.Add(new SortDescription("DateEntered", ListSortDirection.Ascending));
                AdditionalGrid.ItemsSource = AdditionalView;
            }
        }

        private void RecursiveRelated(OleDbConnection cidDB, int ID, List<int[]> lst)
        {
            string strSQL = "SELECT Complaint_ID, Related_ID FROM tbl_Related_Complaints WHERE ((Complaint_ID = " + ID.ToString() + ") OR (Related_ID = " + ID.ToString() + "));";
            OleDbCommand dbSQL = new OleDbCommand(strSQL, cidDB);
            OleDbDataReader dbReader = dbSQL.ExecuteReader();

            if (dbReader != null)
            {
                List<int[]> temp = new List<int[]>();
                while (dbReader.Read())
                {
                    int y = dbReader.GetInt32(0);
                    int x = dbReader.GetInt32(1);
                    if (!temp.Exists(z => z[0] == y) && (y != thisComplaint.ID)) temp.Add(new int[] {y,x});
                    else if (!temp.Exists(z => z[0] == x) && x != thisComplaint.ID) temp.Add(new int[] { x, y });
                }

                foreach (int[] id in temp)
                {
                    if (!lst.Exists(x => x[0] == id[0]))
                    {
                        lst.Add(id);
                        RecursiveRelated(cidDB, id[0], lst);
                    }
                }
            }
            dbReader.Close();
        }

        public void RemoveRelation(RelatedComplaint related)
        {
            int pid = related.pID;
            int rid = related.ID;
            string message = "Are you sure you want remove this relationship?";
            if(pid!= thisComplaint.ID)
            { message = "This is not a direct relationship. This record is related to Complaint ID - " + pid.ToString() + ". Remove anyway?"; }

            if (MessageBox.Show(message, "Confirm Removal", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                MainWindow.OpenMainDBConnection();

                string strSQL = "DELETE * FROM tbl_Related_Complaints WHERE ((Complaint_ID = " + pid.ToString() + ") AND (Related_ID = " + rid.ToString()
                    + ")) OR ((Complaint_ID = " + rid.ToString() + ") AND ( Related_ID = " + pid.ToString() + "));";
                OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                cidCMD.ExecuteNonQuery();
                MainWindow.CloseMainDBConnection();

                FindRelated(thisComplaint.ID);
            }
        }
        
        public void FindPRTasks(int ID)
        {
            MainWindow.OpenMainDBConnection();

            ListCollectionView prlst = null;
            MainWindow.GetListItems<ComplaintTask>(MainWindow.cidDB, "SELECT * FROM tbl_PublicRecords_Tasks WHERE Parent_ID = " + ID.ToString() + " ORDER BY Task_Date;", "TaskDate", new ComplaintTask(), out prlst, false);
            PublicRecordsTaskGrid.ItemsSource = prlst;
            listPRTasks = (prlst != null) ? (List<ComplaintTask>)prlst.SourceCollection : null;
            RetentionDate.IsEnabled = (listPRTasks != null && listPRTasks.Exists(t => t.Task.ID == 37) && IsLocked);

            MainWindow.CloseMainDBConnection();
        }

        public void FindTasks(int ID)
        {
            MainWindow.OpenMainDBConnection();

            ListCollectionView lst = null;
            MainWindow.GetListItems<ComplaintTask>(MainWindow.cidDB, "SELECT * FROM tbl_Complaint_Tasks WHERE Parent_ID = " + ID.ToString() + " ORDER BY Task_Date;", "TaskDate", new ComplaintTask(), out lst, false);
            TaskGrid.ItemsSource = lst;
            listTasks = (lst != null) ? (List<ComplaintTask>)lst.SourceCollection : null;

            MainWindow.CloseMainDBConnection();
        }

        public void FindChangeLog()
        {
            MainWindow.OpenMainDBConnection();

            ListCollectionView lst;
            MainWindow.GetListItems<LogEntry>(MainWindow.cidDB, "SELECT * FROM tbl_AuditTrail WHERE pID = " + thisComplaint.ID.ToString() + " ORDER BY dtSaved;", "EntryDate", new LogEntry(), out lst, false);
            if (lst != null && lst.Count > 0) ChangeGrid.ItemsSource = lst;

            MainWindow.CloseMainDBConnection();
        }
        
        public void AddTask(TaskName task, string table)
        {
            TaskConfirm taskwindow = new TaskConfirm();
            try { taskwindow.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); taskwindow.Close(); return; }
            DateTime theDate = taskwindow.TaskDate;
            string theNotes = taskwindow.TaskNotes;
            DateTime minDate = new DateTime(2000, 1, 1);

            if (theDate > minDate)
            {
                MainWindow.OpenMainDBConnection();

                string strSQL = "INSERT INTO " + table + " (Parent_ID, Task, Task_Date, EnteredBy, TaskDetails) VALUES (@parentid, @task, @thedate, @userid, @details);";
                OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);

                cidCMD.Parameters.AddWithValue("@parentid", thisComplaint.ID);
                cidCMD.Parameters.AddWithValue("@task", task.ID);
                cidCMD.Parameters.Add("@taskdate", OleDbType.Date).Value = theDate;
                cidCMD.Parameters.AddWithValue("@userid", MainWindow.thisUser.ID);
                cidCMD.Parameters.AddWithValue("@details", theNotes);
                cidCMD.ExecuteNonQuery();
                MainWindow.CloseMainDBConnection();

                FindTasks(thisComplaint.ID);
            }
        }

        public void AddPRTask(TaskName task, string table)
        {
            TaskConfirm taskwindow = new TaskConfirm();
            try { taskwindow.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); taskwindow.Close(); return; }
            DateTime theDate = taskwindow.TaskDate;
            string theNotes = taskwindow.TaskNotes;
            DateTime minDate = new DateTime(2000, 1, 1);

            if (theDate > minDate)
            {
                MainWindow.OpenMainDBConnection();

                string strSQL = "INSERT INTO " + table + " (Parent_ID, Task, Task_Date, EnteredBy, TaskDetails) VALUES (@parentid, @task, @thedate, @userid, @details);";
                OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);

                cidCMD.Parameters.AddWithValue("@parentid", thisComplaint.ID);
                cidCMD.Parameters.AddWithValue("@task", task.ID);
                cidCMD.Parameters.Add("@taskdate", OleDbType.Date).Value = theDate;
                cidCMD.Parameters.AddWithValue("@userid", MainWindow.thisUser.ID);
                cidCMD.Parameters.AddWithValue("@details", theNotes);
                cidCMD.ExecuteNonQuery();
                MainWindow.CloseMainDBConnection();

                FindPRTasks(thisComplaint.ID);
            }
        }

        public void EditThisTask(ComplaintTask task, string table)
        {
            EditTask taskwindow = new EditTask(task);
            try { taskwindow.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); taskwindow.Close(); return; }

            if (taskwindow.save)
            {
                ComplaintTask thetask = taskwindow.Task;

                MainWindow.OpenMainDBConnection();

                string strSQL = "UPDATE " + table + " SET Task = @task, Task_Date = @taskdate, EnteredBy = @userid, TaskDetails = @details WHERE ID = @theID;";
                OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);

                cidCMD.Parameters.AddWithValue("@task", thetask.Task.ID);
                cidCMD.Parameters.Add("@taskdate", OleDbType.Date).Value = thetask.TaskDate;
                cidCMD.Parameters.AddWithValue("@userid", thetask.EnteredBy.ID);
                cidCMD.Parameters.AddWithValue("@details", thetask.Details);
                cidCMD.Parameters.AddWithValue("@theID", thetask.ID);
                cidCMD.ExecuteNonQuery();
                MainWindow.CloseMainDBConnection();
                cidCMD = null;

                FindTasks(thisComplaint.ID);
            }
        }

        public void EditPRTask(ComplaintTask task, string table)
        {
            EditTask taskwindow = new EditTask(task);
            try { taskwindow.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); taskwindow.Close(); return; }

            if (taskwindow.save)
            {
                MainWindow.OpenMainDBConnection();

                string strSQL = "UPDATE " + table + " SET Task = @task, Task_Date = @taskdate, EnteredBy = @userid, TaskDetails = @details WHERE ID = @theID;";
                OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);

                ComplaintTask thetask = taskwindow.Task;
                cidCMD.Parameters.AddWithValue("@task", thetask.Task.ID);
                cidCMD.Parameters.Add("@taskdate", OleDbType.Date).Value = thetask.TaskDate;
                cidCMD.Parameters.AddWithValue("@userid", thetask.EnteredBy.ID);
                cidCMD.Parameters.AddWithValue("@details", thetask.Details);
                cidCMD.Parameters.AddWithValue("@theID", thetask.ID);
                cidCMD.ExecuteNonQuery();
                MainWindow.CloseMainDBConnection();
                cidCMD = null;

                FindPRTasks(thisComplaint.ID);
            }
        }

        public void DeleteTask(ComplaintTask task, string table)
        {
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this task?", "Delete this task?", button);

            if (result == MessageBoxResult.Yes)
            {
                MainWindow.OpenMainDBConnection();

                string strSQL = "DELETE * FROM " + table + " WHERE ID = @taskID;";
                OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);

                cidCMD.Parameters.AddWithValue("@taskID", task.ID);
                cidCMD.ExecuteNonQuery();
                MainWindow.CloseMainDBConnection();

                FindTasks(thisComplaint.ID);
                FindPRTasks(thisComplaint.ID);
            }
        }

        public static int AlreadyRelated(int mID, int pID)
        {
            try { if (!MainWindow.IsMainDBConnected()) MainWindow.cidDB.Open(); }
            catch { if (!MainWindow.TryAgain(MainWindow.cidDB, "Main")) return -1; }

            string strSQL = "SELECT ID FROM tbl_Related_Complaints WHERE (Complaint_ID = " + pID.ToString() + " AND Related_ID = "
                + mID.ToString() + ") OR (Complaint_ID = " + mID.ToString() + " AND Related_ID = " + pID.ToString() + ");";
            OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
            OleDbDataReader cidReader = cidCMD.ExecuteReader();

            int x = cidReader.HasRows ? 1 : 0;
            cidReader.Close();
            MainWindow.CloseMainDBConnection();

            return x;
        }

        public void PrintGenerator(TableRowGroup complaintadd, string emailadd, List<ContactPrintStruct> contactList, FacilityContact faccontact)
        {
            DateTime min = new DateTime(2001, 1, 1);
            List<PrintTableStruct> printList = new List<PrintTableStruct>();
            PrintTableStruct print = new PrintTableStruct();
            string tbird = "";
            TableRowGroup group = new TableRowGroup();
            TableRow row = new TableRow();

            //record label
            Paragraph y = new Paragraph(new Run(thisComplaint.CompType.Label + " ID C" + thisComplaint.ID.ToString()));
            y.TextAlignment = TextAlignment.Center;
            y.FontSize = 22;
            y.SetValue(FontWeightProperty, FontWeights.Bold);
            TableCell cell = new TableCell(y);
            cell.ColumnSpan = 6;
            row.Cells.Add(cell);
            group.Rows.Add(row);
            group.SetValue(TitleProperty, "Record Label");
            row = new TableRow();
            y = new Paragraph(new Run("    "));
            cell = new TableCell(y);
            cell.ColumnSpan = 6;
            row.Cells.Add(new TableCell(y));
            row.FontSize = 1;
            group.Rows.Add(row); 
            print.printgroup = group;
            tbird = "<tr style=\"width:100%;\"><td valign=\"top\" align=\"left\" colspan=\"2\"><h2 style=\"width:100%;\">"
            + thisComplaint.CompType.Label + " - C" + thisComplaint.ID.ToString() + "</h2></tr>";
            print.tbirdtext = tbird;
            printList.Add(print);

            //case details
            print = new PrintTableStruct();
            group = new TableRowGroup();
            row = new TableRow();
            y = new Paragraph(new Run("Case Details"));
            y.TextAlignment = TextAlignment.Center;
            y.FontSize = 16;
            y.SetValue(FontWeightProperty, FontWeights.Bold);
            cell = new TableCell(y);
            cell.ColumnSpan = 6;
            row.Cells.Add(cell);
            group.Rows.Add(row);
            row = new TableRow();
            y = new Paragraph(new Run("Date received"));
            y.FontSize = 13;
            y.SetValue(FontWeightProperty, FontWeights.Bold);
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            y = new Paragraph(new Run("Inspector"));
            y.FontSize = 13;
            y.SetValue(FontWeightProperty, FontWeights.Bold);
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            y = new Paragraph(new Run("STARS2 type"));
            y.FontSize = 13;
            y.SetValue(FontWeightProperty, FontWeights.Bold);
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            group.Rows.Add(row);
            row = new TableRow();
            y = new Paragraph(new Run((thisComplaint.DateReceived > min) ? thisComplaint.DateReceived.ToShortDateString() : ""));
            y.FontSize = 12;
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            y = new Paragraph(new Run((thisComplaint.Inspector != null) ? thisComplaint.Inspector.ToString() : ""));
            y.FontSize = 12;
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            y = new Paragraph(new Run((thisComplaint.CETA != null) ? thisComplaint.CETA.Name : ""));
            y.FontSize = 12;
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            group.Rows.Add(row);
            row = new TableRow();
            y = new Paragraph(new Run(""));
            y.FontSize = 6;
            cell = new TableCell(y);
            cell.ColumnSpan = 6;
            row.Cells.Add(cell);
            group.Rows.Add(row);
            row = new TableRow();
            y = new Paragraph(new Run("Received by"));
            y.FontSize = 13;
            y.SetValue(FontWeightProperty, FontWeights.Bold);
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            y = new Paragraph(new Run((thisComplaint.IncidentDate > min) ? "Incident date" : ""));
            y.FontSize = 13;
            y.SetValue(FontWeightProperty, FontWeights.Bold);
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            y = new Paragraph(new Run("Case status"));
            y.FontSize = 13;
            y.SetValue(FontWeightProperty, FontWeights.Bold);
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            group.Rows.Add(row);
            row = new TableRow();
            y = new Paragraph(new Run((thisComplaint.ReceivedBy != null) ? thisComplaint.ReceivedBy.ToString() : ""));
            y.FontSize = 12;
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            y = new Paragraph(new Run((thisComplaint.IncidentDate > min) ? thisComplaint.IncidentDate.ToShortDateString() : ""));
            y.FontSize = 12;
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            y = new Paragraph(new Run((thisComplaint.Status != null) ?thisComplaint.Status.Status : ""));
            y.FontSize = 12;
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            group.Rows.Add(row);
            row = new TableRow();
            y = new Paragraph(new Run(""));
            y.FontSize = 6;
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            group.Rows.Add(row);
            row = new TableRow();
            y = new Paragraph(new Run("Method Received"));
            y.FontSize = 13;
            y.SetValue(FontWeightProperty, FontWeights.Bold);
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            y = new Paragraph(new Run("Investigation Started"));
            y.FontSize = 13;
            y.SetValue(FontWeightProperty, FontWeights.Bold);
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            y = new Paragraph(new Run((thisComplaint.DateClosed > min) ? "Date closed" : ""));
            y.FontSize = 13;
            y.SetValue(FontWeightProperty, FontWeights.Bold);
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            group.Rows.Add(row);
            row = new TableRow();
            y = new Paragraph(new Run((thisComplaint.MethodReceived != null) ? thisComplaint.MethodReceived.Label : ""));
            y.FontSize = 12;
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            y = new Paragraph(new Run((thisComplaint.InvestigatedDate > min) ? thisComplaint.InvestigatedDate.ToShortDateString() : ""));
            y.FontSize = 12;
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            y = new Paragraph(new Run((thisComplaint.DateClosed > min) ? thisComplaint.DateClosed.ToShortDateString() : ""));
            y.FontSize = 12;
            cell = new TableCell(y);
            cell.ColumnSpan = 2;
            row.Cells.Add(cell);
            group.Rows.Add(row);
            group.SetValue(TitleProperty, "Case Details");
            print.printgroup = group;
            tbird = "<tr style=\"width:100%;\"><td valign=\"top\" align=\"left\" colspan=\"2\"><h2 style=\"width:100%;\">Case Details</h2><table style=\"width:100%;\">"
                + "<tr><td><strong>Date received</strong></td><td><strong>Inspector</td></strong><td><strong>STARS2 type</strong></td></tr><tr><td>"
                + ((thisComplaint.DateReceived > min) ? thisComplaint.DateReceived.ToShortDateString() : "") + "</td><td>"
                + ((thisComplaint.Inspector != null) ? thisComplaint.Inspector.ToString() : "") + "</td><td>" + ((thisComplaint.CETA != null) ? thisComplaint.CETA.Name : "")
                + "</td></tr><tr><p style=\"font-size:1px;\"> </p></tr><tr><td><strong>Received by</strong></td><td>";
            if (thisComplaint.IncidentDate > min) tbird += "<strong>Incident date</strong>";
            tbird += "</td><td><strong>Case Status</strong></td></tr>";
            tbird += "<tr><td>" + thisComplaint.ReceivedBy.ToString() + "</td><td>";
            if (thisComplaint.IncidentDate > min) tbird += thisComplaint.IncidentDate.ToShortDateString();
            tbird += "</td><td>" + ((thisComplaint.Status != null) ? thisComplaint.Status.Status : "") + "</td></tr>";
            tbird += "<tr><p style=\"font-size:1px;\"> </p></tr><tr><td><strong>Method Received</strong></td><td><strong>Investigation Started<strong></td><td>";
            if (thisComplaint.Status.ID == 2) tbird += "<strong>Date Closed</strong>";
            string strmethod = (thisComplaint.MethodReceived != null) ? thisComplaint.MethodReceived.Label : "";
            tbird += "</td></tr><tr><td>" + strmethod + "</td><td>";
            if (thisComplaint.InvestigatedDate > min) tbird += thisComplaint.InvestigatedDate.ToShortDateString();
            tbird += "</td><td>";
            if (thisComplaint.DateClosed > min) tbird += thisComplaint.DateClosed.ToShortDateString();
            tbird += "</td></tr></table></td></tr>";
            print.tbirdtext = tbird;
            printList.Add(print);

            //complainant
            print = new PrintTableStruct();
            group = new TableRowGroup();
            row = new TableRow();
            tbird = "";
            y = new Paragraph(new Run("Complainant"));
            y.FontSize = 16;
            y.FontWeight = FontWeights.Bold;
            y.TextAlignment = TextAlignment.Center;
            cell = new TableCell(y);
            cell.ColumnSpan = 3;
            complaintadd.Rows[0].Cells.Add(cell);
            tbird = "<td valign=\"top\" style=\"width:50%;\"><h3 align=\"left\" style=\"font-size:14px;\">Complainant</h3>";
            if (thisComplaint.Anonymous)
            {
                y = new Paragraph(new Run("Anonymous"));
                y.FontSize = 12;
                cell = new TableCell(y);
                cell.ColumnSpan = 3;
                complaintadd.Rows[1].Cells.Add(cell);

                tbird += "<p style=\"font-size:12px;\">Anonymous</p>";
            }
            else
            {
                y = new Paragraph(new Run(((thisComplaint.ComplainantInfo.FName != null) ? thisComplaint.ComplainantInfo.FName : "") + " "
                    + ((thisComplaint.ComplainantInfo.LName!= null) ? thisComplaint.ComplainantInfo.LName : "")));
                y.FontSize = 12;
                cell = new TableCell(y);
                cell.ColumnSpan = 3;
                complaintadd.Rows[1].Cells.Add(cell);

                y = new Paragraph(new Run((thisComplaint.ComplainantInfo.AddressLine1 != null) ? thisComplaint.ComplainantInfo.AddressLine1 : ""));
                y.FontSize = 12;
                cell = new TableCell(y);
                cell.ColumnSpan = 3;
                complaintadd.Rows[2].Cells.Add(cell);

                y = new Paragraph(new Run((thisComplaint.ComplainantInfo.AddressLine2 != null) ? thisComplaint.ComplainantInfo.AddressLine2 : ""));
                y.FontSize = 12;
                cell = new TableCell(y);
                cell.ColumnSpan = 3;
                complaintadd.Rows[3].Cells.Add(cell);

                y = new Paragraph(new Run(((thisComplaint.ComplainantInfo.City != null) ? thisComplaint.ComplainantInfo.City : "") + ", "
                    + ((thisComplaint.ComplainantInfo.State != null) ? thisComplaint.ComplainantInfo.State.Abbr : "") + " "
                    + ((thisComplaint.ComplainantInfo.Zip != null) ? thisComplaint.ComplainantInfo.Zip : "")));
                y.FontSize = 12;
                cell = new TableCell(y);
                cell.ColumnSpan = 3;
                complaintadd.Rows[4].Cells.Add(cell);

                y = new Paragraph(new Run(((thisComplaint.ComplainantInfo.Phone != null) ? thisComplaint.ComplainantInfo.Phone : "")
                    + " " + ((thisComplaint.ComplainantInfo.Email != null) ? thisComplaint.ComplainantInfo.Email : "")));
                y.FontSize = 12;
                cell = new TableCell(y);
                cell.ColumnSpan = 3;
                complaintadd.Rows[5].Cells.Add(cell);

                tbird += "<p style=\"font-size:12px;\">" + ((thisComplaint.ComplainantInfo.FName != null) ? thisComplaint.ComplainantInfo.FName : "") + " "
                    + ((thisComplaint.ComplainantInfo.LName != null) ? thisComplaint.ComplainantInfo.LName : "") + "<br>";
                if (thisComplaint.ComplainantInfo.AddressLine1 != "") tbird += thisComplaint.ComplainantInfo.AddressLine1 + "<br>";
                if (thisComplaint.ComplainantInfo.AddressLine2 != "") tbird += thisComplaint.ComplainantInfo.AddressLine2 + "<br>";
                if (thisComplaint.ComplainantInfo.City != "") tbird += thisComplaint.ComplainantInfo.City + ", "
                    + ((thisComplaint.ComplainantInfo.State != null) ? thisComplaint.ComplainantInfo.State.Abbr : "") + " "
                    + ((thisComplaint.ComplainantInfo.Zip != null) ? thisComplaint.ComplainantInfo.Zip : "") + "<br>";
                tbird += ((thisComplaint.ComplainantInfo.Phone != null) ? thisComplaint.ComplainantInfo.Phone : "") + " "
                    + ((thisComplaint.ComplainantInfo.Email != null) ? thisComplaint.ComplainantInfo.Email : "") + "<br>";
                tbird += "</td>";
            }
            complaintadd.SetValue(TitleProperty, "Site Address/" + Environment.NewLine + "Complainant");
            print.printgroup = complaintadd;
            print.tbirdtext = emailadd + tbird;
            printList.Add(print);

            if (thisComplaint.ComplaintText != "")
            {
                print = new PrintTableStruct();
                group = new TableRowGroup();
                row = new TableRow();
                tbird = "";
                y = new Paragraph(new Run("Complaint Notes"));
                y.FontSize = 16;
                y.FontWeight = FontWeights.Bold;
                y.TextAlignment = TextAlignment.Center;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                row = new TableRow();
                y = new Paragraph(new Run((thisComplaint.ComplaintText != null) ? thisComplaint.ComplaintText : ""));
                y.FontSize = 12;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                group.SetValue(TitleProperty, "Complaint Notes");
                tbird = "<tr><td align=\"left\" colspan=\"2\"><h2 style=\"width:100%;\">Complaint</h2><p>"
                    + ((thisComplaint.ComplaintText != null) ? thisComplaint.ComplaintText : "") + "</p></tr>";
                print.printgroup = group;
                print.tbirdtext = tbird;
                printList.Add(print);
            }

            if (thisComplaint.OtherNotes != "")
            {
                print = new PrintTableStruct();
                group = new TableRowGroup();
                row = new TableRow();
                tbird = "";
                y = new Paragraph(new Run("Other Notes"));
                y.FontSize = 16;
                y.FontWeight = FontWeights.Bold;
                y.TextAlignment = TextAlignment.Center;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                row = new TableRow();
                y = new Paragraph(new Run((thisComplaint.OtherNotes != null) ? thisComplaint.OtherNotes : ""));
                y.FontSize = 12;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                group.SetValue(TitleProperty, "Other Notes");
                tbird = "<tr><td align=\"left\" colspan=\"2\"><h2 style=\"width:100%;\">Other Notes</h2><p>"
                    + ((thisComplaint.OtherNotes != null) ? thisComplaint.OtherNotes : "") + "</p></tr>";
                print.printgroup = group;
                print.tbirdtext = tbird;
                printList.Add(print);
            }

            if (thisComplaint.InspectionNotes != "")
            {
                print = new PrintTableStruct();
                group = new TableRowGroup();
                row = new TableRow();
                tbird = "";
                y = new Paragraph(new Run("Inspection Notes"));
                y.FontSize = 16;
                y.FontWeight = FontWeights.Bold;
                y.TextAlignment = TextAlignment.Center;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                row = new TableRow();
                y = new Paragraph(new Run((thisComplaint.InspectionNotes != null) ? thisComplaint.InspectionNotes : ""));
                y.FontSize = 12;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                group.SetValue(TitleProperty, "Inspection Notes");
                tbird = "<tr><td align=\"left\" colspan=\"2\"><h2 style=\"width:100%;\">Inspection Notes</h2><p>"
                    + ((thisComplaint.InspectionNotes != null) ? thisComplaint.InspectionNotes : "") + "</p></tr>";
                print.printgroup = group;
                print.tbirdtext = tbird;
                printList.Add(print);
            }

            FindRelated(thisComplaint.ID);
            if (lstRelated != null && lstRelated.Count > 0)
            {
                print = new PrintTableStruct();
                group = new TableRowGroup();
                row = new TableRow();
                tbird = "";
                y = new Paragraph(new Run("Associated IDs"));
                y.FontSize = 16;
                y.FontWeight = FontWeights.Bold;
                y.TextAlignment = TextAlignment.Center;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                string strRel = "";
                tbird = "<tr><td align=\"left\" colspan=\"2\"><h2 style=\"width:100%;\">Associated IDs</h2><p>";
                foreach (RelatedComplaint e in lstRelated)
                {
                    strRel += " C-" + e.ID.ToString() + ",";
                    tbird += " " + e.ID.ToString() + ",";
                }
                strRel = strRel.TrimEnd(',');
                tbird = tbird.TrimEnd(',') + "</p></tr>";
                row = new TableRow();
                y = new Paragraph(new Run(strRel));
                y.FontSize = 12;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                group.SetValue(TitleProperty, "Associated IDs");
                print.printgroup = group;
                print.tbirdtext = tbird;
                printList.Add(print);
            }

            if (lstAdditional != null && lstAdditional.Count > 0)
            {
                print = new PrintTableStruct();
                group = new TableRowGroup();
                row = new TableRow();
                tbird = "";
                y = new Paragraph(new Run("Additional IDs"));
                y.FontSize = 16;
                y.FontWeight = FontWeights.Bold;
                y.TextAlignment = TextAlignment.Center;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                string strRel = "";
                tbird = "<tr><td align=\"left\" colspan=\"2\"><h2 style=\"width:100%;\">Additional IDs</h2><p>";
                foreach (RelatedComplaint e in lstAdditional)
                {
                    strRel += " C-" + e.ID.ToString() + ",";
                    tbird += " " + e.ID.ToString() + ",";
                }
                strRel = strRel.TrimEnd(',');
                tbird = tbird.TrimEnd(',') + "</p></tr>";
                row = new TableRow();
                y = new Paragraph(new Run(strRel));
                y.FontSize = 12;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                group.SetValue(TitleProperty, "Additional IDs");
                print.printgroup = group;
                print.tbirdtext = tbird;
                printList.Add(print);
            }

            FindTasks(thisComplaint.ID);
            if (listTasks != null && listTasks.Count > 0)
            {
                print = new PrintTableStruct();
                group = new TableRowGroup();
                row = new TableRow();
                tbird = "";
                y = new Paragraph(new Run("Tasks"));
                y.FontSize = 16;
                y.FontWeight = FontWeights.Bold;
                y.TextAlignment = TextAlignment.Center;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                string strTasks = "";
                tbird = "<tr><td align=\"left\" colspan=\"2\"><h2 style=\"width:100%;\">Tasks</h2><p>";
                ListCollectionView sortedTasks = new ListCollectionView(listTasks);
                sortedTasks.SortDescriptions.Add(new SortDescription("TaskDate", ListSortDirection.Descending));
                foreach (ComplaintTask e in sortedTasks)
                {
                    strTasks += e.TaskDate.ToShortDateString() + "    " + e.Task + Environment.NewLine;
                    tbird += e.TaskDate.ToShortDateString() + "    " + e.Task + "<br>";
                }
                tbird += "</p></tr>";
                row = new TableRow();
                y = new Paragraph(new Run(strTasks));
                y.FontSize = 12;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                group.SetValue(TitleProperty, "Tasks");
                print.printgroup = group;
                print.tbirdtext = tbird;
                printList.Add(print);
            }

            FindPRTasks(thisComplaint.ID);
            if (listPRTasks != null && listPRTasks.Count > 0)
            {
                print = new PrintTableStruct();
                group = new TableRowGroup();
                row = new TableRow();
                tbird = "";
                y = new Paragraph(new Run("Public Records Tasks"));
                y.FontSize = 16;
                y.FontWeight = FontWeights.Bold;
                y.TextAlignment = TextAlignment.Center;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                string strTasks = "";
                tbird = "<tr><td align=\"left\" colspan=\"2\"><h2 style=\"width:100%;\">Public Records Tasks</h2><p>";
                ListCollectionView sortedTasks = new ListCollectionView(listPRTasks);
                sortedTasks.SortDescriptions.Add(new SortDescription("TaskDate", ListSortDirection.Descending));
                foreach (ComplaintTask e in sortedTasks)
                {
                    strTasks += e.TaskDate.ToShortDateString() + "    " + e.Task + Environment.NewLine;
                    tbird += e.TaskDate.ToShortDateString() + "    " + e.Task + "<br>";
                }
                tbird += "</p></tr>";
                row = new TableRow();
                y = new Paragraph(new Run(strTasks));
                y.FontSize = 12;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                group.SetValue(TitleProperty, "Records Tasks");
                print.printgroup = group;
                print.tbirdtext = tbird;
                printList.Add(print);
            }

            string strPath = MainWindow.cidAttachmentspath + thisComplaint.ID.ToString() + "\\";
            string[] files = Directory.GetFiles(strPath, "*", SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                print = new PrintTableStruct();
                group = new TableRowGroup();
                row = new TableRow();
                tbird = "";
                y = new Paragraph(new Run("File Attachments"));
                y.FontSize = 16;
                y.FontWeight = FontWeights.Bold;
                y.TextAlignment = TextAlignment.Center;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                tbird = "<tr><td align=\"left\" colspan=\"2\"><h2 style=\"width:100%;\">File Attachments</h2><p>";
                string names = "";
                for (int xx = 0; xx < files.Length; xx++)
                {
                    files[xx] = files[xx].Remove(0, strPath.Length);
                    names += files[xx] + Environment.NewLine;
                    tbird += files[xx] + "<br>";
                }
                tbird += "</p></tr>";
                row = new TableRow();
                y = new Paragraph(new Run(names));
                y.FontSize = 12;
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(cell);
                group.Rows.Add(row);
                group.SetValue(TitleProperty, "File Attachments");
                print.printgroup = group;
                print.tbirdtext = tbird;
                printList.Add(print);
            }

            if(contactList != null)
            {
                foreach (ContactPrintStruct xxx in contactList)
                {
                    if (xxx.contact.IsMinimum())
                    {
                        print = new PrintTableStruct();
                        group = new TableRowGroup();
                        row = new TableRow();
                        y = new Paragraph(new Run(xxx.label + Environment.NewLine));
                        y.FontSize = 16;
                        y.FontWeight = FontWeights.Bold;
                        y.TextAlignment = TextAlignment.Center;
                        cell = new TableCell(y);
                        cell.ColumnSpan = 3;
                        row.Cells.Add(cell);
                        group.Rows.Add(row);

                        string x = "";
                        if (xxx.contact.FName != "" || xxx.contact.LName != "") x = xxx.contact.FName + " " + xxx.contact.LName;
                        row = new TableRow();
                        y = new Paragraph(new Run(x));
                        y.FontSize = 12;
                        cell = new TableCell(y);
                        cell.ColumnSpan = 3;
                        row.Cells.Add(cell);
                        group.Rows.Add(row);

                        x = "";
                        if (xxx.contact.AddressLine1 != "") x = xxx.contact.AddressLine1;
                        row = new TableRow();
                        y = new Paragraph(new Run(x));
                        y.FontSize = 12;
                        cell = new TableCell(y);
                        cell.ColumnSpan = 3;
                        row.Cells.Add(cell);
                        group.Rows.Add(row);

                        x = "";
                        if (xxx.contact.AddressLine2 != "") x = xxx.contact.AddressLine2;
                        row = new TableRow();
                        y = new Paragraph(new Run(x));
                        y.FontSize = 12;
                        cell = new TableCell(y);
                        cell.ColumnSpan = 3;
                        row.Cells.Add(cell);
                        group.Rows.Add(row);

                        x = "";
                        if (xxx.contact.City != "") x = xxx.contact.City + ", " + xxx.contact.State.Abbr + " " + xxx.contact.Zip;
                        row = new TableRow();
                        y = new Paragraph(new Run(x));
                        y.FontSize = 12;
                        cell = new TableCell(y);
                        cell.ColumnSpan = 3;
                        row.Cells.Add(cell);
                        group.Rows.Add(row);

                        x = "";
                        if (xxx.contact.Phone != "") x = xxx.contact.Phone;
                        row = new TableRow();
                        y = new Paragraph(new Run(x));
                        y.FontSize = 12;
                        cell = new TableCell(y);
                        cell.ColumnSpan = 3;
                        row.Cells.Add(cell);
                        group.Rows.Add(row);

                        x = "";
                        if (xxx.contact.Email != "") x = xxx.contact.Email;
                        row = new TableRow();
                        y = new Paragraph(new Run(x));
                        y.FontSize = 12;
                        cell = new TableCell(y);
                        cell.ColumnSpan = 3;
                        row.Cells.Add(cell);
                        group.Rows.Add(row);

                        row = new TableRow();
                        y = new Paragraph(new Run(""));
                        y.FontSize = 12;
                        cell = new TableCell(y);
                        cell.ColumnSpan = 3;
                        row.Cells.Add(cell);
                        group.Rows.Add(row);

                        group.SetValue(Window.TitleProperty, xxx.shapelabel);
                        print.printgroup = group;

                        string strAdd = "<tr><td valign=\"top\" style=\"width:50%;\"><h3 align=\"left\">" + xxx.label + "</h3>";
                        strAdd += "<p style=\"font-size:12px;\">" + xxx.contact.FName + " " + xxx.contact.LName + "<br>";
                        if (xxx.contact.AddressLine1 != "") strAdd += xxx.contact.AddressLine1 + "<br>";
                        if (xxx.contact.AddressLine2 != "") strAdd += xxx.contact.AddressLine2 + "<br>";
                        if (xxx.contact.City != "") strAdd += xxx.contact.City + ", " + xxx.contact.State.Abbr + " " + xxx.contact.Zip + "<br>";
                        strAdd += "</td></tr>";

                        print.tbirdtext = strAdd;
                        printList.Add(print);
                    }
                }

                if (faccontact != null && faccontact.ID > 0)
                {
                    print = new PrintTableStruct();
                    group = new TableRowGroup();
                    row = new TableRow();
                    y = new Paragraph(new Run("Facility Contact" + Environment.NewLine));
                    y.FontSize = 16;
                    y.FontWeight = FontWeights.Bold;
                    y.TextAlignment = TextAlignment.Center;
                    cell = new TableCell(y);
                    cell.ColumnSpan = 3;
                    row.Cells.Add(cell);
                    group.Rows.Add(row);

                    string x = "";
                    if (faccontact.ContactType != "") x = "Contact Type - " + faccontact.ContactType;
                    row = new TableRow();
                    y = new Paragraph(new Run(x));
                    y.FontSize = 12;
                    cell = new TableCell(y);
                    cell.ColumnSpan = 3;
                    row.Cells.Add(cell);
                    group.Rows.Add(row);

                    x = "";
                    if (faccontact.FName != "" || faccontact.LName != "") x = faccontact.FName + " " + faccontact.LName;
                    row = new TableRow();
                    y = new Paragraph(new Run(x));
                    y.FontSize = 12;
                    cell = new TableCell(y);
                    cell.ColumnSpan = 3;
                    row.Cells.Add(cell);
                    group.Rows.Add(row);

                    x = "";
                    if (faccontact.Phone != "") x = faccontact.Phone;
                    row = new TableRow();
                    y = new Paragraph(new Run(x));
                    y.FontSize = 12;
                    cell = new TableCell(y);
                    cell.ColumnSpan = 3;
                    row.Cells.Add(cell);
                    group.Rows.Add(row);

                    x = "";
                    if (faccontact.Email != "") x = faccontact.Email;
                    row = new TableRow();
                    y = new Paragraph(new Run(x));
                    y.FontSize = 12;
                    cell = new TableCell(y);
                    cell.ColumnSpan = 3;
                    row.Cells.Add(cell);
                    group.Rows.Add(row);

                    row = new TableRow();
                    y = new Paragraph(new Run(""));
                    y.FontSize = 12;
                    cell = new TableCell(y);
                    cell.ColumnSpan = 3;
                    row.Cells.Add(cell);
                    group.Rows.Add(row);

                    group.SetValue(Window.TitleProperty, "Facility Contact");
                    print.printgroup = group;

                    string strAdd = "<tr><td valign=\"top\" style=\"width:50%;\"><h3 align=\"left\">Facility Contact</h3>";
                    strAdd += "<p style=\"font-size:12px;\">" + faccontact.FName + " " + faccontact.LName + "<br>";
                    if (faccontact.Phone != null) strAdd += faccontact.Phone + "<br>";
                    if (faccontact.Email != null) strAdd += faccontact.Email + "<br>";
                    strAdd += "</td></tr>";

                    print.tbirdtext = strAdd;
                    printList.Add(print);
                }

            }

            PrintShapes shapes = new PrintShapes(printList);
            try { shapes.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); shapes.Close(); return; }
        }

        public void CheckStatus(bool check)
        {
            if (check && aComplaint.StatusBox.IsLoaded && aComplaint.DateClosedBox.IsLoaded && (aComplaint.StatusBox.SelectedValue != null))
            {
                aComplaint.DateClosedBox.IsEnabled = IsLocked;
                aComplaint.Status = (casestatus)aComplaint.StatusBox.SelectedItem;
                if ((int)(aComplaint.StatusBox.SelectedValue) == 2)
                {
                    DateTime minDate = new DateTime(2000, 1, 1);
                    if (aComplaint.DateClosed < minDate) aComplaint.DateClosed = DateTime.Now;
                    RetentionBlock.Visibility = System.Windows.Visibility.Visible;
                    RetentionDate.Visibility = System.Windows.Visibility.Visible;
                    if (aComplaint.RetentionDate < minDate) aComplaint.RetentionDate = DateTime.Now.AddMonths(aComplaint.CompType.RetentionMonths);
                    aComplaint.UpdateControlContent();
                }
                else
                {
                    aComplaint.DateClosedBox.SelectedDate = null;
                    aComplaint.DateClosedBox.IsEnabled = false;
                    aComplaint.DateClosed = DateTime.MinValue;
                    RetentionBlock.Visibility = System.Windows.Visibility.Hidden;
                    RetentionDate.Visibility = System.Windows.Visibility.Hidden;
                    RetentionDate.SelectedDate = null;
                }
                aComplaint.UpdateContentFromControls();
            }
        }

        public void OpenAssAddMini(RelatedComplaint rel, bool showmarkbuttons)
        {
            MainWindow.OpenMainDBConnection();

            string strSQL = "SELECT Type_SubID, Complaint_Type, Status FROM tbl_Complaints WHERE ID = " + rel.ID + ";";
            OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
            OleDbDataReader cidReader = cidCMD.ExecuteReader();
            string facname = "";

            int subid;
            casestatus stat = new casestatus();
            ComplaintType type = new ComplaintType();
            Address add = new Address();
            while (cidReader.Read())
            {
                subid = cidReader.GetInt32(0);
                MainWindow.GetSingleItem<ComplaintType>(out type, cidReader.GetInt32(1), MainWindow.ComplaintTypes);
                MainWindow.GetSingleItem<casestatus>(out stat, cidReader.GetInt32(2), MainWindow.Statuses);
                add = MainWindow.GetAddress(type, out facname, subid);
            }
            cidReader.Close();
            MainWindow.CloseMainDBConnection();

            MiniComplaint minicomp = new MiniComplaint(rel.pID, rel.ID, rel.DateEntered, stat, type, rel.ComplaintNotes, facname, add, showmarkbuttons);
            try { minicomp.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); minicomp.Close(); return; }

            FindRelated(thisComplaint.ID);
        }

        public void SetupWindowLocation()
        {
            if (!MainWindow.IsSettingsDBConnected()) MainWindow.settingsDB.Open();

            OleDbCommand dbCMD = new OleDbCommand("SELECT * FROM tblComplaintFormPositions WHERE ComplaintType = " + aComplaint.CompType.ID.ToString() + ";", MainWindow.settingsDB);
            OleDbDataReader dbReader = dbCMD.ExecuteReader();
            dbReader.Read();
            Top = dbReader.GetInt32(2);
            Left = dbReader.GetInt32(3);
            Height = dbReader.GetInt32(4);
            Width = dbReader.GetInt32(5);

            dbReader.Close();
        }

        public void SaveWindowLocation()
        {
            if (!MainWindow.IsSettingsDBConnected()) MainWindow.settingsDB.Open();

            string strSQL = "UPDATE tblComplaintFormPositions SET winTop = @top, winLeft = @left, winHeight = @height, winWidth = @width WHERE ComplaintType = " + aComplaint.CompType.ID.ToString() + ";";
            OleDbCommand cmd = new OleDbCommand(strSQL, MainWindow.settingsDB);
            cmd.Parameters.AddWithValue("@top", Top);
            cmd.Parameters.AddWithValue("@left", Left);
            cmd.Parameters.AddWithValue("@height", Height);
            cmd.Parameters.AddWithValue("@width", Width);
            cmd.ExecuteNonQuery();

            //MainWindow.settingsDB.Close();
        }

        public void Button_Loaded(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TextBlock block = (TextBlock)button.Content;
            if (block.Text == null || block.Text == "") button.IsEnabled = false;
            else
            {
                button.IsEnabled = true;
                button.Background = Brushes.DarkGray;
            }
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    if (row.IsSelected)
                    {
                        TextBlock block = (TextBlock)button.Content;
                        block.Text = (block.Text == "+") ? "-" : "+";

                        row.DetailsVisibility = (row.DetailsVisibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
                    }
                    break;
                }
        }
        
        public void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock block = sender as TextBlock;
            if (block.Text.StartsWith("http://") || block.Text.StartsWith("file://") || block.Text.StartsWith("www.")
                || block.Text.EndsWith(".com") || block.Text.EndsWith(".net") || block.Text.EndsWith(".org")
                || block.Text.EndsWith(".com") || block.Text.EndsWith(".gov") || block.Text.EndsWith(".edu"))
            {
                Hyperlink link = new Hyperlink(new Run(block.Text));
                if (!(block.Text.StartsWith("http://") || block.Text.StartsWith("file://")))
                {
                    if (block.Text.Contains("@")) block.Text = "mailto:" + block.Text;
                    else block.Text = "http://" + block.Text;
                }
                link.NavigateUri = new Uri(block.Text);
                link.TargetName = "_new";
                block.Text = "";
                block.Inlines.Clear();
                block.Inlines.Add(link);
            }
        }

        public void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            Process.Start(link.NavigateUri.ToString());
        }

        public void listPRTasks_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            var parent = VisualTreeHelper.GetParent(row);
            while(parent != null && parent.GetType() != typeof(DataGrid))
            { parent = VisualTreeHelper.GetParent(parent); }
            DataGrid list = parent as DataGrid;

            if (!(list.CurrentCell.Column is DataGridTemplateColumn))
            {
                if (row.Item != null) EditPRTask((ComplaintTask)row.Item, "tbl_PublicRecords_Tasks");
                e.Handled = true;
            }
        }

        public void listTasks_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            var parent = VisualTreeHelper.GetParent(row);
            while (parent != null && parent.GetType() != typeof(DataGrid))
            { parent = VisualTreeHelper.GetParent(parent); }
            DataGrid list = parent as DataGrid;

            if (!(list.CurrentCell.Column is DataGridTemplateColumn))
            {
                if (row.Item != null) EditThisTask((ComplaintTask)row.Item, "tbl_Complaint_tasks");
                e.Handled = true;
            }
        }

        public void cboStatusBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { CheckStatus(true); }

        public void listAddresses_MouseEnter(object sender, MouseEventArgs e)
        { Mouse.OverrideCursor = parcelCursor; }

        public void listAddresses_MouseLeave(object sender, MouseEventArgs e)
        { Mouse.OverrideCursor = null; }

        public void ChangeLogTab_MouseDown(object sender, MouseButtonEventArgs e)
        { if (!NewRecord) FindChangeLog(); }

        public void Tasks_LoadingRow(object sender, DataGridRowEventArgs e)
        { e.Row.DetailsVisibility = System.Windows.Visibility.Collapsed; }

        public void txtcoords_PreviewTextInput(object sender, TextCompositionEventArgs e)
        { e.Handled = !MainWindow.IsTextAllowed(e.Text); }

        public void txtcoords_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!MainWindow.IsTextAllowed(text)) e.CancelCommand();
            }
            else e.CancelCommand();
        }

        public void ComplaintForm_Closing(object sender, CancelEventArgs e)
        {
            if (IsLocked)
                if (MessageBox.Show("If you close now then your changes will not be saved. Are you sure you want to close?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.No) e.Cancel = true;

            MainWindow.ZipCodeList.Filter = null;
            MainWindow.Townships.Filter = null;

            //foreach (Window w in System.Windows.Application.Current.Windows)
            //{ if (w is OpenComplaints) ((OpenComplaints)w).GetComplaints(); }

            SaveWindowLocation();
        }

        public void btnUnlockRecord_Unloaded(object sender, RoutedEventArgs e)
        { UnloadRecord(); }

        public void btnRefershRelated_Click(object sender, RoutedEventArgs e)
        { FindRelated(thisComplaint.ID); }

        public void listRelated_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            if (row.Item != null) OpenAssAddMini((RelatedComplaint)row.Item, true);
        }

        public void listAdditional_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            if (row.Item != null) OpenAssAddMini((RelatedComplaint)row.Item, false);
        }

        public void tabRelated_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        { if (!NewRecord) FindRelated(thisComplaint.ID); }

        public void tabTasks_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        { if (!NewRecord) FindTasks(thisComplaint.ID); }

        public void tabRecords_MouseDown(object sender, MouseButtonEventArgs e)
        { if (!NewRecord) FindPRTasks(thisComplaint.ID); }

        public void MapBrowse_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.Uri != null)
            {
                e.Cancel = true;
                CatchMapLink(e.Uri.ToString());
            }
        }

        public void Button_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Button x = sender as Button;
            bool enabled = ((bool)e.NewValue);

            ButtonStateChanged(x, enabled);
        }
        
        public void ParcelBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        { ParcelPageLoaded(); }

        public void UpdateComplaintObject(object sender, RoutedEventArgs e)
        { thisComplaint.UpdateContentFromControls(); }

        public void btnAttachHome_Click(object sender, RoutedEventArgs e)
        { ExplorerControl.Control.ExplorerBrowserControl.NavigateLogLocation(0); }

        public void btnAttachBack_Click(object sender, RoutedEventArgs e)
        { if (ExplorerControl.Control.ExplorerBrowserControl.NavigationLog.CanNavigateBackward) ExplorerControl.Control.ExplorerBrowserControl.NavigateLogLocation(ExplorerControl.Control.ExplorerBrowserControl.NavigationLog.CurrentLocationIndex - 1); }

        public void btnAttachForward_Click(object sender, RoutedEventArgs e)
        { if (ExplorerControl.Control.ExplorerBrowserControl.NavigationLog.CanNavigateForward) ExplorerControl.Control.ExplorerBrowserControl.NavigateLogLocation(ExplorerControl.Control.ExplorerBrowserControl.NavigationLog.CurrentLocationIndex + 1); }

        public virtual void AddPortableTab() {}
        public virtual void RemovePortableTab() { }
    }
}
