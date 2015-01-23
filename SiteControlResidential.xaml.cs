using System;
using System.Windows;
using System.Windows.Controls;

namespace CID2
{
    /// <summary>
    /// Interaction logic for SiteNonPermFac.xaml
    /// </summary>
    public partial class SiteControlResidential : SiteControlBase
    {
        public new ResidentialLocation ControlOwner { get; set; }

        public SiteControlResidential()
        {
            InitializeComponent();
            InitializeControls();
        }

        public SiteControlResidential(ResidentialLocation owner)
        {
            ControlOwner = owner;
            thisAddress = (ComplaintAddress)ControlOwner;

            InitializeComponent();
            InitializeControls();
        }

        public void InitializeControls()
        {
            FillCombo(cboPropCity, MainWindow.Cities);
            FillCombo(cboPropState, MainWindow.States);
            FillCombo(cboZipBox, MainWindow.ZipCodeList);
            FillCombo(cboPropCounty, MainWindow.Counties);
            FillCombo(cboPropTownship, MainWindow.Townships);
        }

        public void btnPropertyList_Click(object sender, RoutedEventArgs e)
        {
            /*ResidentialSearch search = new ResidentialSearch(LoadNewAddress);
            try { search.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); search.Close(); return; }*/
        }

        private void CoordsChanged(object sender, RoutedEventArgs e)
        {
            if ((!txtPropLat.IsFocused) && (!txtPropLon.IsFocused) && MainWindow.IsNumeric(txtPropLat.Text) && MainWindow.IsNumeric(txtPropLon.Text))
                ControlOwner.Owner.Form.LoadMap(txtPropLat.Text, txtPropLon.Text, false);
        }

        private void cboPropCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            city x = (city)cboPropCity.SelectedItem;
            if (x.ID > 0)
            {
                if (x.ZipCodeList != null && x.ZipCodeList.Count > 0) cboZipBox.Items.Filter = (y) => x.ZipCodeList.IndexOf(((ZipCode)y).Zip) > -1 || ((ZipCode)y).ID == 0;
                else cboZipBox.Items.Filter = null;
                if (x.TownshipIDList != null && x.TownshipIDList.Count > 0) cboPropTownship.Items.Filter = (y) => x.TownshipIDList.IndexOf(((township)y).ID) > -1 || ((township)y).ID == 0;
                else cboPropTownship.Items.Filter = null;
                if (cboPropCounty.Text == "") cboPropCounty.SelectedValue = (object)x.DefaultCounty.ID;
                if (cboPropTownship.Text == "") cboPropTownship.SelectedValue = (object)x.DefaultTownship.ID;
            }
            else
            {
                cboZipBox.Items.Filter = null;
                cboPropTownship.Items.Filter = null;
            }
        }

        private void cboPropCounty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { if (IsLoaded && cboPropState.Text == "") cboPropState.SelectedValue = (object)(((county)cboPropCounty.SelectedItem).DefaultState.ID); }
    }
}
