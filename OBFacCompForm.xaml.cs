using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Controls;
using mshtml;

namespace CID2
{
    /// <summary>
    /// Interaction logic for OBFacCompForm.xaml
    /// </summary>
    public partial class OBFacCompForm : FacilityComplaintForm
    {
        public new OBFacilityComplaint aComplaint;
        public new OBFacilityComplaint oComplaint;

        public Complaint tempComp;

        public OBFacCompForm()
        {
            InitializeComponent();

            aComplaint = new OBFacilityComplaint();
            NewRecord = true;

            InitializeFormControls();
        }

        public OBFacCompForm(OBFacilityComplaint complaint, bool isnew)
        {
            InitializeComponent();

            NewRecord = isnew;
            aComplaint = complaint;

            InitializeFormControls();
        }

        public void InitializeFormControls()
        {
            thisComplaint = aComplaint;
            if (!NewRecord) oComplaint = (OBFacilityComplaint)aComplaint.MyClone();
            else oComplaint = null;

            RegisterEvents(expAttach);

            FillTaskCombos(cboTasks_nolock, cboPRTasks_nolock);
            FillCombo(cboStatusBox, MainWindow.Statuses);
            FillCombo(cboRcvdMethod, MainWindow.ReceivedMethods);
            FillCombo(cboInspectorBox, MainWindow.Users);
            FillCombo(cboReceivedByBox, MainWindow.Users);
            FillCombo(cboCETABox, aComplaint.CompType.cetaTypes);

            FillMapCombos(cboMapCompType_neverlock, cboMapRadius_neverlock);

            SetupWindowLocation();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FileBrowserControl control = new FileBrowserControl(expAttach);
            SetFormControls(MainGrid, null, MapBrowse, listRelated, listTasks, null, null, control, cboMapRadius_neverlock,
                cboMapCompType_neverlock, btnAttachBack_nolock, btnAttachForward_nolock, listPRTasks, listAdditional, ChangeLogGrid, txtBlankCoords);


            if (aComplaint != null)
            {
                aComplaint.SetForm(this);
                if (aComplaint.ComplainantInfo != null) tabComplainant.Content = aComplaint.ComplainantInfo.thisControl;
                if (aComplaint.FacilityControl != null) tabLocation.Content = aComplaint.FacilityControl;
                if (aComplaint.ContactControl != null) tabContact.Content = aComplaint.ContactControl;
                aComplaint.SetControls(txtIDBlock, txtComplaint, InspectionBox, OtherNotesBox, cboReceivedByBox, cboInspectorBox,
                    cboStatusBox, dateReceivedBox, dateIncidentBox, dateClosedBox, dtRetEnds, cboCETABox, cboRcvdMethod, dateInvestigatedBox);
                aComplaint.UpdateControlContent();
                SetAttachmentsFolder();

                EventManager.RegisterClassHandler(typeof(Control), Control.LostFocusEvent, new RoutedEventHandler(UpdateComplaintObject));
            }

            SetRecordControlButtons(NewRecord, IDBlock, btnUnlockRecord_nolock, btnSaveRecord, txtRetEnds, dtRetEnds);

            if (!NewRecord)
            {
                FindRelated(aComplaint.ID);
                FindTasks(aComplaint.ID);
                FindPRTasks(aComplaint.ID);
                FindMultiples(aComplaint.FacilityControl.thisFacility.ID);
                FindChangeLog();

                IsLocked = false;
                bool closed = ((aComplaint.Status.ID == 2) && (MainWindow.thisUser.Role > 3));
                foreach (object x in LogicalTreeHelper.GetChildren(ParentGrid))
                { if (x is Visual) GetAllControls((Visual)x, IsLocked, closed); }
                CheckStatus(true);
            }
            else
            {
                txtIDBlock.Text = "ID: Unassigned";
                IsLocked = true;

                tabRelated.IsEnabled = false;
                tabTasks.IsEnabled = false;
                tabAttach.IsEnabled = false;
                tabRecords.IsEnabled = false;

                if (aComplaint.FacilityControl != null && aComplaint.FacilityControl.thisFacility.ID != 0) FindMultiples(aComplaint.FacilityControl.thisFacility.ID);
                else tabAdditionalComplaints.IsEnabled = false;
            }

            if (MainWindow.thisUser.Role > 3)
            {
                foreach (casestatus item in cboStatusBox.Items)
                { if (item.ID == 2 || aComplaint.Status.ID == 2) item.Active = false; }
            }
        }

        public override void AddPortableTab()
        {
            TabItem newtab = new TabItem();
            newtab.Header = "Portable Location";
            newtab.Name = "PortableTab";

            if (aComplaint.PortableLocation == null)
            {
                aComplaint.PortableLocation = new OtherLocation();
                aComplaint.PortableLocation.SetOwner((Complaint)aComplaint);
            }

            newtab.Content = aComplaint.PortableLocation.SiteControl;

            SiteInfoTab.Items.Insert(1, newtab);
        }

        public override void RemovePortableTab()
        {
            SiteInfoTab.Items.RemoveAt(1);
            aComplaint.PortableLocation = new OtherLocation(aComplaint.PortableLocation.ID, (Complaint)aComplaint);
        }

        private void btnSaveRecord_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to save this record?", "Confirm", button);
            string log = "";

            if (aComplaint.FacilityControl.thisFacility.County != null && (aComplaint.FacilityControl.thisFacility.Latitude != 0 || aComplaint.FacilityControl.thisFacility.Longitude != 0))
            {
                if (OutOfBounds(aComplaint.FacilityControl.thisFacility.County, aComplaint.FacilityControl.thisFacility.Latitude, aComplaint.FacilityControl.thisFacility.Longitude))
                {
                    result = MessageBoxResult.Cancel;
                    MessageBox.Show("The selected coordinates are outside the bounds of the selected county. The record will not be saved.");
                }
            }

            if (result == MessageBoxResult.Yes)
            {
                if (aComplaint.ID == 0)
                {
                    bool closed = ((aComplaint.Status.ID == 2) && (MainWindow.thisUser.Role > 3));
                    foreach (object x in LogicalTreeHelper.GetChildren(ParentGrid))
                    { if (x is Visual) GetAllControls((Visual)x, false, closed); }
                    IsLocked = false;
                    btnUnlockRecord_nolock.IsEnabled = true;
                    btnUnlockRecord_nolock.Content = "EDIT RECORD";
                    btnSaveRecord.IsEnabled = false;
                    tabRelated.IsEnabled = true;
                    tabTasks.IsEnabled = true;
                    tabAttach.IsEnabled = true;
                    tabRecords.IsEnabled = true;
                }
                else
                {
                    LockRecord(false);

                    if (!NewRecord)
                    {
                        List<string> difList = new List<string>();
                        difList = aComplaint.CompareOBFacDataMembers(oComplaint, difList);
                        foreach (string x in difList)
                        { log += " " + x + ","; }
                        log = log.TrimEnd(',');
                    }
                }
                aComplaint.SaveComplaint();
                if (log != "") aComplaint.SaveLog(MainWindow.thisUser.Inits + " on " + DateTime.Now.ToShortDateString() + " changed C" + aComplaint.ID.ToString() + " fields - " + log);
                oComplaint = (OBFacilityComplaint)aComplaint.MyClone();
                aComplaint.UpdateContentFromControls();
            }
        }

        private void MapBrowse_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((bool)e.NewValue) == true && aComplaint.FacilityControl.thisFacility.Latitude != 0 && aComplaint.FacilityControl.thisFacility.Longitude != 0)
            LoadMap(aComplaint.FacilityControl.thisFacility.Latitude.ToString(), aComplaint.FacilityControl.thisFacility.Longitude.ToString(), true);
        }

        private void btnTaskAdd_Click(object sender, RoutedEventArgs e)
        { if (cboTasks_nolock.Text != "") AddTask((TaskName)cboTasks_nolock.SelectedItem, "tbl_Complaint_Tasks"); }

        private void btnTaskEdit_Click(object sender, RoutedEventArgs e)
        { if (listTasks.SelectedItem != null) EditThisTask((ComplaintTask)listTasks.SelectedItem, "tbl_Complaint_tasks"); }

        private void btnTaskRemove_Click(object sender, RoutedEventArgs e)
        { if (listTasks.SelectedItem != null) DeleteTask((ComplaintTask)listTasks.SelectedItem, "tbl_Complaint_Tasks"); }

        private void btnPRTaskAdd_Click(object sender, RoutedEventArgs e)
        { if (cboPRTasks_nolock.Text != "") AddPRTask((TaskName)cboPRTasks_nolock.SelectedItem, "tbl_PublicRecords_Tasks"); }

        private void btnPRTaskEdit_Click(object sender, RoutedEventArgs e)
        { if (listPRTasks.SelectedItem != null) EditPRTask((ComplaintTask)listPRTasks.SelectedItem, "tbl_PublicRecords_Tasks"); }

        private void btnPRTaskRemove_Click(object sender, RoutedEventArgs e)
        { if (listPRTasks.SelectedItem != null) DeleteTask((ComplaintTask)listPRTasks.SelectedItem, "tbl_PublicRecords_Tasks"); }

        private void btnRelatedSearch_Click(object sender, RoutedEventArgs e)
        {
            if (aComplaint.ID > 0)
            {
                RelatedSearch search = new RelatedSearch(aComplaint.ID, listRelated.Items);
                try { search.ShowDialog(); }
                catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); search.Close(); return; }

                FindRelated(aComplaint.ID);
            }
        }

        public void btnRefreshMap_Click(object sender, RoutedEventArgs e)
        { LoadMap(aComplaint.FacilityControl.thisFacility.Latitude.ToString(), aComplaint.FacilityControl.thisFacility.Longitude.ToString(), false); }

        private void btnRemoveRelated_Click(object sender, RoutedEventArgs e)
        { if (listRelated.SelectedIndex >= 0) RemoveRelation((RelatedComplaint)listRelated.SelectedItem); }

        public override void CallRestoreDataMembers()
        {  aComplaint.RestoreComplaintDataMembers(oComplaint); }

        private void btnUnlockRecord_Click(object sender, RoutedEventArgs e)
        {
            bool locked = txtComplaint.IsEnabled;
            bool confirm = true;
            if (locked)
            {
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show("To save your changes, click save. If you continue then all changes will be discarded. Are you sure you want to continue?", "Confirm", button);
                confirm = (result == MessageBoxResult.Yes);
            }
            if (confirm)
            {
                LockRecord(!locked);
                if (!NewRecord) aComplaint.RestoreOBFacDataMembers(oComplaint);
                aComplaint.UpdateControlContent();
                FindTasks(aComplaint.ID);
                FindPRTasks(aComplaint.ID);
            }

            CheckStatus(false);
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            TableRowGroup group = new TableRowGroup();
            TableRow row = new TableRow();
            Paragraph y = new Paragraph(new Run("Facility Info" + Environment.NewLine));
            y.FontSize = 16;
            y.FontWeight = FontWeights.Bold;
            y.TextAlignment = TextAlignment.Center;
            TableCell cell = new TableCell(y);
            cell.ColumnSpan = 3;
            row.Cells.Add(cell);
            group.Rows.Add(row);

            string x = (aComplaint.FacilityControl.thisFacility.FacilityID != "") ? aComplaint.FacilityControl.thisFacility.FacilityID : "";
            row = new TableRow();
            y = new Paragraph(new Run(x));
            y.FontSize = 12;
            cell = new TableCell(y);
            cell.ColumnSpan = 3;
            row.Cells.Add(cell);
            group.Rows.Add(row);

            x = (aComplaint.FacilityControl.thisFacility.FacName != "") ? aComplaint.FacilityControl.thisFacility.FacName : "";
            row = new TableRow();
            y = new Paragraph(new Run(x));
            y.FontSize = 12;
            cell = new TableCell(y);
            cell.ColumnSpan = 3;
            row.Cells.Add(cell);
            group.Rows.Add(row);

            x = (aComplaint.FacilityControl.thisFacility.AddressLine1 != "") ? aComplaint.FacilityControl.thisFacility.AddressLine1 : "";
            row = new TableRow();
            y = new Paragraph(new Run(x));
            y.FontSize = 12;
            cell = new TableCell(y);
            cell.ColumnSpan = 3;
            row.Cells.Add(cell);
            group.Rows.Add(row);

            x = ((aComplaint.FacilityControl.thisFacility.City != null) ? aComplaint.FacilityControl.thisFacility.City : "") + ", "
                + ((aComplaint.FacilityControl.thisFacility.State != null) ? aComplaint.FacilityControl.thisFacility.State.Abbr : "")
                + " " + ((aComplaint.FacilityControl.thisFacility.Zip != null) ? aComplaint.FacilityControl.thisFacility.Zip : "");
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

            group.SetValue(TitleProperty, "Facility Info");

            string strAdd = "<td valign=\"top\" style=\"width:50%;\"><h3 align=\"left\">Facility Info</h3>";
            strAdd += "<p style=\"font-size:12px;\">" + aComplaint.FacilityControl.thisFacility.FacilityID + "<br>";
            strAdd += "<p style=\"font-size:12px;\">" + aComplaint.FacilityControl.thisFacility.FacName + "<br>";
            strAdd += "<p style=\"font-size:12px;\">" + ((aComplaint.FacilityControl.thisFacility.AddressLine1 != null) ? aComplaint.FacilityControl.thisFacility.AddressLine1 : "") + "<br>";
            if (aComplaint.FacilityControl.thisFacility.City != null) strAdd += ((aComplaint.FacilityControl.thisFacility.City != null) ? aComplaint.FacilityControl.thisFacility.City : "")
                + ", " + ((aComplaint.FacilityControl.thisFacility.State != null) ? aComplaint.FacilityControl.thisFacility.State.Abbr : "") + " "
                + ((aComplaint.FacilityControl.thisFacility.Zip != null) ? aComplaint.FacilityControl.thisFacility.Zip : "") + "<br>";
            strAdd += "</td>";

            FindMultiples(aComplaint.FacilityControl.thisFacility.ID);
            PrintGenerator(group, strAdd, null, aComplaint.ContactControl.thisContact);
        }

        private void tabAdditionalComplaints_MouseDown(object sender, MouseButtonEventArgs e)
        { if (aComplaint.FacilityControl.thisFacility != null && aComplaint.FacilityControl.thisFacility.ID != 0) FindMultiples(aComplaint.FacilityControl.thisFacility.ID); }

        public override void SetFacility(int id)
        {
            /*Facility fac = new Facility();
            MainWindow.GetSingleItem<Facility>(out fac, id, MainWindow.Facilities);
            aComplaint.Fac = fac;
            aComplaint.SetFacilityControls(cboFacilityID, txtFacName_nolock, txtFacAddress_nolock, txtFacCity_nolock, cboFacState_nolock, txtZipBox_nolock,
                cboFacCounty, cboFacTownship, txtFacLon, txtFacLat, cboFacStat_nolock, cboFacClass_nolock, chkPortable_nolock);
            ContactList.Filter = (y) => ((FacilityContact)y).FacID == aComplaint.Fac.FacilityID || ((FacilityContact)y).ID == 0;

            if (aComplaint.Fac.Portable)
            {
                bool exists = false;
                foreach(TabItem x in SiteInfoTab.Items)
                { if ((string)(x.Header) == "Portable Location") exists = true; }

                if (!exists)
                {
                    TabItem tab = new TabItem();
                    tab.Header = "Portable Location";

                    Grid grid = new Grid();
                    grid.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(229, 229, 229));
                    for (int i = 0; i < 4; i++)
                    {
                        RowDefinition row = new RowDefinition();
                        row.Height = new GridLength(1, GridUnitType.Star);
                        row.MinHeight = 50;
                        row.MaxHeight = 60;
                        grid.RowDefinitions.Add(row);
                    }
                    
                    TextBlock block = new TextBlock(new Run("Location Description"));
                    block.FontSize = 12;
                    block.FontWeight = FontWeights.Bold;
                    block.Margin = new Thickness(10, 5, 0, 0);
                    block.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    block.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    block.SetValue(Grid.RowProperty, 0);
                    block.SetValue(Grid.ColumnProperty, 0);
                    grid.Children.Add(block);

                    TextBox box = new TextBox();
                    box.Name = "txtLocDesc";
                    box.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    box.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    box.Margin = new Thickness(10, 20, 10, 0);
                    box.FontSize = 14;
                    box.MaxLength = 512;
                    box.TabIndex = 10511;
                    box.SetValue(Grid.RowProperty, 0);
                    box.SetValue(Grid.ColumnProperty, 0);
                    grid.Children.Add(box);

                    Grid grid2 = new Grid();
                    ColumnDefinition col = new ColumnDefinition();
                    col.Width = new GridLength(2, GridUnitType.Star);
                    grid2.ColumnDefinitions.Add(col);
                    col = new ColumnDefinition();
                    col.Width = new GridLength(1, GridUnitType.Star);
                    grid2.ColumnDefinitions.Add(col);
                    col = new ColumnDefinition();
                    col.Width = new GridLength(1, GridUnitType.Star);
                    grid2.ColumnDefinitions.Add(col);
                    grid2.SetValue(Grid.RowProperty, 1);
                    grid2.SetValue(Grid.ColumnProperty, 0);
                    grid2.SetValue(Grid.ColumnSpanProperty, 2);

                    block = new TextBlock(new Run("Address Line 1"));
                    block.FontSize = 12;
                    block.FontWeight = FontWeights.Bold;
                    block.Margin = new Thickness(10, 5, 0, 0);
                    block.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    block.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    block.SetValue(Grid.RowProperty, 0);
                    block.SetValue(Grid.ColumnProperty, 0);
                    grid2.Children.Add(block);

                    box = new TextBox();
                    box.Name = "txtLocAddLine1";
                    box.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    box.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    box.Margin = new Thickness(10, 20, 5, 0);
                    box.FontSize = 14;
                    box.MaxLength = 512;
                    box.TabIndex = 10513;
                    box.SetValue(Grid.RowProperty, 0);
                    box.SetValue(Grid.ColumnProperty, 0);
                    box.SetValue(Grid.ColumnSpanProperty, 2);
                    grid2.Children.Add(box);

                    block = new TextBlock(new Run("Latitude"));
                    block.FontSize = 12;
                    block.FontWeight = FontWeights.Bold;
                    block.Margin = new Thickness(5, 5, 0, 0);
                    block.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    block.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    block.SetValue(Grid.RowProperty, 0);
                    block.SetValue(Grid.ColumnProperty, 2);
                    grid2.Children.Add(block);

                    box = new TextBox();
                    box.Name = "txtLocLat";
                    box.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    box.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    box.Margin = new Thickness(5, 20, 10, 0);
                    box.FontSize = 14;
                    box.MaxLength = 128;
                    box.TabIndex = 10515;
                    box.SetValue(Grid.RowProperty, 0);
                    box.SetValue(Grid.ColumnProperty, 2);
                    box.PreviewTextInput += txtcoords_PreviewTextInput;
                    DataObject.AddPastingHandler(box, txtcoords_Pasting);
                    grid2.Children.Add(box);

                    grid.Children.Add(grid2);

                    Grid grid3 = new Grid();
                    col = new ColumnDefinition();
                    col.Width = new GridLength(2, GridUnitType.Star);
                    grid3.ColumnDefinitions.Add(col);
                    col = new ColumnDefinition();
                    col.Width = new GridLength(1, GridUnitType.Star);
                    grid3.ColumnDefinitions.Add(col);
                    col = new ColumnDefinition();
                    col.Width = new GridLength(1, GridUnitType.Star);
                    grid3.ColumnDefinitions.Add(col);
                    grid3.SetValue(Grid.RowProperty, 2);
                    grid3.SetValue(Grid.ColumnProperty, 0);
                    grid3.SetValue(Grid.ColumnSpanProperty, 2);

                    block = new TextBlock(new Run("Address Line 2"));
                    block.FontSize = 12;
                    block.FontWeight = FontWeights.Bold;
                    block.Margin = new Thickness(10, 5, 0, 0);
                    block.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    block.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    block.SetValue(Grid.RowProperty, 0);
                    block.SetValue(Grid.ColumnProperty, 0);
                    grid3.Children.Add(block);

                    box = new TextBox();
                    box.Name = "txtLocAddLine2";
                    box.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    box.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    box.Margin = new Thickness(10, 20, 5, 0);
                    box.FontSize = 14;
                    box.MaxLength = 512;
                    box.TabIndex = 10516;
                    box.SetValue(Grid.RowProperty, 1);
                    box.SetValue(Grid.ColumnProperty, 0);
                    grid3.Children.Add(box);

                    block = new TextBlock(new Run("Township"));
                    block.FontSize = 12;
                    block.FontWeight = FontWeights.Bold;
                    block.Margin = new Thickness(5, 5, 0, 0);
                    block.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    block.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    block.SetValue(Grid.RowProperty, 1);
                    block.SetValue(Grid.ColumnProperty, 1);
                    grid3.Children.Add(block);

                    box = new TextBox();
                    box.Name = "txtLocTownship";
                    box.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    box.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    box.Margin = new Thickness(5, 20, 5, 0);
                    box.FontSize = 14;
                    box.MaxLength = 512;
                    box.TabIndex = 10517;
                    box.SetValue(Grid.RowProperty, 1);
                    box.SetValue(Grid.ColumnProperty, 1);
                    grid3.Children.Add(box);

                    block = new TextBlock(new Run("Longitude"));
                    block.FontSize = 12;
                    block.FontWeight = FontWeights.Bold;
                    block.Margin = new Thickness(5, 5, 0, 0);
                    block.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    block.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    block.SetValue(Grid.RowProperty, 1);
                    block.SetValue(Grid.ColumnProperty, 2);
                    grid3.Children.Add(block);

                    box = new TextBox();
                    box.Name = "txtLocLon";
                    box.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    box.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    box.Margin = new Thickness(5, 20, 10, 0);
                    box.FontSize = 14;
                    box.MaxLength = 128;
                    box.TabIndex = 10518;
                    box.SetValue(Grid.RowProperty, 1);
                    box.SetValue(Grid.ColumnProperty, 2);
                    box.PreviewTextInput += txtcoords_PreviewTextInput;
                    DataObject.AddPastingHandler(box, txtcoords_Pasting);
                    grid3.Children.Add(box);

                    grid.Children.Add(grid3);

                    Grid grid4 = new Grid();
                    col = new ColumnDefinition();
                    col.Width = new GridLength(2, GridUnitType.Star);
                    grid4.ColumnDefinitions.Add(col);
                    col = new ColumnDefinition();
                    col.Width = new GridLength(1, GridUnitType.Star);
                    grid4.ColumnDefinitions.Add(col);
                    col = new ColumnDefinition();
                    col.Width = new GridLength(1, GridUnitType.Star);
                    grid4.ColumnDefinitions.Add(col);
                    grid4.SetValue(Grid.RowProperty, 3);
                    grid4.SetValue(Grid.ColumnProperty, 0);
                    grid4.SetValue(Grid.ColumnSpanProperty, 2);

                    Grid grid4a = new Grid();
                    col = new ColumnDefinition();
                    col = new ColumnDefinition();
                    col.Width = new GridLength(1, GridUnitType.Star);
                    grid4a.ColumnDefinitions.Add(col);
                    col = new ColumnDefinition();
                    col.Width = new GridLength(65, GridUnitType.Pixel);
                    grid4a.ColumnDefinitions.Add(col);
                    grid4a.SetValue(Grid.RowProperty, 0);
                    grid4a.SetValue(Grid.ColumnProperty, 0);

                    block = new TextBlock(new Run("City"));
                    block.FontSize = 12;
                    block.FontWeight = FontWeights.Bold;
                    block.Margin = new Thickness(10, 5, 0, 0);
                    block.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    block.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    block.SetValue(Grid.RowProperty, 0);
                    block.SetValue(Grid.ColumnProperty, 0);
                    grid4a.Children.Add(block);

                    ComboBox combo = new ComboBox();
                    combo.Name = "cboLocCity";
                    combo.SelectedValuePath = "ID";
                    FillCombo(combo, MainWindow.Cities);
                    combo.IsSynchronizedWithCurrentItem = false;
                    combo.Margin = new Thickness(10, 20, 5, 0);
                    combo.TabIndex = 10519;
                    combo.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    combo.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    Style style = Application.Current.Resources["flatcombo"] as Style;
                    combo.Style = style;
                    combo.SetValue(Grid.RowProperty, 0);
                    combo.SetValue(Grid.ColumnProperty, 0);
                    grid4a.Children.Add(combo);

                    block = new TextBlock(new Run("State"));
                    block.FontSize = 12;
                    block.FontWeight = FontWeights.Bold;
                    block.Margin = new Thickness(5, 5, 0, 0);
                    block.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    block.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    block.SetValue(Grid.RowProperty, 0);
                    block.SetValue(Grid.ColumnProperty, 1);
                    grid4a.Children.Add(block);

                    combo = new ComboBox();
                    combo.Name = "cboLocState";
                    combo.SelectedValuePath = "ID";
                    FillCombo(combo, MainWindow.States);
                    combo.IsSynchronizedWithCurrentItem = false;
                    combo.Margin = new Thickness(5, 20, 5, 0);
                    combo.TabIndex = 10520;
                    combo.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    combo.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    combo.SetValue(Grid.RowProperty, 0);
                    combo.SetValue(Grid.ColumnProperty, 1);
                    combo.SelectedValue = 41;
                    combo.IsEnabled = false;
                    grid4a.Children.Add(combo);

                    grid4.Children.Add(grid4a);
                    grid.Children.Add(grid4);

                    tab.Content = grid;

                    SiteControlNonPermFac sitectrl = new SiteControlNonPermFac();
                    tab.Content = sitectrl;
                    SiteInfoTab.Items.Add(tab);
                }
            }
            else
            {
                foreach (TabItem x in SiteInfoTab.Items)
                {
                    if ((string)(x.Header) == "Portable Location")
                    {
                        SiteInfoTab.Items.Remove(x);
                        break;
                    }
                }
            }

            aComplaint.UpdateControlContent();
            aComplaint.UpdateContentFromControls();*/
        }
    }
}
