using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CID2
{
    /// <summary>
    /// Interaction logic for FacilitySearch.xaml
    /// </summary>
    public partial class FacilitySearch : Window
    {
        public static ListCollectionView FacilitiesView { get; set; }
        public static Action<int> SetFacility { get; set; }


        public FacilitySearch(Action<int> setfacility)
        {
            InitializeComponent();

            SetFacility = setfacility;

            cboFacClass.ItemsSource = MainWindow.PermittingClassifications;
            cboFacClass.SelectedIndex = 0;
            cboFacStatus.ItemsSource = MainWindow.OperatingStatuses;
            cboFacStatus.SelectedIndex = 0;

            EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyUpEvent, new KeyEventHandler(RefreshList), true);
            EventManager.RegisterClassHandler(typeof(Window), Mouse.MouseUpEvent, new MouseButtonEventHandler(RefreshList), true);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Facility> list = new List<Facility>();
            foreach(Facility x in MainWindow.Facilities)
            { list.Add(x); }

            FacilitiesView = new ListCollectionView(list);
            FacilitiesView.SortDescriptions.Add(new SortDescription("FacName", ListSortDirection.Ascending));
            FacilitiesView.Filter = ListFilter;

            listFacilities.ItemsSource = FacilitiesView;
        }

        public static void RefreshList(object sender, EventArgs e)
        { if (sender is FacilitySearch) FacilitiesView.Refresh(); }

        private bool ListFilter(object item)
        {
            Facility fac = (Facility)item;
            bool match = true;

            if (txtFacName.Text != null && txtFacName.Text != "") match = (fac.FacName.IndexOf(txtFacName.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (match && txtFacID.Text != null && txtFacID.Text != "") match = (fac.FacilityID.IndexOf(txtFacID.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (match && txtFacStreet.Text != null && txtFacStreet.Text != "") match = (fac.AddressLine1.IndexOf(txtFacStreet.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (match && txtFacCity.Text != null && txtFacCity.Text != "") match = (fac.City.IndexOf(txtFacCity.Text, StringComparison.CurrentCultureIgnoreCase) != -1);
            if (match && cboFacClass.SelectedIndex > 0) match = (fac.PermitClassification != null && fac.PermitClassification.ID == (int)cboFacClass.SelectedValue);
            if (match && cboFacStatus.SelectedIndex > 0) match = (fac.OpStatus != null && fac.OpStatus.ID == (int)cboFacStatus.SelectedValue);

            return match;
        }

        private void listFacilities_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (row.Item != null)
            {
                SetFacility(((Facility)row.Item).ID);

                Close();
            }

            e.Handled = true;
        }
    }
}
