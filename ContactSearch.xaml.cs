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
    /// Interaction logic for ContactSearch.xaml
    /// </summary>
    public partial class ContactSearch : Window
    {
        public OleDbConnection cidDB = MainWindow.cidDB;
        public static ListCollectionView AddressView { get; set; }
        public List<Contact> list { get; set; }
        public string Table { get; set; }
        public static Action<Contact> SetContact { get; set; }

        public ContactSearch(string table, Action<Contact> setcontactfunction)
        {
            InitializeComponent();
            InitializeForm();

            Table = table;
            SetContact = setcontactfunction;
        }

        public void InitializeForm()
        {
            cboState.ItemsSource = MainWindow.States;
            cboState.SelectedIndex = 0;

            EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyUpEvent, new KeyEventHandler(RefreshList), true);
            EventManager.RegisterClassHandler(typeof(Window), Mouse.MouseUpEvent, new MouseButtonEventHandler(RefreshList), true);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        { GetAddresses(); }

        public static void RefreshList(object sender, EventArgs e)
        { if (sender is ResidentialSearch) AddressView.Refresh(); }

        private void GetAddresses()
        {
            try { cidDB.Open(); }
            catch { MainWindow.TryAgain(cidDB, "Main"); }

            string strSQL = "SELECT * FROM " + Table;

            OleDbCommand cidSQL = new OleDbCommand(strSQL, cidDB);
            OleDbDataReader cidReader = cidSQL.ExecuteReader();

            list = new List<Contact>();
            while (cidReader.Read())
            {
                Contact obj = new Contact();
                obj.ID = cidReader.GetInt32(0);
                obj.LName = (cidReader[1] != DBNull.Value) ? cidReader.GetString(1) : "";
                obj.FName = (cidReader[2] != DBNull.Value) ? cidReader.GetString(2) : "";
                obj.AddressLine1 = (cidReader[3] != DBNull.Value) ? cidReader.GetString(3) : "";
                obj.AddressLine2 = (cidReader[4] != DBNull.Value) ? cidReader.GetString(4) : "";
                obj.City = (cidReader[5] != DBNull.Value) ? cidReader.GetString(5) : "";
                if (cidReader[6] != DBNull.Value)
                {
                    state s = new state();
                    MainWindow.States.Filter = null;
                    MainWindow.GetSingleItem<state>(out s, cidReader.GetInt32(6), MainWindow.States);
                    obj.State = s;
                }
                obj.Zip = (cidReader[7] != DBNull.Value) ? cidReader.GetString(7) : "";
                obj.Email = (cidReader[8] != DBNull.Value) ? cidReader.GetString(8) : "";
                obj.Phone = (cidReader[9] != DBNull.Value) ? cidReader.GetString(9) : "";

                list.Add(obj);
            }

            AddressView = new ListCollectionView(list);
            AddressView.SortDescriptions.Add(new SortDescription("FName", ListSortDirection.Ascending));

            cidReader.Close();
            cidDB.Close();

            AddressView.Filter = ListFilter;
            listAddresses.ItemsSource = AddressView;
        }

        private bool ListFilter(object item)
        {
            Contact comp = (Contact)item;
            bool match = true;

            if (txtFName.Text != "") match = (comp.FName.IndexOf(txtFName.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (txtAdd1.Text != "") match = (comp.AddressLine1.IndexOf(txtAdd1.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (match && txtAdd2.Text != "") match = (comp.AddressLine2.IndexOf(txtAdd2.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (match && txtCity.Text != "") match = (comp.City.IndexOf(txtCity.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (match && cboState.SelectedIndex > 0) match = (comp.State.ID == (int)cboState.SelectedValue);
            if (match && txtZip.Text != "") match = (comp.Zip.IndexOf(txtZip.Text, StringComparison.CurrentCultureIgnoreCase) != -1);

            return match;
        }

        private void listAddresses_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (row.Item != null)
            {
                SetContact((Contact)row.Item);
                Close();
            }

            e.Handled = true;
        }
    }
}
