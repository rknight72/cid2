using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace CID2
{
    /// <summary>
    /// Interaction logic for SiteNonPermFac.xaml
    /// </summary>
    public partial class SiteControlFacility : UserControl
    {
        public Complaint ControlOwner { get; set; }
        public Facility thisFacility { get; set; }
        public string FacilityNameOverride { get; set; }

        public SiteControlFacility()
        {
            InitializeComponent();
            InitializeControls();
        }

        public SiteControlFacility(Complaint owner, Facility facility)
        {
            ControlOwner = owner;
            thisFacility = facility;
            thisFacility.SetFacilityControl(this);

            InitializeComponent();
            InitializeControls();
        }

        public void InitializeControls()
        {
            FillCombo(cboFacStat_nolock, MainWindow.OperatingStatuses);
            FillCombo(cboFacClass_nolock, MainWindow.PermittingClassifications);
            FillCombo(cboFacilityID, MainWindow.Facilities);
            FillCombo(cboFacCounty, MainWindow.Counties);
            FillCombo(cboFacTownship, MainWindow.Townships);
        }

        public void btnFacilityList_Click(object sender, RoutedEventArgs e)
        {
            FacilitySearch search = new FacilitySearch(SetFacility);
            try { search.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); search.Close(); return; }
        }

        public void cboFacilityID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { if (IsLoaded && cboFacilityID.SelectedValue != null) SetFacility((int)cboFacilityID.SelectedValue); }

        private void CoordsChanged(object sender, RoutedEventArgs e)
        {
            if ((!txtFacLat.IsFocused) && (!txtFacLon.IsFocused) && MainWindow.IsNumeric(txtFacLat.Text) && MainWindow.IsNumeric(txtFacLon.Text))
                if (ControlOwner != null) ControlOwner.Form.LoadMap(txtFacLat.Text, txtFacLon.Text, false);
        }

        public void SetFacility(int id)
        {
            Facility fac = new Facility();
            MainWindow.GetSingleItem<Facility>(out fac, id, MainWindow.Facilities);
            thisFacility = fac;
            thisFacility.SetFacilityControl(this);
            if (ControlOwner != null) ControlOwner.SetContactFilter();
            if (ControlOwner != null) ControlOwner.SetComplaintSpecificAddressInfo(thisFacility.FacName);

            thisFacility.UpdateSiteControlContent();
        }

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

        public void FillCombo(ComboBox combo, ListCollectionView list)
        {
            combo.ItemsSource = list;
            combo.SelectedItem = null;
        }

        public void btnGetCoords_Click(object sender, RoutedEventArgs e)
        {
            string lat, lon;
            MainWindow.GetCoords(thisFacility.GetFacilityAddress(), out(lat), out(lon));
            thisFacility.Latitude = Convert.ToDouble(lat);
            thisFacility.Longitude = Convert.ToDouble(lon);

            thisFacility.UpdateSiteControlContent();
        }

        public void Button_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        { MainWindow.Button_IsEnabledChanged(sender, e); }

        private void cboFacCounty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { if (IsLoaded && cboFacState_nolock.Text == "") cboFacState_nolock.SelectedValue = (object)(((county)cboFacCounty.SelectedItem).DefaultState.ID); }

        private void chkPortable_nolock_Checked(object sender, RoutedEventArgs e)
        { if (ControlOwner != null) ControlOwner.Form.AddPortableTab(); }

        private void chkPortable_nolock_Unchecked(object sender, RoutedEventArgs e)
        { if (ControlOwner != null) ControlOwner.Form.RemovePortableTab(); }
    }
}
