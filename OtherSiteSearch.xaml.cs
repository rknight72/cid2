using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace CID2
{
    /// <summary>
    /// Interaction logic for NonPermFacSearch.xaml
    /// </summary>
    public partial class OtherSiteSearch : Window
    {
        public OleDbConnection cidDB = MainWindow.cidDB;
        public static ListCollectionView AddressView { get; set; }
        public List<OtherLocation> list { get; set; }
        public Window ParentWindow { get; set; }

        public OtherSiteSearch(Window parentwindow)
        {
            InitializeComponent();
            InitializeForm();

            ParentWindow = parentwindow;
        }

        public void InitializeForm()
        {
            List<city> cityList = new List<city>();
            foreach (city x in MainWindow.Cities) { cityList.Add(x); }
            cityList.Add(new city(0, "All Cities", null, null, null, null));
            ListCollectionView derp = new ListCollectionView(cityList);
            derp.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            cboCity.ItemsSource = derp;
            cboCity.SelectedIndex = 0;

            cboState.ItemsSource = MainWindow.States;
            cboState.SelectedIndex = 41;

            EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyUpEvent, new KeyEventHandler(RefreshList), true);
            EventManager.RegisterClassHandler(typeof(Window), Mouse.MouseUpEvent, new MouseButtonEventHandler(RefreshList), true);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        { GetAddresses(); }

        public static void RefreshList(object sender, EventArgs e)
        { if (sender is NonPermFacSearch) AddressView.Refresh(); }

        private void GetAddresses()
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            string strSQL = "SELECT * FROM tbl_Complaint_Other_Sites";

            OleDbCommand cidSQL = new OleDbCommand(strSQL, cidDB);
            OleDbDataReader cidReader = cidSQL.ExecuteReader();

            list = new List<OtherLocation>();
            while (cidReader.Read())
            {
                OtherLocation obj = new OtherLocation();
                obj.ID = cidReader.GetInt32(0);
                if (cidReader[1] != DBNull.Value) obj.LocationDescription = cidReader.GetString(1);
                if (cidReader[3] != DBNull.Value) obj.AddressLine1 = cidReader.GetString(3);
                if (cidReader[4] != DBNull.Value) obj.AddressLine2 = cidReader.GetString(4);
                if (cidReader[6] != DBNull.Value)
                {
                    city c = new city();
                    MainWindow.Cities.Filter = null;
                    MainWindow.GetSingleItem<city>(out c, cidReader.GetInt32(6), MainWindow.Cities);
                    obj.City = c;
                }
                if (cidReader[7] != DBNull.Value) obj.Zip = cidReader.GetString(7);
                if (cidReader[8] != DBNull.Value) obj.Latitude = cidReader.GetDouble(8);
                if (cidReader[9] != DBNull.Value)
                {
                    state s = new state();
                    MainWindow.States.Filter = null;
                    MainWindow.GetSingleItem<state>(out s, cidReader.GetInt32(9), MainWindow.States);
                    obj.State = s;
                }
                if (cidReader[10] != DBNull.Value) obj.Longitude = cidReader.GetDouble(10);
                if (cidReader[11] != DBNull.Value)
                {
                    county cc = new county();
                    MainWindow.Counties.Filter = null;
                    MainWindow.GetSingleItem<county>(out cc, cidReader.GetInt32(11), MainWindow.Counties);
                    obj.County = cc;
                }
                if (cidReader[12] != DBNull.Value)
                {
                    township t = new township();
                    MainWindow.Townships.Filter = null;
                    MainWindow.GetSingleItem<township>(out t, cidReader.GetInt32(12), MainWindow.Townships);
                    obj.Township = t;
                }

                list.Add(obj);
            }

            AddressView = new ListCollectionView(list);
            AddressView.SortDescriptions.Add(new SortDescription("ID", ListSortDirection.Ascending));

            cidReader.Close();
            cidDB.Close();

            AddressView.Filter = ListFilter;
            listAddresses.ItemsSource = AddressView;
        }

        private bool ListFilter(object item)
        {
            OtherLocation comp = (OtherLocation)item;
            bool match = true;

            if (txtName1.Text != "") match = (comp.LocationDescription.IndexOf(txtName1.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (txtAdd1.Text != "") match = (comp.AddressLine1.IndexOf(txtAdd1.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (match && txtAdd2.Text != "") match = (comp.AddressLine2.IndexOf(txtAdd2.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (match && cboCity.SelectedIndex > 0) match = (comp.City.ID == (int)cboCity.SelectedValue);
            if (match && cboState.SelectedIndex > 0) match = (comp.State.ID == (int)cboState.SelectedValue);
            if (match && txtZip.Text != "") match = (comp.Zip.IndexOf(txtZip.Text, StringComparison.CurrentCultureIgnoreCase) != -1);

            if (match && txtLat.Text != "" && txtLon.Text != "")
            {
                if (MainWindow.IsNumeric(txtLat.Text) && MainWindow.IsNumeric(txtLon.Text))
                {
                    double rad = 0.0001;
                    double lon = Convert.ToDouble(txtLon.Text);
                    double lat = Convert.ToDouble(txtLat.Text);
                    double result = (((comp.Longitude - lon) * (comp.Longitude - lon) / 1.3) + ((comp.Latitude - lat) * (comp.Latitude - lat)));
                    match = (result <= rad);
                }
            }

            return match;
        }

        private void listAddresses_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (row.Item != null)
            {
                if (ParentWindow is ComplaintWizard) ((ComplaintWizard)ParentWindow).SetOtherAddress(((OtherLocation)row.Item));
                else if (ParentWindow is NonPermittedFacComplaintForm) ((OtherComplaintForm)ParentWindow).SetComplaintAddress(((OtherLocation)row.Item).ID);
                Close();
            }

            e.Handled = true;
        }
    }
}
