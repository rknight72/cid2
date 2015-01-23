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
    /// Interaction logic for AsbestosNonPermFacCompForm.xaml
    /// </summary>
    public partial class AsbestosNonPermFacForm : NonPermittedFacComplaintForm
    {
        public new AsbestosNonPermFacComplaint aComplaint;
        public new AsbestosNonPermFacComplaint oComplaint;

        public Complaint tempComp;

        public AsbestosNonPermFacForm()
        {
            InitializeComponent();

            aComplaint = new AsbestosNonPermFacComplaint();

            NewRecord = true;

            InitializeFormControls();
        }

        public AsbestosNonPermFacForm(AsbestosNonPermFacComplaint complaint, bool isnew)
        {
            InitializeComponent();

            NewRecord = isnew;
            aComplaint = complaint;

            InitializeFormControls();
        }

        public void InitializeFormControls()
        {
            thisComplaint = aComplaint;
            if (!NewRecord) oComplaint = (AsbestosNonPermFacComplaint)aComplaint.MyClone();
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
            SetFormControls(MainGrid, ParcelBrowse, MapBrowse, listRelated, listTasks, listAddresses, Parcel_Tab, control, cboMapRadius_neverlock,
                cboMapCompType_neverlock, btnAttachBack_nolock, btnAttachForward_nolock, listPRTasks, listAdditional, ChangeLogGrid, txtBlankCoords);

            if (aComplaint != null)
            {
                aComplaint.SetForm(this);
                if (aComplaint.ComplainantInfo != null) tabComplainant.Content = aComplaint.ComplainantInfo.thisControl;
                if (aComplaint.ComplaintAddress != null) tabLocation.Content = aComplaint.ComplaintAddress.SiteControl;
                if (aComplaint.Owner != null) tabOwner.Content = aComplaint.Owner.thisControl;
                if (aComplaint.DemoContractor != null) tabDemo.Content = aComplaint.DemoContractor.thisControl;
                if (aComplaint.AbatementContractor != null) tabAbatement.Content = aComplaint.AbatementContractor.thisControl;
                if (aComplaint.EntityCoordinator != null) tabCoordinator.Content = aComplaint.EntityCoordinator.thisControl;
                if (aComplaint.Landfill != null) tabLandfill.Content = aComplaint.Landfill.thisControl;
                if (aComplaint.OtherOperator != null) tabOtherOperator.Content = aComplaint.OtherOperator.thisControl;
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
                FindMultiples(aComplaint.ComplaintAddress.ID);
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

                if (aComplaint.ComplaintAddress != null && aComplaint.ComplaintAddress.ID != 0) FindMultiples(aComplaint.ComplaintAddress.ID);
                else tabAdditionalComplaints.IsEnabled = false;
            }
        }

        private void listAddresses_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            if (row.Item != null) DoubleClickParcel(row.Item);
        }

        private void btnSaveRecord_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to save this record?", "Confirm", button);
            string log = "";

            if (aComplaint.ComplaintAddress.County != null && (aComplaint.ComplaintAddress.Latitude != 0 || aComplaint.ComplaintAddress.Longitude != 0))
            {
                if (OutOfBounds(aComplaint.ComplaintAddress.County, aComplaint.ComplaintAddress.Latitude, aComplaint.ComplaintAddress.Longitude))
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
                        difList = aComplaint.CompareAsbestosNonPermFacDataMembers(oComplaint, difList);
                        foreach (string x in difList)
                        { log += " " + x + ","; }
                        log = log.TrimEnd(',');
                    }
                }
                aComplaint.SaveComplaint();
                if (log != "") aComplaint.SaveLog(MainWindow.thisUser.Inits + " on " + DateTime.Now.ToShortDateString() + " changed C" + aComplaint.ID.ToString() + " fields - " + log);
                oComplaint = (AsbestosNonPermFacComplaint)aComplaint.MyClone();
                aComplaint.UpdateContentFromControls();
                LoadMap(aComplaint.ComplaintAddress.SiteControl.txtPropLat.Text, aComplaint.ComplaintAddress.SiteControl.txtPropLon.Text, false);
            }
        }

        private void MapBrowse_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((bool)e.NewValue) == true && aComplaint.ComplaintAddress.Latitude != 0 && aComplaint.ComplaintAddress.Longitude != 0)
                LoadMap(aComplaint.ComplaintAddress.SiteControl.txtPropLat.Text, aComplaint.ComplaintAddress.SiteControl.txtPropLon.Text, true);
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
        { LoadMap(aComplaint.ComplaintAddress.SiteControl.txtPropLat.Text, aComplaint.ComplaintAddress.SiteControl.txtPropLon.Text, false); }

        private void btnRemoveRelated_Click(object sender, RoutedEventArgs e)
        { if (listRelated.SelectedIndex >= 0) RemoveRelation((RelatedComplaint)listRelated.SelectedItem); }

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
                if (!NewRecord) aComplaint.RestoreAsbestosNonPermFacDataMembers(oComplaint);
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
            Paragraph y = new Paragraph(new Run("Site Address" + Environment.NewLine));
            y.FontSize = 16;
            y.FontWeight = FontWeights.Bold;
            y.TextAlignment = TextAlignment.Center;
            TableCell cell = new TableCell(y);
            cell.ColumnSpan = 3;
            row.Cells.Add(cell);
            group.Rows.Add(row);

            string x = "";
            if (aComplaint.FacilityName != "") x = aComplaint.FacilityName;
            row = new TableRow();
            y = new Paragraph(new Run(x));
            y.FontSize = 12;
            cell = new TableCell(y);
            cell.ColumnSpan = 3;
            row.Cells.Add(cell);
            group.Rows.Add(row);

            x = "";
            if (aComplaint.ComplaintAddress.AddressLine1 != "") x = aComplaint.ComplaintAddress.AddressLine1;
            row = new TableRow();
            y = new Paragraph(new Run(x));
            y.FontSize = 12;
            cell = new TableCell(y);
            cell.ColumnSpan = 3;
            row.Cells.Add(cell);
            group.Rows.Add(row);

            x = "";
            if (aComplaint.ComplaintAddress.AddressLine2 != "") x = aComplaint.ComplaintAddress.AddressLine2;
            row = new TableRow();
            y = new Paragraph(new Run(x));
            y.FontSize = 12;
            cell = new TableCell(y);
            cell.ColumnSpan = 3;
            row.Cells.Add(cell);
            group.Rows.Add(row);

            x = "";
            if (aComplaint.ComplaintAddress.City != null) x = aComplaint.ComplaintAddress.City.Name + ", " + aComplaint.ComplaintAddress.State.Abbr + " " + aComplaint.ComplaintAddress.Zip;
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

            group.SetValue(TitleProperty, "Site Address");

            string strAdd = "<td valign=\"top\" style=\"width:50%;\"><h3 align=\"left\">Site Address</h3>";
            strAdd += "<p style=\"font-size:12px;\">" + aComplaint.ComplaintAddress.AddressLine1 + "<br>";
            if (aComplaint.ComplaintAddress.AddressLine2 != "") strAdd += aComplaint.ComplaintAddress.AddressLine2 + "<br>";
            if (aComplaint.ComplaintAddress.City != null) strAdd += aComplaint.ComplaintAddress.City.Name + ", " + aComplaint.ComplaintAddress.State.Abbr
                + " " + aComplaint.ComplaintAddress.Zip + "<br>";
            strAdd += "</td>";

            List<ContactPrintStruct> contactList = new List<ContactPrintStruct>();
            contactList.Add(new ContactPrintStruct(aComplaint.Owner, "Owner", "Owner"));
            contactList.Add(new ContactPrintStruct(aComplaint.DemoContractor, "Demolition Contractor", "Demolition"));
            contactList.Add(new ContactPrintStruct(aComplaint.AbatementContractor, "Abatement Contractor", "Abatement"));
            contactList.Add(new ContactPrintStruct(aComplaint.EntityCoordinator, "Entity Coordinator", "Coordinator"));
            contactList.Add(new ContactPrintStruct(aComplaint.Landfill, "Landfill", "Landfill"));
            contactList.Add(new ContactPrintStruct(aComplaint.OtherOperator, "Other Operator", "Other Op"));

            FindMultiples(aComplaint.ComplaintAddress.ID);
            PrintGenerator(group, strAdd, contactList, null);
        }

        private void tabAdditionalComplaints_MouseDown(object sender, MouseButtonEventArgs e)
        { if (aComplaint.ComplaintAddress != null && aComplaint.ComplaintAddress.ID != 0) FindMultiples(aComplaint.ComplaintAddress.ID); }
    }
}
