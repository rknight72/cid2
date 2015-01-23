using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CID2
{
    /// <summary>
    /// Interaction logic for RecordManager.xaml
    /// </summary>
    public partial class RecordManager : Window
    {
        public List<DataGridColumn> ExpiryColumns { get; set; }
        public List<DataGridColumn> LogColumns { get; set; }

        public struct RetentionComplaint
        {
            public bool isChecked { get; set; }
            public GenericComplaint genericcomp { get; set; }
        }

        public struct DeletedLogEntry
        {
            public string ID { get; set; }
            public ComplaintType Type { get; set; }
            public DateTime DateReceived { get; set;}
            public DateTime DateExpired { get; set; }
            public DateTime DateDeleted { get; set; }
            public user DeletedBy { get; set; }
        }

        public RecordManager()
        {
            InitializeComponent();

            ExpiryColumns = new List<DataGridColumn>();
            DataGridCheckBoxColumn col0 = new DataGridCheckBoxColumn();
            col0.Width = 30;
            col0.Binding = new Binding("isChecked");
            col0.IsReadOnly = false;
            ExpiryColumns.Add(col0);
            DataGridTextColumn col1 = new DataGridTextColumn();
            col1.Header = "ID";
            col1.Width = 40;
            col1.Binding = new Binding("genericcomp.ID");
            col1.Binding.StringFormat = "C{0}";
            col1.IsReadOnly = true;
            ExpiryColumns.Add(col1);
            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Header = "Record Type";
            col2.Width = 210;
            col2.Binding = new Binding("genericcomp.Type.Label");
            col2.IsReadOnly = true;
            ExpiryColumns.Add(col2);
            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Header = "Fac ID/ Place ID";
            col3.Width = 95;
            col3.Binding = new Binding("genericcomp.PlaceID_FacID");
            col3.IsReadOnly = true;
            ExpiryColumns.Add(col3);
            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Header = "Address";
            col4.Width = 250;
            col4.Binding = new Binding("genericcomp.SiteAddress1");
            col4.SortMemberPath = "SortAddress";
            col4.IsReadOnly = true;
            ExpiryColumns.Add(col4);
            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Header = "Date Created";
            col5.Binding = new Binding("genericcomp.DateReceived");
            col5.Binding.StringFormat = "MM/dd/yyyy";
            col5.IsReadOnly = true;
            ExpiryColumns.Add(col5);
            DataGridTextColumn col6 = new DataGridTextColumn();
            col6.Header = "Date Closed";
            col6.Binding = new Binding("genericcomp.DateClosed");
            col6.Binding.StringFormat = "MM/dd/yyyy";
            col6.IsReadOnly = true;
            ExpiryColumns.Add(col6);
            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Header = "Expiry Date";
            col7.Binding = new Binding("genericcomp.RetentionDate");
            col7.Binding.StringFormat = "MM/dd/yyyy";
            col7.IsReadOnly = true;
            ExpiryColumns.Add(col7);

            LogColumns = new List<DataGridColumn>();
            DataGridTextColumn lcol0 = new DataGridTextColumn();
            lcol0.Header = "Record ID";
            lcol0.Width = 80;
            lcol0.Binding = new Binding("ID");
            lcol0.IsReadOnly = true;
            LogColumns.Add(lcol0);
            DataGridTextColumn lcol1 = new DataGridTextColumn();
            lcol1.Header = "Record Type";
            lcol1.Width = 290;
            lcol1.Binding = new Binding("Type.Label");
            lcol1.IsReadOnly = true;
            LogColumns.Add(lcol1);
            DataGridTextColumn lcol2 = new DataGridTextColumn();
            lcol2.Header = "Date Received";
            lcol2.Width = 110;
            lcol2.Binding = new Binding("DateReceived");
            lcol2.Binding.StringFormat = "MM/dd/yyyy";
            lcol2.IsReadOnly = true;
            LogColumns.Add(lcol2);
            DataGridTextColumn lcol3 = new DataGridTextColumn();
            lcol3.Header = "Date Expired";
            lcol3.Width = 110;
            lcol3.Binding = new Binding("DateExpired");
            lcol3.Binding.StringFormat = "MM/dd/yyyy";
            LogColumns.Add(lcol3);
            DataGridTextColumn lcol4 = new DataGridTextColumn();
            lcol4.Header = "Date Deleted";
            lcol4.Width = 110;
            lcol4.Binding = new Binding("DateDeleted");
            lcol4.Binding.StringFormat = "MM/dd/yyyy";
            LogColumns.Add(lcol4);
            DataGridTextColumn lcol5 = new DataGridTextColumn();
            lcol5.Header = "Deleted By";
            lcol5.Width = 150;
            lcol5.Binding = new Binding("DeletedBy");
            LogColumns.Add(lcol5);
        }

        private void cboGridPick_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gridList.Columns.Clear();

            if (cboGridPick.SelectedValue != null && (int)(cboGridPick.SelectedValue) == 2)
            {
                GetDeletedLog();
                foreach (DataGridColumn col in LogColumns)
                { gridList.Columns.Add(col); }
                btnDelete.Visibility = System.Windows.Visibility.Hidden;   
            }
            else
            {
                GetComplaints();
                foreach (DataGridColumn col in ExpiryColumns)
                { gridList.Columns.Add(col); }
                btnDelete.Visibility = System.Windows.Visibility.Visible;
            }            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.thisUser.Role > 3) btnDelete.IsEnabled = false;
            else btnDelete.IsEnabled = true;

            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(1, "Closed records");
            dict.Add(2, "Deleted record log");
            cboGridPick.ItemsSource = dict;
            cboGridPick.DisplayMemberPath = "Value";
            cboGridPick.SelectedValuePath = "Key";
            cboGridPick.SelectedValue = 1;
        }

        private void gridList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(gridList.CurrentCell.Column is DataGridCheckBoxColumn))
            {
                DataGridRow row = sender as DataGridRow;
                if (row.Item != null)
                {
                    MiniComplaint mini = new MiniComplaint(((RetentionComplaint)row.Item).genericcomp);
                    try { mini.ShowDialog(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); return; }
                }
                e.Handled = true;
            }
        }

        public void GetDeletedLog()
        {
            MainWindow.OpenMainDBConnection();

            string strSQL = "SELECT * FROM tbl_Deletion_Log";

            OleDbCommand cidSQL = new OleDbCommand(strSQL, MainWindow.cidDB);
            OleDbDataReader cidReader = cidSQL.ExecuteReader();

            List<DeletedLogEntry> list = new List<DeletedLogEntry>();
            while (cidReader.Read())
            {
                DeletedLogEntry entry = new DeletedLogEntry();

                entry.ID = cidReader.GetString(1);
                ComplaintType type;
                MainWindow.GetSingleItem<ComplaintType>(out type, cidReader.GetInt32(2), MainWindow.ComplaintTypes);
                entry.Type = type;
                entry.DateReceived = cidReader.GetDateTime(3);
                entry.DateExpired = cidReader.GetDateTime(4);
                entry.DateDeleted = cidReader.GetDateTime(5);
                user u;
                MainWindow.GetSingleItem<user>(out u, cidReader.GetInt32(6), MainWindow.Users);
                entry.DeletedBy = u;

                list.Add(entry);
            }

            ListCollectionView view = new ListCollectionView(list);
            view.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Ascending));
            gridList.ItemsSource = view;

            cidReader.Close();
            MainWindow.CloseMainDBConnection();
        }

        public void GetComplaints()
        {
            MainWindow.OpenMainDBConnection();

            string strSQL = "SELECT * FROM tbl_Complaints WHERE Status = 2;";

            OleDbCommand cidSQL = new OleDbCommand(strSQL, MainWindow.cidDB);
            OleDbDataReader cidReader = cidSQL.ExecuteReader();

            List<RetentionComplaint> list = new List<RetentionComplaint>();
            while (cidReader.Read())
            {
                GenericComplaint obj = MainWindow.Main.CreateGenericComplaint(cidReader);

                string tempSQL = "SELECT * FROM " + obj.Type.ComplaintTable + " WHERE ID = " + obj.SubID.ToString() + ";";
                OleDbCommand listSQL = new OleDbCommand(tempSQL, MainWindow.cidDB);
                OleDbDataReader listReader = listSQL.ExecuteReader();
                listReader.Read();
                obj.SiteID = listReader.GetInt32(1);
                int contact1 = (obj.Type.PrimaryContactTable != "null" && listReader[2] != DBNull.Value) ? listReader.GetInt32(2) : 0;
                int contact2 = obj.Type.SecondaryContactTable != "null" && (listReader[3] != DBNull.Value) ? listReader.GetInt32(3) : 0;
                int contact3 = (obj.Type.TertiaryContactTable != "null" && listReader[4] != DBNull.Value) ? listReader.GetInt32(4) : 0;
                int contact4 = (obj.Type.QuaternaryContactTable != "null" && listReader[5] != DBNull.Value) ? listReader.GetInt32(5) : 0;

                if (obj.Type.QuaternaryContactTable != "null" && listReader.FieldCount > 6) obj.ANTSID = (listReader[6] != DBNull.Value) ? listReader.GetString(6) : "";

                listReader.Close();

                obj.Contact1 = obj.Contact2 = obj.Contact3 = obj.Contact4 = "";
                if (contact1 > 0)
                {
                    tempSQL = "SELECT Last_Name, First_Name FROM " + obj.Type.PrimaryContactTable + " WHERE ID = " + contact1.ToString() + ";";
                    listSQL = new OleDbCommand(tempSQL, MainWindow.cidDB);
                    listReader = listSQL.ExecuteReader();
                    listReader.Read();
                    if (listReader[0] != DBNull.Value) obj.Contact1 = listReader.GetString(0);
                    if (listReader[1] != DBNull.Value) obj.Contact1 += (obj.Contact1 != "") ? ", " + listReader.GetString(1) : listReader.GetString(1);
                    listReader.Close();
                }

                if (contact2 > 0)
                {
                    tempSQL = "SELECT Last_Name, First_Name FROM " + obj.Type.SecondaryContactTable + " WHERE ID = " + contact2.ToString() + ";";
                    listSQL = new OleDbCommand(tempSQL, MainWindow.cidDB);
                    listReader = listSQL.ExecuteReader();
                    listReader.Read();
                    if (listReader[0] != DBNull.Value) obj.Contact2 = listReader.GetString(0);
                    if (listReader[1] != DBNull.Value) obj.Contact2 += (obj.Contact2 != "") ? ", " + listReader.GetString(1) : listReader.GetString(1);
                    listReader.Close();
                }

                if (contact3 > 0)
                {
                    tempSQL = "SELECT Last_Name, First_Name FROM " + obj.Type.TertiaryContactTable + " WHERE ID = " + contact3.ToString() + ";";
                    listSQL = new OleDbCommand(tempSQL, MainWindow.cidDB);
                    listReader = listSQL.ExecuteReader();
                    listReader.Read();
                    if (listReader[0] != DBNull.Value) obj.Contact3 = listReader.GetString(0);
                    if (listReader[1] != DBNull.Value) obj.Contact3 += (obj.Contact3 != "") ? ", " + listReader.GetString(1) : listReader.GetString(1);
                    listReader.Close();
                }

                if (contact4 > 0)
                {
                    tempSQL = "SELECT Last_Name, First_Name FROM " + obj.Type.QuaternaryContactTable + " WHERE ID = " + contact4.ToString() + ";";
                    listSQL = new OleDbCommand(tempSQL, MainWindow.cidDB);
                    listReader = listSQL.ExecuteReader();
                    listReader.Read();
                    if (listReader[0] != DBNull.Value) obj.Contact4 = listReader.GetString(0);
                    if (listReader[1] != DBNull.Value) obj.Contact4 += (obj.Contact4 != "") ? ", " + listReader.GetString(1) : listReader.GetString(1);
                    listReader.Close();
                }

                tempSQL = "SELECT Name, Address, City, Facility_ID, Parcel, Township FROM " + obj.Type.SiteTable + " WHERE ID = " + obj.SiteID.ToString() + ";";
                listSQL = new OleDbCommand(tempSQL, MainWindow.cidDB);
                listReader = listSQL.ExecuteReader();
                listReader.Read();
                obj.FacName = (listReader[0] != DBNull.Value) ? listReader.GetString(0) : "";
                obj.SiteAddress1 = (listReader[1] != DBNull.Value) ? listReader.GetString(1) : "";
                string a, b;
                MainWindow.SplitAdd(obj.SiteAddress1, out a, out b);
                obj.SortAddress = b;
                if (listReader[2] != DBNull.Value && listReader[2] is Int32)
                {
                    city acity = new city();
                    MainWindow.GetSingleItem<city>(out acity, listReader.GetInt32(2), MainWindow.Cities);
                    obj.SiteCity = acity;
                }
                else
                {
                    obj.SiteCity = null;
                    if (listReader[2] != DBNull.Value && listReader[2] is String)
                    {
                        string temp = listReader.GetString(2);
                        foreach (city x in MainWindow.Cities)
                        { if (String.Equals(temp, x.Name, StringComparison.CurrentCultureIgnoreCase)) obj.SiteCity = x; }
                    }
                }
                if (listReader[3] != DBNull.Value && listReader[3] is String) obj.PlaceID_FacID = listReader.GetString(3);
                else if (listReader[3] != DBNull.Value && listReader[3] is Int32) obj.PlaceID_FacID = listReader.GetInt32(3).ToString();
                else obj.PlaceID_FacID = "";
                obj.Parcel = (listReader[4] != DBNull.Value) ? listReader.GetString(4) : obj.Parcel = "";
                if (listReader[5] != DBNull.Value)
                {
                    township t = new township();
                    MainWindow.GetSingleItem<township>(out t, listReader.GetInt32(5), MainWindow.Townships);
                    obj.Township = t;
                }

                listReader.Close();

                RetentionComplaint comp = new RetentionComplaint();
                comp.isChecked = false;
                comp.genericcomp = obj;

                list.Add(comp);
            }

            ListCollectionView view = new ListCollectionView(list);
            view.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Ascending));
            gridList.ItemsSource = view;

            cidReader.Close();
            MainWindow.CloseMainDBConnection();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("These records will be permanently deleted. Are you sure that you want to proceed?", "Confirm Deletion", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int counta = 0;
                int countb = 0;
                List<GenericComplaint> list = new List<GenericComplaint>();
                foreach (RetentionComplaint comp in gridList.Items.SourceCollection)
                {
                    if (comp.isChecked && comp.genericcomp.RetentionDate <= DateTime.Now)
                    {
                        list.Add(comp.genericcomp);
                        counta++;
                    }
                    else if (comp.isChecked)
                    {
                        MessageBox.Show("Record C" + comp.genericcomp.ID.ToString() + " has an expiration date later than today and will not be deleted.");
                        countb++;
                    }
                }

                string msg = "";
                if (counta > 0)
                {
                    DeleteRecords(list);

                    msg = counta.ToString() + " were deleted.";
                }
                if (countb > 0) msg += "\n" + countb.ToString() + " were not deleted.";
                MessageBox.Show(msg);

                GetComplaints();
            }
        }

        private void DeleteRecords(List<GenericComplaint> list)
        {
            MainWindow.OpenMainDBConnection();

            foreach(GenericComplaint comp in list)
            {
                if (comp.CETA.ID != 2 && comp.CETA.ID != 3)
                {
                    string strSQL = "SELECT PrimaryContact, SecondaryContact FROM " + comp.Type.ComplaintTable + " WHERE ID = " + comp.SubID + ";";
                    OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                    OleDbDataReader cidReader = cidCMD.ExecuteReader();
                    cidReader.Read();

                    if (cidReader[0] != DBNull.Value)
                    {
                        string primSQL = "DELETE * FROM " + comp.Type.PrimaryContactTable + " WHERE ID = " + cidReader.GetInt32(0) + ";";
                        OleDbCommand primCMD = new OleDbCommand(primSQL, MainWindow.cidDB);
                        cidCMD.ExecuteNonQuery();
                    }
                    if (cidReader[1] != DBNull.Value)
                    {
                        string secondarySQL = "DELETE * FROM " + comp.Type.SecondaryContactTable + " WHERE ID + " + cidReader.GetInt32(1) + ";";
                        OleDbCommand secondaryCMD = new OleDbCommand(secondarySQL, MainWindow.cidDB);
                        secondaryCMD.ExecuteNonQuery();
                    }

                    cidReader.Close();
                }

                string subSQL = "DELETE * FROM " + comp.Type.ComplaintTable + " WHERE ID = " + comp.SubID + ";";
                OleDbCommand subCMD = new OleDbCommand(subSQL, MainWindow.cidDB);
                subCMD.ExecuteNonQuery();

                string mainSQL = "DELETE * FROM tbl_Complaints WHERE ID = " + comp.ID + ";";
                OleDbCommand mainCMD = new OleDbCommand(mainSQL, MainWindow.cidDB);
                mainCMD.ExecuteNonQuery();

                string logSQL = "INSERT INTO tbl_Deletion_Log (RecordID, RecordType, DateReceived, DateExpired, DateDeleted, DeletedBy) VALUES"
                    + " (@id, @type, @dtrcvd, @dtexp, @dtnow, @user);";
                OleDbCommand cmd = new OleDbCommand(logSQL, MainWindow.cidDB);
                cmd.Parameters.AddWithValue("@id", "C" + comp.ID.ToString());
                cmd.Parameters.AddWithValue("@type", comp.Type.ID);
                cmd.Parameters.Add("@dtrcvd", OleDbType.Date).Value = comp.DateReceived;
                cmd.Parameters.Add("@dtexp", OleDbType.Date).Value = comp.RetentionDate;
                cmd.Parameters.Add("@dtnow", OleDbType.Date).Value = DateTime.Now;
                cmd.Parameters.AddWithValue("@user", MainWindow.thisUser.ID);
                cmd.ExecuteNonQuery();
                MainWindow.CloseMainDBConnection();
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if ((int)cboGridPick.SelectedValue == 1) GetComplaints();
            else GetDeletedLog();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            CIDImport window = new CIDImport();
            window.ShowDialog();
        }
    }
}