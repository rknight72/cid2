using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace CID2
{
    /// <summary>
    /// Interaction logic for OpenComplaints.xaml
    /// </summary>
    ///

    public partial class OpenComplaints : Window, IDisposable
    {
        public OleDbConnection cidDB = MainWindow.cidDB;
        public List<GenericComplaint> list { get; set; }
        public static ListCollectionView ComplaintsView { get; set; }
        public int FormType { get; set; }
        public double font { get; set; }

        public OpenComplaints()
        {
            InitializeComponent();
            FormType = 1;
            InitializeForm();
        }

        public OpenComplaints(int formtype)
        {
            InitializeComponent();
            FormType = formtype;
            InitializeForm();
        }

        public void InitializeForm()
        {
            Dictionary<int, string> dictDateTypes = new Dictionary<int, string>();
            dictDateTypes.Add(0, "All Date Types");
            dictDateTypes.Add(1, "Date Received");
            dictDateTypes.Add(2, "Incident Date");
            cboDateType.ItemsSource = dictDateTypes;
            cboDateType.SelectedIndex = 0;

            List<ComplaintCategory> catList = new List<ComplaintCategory>();
            foreach (ComplaintCategory cat in MainWindow.ComplaintCategories) { catList.Add(cat); }
            catList.Add(new ComplaintCategory(0, " All Categories"));
            ListCollectionView catView = new ListCollectionView(catList);
            catView.SortDescriptions.Add(new SortDescription("Label", ListSortDirection.Ascending));
            FillCombo(cboCompCategoryList, catView);

            List<ComplaintLocation> locList = new List<ComplaintLocation>();
            foreach (ComplaintLocation loc in MainWindow.ComplaintLocations) { locList.Add(loc); }
            locList.Add(new ComplaintLocation(0, "All Locations"));
            ListCollectionView locView = new ListCollectionView(locList);
            locView.SortDescriptions.Add(new SortDescription("Label", ListSortDirection.Ascending));
            FillCombo(cboCompLocationList, locView);

            List<ComplaintType> compList = new List<ComplaintType>();
            foreach (ComplaintType q in MainWindow.ComplaintTypes) { compList.Add(q); }
            compList.Add(new ComplaintType(0, " All Complaint Types", "", "", "", "", "", "", "", "", null, null, null, 0, null, null, null));
            ListCollectionView derp = new ListCollectionView(compList);
            derp.SortDescriptions.Add(new SortDescription("Label", ListSortDirection.Ascending));
            FillCombo(cboCompTypeList, derp);

            List<CETAtype> cetaList = new List<CETAtype>();
            foreach (CETAtype qq in MainWindow.CETATypes) { cetaList.Add(qq); }
            cetaList.Add(new CETAtype(0, "All CETA Types"));
            derp = new ListCollectionView(cetaList);
            derp.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            FillCombo(cboCETAType, derp);

            List<casestatus> statusList = new List<casestatus>();
            foreach (casestatus qqq in MainWindow.Statuses) { statusList.Add(qqq); }
            statusList.Add(new casestatus(0, "All Statuses", true));
            derp = new ListCollectionView(statusList);
            derp.SortDescriptions.Add(new SortDescription("Status", ListSortDirection.Ascending));
            FillCombo(cboStatusList, derp);

            FillCombo(cboTwpList, MainWindow.Townships);
            FillCombo(cboCityList, MainWindow.Cities);
            FillCombo(cboInspector, MainWindow.Users);

            EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyUpEvent, new KeyEventHandler(RefreshList), true);
            EventManager.RegisterClassHandler(typeof(Window), Mouse.MouseUpEvent, new MouseButtonEventHandler(RefreshList), true);

            SetupWindowLocation();
        }

        public static void RefreshList(object sender, EventArgs e)
        { if (sender is OpenComplaints) ComplaintsView.Refresh(); }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double[] dubs = { 6, 8, 10, 12, 14, 16, 18, 20, 22, 24 };
            List<double> dubsList = new List<double>(dubs);
            cboFont.ItemsSource = dubsList;

            if (!MainWindow.IsSettingsDBConnected()) MainWindow.settingsDB.Open();

            switch (FormType)
            {
                case 1:
                    SetupForm(MainWindow.settingsDB, "tblOpenComplaints", 2);
                    break;
                case 2:
                    SetupForm(MainWindow.settingsDB, "tblClosedComplaints", 3);
                    break;
                default:
                    SetupForm(MainWindow.settingsDB, "tblAllComplaints", 1);
                    break;
            }
            
            GetComplaints();
        }

        private void SetupWindowLocation()
        {
            if (!MainWindow.IsSettingsDBConnected()) MainWindow.settingsDB.Open();

            int id = FormType + 2;
            OleDbCommand dbCMD = new OleDbCommand("SELECT * FROM tblWindowPositions WHERE ID = " + id.ToString() + ";", MainWindow.settingsDB);
            OleDbDataReader dbReader = dbCMD.ExecuteReader();
            dbReader.Read();
            Title = dbReader.GetString(1);
            Top = dbReader.GetInt32(2);
            Left = dbReader.GetInt32(3);
            Height = dbReader.GetInt32(4);
            Width = dbReader.GetInt32(5);

            dbReader.Close();
        }

        private void SetupForm(OleDbConnection db, string table, int id)
        {
            cboStatusList.Visibility = System.Windows.Visibility.Visible;
            txtStatus.Visibility = System.Windows.Visibility.Visible;

            string strSQL = "SELECT FontSize from tblFonts where ID = " + id.ToString() + ";";
            OleDbCommand cmd = new OleDbCommand(strSQL, db);
            OleDbDataReader read = cmd.ExecuteReader();
            read.Read();
            font = read.GetDouble(0);
            cboFont.SelectedItem = font;
            read.Close();

            strSQL = "SELECT * FROM " + table +";";
            SetupColumns(strSQL, db);

            db.Close();
        }

        private void SetupColumns(string strSQL, OleDbConnection db)
        {
            Style sty = new System.Windows.Style(typeof(DataGridColumnHeader));
            sty.Setters.Add(new Setter(FontWeightProperty, FontWeights.SemiBold));

            Style s = new Style();
            s.Setters.Add(new Setter(FontSizeProperty, font));

            OleDbCommand cmd = new OleDbCommand(strSQL, db);
            OleDbDataReader read = cmd.ExecuteReader();

            List<DataGridTextColumn> colList = new List<DataGridTextColumn>();
            while (read.Read())
            {
                DataGridColumnHeader head = new DataGridColumnHeader();
                head.Content = read.GetString(1);
                head.MinWidth = 5.0;
                head.Style = sty;

                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = head;
                col.Binding = new Binding(read.GetString(2));
                if (read[3] != DBNull.Value) col.SortMemberPath = read.GetString(3);
                if (read[4] != DBNull.Value) col.Binding.StringFormat = read.GetString(4);
                col.Width = read.GetDouble(5);
                col.MinWidth = 5.0;
                col.CellStyle = s;

                listComplaints.Columns.Add(col);
            }
            read.Close();
        }

        public void GetComplaints()
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            string strSQL = "SELECT TOP 150 * FROM tbl_Complaints";
            if (FormType > 0) strSQL += " WHERE Status = " + FormType.ToString();
            strSQL += ";";

            OleDbCommand cidSQL = new OleDbCommand(strSQL, cidDB);
            OleDbDataReader cidReader = cidSQL.ExecuteReader();

            list = new List<GenericComplaint>();
            while (cidReader.Read())
            {
                GenericComplaint obj = MainWindow.Main.CreateGenericComplaint(cidReader);

                string tempSQL = "SELECT * FROM " + obj.Type.ComplaintTable + " WHERE ID = " + obj.SubID.ToString() + ";";
                OleDbCommand listSQL = new OleDbCommand(tempSQL, cidDB);
                OleDbDataReader listReader = listSQL.ExecuteReader();
                listReader.Read();
                obj.SiteID = listReader.GetInt32(1);
                int contact1 = (obj.Type.PrimaryContactTable != "null" && listReader[2] != DBNull.Value) ? listReader.GetInt32(2) : 0;
                int contact2 = obj.Type.SecondaryContactTable != "null" && (listReader[3] != DBNull.Value) ? listReader.GetInt32(3) : 0;
                int contact3 = (obj.Type.TertiaryContactTable != "null" && listReader[4] != DBNull.Value) ? listReader.GetInt32(4) : 0;
                int contact4 = (obj.Type.QuaternaryContactTable != "null" && listReader[5] != DBNull.Value) ? listReader.GetInt32(5) : 0;
                int contact5 = (obj.Type.QuinaryContactTable != "null" && listReader[6] != DBNull.Value) ? listReader.GetInt32(6) : 0;
                int contact6 = (obj.Type.SenaryContactTable != "null" && listReader[7] != DBNull.Value) ? listReader.GetInt32(7) : 0;

                for (int i = 0; i < listReader.FieldCount; i++)
                {
                    string temp = listReader.GetName(i);
                    if (temp == "SiteName") obj.FacName = (listReader[i] != DBNull.Value) ? listReader.GetString(i) : "";
                }
                
                if (obj.Type.QuaternaryContactTable != "null" && listReader.FieldCount > 8) obj.ANTSID = (listReader[8] != DBNull.Value) ? listReader.GetString(8) : "";

                listReader.Close();

                obj.Contact1 = obj.Contact2 = obj.Contact3 = obj.Contact4 = obj.Contact5 = obj.Contact6 = "";
                if (contact1 > 0)
                {
                    tempSQL = "SELECT Last_Name, First_Name FROM " + obj.Type.PrimaryContactTable + " WHERE ID = " + contact1.ToString() + ";";
                    listSQL = new OleDbCommand(tempSQL, cidDB);
                    listReader = listSQL.ExecuteReader();
                    listReader.Read();
                    if (listReader[0] != DBNull.Value) obj.Contact1 = listReader.GetString(0);
                    if (listReader[1] != DBNull.Value) obj.Contact1 += (obj.Contact1 != "") ? ", " + listReader.GetString(1) : listReader.GetString(1);
                    listReader.Close();
                }

                if (contact2 > 0)
                {
                    tempSQL = "SELECT Last_Name, First_Name FROM " + obj.Type.SecondaryContactTable + " WHERE ID = " + contact2.ToString() + ";";
                    listSQL = new OleDbCommand(tempSQL, cidDB);
                    listReader = listSQL.ExecuteReader();
                    listReader.Read();
                    if (listReader[0] != DBNull.Value) obj.Contact2 = listReader.GetString(0);
                    if (listReader[1] != DBNull.Value) obj.Contact2 += (obj.Contact2 != "") ? ", " + listReader.GetString(1) : listReader.GetString(1);
                    listReader.Close();
                }

                if (contact3 > 0)
                {
                    tempSQL = "SELECT Last_Name, First_Name FROM " + obj.Type.TertiaryContactTable + " WHERE ID = " + contact3.ToString() + ";";
                    listSQL = new OleDbCommand(tempSQL, cidDB);
                    listReader = listSQL.ExecuteReader();
                    listReader.Read();
                    if (listReader[0] != DBNull.Value) obj.Contact3 = listReader.GetString(0);
                    if (listReader[1] != DBNull.Value) obj.Contact3 += (obj.Contact3 != "") ? ", " + listReader.GetString(1) : listReader.GetString(1);
                    listReader.Close();
                }

                if (contact4 > 0)
                {
                    tempSQL = "SELECT Last_Name, First_Name FROM " + obj.Type.QuaternaryContactTable + " WHERE ID = " + contact4.ToString() + ";";
                    listSQL = new OleDbCommand(tempSQL, cidDB);
                    listReader = listSQL.ExecuteReader();
                    listReader.Read();
                    if (listReader[0] != DBNull.Value) obj.Contact4 = listReader.GetString(0);
                    if (listReader[1] != DBNull.Value) obj.Contact4 += (obj.Contact4 != "") ? ", " + listReader.GetString(1) : listReader.GetString(1);
                    listReader.Close();
                }

                if (contact5 > 0)
                {
                    tempSQL = "SELECT Last_Name, First_Name FROM " + obj.Type.QuinaryContactTable + " WHERE ID = " + contact5.ToString() + ";";
                    listSQL = new OleDbCommand(tempSQL, cidDB);
                    listReader = listSQL.ExecuteReader();
                    listReader.Read();
                    if (listReader[0] != DBNull.Value) obj.Contact5 = listReader.GetString(0);
                    if (listReader[1] != DBNull.Value) obj.Contact5 += (obj.Contact5 != "") ? ", " + listReader.GetString(1) : listReader.GetString(1);
                    listReader.Close();
                }

                if (contact6 > 0)
                {
                    tempSQL = "SELECT Last_Name, First_Name FROM " + obj.Type.SenaryContactTable + " WHERE ID = " + contact6.ToString() + ";";
                    listSQL = new OleDbCommand(tempSQL, cidDB);
                    listReader = listSQL.ExecuteReader();
                    listReader.Read();
                    if (listReader[0] != DBNull.Value) obj.Contact6 = listReader.GetString(0);
                    if (listReader[1] != DBNull.Value) obj.Contact6 += (obj.Contact6 != "") ? ", " + listReader.GetString(1) : listReader.GetString(1);
                    listReader.Close();
                }

                tempSQL = "SELECT Address, City, Facility_ID, Parcel, Township FROM " + obj.Type.SiteTable + " WHERE ID = " + obj.SiteID.ToString() + ";";
                listSQL = new OleDbCommand(tempSQL, cidDB);
                listReader = listSQL.ExecuteReader();
                listReader.Read();
                //obj.FacName = (listReader[0] != DBNull.Value) ? listReader.GetString(0) : obj.FacName = "";
                if (listReader[0] != DBNull.Value)
                {
                    obj.SiteAddress1 = listReader.GetString(0);
                    string a, b;
                    MainWindow.SplitAdd(obj.SiteAddress1, out a, out b);
                    obj.SortAddress = b;
                }
                else
                {
                    obj.SiteAddress1 = "";
                    obj.SortAddress = "";
                }
                if (listReader[1] != DBNull.Value && listReader[1] is Int32)
                {
                    city acity = new city();
                    MainWindow.GetSingleItem<city>(out acity, listReader.GetInt32(1), MainWindow.Cities);
                    obj.SiteCity = acity;
                }
                else
                {
                    obj.SiteCity = null;
                    if (listReader[1] != DBNull.Value && listReader[1] is String)
                    {
                        string temp = listReader.GetString(1);
                        foreach (city x in MainWindow.Cities)
                        { if (String.Equals(temp, x.Name, StringComparison.CurrentCultureIgnoreCase)) obj.SiteCity = x; }
                    }
                }
                if (listReader[2] != DBNull.Value && listReader[2] is String) obj.PlaceID_FacID = listReader.GetString(2);
                else if (listReader[2] != DBNull.Value && listReader[2] is Int32) obj.PlaceID_FacID = listReader.GetInt32(2).ToString();
                else obj.PlaceID_FacID = "";
                obj.Parcel = (listReader[3] != DBNull.Value) ? listReader.GetString(3) : obj.Parcel = "";
                if (listReader[4] != DBNull.Value)
                {
                    township t = new township();
                    MainWindow.GetSingleItem<township>(out t, listReader.GetInt32(4), MainWindow.Townships);
                    obj.Township = t;
                }
                else obj.Township = null;

                listReader.Close();

                list.Add(obj);
            }

            int sortcol = 0;
            ListSortDirection? sortdir = null;
            ComplaintsView = new ListCollectionView(list);
            SortDescription sort = new SortDescription("ID", ListSortDirection.Ascending);
            if (listComplaints.Items.Count > 0 && listComplaints.Items.SortDescriptions.Count > 0)
            {
                sort = listComplaints.Items.SortDescriptions[0];
                for(int i = 0; i < listComplaints.Columns.Count; i++)
                {
                    if (listComplaints.Columns[i].SortDirection != null)
                    {
                        sortcol = i;
                        sortdir = listComplaints.Columns[i].SortDirection;
                    }
                }
            }
            ComplaintsView.SortDescriptions.Clear();
            ComplaintsView.SortDescriptions.Add(sort);

            cidReader.Close();
            cidDB.Close();

            ComplaintsView.Filter = ListFilter;
            listComplaints.ItemsSource = ComplaintsView;
            if (sortdir != null)
            {
                listComplaints.Items.SortDescriptions.Add(sort);
                listComplaints.Columns[sortcol].SortDirection = sortdir;
                listComplaints.Items.Refresh();
            }
        }

        private bool ListFilter(object item)
        {
            GenericComplaint comp = (GenericComplaint)item;
            bool match = true;
            
            DateTime adate;
            if (cboDateType.SelectedIndex == 1 && DateTime.TryParse(dtStart.Text, out adate)) match = (comp.DateReceived >= dtStart.SelectedDate);
            if (match && cboCompCategoryList.SelectedIndex > 0) match = (comp.Type.Category.ID == (int)cboCompCategoryList.SelectedValue);
            if (match && cboCompLocationList.SelectedIndex > 0) match = (comp.Type.Location.ID == (int)cboCompLocationList.SelectedValue);
            if (match && cboDateType.SelectedIndex == 1 && DateTime.TryParse(dtEnd.Text, out adate)) match = (comp.DateReceived <= dtEnd.SelectedDate);
            if (match && cboDateType.SelectedIndex == 2 && DateTime.TryParse(dtStart.Text, out adate)) match = (comp.IncidentDate >= dtStart.SelectedDate);
            if (match && cboDateType.SelectedIndex == 2 && DateTime.TryParse(dtEnd.Text, out adate)) match = (comp.IncidentDate <= dtEnd.SelectedDate);
            if (match && cboDateType.SelectedIndex == 0 && DateTime.TryParse(dtStart.Text, out adate)) match = (comp.DateReceived >= dtStart.SelectedDate) || (comp.IncidentDate >= dtStart.SelectedDate);
            if (match && cboDateType.SelectedIndex == 0 && DateTime.TryParse(dtEnd.Text, out adate)) match = (comp.DateReceived <= dtEnd.SelectedDate) || (comp.IncidentDate <= dtEnd.SelectedDate);
            if (match && txtAddress.Text != "") match = (comp.SiteAddress1.IndexOf(txtAddress.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (match && cboCompTypeList.SelectedIndex > 0) match = (comp.Type.ID == (int)cboCompTypeList.SelectedValue);
            if (match && cboCityList.SelectedIndex > 0) match = (comp.SiteCity !=null && comp.SiteCity.ID == (int)cboCityList.SelectedValue);
            if (match && txtComplaint.Text != "") match = (comp.ComplaintNotes.IndexOf(txtComplaint.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (match && txtNotes.Text != "") match = (comp.InspectionNotes.IndexOf(txtNotes.Text, StringComparison.CurrentCultureIgnoreCase) != -1) || (comp.OtherNotes.IndexOf(txtNotes.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (match && txtParcel.Text != "") match = (comp.PlaceID_FacID.IndexOf(txtParcel.Text, StringComparison.CurrentCultureIgnoreCase) != -1) || (comp.Parcel.IndexOf(txtParcel.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (match && txtContact.Text != "") match = (comp.Contact1.IndexOf(txtContact.Text, StringComparison.CurrentCultureIgnoreCase) != -1) || (comp.Contact2.IndexOf(txtContact.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (match && cboInspector.SelectedIndex > 0) match = (comp.Inspector != null && comp.Inspector.ID == (int)cboInspector.SelectedValue);
            if (match && cboCETAType.SelectedIndex > 0) match = (comp.CETA != null && comp.CETA.ID == (int)cboCETAType.SelectedValue);
            if (match && cboTwpList.SelectedIndex > 0) match = (comp.Township != null && comp.Township.ID == (int)cboTwpList.SelectedValue);
            if (match && txtFacName.Text != "") match = (comp.FacName.IndexOf(txtFacName.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (FormType == 0 && match && cboStatusList.SelectedIndex > 0) match = (comp.Status.ID == (int)cboStatusList.SelectedValue);

            return match;
        }

        public void FillCombo(ComboBox combo, ListCollectionView list)
        {
            combo.ItemsSource = list;
            combo.SelectedIndex = 0;
        }

        private void listComplaints_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GenericComplaint complaint;
            DataGridRow row = sender as DataGridRow;

            if (row.Item != null)
            {
                complaint = ((GenericComplaint)row.Item);

                if (complaint.ID > 0)
                {
                    switch (complaint.Type.ID)
                    {
                        case 1:
                            AsbestosOtherCompForm form1 = new AsbestosOtherCompForm(MainWindow.Main.CreateAsbestosOther(complaint.ID), false);
                            try { form1.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form1.Close(); return; }
                            break;
                        case 2:
                            AsbestosFacilityCompForm form2 = new AsbestosFacilityCompForm(MainWindow.Main.CreateAsbestosFacility(complaint.ID), false);
                            try { form2.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form2.Close(); return; }
                            break;
                        case 3:
                            AsbestosNonPermFacForm form3 = new AsbestosNonPermFacForm(MainWindow.Main.CreateAsbestosNonPermFac(complaint.ID), false);
                            try { form3.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form3.Close(); return; }
                            break;
                        case 4:
                            OBOtherForm form4 = new OBOtherForm(MainWindow.Main.CreateOBOther(complaint.ID), false);
                            try { form4.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form4.Close(); return; }
                            break;
                        case 5:
                            OBFacCompForm form5 = new OBFacCompForm(MainWindow.Main.CreateOBFacility(complaint.ID), false);
                            try { form5.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form5.Close(); return; }
                            break;
                        case 6: //Open Burn Non-permitted Facility
                            OBNonPermFacForm form6 = new OBNonPermFacForm(MainWindow.Main.CreateOBNonFac(complaint.ID), false);
                            try { form6.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form6.Close(); return; }
                            break;
                        case 7: //Open Burn Residential
                            OBResCompForm form7 = new OBResCompForm(MainWindow.Main.CreateOBResidential(complaint.ID), false);
                            try { form7.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form7.Close(); return; }
                            break;
                        case 8:
                            OdorOtherCompForm form8= new OdorOtherCompForm(MainWindow.Main.CreateOdorOther(complaint.ID), false);
                            try { form8.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form8.Close(); return; }
                            break;
                        case 9:
                            OdorFacCompForm form9 = new OdorFacCompForm(MainWindow.Main.CreateOdorFacility(complaint.ID), false);
                            try { form9.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form9.Close(); return; }
                            break;
                        case 10:
                            OdorNonPermFacCompForm form10 = new OdorNonPermFacCompForm(MainWindow.Main.CreateOdorNonPermFac(complaint.ID), false);
                            try { form10.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form10.Close(); return; }
                            break;
                        case 11://Odor Residential
                            OdorResCompForm form11 = new OdorResCompForm(MainWindow.Main.CreateOdorResidential(complaint.ID), false);
                            try { form11.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form11.Close(); return; }
                            break;
                        case 12:
                            OtherOtherCompForm form12 = new OtherOtherCompForm(MainWindow.Main.CreateOtherOther(complaint.ID), false);
                            try { form12.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form12.Close(); return; }
                            break;
                        case 13:
                            OtherFacCompForm form13 = new OtherFacCompForm(MainWindow.Main.CreateOtherFacility(complaint.ID), false);
                            try { form13.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form13.Close(); return; }
                            break;
                        case 14:
                            OtherNonPermFacCompForm form14 = new OtherNonPermFacCompForm(MainWindow.Main.CreateOtherNonPermFac(complaint.ID), false);
                            try { form14.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form14.Close(); return; }
                            break;
                        case 15:
                            OtherResCompForm form15 = new OtherResCompForm(MainWindow.Main.CreateOtherResidential(complaint.ID), false);
                            try { form15.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form15.Close(); return; }
                            break;
                        case 16:
                            DustOtherCompForm form16 = new DustOtherCompForm(MainWindow.Main.CreateDustOther(complaint.ID), false);
                            try { form16.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form16.Close(); return; }
                            break;
                        case 17:
                            DustFacilityCompForm form17 = new DustFacilityCompForm(MainWindow.Main.CreateDustFacility(complaint.ID), false);
                            try { form17.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form17.Close(); return; }
                            break;
                        case 18:
                            DustNonPermFacCompForm form18 = new DustNonPermFacCompForm(MainWindow.Main.CreateDustNonPermFac(complaint.ID), false);
                            try { form18.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form18.Close(); return; }
                            break;
                        case 19:
                            DustResCompForm form19 = new DustResCompForm(MainWindow.Main.CreateDustResidential(complaint.ID), false);
                            try { form19.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form19.Close(); return; }
                            break;
                        case 20:
                            ReleaseOtherCompForm form20 = new ReleaseOtherCompForm(MainWindow.Main.CreateReleaseOther(complaint.ID), false);
                            try { form20.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form20.Close(); return; }
                            break;
                        case 21:
                            ReleaseFacCompForm form21 = new ReleaseFacCompForm(MainWindow.Main.CreateReleaseFacility(complaint.ID), false);
                            try { form21.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form21.Close(); return; }
                            break;
                        case 22:
                            ReleaseNonPermFacCompForm form22 = new ReleaseNonPermFacCompForm(MainWindow.Main.CreateReleaseNonPermFac(complaint.ID), false);
                            try { form22.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form22.Close(); return; }
                            break;
                        case 23:
                            ReleaseResCompForm form23 = new ReleaseResCompForm(MainWindow.Main.CreateReleaseResidential(complaint.ID), false);
                            try { form23.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form23.Close(); return; }
                            break;
                        case 24:
                            AsbestosResCompForm form24 = new AsbestosResCompForm(MainWindow.Main.CreateAsbestosResidential(complaint.ID), false);
                            try { form24.Show(); }
                            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form24.Close(); return; }
                            break;
                        default:
                            break;
                    }
                }
            }

            e.Handled = true;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        { GetComplaints(); }

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        // NOTE: Leave out the finalizer altogether if this class doesn't 
        // own unmanaged resources itself, but leave the other methods
        // exactly as they are. 
        ~OpenComplaints()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }
        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (cidDB != null)
                {
                    cidDB.Dispose();
                    cidDB = null;
                }
            }
            // free native resources if there are any.
            /*if (nativeResource != IntPtr.Zero) 
            {
                Marshal.FreeHGlobal(nativeResource);
                nativeResource = IntPtr.Zero;
            }*/
        }

        private void SaveSettings()
        {
            int id = FormType + 1;
            string table= "";
            switch(FormType)
            {
                case 1:
                    table = "tblOpenComplaints";
                    break;
                case 2:
                    table = "tblClosedComplaints";
                    break;
                default:
                    table = "tblAllComplaints";
                    break;
            }

            if (!MainWindow.IsSettingsDBConnected()) MainWindow.settingsDB.Open();

            string strSQL = "UPDATE tblFonts SET FontSize = @font WHERE ID = " + id.ToString() + ";";
            OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.settingsDB);
            cidCMD.Parameters.AddWithValue("@font", (double)cboFont.SelectedValue);
            cidCMD.ExecuteNonQuery();

            foreach(DataGridTextColumn x in listComplaints.Columns)
            {
                int colid = x.DisplayIndex + 1;
                strSQL = "UPDATE " + table + " SET Label = @label, BindingPath = @path, SortMemberPath = @sort, Format = @format, Width = @width WHERE ID = " + colid.ToString() + ";";
                cidCMD = new OleDbCommand(strSQL, MainWindow.settingsDB);
                cidCMD.Parameters.AddWithValue("@label", (string)((DataGridColumnHeader)x.Header).Content);
                cidCMD.Parameters.AddWithValue("@path", ((Binding)x.Binding).Path.Path);
                cidCMD.Parameters.AddWithValue("@sort", x.SortMemberPath);
                cidCMD.Parameters.Add("@format", OleDbType.Char).Value = (x.Binding.StringFormat != null) ? x.Binding.StringFormat : (object)DBNull.Value;
                cidCMD.Parameters.AddWithValue("@width", x.Width.Value);
                cidCMD.ExecuteNonQuery();
            }
        }

        private void cboFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Style s = new Style();
            s.Setters.Add(new Setter(FontSizeProperty, (double)cboFont.SelectedValue));

            foreach (DataGridTextColumn x in listComplaints.Columns)
            { x.CellStyle = s; }

            FontFamily fontfam = (FontFamily)listComplaints.GetValue(FontFamilyProperty);
            listComplaints.RowHeight = ((double)cboFont.SelectedValue +1.25) * fontfam.LineSpacing;

            e.Handled = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!MainWindow.IsSettingsDBConnected()) MainWindow.settingsDB.Open();

            int id = FormType + 2;
            string strSQL = "UPDATE tblWindowPositions SET winTop = @top, winLeft = @left, winHeight = @height, winWidth = @width WHERE ID = " + id.ToString() + ";";
            OleDbCommand cmd = new OleDbCommand(strSQL, MainWindow.settingsDB);
            cmd.Parameters.AddWithValue("@top", Top);
            cmd.Parameters.AddWithValue("@left", Left);
            cmd.Parameters.AddWithValue("@height", Height);
            cmd.Parameters.AddWithValue("@width", Width);
            cmd.ExecuteNonQuery();

            SaveSettings();
        }
    }
}
