using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CID2
{
    public static class ComplaintDetails
    {
        public static Grid gridCurrent { get; set; }
        public static int intComplaintType { get; set; }
        public static Grid gridLocation { get; set; }
        public static int intLocationType { get; set; }
        public static Grid gridContact { get; set; }
        public static Grid gridOwner { get; set; }
        public static string strTypeLabel { get; set; }
    }

    /// <summary>
    /// Interaction logic for ComplaintWizard.xaml
    /// </summary>
    public partial class ComplaintWizard : Window
    {
        public ResidentialLocation residentLoc { get; set; }
        public string BuildingDesc { get; set; }
        public NonPermittedFacLocation nonpermfacloc { get; set; }
        public OtherLocation otherloc { get; set; }
        public Facility facilityloc { get; set; }
        public string FacName { get; set; }

        public bool AppendixA { get; set; }
        public bool Restricted { get; set; }        

        public ComplaintWizard()
        {
            InitializeComponent();
            ComplaintDetails.intComplaintType = 0;
            ComplaintDetails.intLocationType = 0;
            ComplaintDetails.gridCurrent = grid01;
            SwitchGridButtons(true, false);

            FillZipsCombo();
            FillTownshipsCombo();
            FillCityCombos();
            FillCountyCombos();
            FillFireDeptCombo();
            FillStatesCombo();

            cboFacID.ItemsSource = MainWindow.Facilities;
            cboFacID.SelectedItem = null;
            cboFacClass.ItemsSource = MainWindow.PermittingClassifications;
            cboFacClass.SelectedItem = null;

            residentLoc = new ResidentialLocation();
            BuildingDesc = "";
            nonpermfacloc = new NonPermittedFacLocation();
            otherloc = new OtherLocation();
            facilityloc = new Facility();
            FacName = "";

            AppendixA = false;
            Restricted = false;
        }
        
        private void FillZipsCombo()
        {
            cboPropZip.ItemsSource = MainWindow.ZipCodeList;
            cboPropZip.SelectedItem = null;
            cboBusZip.ItemsSource = MainWindow.ZipCodeList;
            cboBusZip.SelectedItem = null;
            cboOtherZip.ItemsSource = MainWindow.ZipCodeList;
            cboOtherZip.SelectedItem = null;
        }

        private void FillTownshipsCombo()
        {
            cboPropTownship.ItemsSource = MainWindow.Townships;
            cboPropTownship.SelectedItem = null;
            cboBusTownship.ItemsSource = MainWindow.Townships;
            cboBusTownship.SelectedItem = null;
            cboFacTownship.ItemsSource = MainWindow.Townships;
            cboFacTownship.SelectedItem = null;
            cboOtherTownship.ItemsSource = MainWindow.Townships;
            cboOtherTownship.SelectedItem = null;
        }

        private void FillStatesCombo()
        {
            cboBusState.ItemsSource = MainWindow.States;
            cboBusState.SelectedItem = null;
            cboCompState.ItemsSource = MainWindow.States;
            cboCompState.SelectedItem = null;
            cboFacState.ItemsSource = MainWindow.States;
            cboFacState.SelectedItem = null;
            cboPropState.ItemsSource = MainWindow.States;
            cboPropState.SelectedItem = null;
            cboOtherState.ItemsSource = MainWindow.States;
            cboOtherState.SelectedItem = null;
        }

        private void FillFireDeptCombo()
        {
            cboCompFD.ItemsSource = MainWindow.FireDepartments;
            cboCompFD.SelectedItem = null;
        }

        private void FillCountyCombos()
        {
            cboBusCounty.ItemsSource = MainWindow.Counties;
            cboBusCounty.SelectedItem = null;
            cboFacCounty.ItemsSource = MainWindow.Counties;
            cboFacCounty.SelectedItem = null;
            cboPropCounty.ItemsSource = MainWindow.Counties;
            cboPropCounty.SelectedItem = null;
            cboOtherCounty.ItemsSource = MainWindow.Counties;
            cboOtherCounty.SelectedItem = null;
        }

        private void FillCityCombos()
        {
            cboPropCity.ItemsSource = MainWindow.Cities;
            cboPropCity.SelectedItem = null;
            cboBusCity.ItemsSource = MainWindow.Cities;
            cboBusCity.SelectedItem = null;
            cboOtherCity.ItemsSource = MainWindow.Cities;
            cboOtherCity.SelectedItem = null;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        { GetNextGrid(); }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        { GetPrevGrid(); }

        private void SwitchGridButtons(bool bNext, bool bPrev)
        {
            switch(bNext)
            {
                case true:
                    btnNext.Visibility = System.Windows.Visibility.Visible;
                    break;
                case false:
                    btnNext.Visibility = System.Windows.Visibility.Hidden;
                    break;
                default:
                    btnNext.Visibility = System.Windows.Visibility.Visible;
                    break;
            }

            switch (bPrev)
            {
                case true:
                    btnBack.Visibility = System.Windows.Visibility.Visible;
                    break;
                case false:
                    btnBack.Visibility = System.Windows.Visibility.Hidden;
                    break;
                default:
                    btnBack.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }

        private void GetNextGrid()
        {
            switch (ComplaintDetails.gridCurrent.Name)
            {
                case "grid01":
                    if (ComplaintDetails.intComplaintType != 0)
                    {
                        if (ComplaintDetails.intLocationType != 0)
                        {
                            ComplaintDetails.intLocationType = 0;
                            btnLoc1.IsChecked = false;
                            btnLoc2.IsChecked = false;
                            btnLoc3.IsChecked = false;
                            btnLoc4.IsChecked = false;
                        }
                        grid01.Visibility = System.Windows.Visibility.Hidden;
                        grid02.Visibility = System.Windows.Visibility.Visible;
                        ComplaintDetails.gridCurrent = grid02;
                        SwitchGridButtons(true, true);
                    }
                    break;
                case "grid02":
                    if (ComplaintDetails.intLocationType != 0)
                    {
                        grid02.Visibility = System.Windows.Visibility.Hidden;
                        switch (ComplaintDetails.intLocationType)
                        {
                            case 1:
                                grid03a.Visibility = System.Windows.Visibility.Visible;
                                ComplaintDetails.gridCurrent = grid03a;
                                break;
                            case 2:
                                grid03b.Visibility = System.Windows.Visibility.Visible;
                                ComplaintDetails.gridCurrent = grid03b;
                                break;
                            case 3:
                                grid03c.Visibility = System.Windows.Visibility.Visible;
                                ComplaintDetails.gridCurrent = grid03c;
                                break;
                            case 4:
                                grid03d.Visibility = System.Windows.Visibility.Visible;
                                ComplaintDetails.gridCurrent = grid03d;
                                break;
                            default:
                                break;
                        }
                        SwitchGridButtons(true, true);
                        this.Width = 685;
                        this.Height = 370;
                    }
                    break;
                case "grid03a":
                    if ((txtBusDesc.Text != "") && (txtBusAddLine1.Text != "") && ((cboBusZip.SelectedItem != null) && ((ZipCode)cboBusZip.SelectedItem).ID > 0))
                    {
                        grid03a.Visibility = System.Windows.Visibility.Hidden;
                        grid04.Visibility = System.Windows.Visibility.Visible;
                        ComplaintDetails.gridCurrent = grid04;
                        SwitchGridButtons(true, true);
                    }
                    break;
                case "grid03b":
                    if ((cboFacID.SelectedItem != null ) && (((Facility)cboFacID.SelectedItem).ID > 0))
                    {
                        grid03b.Visibility = System.Windows.Visibility.Hidden;
                        grid04.Visibility = System.Windows.Visibility.Visible;
                        ComplaintDetails.gridCurrent = grid04;
                        SwitchGridButtons(true, true);
                    }
                    break;
                case "grid03c":
                    if ((txtPropAddLine1.Text != "") && (((city)cboPropCity.SelectedItem).ID > 0))
                    {
                        grid03c.Visibility = System.Windows.Visibility.Hidden;
                        grid04.Visibility = System.Windows.Visibility.Visible;
                        ComplaintDetails.gridCurrent = grid04;
                        SwitchGridButtons(true, true);
                    }
                    break;
                case "grid03d":
                    if ((cboOtherCity.SelectedItem != null) && (((city)cboOtherCity.SelectedItem).ID > 0))
                    {
                        grid03d.Visibility = System.Windows.Visibility.Hidden;
                        grid04.Visibility = System.Windows.Visibility.Visible;
                        ComplaintDetails.gridCurrent = grid04;
                        SwitchGridButtons(true, true);
                    }
                    break;
                case "grid04":
                    if (txtCompDetails.Text != "")
                    {
                        grid04.Visibility = System.Windows.Visibility.Hidden;
                        grid05.Visibility = System.Windows.Visibility.Visible;
                        if (ComplaintDetails.intComplaintType == 5)
                        {
                            chkCompFD.IsEnabled = true;
                            chkCompFD.Visibility = System.Windows.Visibility.Visible;
                            txtCompFDlabel.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            chkCompFD.IsEnabled = false;
                            chkCompFD.Visibility = System.Windows.Visibility.Hidden;
                            txtCompFDlabel.Visibility = System.Windows.Visibility.Hidden;
                        }
                        ComplaintDetails.gridCurrent = grid05;
                        btnNext.Content = "Finish";
                    }
                    break;
                case "grid05":
                    if ((txtCompFName.Text != "" ) && (txtCompLName.Text != "") || (chkCompAnon.IsChecked == true) || (chkCompFD.IsChecked == true && cboCompFD.SelectedIndex > 0))
                    {
                        double xlon, xlat;
                        xlon = xlat = 0;
                        string templbl = "";
                        Complainant complainant = new Complainant(0, txtCompFName.Text, txtCompLName.Text, txtCompAddLine1.Text, txtCompAddLine2.Text,
                            txtCompCity.Text, (state)cboCompState.SelectedItem, txtCompZip.Text, txtCompEmail.Text, txtCompPhone.Text);
                        city loccity = new city();
                        state locstate = new state();
                        ZipCode loczip = new ZipCode();
                        county loccounty = new county();
                        township loctownship = new township();

                        grid05.Visibility = System.Windows.Visibility.Hidden;
                        switch (ComplaintDetails.intLocationType)
                        {
                            case 1:
                                templbl = "Non-permitted Facility";
                                if (cboBusCity.SelectedItem != null) loccity = (city)cboBusCity.SelectedItem;
                                if (cboCompState.SelectedItem != null) locstate = (state)cboCompState.SelectedItem;
                                if (cboBusZip.SelectedItem != null) loczip = (ZipCode)cboBusZip.SelectedItem;
                                if (cboBusCounty.SelectedItem != null) loccounty = (county)cboBusCounty.SelectedItem;
                                if (cboBusTownship.SelectedItem != null) loctownship = (township)cboBusTownship.SelectedItem;
                                if (txtBusLon.Text != "" && txtBusLon.Text != "")
                                {
                                    xlon = Convert.ToDouble(txtBusLon.Text);
                                    xlat = Convert.ToDouble(txtBusLat.Text);
                                }
                                nonpermfacloc = new NonPermittedFacLocation(nonpermfacloc.ID, nonpermfacloc.PlaceID, txtBusAddLine1.Text,
                                    txtBusAddLine2.Text, loccity, locstate, loczip.Zip, loccounty, loctownship, txtBusParcel.Text, xlon, xlat);
                                FacName = txtBusDesc.Text;
                                break;
                            case 2:
                                templbl = "Permitted Facility";
                                break;
                            case 3:
                                templbl = "Residential";
                                if (cboPropCity.SelectedItem != null) loccity = (city)cboPropCity.SelectedItem;
                                if (cboPropState.SelectedItem != null) locstate = (state)cboPropState.SelectedItem;
                                if (cboPropZip.SelectedItem != null) loczip = (ZipCode)cboPropZip.SelectedItem;
                                if (cboPropCounty.SelectedItem != null) loccounty = (county)cboPropCounty.SelectedItem;
                                if (cboPropTownship.SelectedItem != null) loctownship = (township)cboPropTownship.SelectedItem;
                                residentLoc = new ResidentialLocation(residentLoc.ID, residentLoc.PlaceID, txtPropAddLine1.Text, txtPropAddLine2.Text,
                                    loccity, locstate, loczip.Zip, loccounty, loctownship, txtPropParcel.Text, xlon, xlat);
                                BuildingDesc = txtPropDesc.Text;
                                break;
                            case 4:
                                templbl = "Source Unknown";
                                if (cboOtherCity.SelectedItem != null) loccity = (city)cboOtherCity.SelectedItem;
                                if (cboOtherState.SelectedItem != null) locstate = (state)cboOtherState.SelectedItem;
                                if (cboOtherZip.SelectedItem != null) loczip = (ZipCode)cboOtherZip.SelectedItem;
                                if (cboOtherCounty.SelectedItem != null) loccounty = (county)cboOtherCounty.SelectedItem;
                                if (cboOtherTownship.SelectedItem != null) loctownship = (township)cboOtherTownship.SelectedItem;
                                otherloc = new OtherLocation(otherloc.ID, txtOtherDesc.Text, txtOtherAddLine1.Text, txtOtherAddLine2.Text,
                                    loccity, locstate, loczip.Zip, loccounty, loctownship, xlon, xlat);
                                break;
                            default:
                                MessageBox.Show("this shouldn't ever appear");
                                break;
                        }
                        switch (ComplaintDetails.intComplaintType)
                        {
                            case 1:
                                ComplaintDetails.strTypeLabel = "Air Release - " + templbl;
                                switch (ComplaintDetails.intLocationType)
                                {
                                    case 1:
                                        ReleaseNonPermFacComplaint releasenonpermfaccomp = new ReleaseNonPermFacComplaint(FacName, nonpermfacloc,
                                            complainant, txtCompDetails.Text, (bool)chkCompAnon.IsChecked);
                                        releasenonpermfaccomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        ReleaseNonPermFacCompForm compwindow1 = new ReleaseNonPermFacCompForm(releasenonpermfaccomp, true);
                                        this.Close();
                                        try { compwindow1.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow1.Close(); return; }
                                        break;
                                    case 2:
                                        ReleaseFacilityComplaint releasefaccomp = new ReleaseFacilityComplaint((Facility)cboFacID.SelectedItem, complainant, txtCompDetails.Text,
                                            (bool)chkCompAnon.IsChecked);
                                        releasefaccomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        ReleaseFacCompForm compwindow2 = new ReleaseFacCompForm(releasefaccomp, true);
                                        this.Close();
                                        try { compwindow2.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow2.Close(); return; }
                                        break;
                                    case 3:
                                        ReleaseResidentialComplaint releaserescomp = new ReleaseResidentialComplaint(BuildingDesc, residentLoc, complainant,
                                            txtCompDetails.Text, (bool)chkCompAnon.IsChecked);
                                        releaserescomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        ReleaseResCompForm compwindow3 = new ReleaseResCompForm(releaserescomp, true);
                                        this.Close();
                                        try { compwindow3.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow3.Close(); return; }
                                        break;
                                    case 4:
                                        ReleaseOtherComplaint releaseothercomp = new ReleaseOtherComplaint(otherloc, complainant, txtCompDetails.Text,
                                            (bool)chkCompAnon.IsChecked);
                                        releaseothercomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        ReleaseOtherCompForm compwindow4 = new ReleaseOtherCompForm(releaseothercomp, true);
                                        this.Close();
                                        try { compwindow4.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow4.Close(); return; }
                                        break;
                                    default:
                                        MessageBox.Show("this shouldn't ever appear");
                                        break;
                                }
                                break;
                            case 2:
                                ComplaintDetails.strTypeLabel = "Asbestos - " + templbl;
                                switch (ComplaintDetails.intLocationType)
                                {
                                    case 1:
                                        AsbestosNonPermFacComplaint asbestosnonpermfaccomp = new AsbestosNonPermFacComplaint(FacName, nonpermfacloc,
                                            complainant, txtCompDetails.Text, (bool)chkCompAnon.IsChecked);
                                        AsbestosNonPermFacForm compwindow1 = new AsbestosNonPermFacForm(asbestosnonpermfaccomp, true);
                                        asbestosnonpermfaccomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        this.Close();
                                        try { compwindow1.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow1.Close(); return; }
                                        break;
                                    case 2:
                                        AsbestosFacilityComplaint asbestosfaccomp = new AsbestosFacilityComplaint((Facility)cboFacID.SelectedItem, complainant, txtCompDetails.Text,
                                            (bool)chkCompAnon.IsChecked);
                                        asbestosfaccomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        AsbestosFacilityCompForm compwindow2 = new AsbestosFacilityCompForm(asbestosfaccomp, true);
                                        this.Close();
                                        try { compwindow2.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow2.Close(); return; }
                                        break;
                                    case 3:
                                        AsbestosResidentialComplaint asbestosrescomp = new AsbestosResidentialComplaint(BuildingDesc, residentLoc,
                                            complainant, txtCompDetails.Text, (bool)chkCompAnon.IsChecked);
                                        asbestosrescomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        AsbestosResCompForm compwindow3 = new AsbestosResCompForm(asbestosrescomp, true);
                                        this.Close();
                                        try { compwindow3.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow3.Close(); return; }
                                        break;
                                    case 4:
                                        AsbestosOtherComplaint asbestosothercomp = new AsbestosOtherComplaint(otherloc, complainant, txtCompDetails.Text,
                                            (bool)chkCompAnon.IsChecked);
                                        asbestosothercomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        AsbestosOtherCompForm compwindow4 = new AsbestosOtherCompForm(asbestosothercomp, true);
                                        this.Close();
                                        try { compwindow4.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow4.Close(); return; }
                                        break;
                                    default:
                                        MessageBox.Show("this shouldn't ever appear");
                                        break;
                                }
                                break;
                            case 3:
                                ComplaintDetails.strTypeLabel = "Fugitive Dust - " + templbl;
                                switch (ComplaintDetails.intLocationType)
                                {
                                    case 1:
                                        DustNonPermFacComplaint dustnonpermfaccomp = new DustNonPermFacComplaint(FacName, nonpermfacloc,
                                            new Complainant(complainant), txtCompDetails.Text, (bool)chkCompAnon.IsChecked);
                                        dustnonpermfaccomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        DustNonPermFacCompForm compwindow1 = new DustNonPermFacCompForm(dustnonpermfaccomp, true);
                                        this.Close();
                                        try { compwindow1.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow1.Close(); return; }
                                        break;
                                    case 2:
                                        DustFacilityComplaint dustfaccomp = new DustFacilityComplaint((Facility)cboFacID.SelectedItem, complainant, txtCompDetails.Text,
                                            (bool)chkCompAnon.IsChecked);
                                        dustfaccomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        DustFacilityCompForm compwindow2 = new DustFacilityCompForm(dustfaccomp, true);
                                        this.Close();
                                        try { compwindow2.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow2.Close(); return; }
                                        break;
                                    case 3:
                                        DustResidentialComplaint dustrescomp = new DustResidentialComplaint(BuildingDesc, residentLoc,
                                            complainant, txtCompDetails.Text, (bool)chkCompAnon.IsChecked);
                                        dustrescomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        DustResCompForm compwindow3 = new DustResCompForm(dustrescomp, true);
                                        this.Close();
                                        try { compwindow3.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow3.Close(); return; }
                                        break;
                                    case 4:
                                        DustOtherComplaint dustothercomp = new DustOtherComplaint(otherloc, complainant, txtCompDetails.Text,
                                            (bool)chkCompAnon.IsChecked);
                                        dustothercomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        DustOtherCompForm compwindow4 = new DustOtherCompForm(dustothercomp, true);
                                        this.Close();
                                        try { compwindow4.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow4.Close(); return; }
                                        break;
                                    default:
                                        MessageBox.Show("this shouldn't ever appear");
                                        break;
                                }
                                break;
                            case 4:
                                ComplaintDetails.strTypeLabel = "Odor - " + templbl;
                                switch (ComplaintDetails.intLocationType)
                                {
                                    case 1:
                                        OdorNonPermFacComplaint odornonpermfaccomp = new OdorNonPermFacComplaint(FacName, nonpermfacloc,
                                            complainant, txtCompDetails.Text, (bool)chkCompAnon.IsChecked);
                                        odornonpermfaccomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        OdorNonPermFacCompForm compwindow1 = new OdorNonPermFacCompForm(odornonpermfaccomp, true);
                                        this.Close();
                                        try { compwindow1.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow1.Close(); return; }
                                        break;
                                    case 2:
                                        OdorFacilityComplaint odorfaccomp = new OdorFacilityComplaint((Facility)cboFacID.SelectedItem, complainant, txtCompDetails.Text,
                                            (bool)chkCompAnon.IsChecked);
                                        odorfaccomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        OdorFacCompForm compwindow2 = new OdorFacCompForm(odorfaccomp, true);
                                        this.Close();
                                        try { compwindow2.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow2.Close(); return; }
                                        break;
                                    case 3:
                                        OdorResidentialComplaint odorrescomp = new OdorResidentialComplaint(BuildingDesc, residentLoc,
                                            complainant, txtCompDetails.Text, (bool)chkCompAnon.IsChecked);
                                        odorrescomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        OdorResCompForm compwindow3 = new OdorResCompForm(odorrescomp, true);
                                        this.Close();
                                        try { compwindow3.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow3.Close(); return; }
                                        break;
                                    case 4:
                                        OdorOtherComplaint odorothercomp = new OdorOtherComplaint(otherloc, complainant, txtCompDetails.Text,
                                            (bool)chkCompAnon.IsChecked);
                                        odorothercomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        OdorOtherCompForm compwindow4 = new OdorOtherCompForm(odorothercomp, true);
                                        this.Close();
                                        try { compwindow4.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow4.Close(); return; }
                                        break;
                                    default:
                                        MessageBox.Show("this shouldn't ever appear");
                                        break;
                                }
                                break;
                            case 5:
                                ComplaintDetails.strTypeLabel = "Open Burn - " + templbl;
                                switch (ComplaintDetails.intLocationType)
                                {
                                    case 1:
                                        OBNonPermFacComplaint obnonpermfaccomp = new OBNonPermFacComplaint(FacName, nonpermfacloc,
                                            complainant, txtCompDetails.Text, (bool)chkCompAnon.IsChecked, (bool)chkCompFD.IsChecked);
                                        if ((bool)chkCompFD.IsChecked) obnonpermfaccomp.FDinfo = (FireDepartment)cboCompFD.SelectedItem;
                                        obnonpermfaccomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        OBNonPermFacForm compwindow1 = new OBNonPermFacForm(obnonpermfaccomp, true);
                                        this.Close();
                                        try { compwindow1.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow1.Close(); return; }
                                        break;
                                    case 2:
                                        OBFacilityComplaint obfaccomp = new OBFacilityComplaint((Facility)cboFacID.SelectedItem, complainant, txtCompDetails.Text,
                                            (bool)chkCompAnon.IsChecked, (bool)chkCompFD.IsChecked);
                                        if ((bool)chkCompFD.IsChecked) obfaccomp.FDinfo = (FireDepartment)cboCompFD.SelectedItem;
                                        obfaccomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        OBFacCompForm compwindow2 = new OBFacCompForm(obfaccomp, true);
                                        this.Close();
                                        try { compwindow2.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow2.Close(); return; }
                                        break;
                                    case 3:
                                        OBResidentialComplaint obrescomp = new OBResidentialComplaint(BuildingDesc, residentLoc,
                                            complainant, txtCompDetails.Text, (bool)chkCompAnon.IsChecked, (bool)chkCompFD.IsChecked);
                                        if ((bool)chkCompFD.IsChecked) obrescomp.FDinfo = (FireDepartment)cboCompFD.SelectedItem;
                                        obrescomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        OBResCompForm compwindow3 = new OBResCompForm(obrescomp, true);
                                        this.Close();
                                        try { compwindow3.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow3.Close(); return; }
                                        break;
                                    case 4:
                                        OBOtherComplaint obothercomp = new OBOtherComplaint(otherloc, complainant, txtCompDetails.Text,
                                            (bool)chkCompAnon.IsChecked, (bool)chkCompFD.IsChecked);
                                        if ((bool)chkCompFD.IsChecked) obothercomp.FDinfo = (FireDepartment)cboCompFD.SelectedItem;
                                        obothercomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        OBOtherForm compwindow4 = new OBOtherForm(obothercomp, true);
                                        this.Close();
                                        try { compwindow4.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow4.Close(); return; }
                                        break;
                                    default:
                                        MessageBox.Show("this shouldn't ever appear");
                                        break;
                                }
                                break;
                            case 6:
                                ComplaintDetails.strTypeLabel = "Other - " + templbl;
                                switch (ComplaintDetails.intLocationType)
                                {
                                    case 1:
                                        OtherNonPermFacComplaint othernonpermfaccomp = new OtherNonPermFacComplaint(FacName, nonpermfacloc,
                                            complainant, txtCompDetails.Text, (bool)chkCompAnon.IsChecked);
                                        othernonpermfaccomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        OtherNonPermFacCompForm compwindow1 = new OtherNonPermFacCompForm(othernonpermfaccomp, true);
                                        this.Close();
                                        try { compwindow1.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow1.Close(); return; }
                                        break;
                                    case 2:
                                        OtherFacilityComplaint otherfaccomp = new OtherFacilityComplaint((Facility)cboFacID.SelectedItem, complainant, txtCompDetails.Text,
                                            (bool)chkCompAnon.IsChecked);
                                        otherfaccomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        OtherFacCompForm compwindow2 = new OtherFacCompForm(otherfaccomp, true);
                                        this.Close();
                                        try { compwindow2.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow2.Close(); return; }
                                        break;
                                    case 3:
                                        OtherResidentialComplaint otherrescomp = new OtherResidentialComplaint(BuildingDesc, residentLoc,
                                            complainant, txtCompDetails.Text, (bool)chkCompAnon.IsChecked);
                                        otherrescomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        OtherResCompForm compwindow3 = new OtherResCompForm(otherrescomp, true);
                                        this.Close();
                                        try { compwindow3.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow3.Close(); return; }
                                        break;
                                    case 4:
                                        OtherOtherComplaint obothercomp = new OtherOtherComplaint(otherloc, complainant, txtCompDetails.Text,
                                            (bool)chkCompAnon.IsChecked);
                                        obothercomp.SetCompType(ComplaintDetails.strTypeLabel);
                                        OtherOtherCompForm compwindow4 = new OtherOtherCompForm(obothercomp, true);
                                        this.Close();
                                        try { compwindow4.Show(); }
                                        catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); compwindow4.Close(); return; }
                                        break;
                                    default:
                                        MessageBox.Show("this shouldn't ever appear");
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                    default:
                        break;
                }
        }

        private void GetPrevGrid()
        {
            switch (ComplaintDetails.gridCurrent.Name)
            {
                case "grid02":
                    grid02.Visibility = System.Windows.Visibility.Hidden;
                    grid01.Visibility = System.Windows.Visibility.Visible;
                    ComplaintDetails.gridCurrent = grid01;
                    SwitchGridButtons(true, false);
                    break;
                case "grid03a":
                case "grid03b":
                case "grid03c":
                case "grid03d":
                    grid03a.Visibility = System.Windows.Visibility.Hidden;
                    grid03b.Visibility = System.Windows.Visibility.Hidden;
                    grid03c.Visibility = System.Windows.Visibility.Hidden;
                    grid03d.Visibility = System.Windows.Visibility.Hidden;
                    grid02.Visibility = System.Windows.Visibility.Visible;
                    ComplaintDetails.gridCurrent = grid02;
                    SwitchGridButtons(true, true);
                    this.Width = 400;
                    this.Height = 300;
                    break;
                case "grid04":
                    grid04.Visibility = System.Windows.Visibility.Hidden;
                    switch (ComplaintDetails.intLocationType)
                    {
                        case 1:
                            grid03a.Visibility = System.Windows.Visibility.Visible;
                            ComplaintDetails.gridCurrent = grid03a;
                            break;
                        case 2:
                            grid03b.Visibility = System.Windows.Visibility.Visible;
                            ComplaintDetails.gridCurrent = grid03b;
                            break;
                        default:
                            grid03c.Visibility = System.Windows.Visibility.Visible;
                            ComplaintDetails.gridCurrent = grid03c;
                            break;
                    }
                    SwitchGridButtons(true, true);
                    break;
                case "grid05":
                    grid05.Visibility = System.Windows.Visibility.Hidden;
                    grid04.Visibility = System.Windows.Visibility.Visible;
                    ComplaintDetails.gridCurrent = grid04;
                    btnNext.Content = "Next⇛";
                    SwitchGridButtons(true, true);
                    break;
                default:
                    break;
            }
        }

        private void ComplaintButton1_Click(object sender, RoutedEventArgs e)
        { ComplaintDetails.intComplaintType = 1; }

        private void ComplaintButton2_Click(object sender, RoutedEventArgs e)
        { ComplaintDetails.intComplaintType = 2; }

        private void ComplaintButton3_Click(object sender, RoutedEventArgs e)
        { ComplaintDetails.intComplaintType = 3; }

        private void ComplaintButton4_Click(object sender, RoutedEventArgs e)
        { ComplaintDetails.intComplaintType = 4; }

        private void ComplaintButton5_Click(object sender, RoutedEventArgs e)
        { ComplaintDetails.intComplaintType = 5; }

        private void ComplaintButton6_Click(object sender, RoutedEventArgs e)
        { ComplaintDetails.intComplaintType = 6; }

        private void LocationButton1_Click(object sender, RoutedEventArgs e)
        { ComplaintDetails.intLocationType = 1; }

        private void LocationButton2_Click(object sender, RoutedEventArgs e)
        { ComplaintDetails.intLocationType = 2; }

        private void LocationButton3_Click(object sender, RoutedEventArgs e)
        { ComplaintDetails.intLocationType = 3; }

        private void LocationButton4_Click(object sender, RoutedEventArgs e)
        { ComplaintDetails.intLocationType = 4; }

        private void chkCompAnon_Checked(object sender, RoutedEventArgs e)
        {
            disablecomplainant();
            chkCompFD.IsEnabled = false;
        }

        private void chkCompAnon_Unchecked(object sender, RoutedEventArgs e)
        {
            enablecomplainant();
            chkCompFD.IsEnabled = true;
        }

        private void chkCompFD_Checked(object sender, RoutedEventArgs e)
        {
            disablecomplainant();
            chkCompAnon.IsEnabled = false;
            txtCompFD.Visibility = System.Windows.Visibility.Visible;
            cboCompFD.Visibility = System.Windows.Visibility.Visible;
        }

        private void chkCompFD_Unchecked(object sender, RoutedEventArgs e)
        {
            enablecomplainant();
            chkCompAnon.IsEnabled = true;
            txtCompFD.Visibility = System.Windows.Visibility.Hidden;
            cboCompFD.Visibility = System.Windows.Visibility.Hidden;
        }

        private void disablecomplainant()
        {
            txtCompFName.Text = "";
            txtCompFName.IsEnabled = false;
            txtCompLName.Text = "";
            txtCompLName.IsEnabled = false;
            txtCompAddLine1.Text = "";
            txtCompAddLine1.IsEnabled = false;
            txtCompAddLine2.Text = "";
            txtCompAddLine2.IsEnabled = false;
            txtCompCity.Text = "";
            txtCompCity.IsEnabled = false;
            cboCompState.SelectedItem = null;
            cboCompState.IsEnabled = false;
            txtCompZip.Text = "";
            txtCompZip.IsEnabled = false;
            txtCompEmail.Text = "";
            txtCompEmail.IsEnabled = false;
            txtCompEmail.Text = "";
            txtCompPhone.IsEnabled = false;
        }

        private void enablecomplainant()
        {
            txtCompFName.IsEnabled = true;
            txtCompLName.IsEnabled = true;
            txtCompAddLine1.IsEnabled = true;
            txtCompAddLine2.IsEnabled = true;
            txtCompCity.IsEnabled = true;
            cboCompState.IsEnabled = true;
            txtCompZip.IsEnabled = true;
            txtCompEmail.IsEnabled = true;
            txtCompPhone.IsEnabled = true;
        }

        public void GetContactInfoFromZip(string zipcode, TextBox citybox, ComboBox statebox)
        {
            string[] strLoc = new string[2];
            strLoc = MainWindow.GetLocFromZip(zipcode);

            if (strLoc[0] != "")
                if (citybox.Text == "") citybox.Text = strLoc[0];
            if (strLoc[1] != "")
                if (statebox.Text == "")
                {
                    foreach (state x in MainWindow.States)
                    {
                        if (x.Abbr == strLoc[1]) statebox.SelectedValue = x.ID;
                    }
                }

        }

        private void cboPropCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                city x = (city)cboPropCity.SelectedItem;
                if (x.ID > 0)
                {
                    if (x.ZipCodeList != null && x.ZipCodeList.Count > 0) cboPropZip.Items.Filter = (y) => x.ZipCodeList.IndexOf(((ZipCode)y).Zip) > -1 || ((ZipCode)y).ID == 0;
                    else cboPropZip.Items.Filter = null;
                    if (x.TownshipIDList != null && x.TownshipIDList.Count > 0) cboPropTownship.Items.Filter = (y) => x.TownshipIDList.IndexOf(((township)y).ID) > -1 || ((township)y).ID == 0;
                    else cboPropTownship.Items.Filter = null;
                    if (cboPropCounty.Text == "") cboPropCounty.SelectedValue = (object)x.DefaultCounty.ID;
                    if (cboPropTownship.Text == "") cboPropTownship.SelectedValue = (object)x.DefaultTownship.ID;
                }
                else
                {
                    cboPropZip.Items.Filter = null;
                    cboPropTownship.Items.Filter = null;
                }
            }
        }

        private void cboOtherCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                city x = (city)cboOtherCity.SelectedItem;
                if (x.ID > 0)
                {
                    if (x.ZipCodeList != null && x.ZipCodeList.Count > 0) cboOtherZip.Items.Filter = (y) => x.ZipCodeList.IndexOf(((ZipCode)y).Zip) > -1 || ((ZipCode)y).ID == 0;
                    else cboOtherZip.Items.Filter = null;
                    if (x.TownshipIDList != null && x.TownshipIDList.Count > 0) cboOtherTownship.Items.Filter = (y) => x.TownshipIDList.IndexOf(((township)y).ID) > -1 || ((township)y).ID == 0;
                    else cboOtherTownship.Items.Filter = null;
                    if (cboOtherCounty.Text == "") cboOtherCounty.SelectedValue = (object)x.DefaultCounty.ID;
                    if (cboOtherTownship.Text == "") cboOtherTownship.SelectedValue = (object)x.DefaultTownship.ID;
                }
                else
                {
                    cboOtherZip.Items.Filter = null;
                    cboOtherTownship.Items.Filter = null;
                }
            }
        }

        private void cboOtherCounty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { if (IsLoaded && cboOtherState.Text == "") cboOtherState.SelectedValue = (object)(((county)cboOtherCounty.SelectedItem).DefaultState.ID); }

        private void cboBusCounty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { if (IsLoaded && cboBusState.Text == "") cboBusState.SelectedValue = (object)(((county)cboBusCounty.SelectedItem).DefaultState.ID); }

        private void cboPropCounty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { if (IsLoaded && cboPropState.Text == "") cboPropState.SelectedValue = (object)(((county)cboPropCounty.SelectedItem).DefaultState.ID); }

        private void txtCompZip_LostFocus(object sender, RoutedEventArgs e)
        { if ((txtCompZip.Text != "") && (txtCompZip.Text.Length >= 5)) GetContactInfoFromZip(txtCompZip.Text, txtCompCity, cboCompState); }

        private void btnFindExistingProp_Click(object sender, RoutedEventArgs e)
        {
            ResidentialSearch search = new ResidentialSearch(this);
            try { search.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); search.Close(); return; }
        }

        private void btnCreateNewProp_Click(object sender, RoutedEventArgs e)
        { SetResidentialAddress(new ResidentialLocation()); }

        public void SetResidentialAddress(ResidentialLocation loc)
        {
            residentLoc.AddressLine1 = txtPropAddLine1.Text = loc.AddressLine1;
            residentLoc.AddressLine2 = txtPropAddLine2.Text = loc.AddressLine2;
            residentLoc.City = loc.City;
            if (loc.City != null && loc.City.ID != 0) cboPropCity.SelectedItem = loc.City;
            else cboPropCity.SelectedIndex = 0;
            residentLoc.State = loc.State;
            if (loc.State != null && loc.State.ID != 0) cboPropState.SelectedItem = loc.State;
            else cboPropState.SelectedIndex = 0;
            residentLoc.Zip = loc.Zip;
            if (loc.Zip != "")
            {
                foreach(ZipCode z in cboPropZip.Items)
                { if (z.Zip == loc.Zip) cboPropZip.SelectedItem = z; }
            }
            residentLoc.County = loc.County;
            if (loc.County != null && loc.County.ID != 0) cboPropCounty.SelectedItem = loc.County;
            txtPropLat.Text = (loc.Latitude != 0) ? loc.Latitude.ToString() : "";
            residentLoc.Latitude = loc.Latitude;
            txtPropLon.Text = (loc.Longitude != 0) ? loc.Longitude.ToString() : "";
            residentLoc.Longitude = loc.Longitude;
            residentLoc.Parcel = txtPropParcel.Text = loc.Parcel;
            residentLoc.ID = loc.ID;
            residentLoc.PlaceID = loc.PlaceID;

            bool newenable = false;
            if (residentLoc.ID != 0) newenable = true;
            btnFindExistingProp.IsEnabled = !newenable;
            btnFindExistingProp.Foreground = (btnFindExistingProp.IsEnabled) ? System.Windows.Media.Brushes.DarkRed : System.Windows.Media.Brushes.DarkGray;
            btnCreateNewProp.IsEnabled = newenable;
            btnCreateNewProp.Foreground = (btnCreateNewProp.IsEnabled) ? System.Windows.Media.Brushes.DarkRed : System.Windows.Media.Brushes.DarkGray;
        }

        private void btnPropAuditor_Click(object sender, RoutedEventArgs e)
        {
            if (txtPropAddLine1.Text != "")
            {
                AuditorSearch search = new AuditorSearch(txtPropAddLine1.Text, this, txtPropParcel, cboPropZip);
                try { search.ShowDialog(); }
                catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); search.Close(); return; }
            }
        }
        
        private void btnBusAuditor_Click(object sender, RoutedEventArgs e)
        {
            if (txtBusAddLine1.Text != "")
            {
                AuditorSearch search = new AuditorSearch(txtBusAddLine1.Text, this, txtBusParcel, cboBusZip);
                try { search.ShowDialog(); }
                catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); search.Close(); return; }
            }
        }

        private void btnGetPropCoords_Click(object sender, RoutedEventArgs e)
        {
            Address loc = new Address();
            loc.AddressLine1 = txtPropAddLine1.Text;
            loc.AddressLine2 = txtPropAddLine2.Text;
            loc.City = cboPropCity.Text;
            loc.State = (cboPropState.SelectedIndex > 0) ? (state)cboPropState.SelectedItem : null;
            loc.Zip = (cboPropZip.SelectedIndex > 0) ? ((ZipCode)cboPropZip.SelectedItem).Zip : "";

            string lat, lon;

            MainWindow.GetCoords(loc, out(lat), out(lon));
            txtPropLat.Text = lat;
            txtPropLon.Text = lon;
        }

        private void btnGetBusCoords_Click(object sender, RoutedEventArgs e)
        {
            Address loc = new Address();
            loc.AddressLine1 = txtBusAddLine1.Text;
            loc.AddressLine2 = txtBusAddLine2.Text;
            loc.City = cboBusCity.Text;
            loc.State = (cboBusState.SelectedIndex > 0) ? (state)cboBusState.SelectedItem : null;
            loc.Zip = (cboBusZip.SelectedIndex > 0) ? ((ZipCode)cboBusZip.SelectedItem).Zip : "";

            string lat, lon;

            MainWindow.GetCoords(loc, out(lat), out(lon));
            txtBusLat.Text = lat;
            txtBusLon.Text = lon;
        }

        private void btnOtherCoords_Click(object sender, RoutedEventArgs e)
        {
            Address loc = new Address();
            loc.AddressLine1 = txtOtherAddLine1.Text;
            loc.AddressLine2 = txtOtherAddLine2.Text;
            loc.City = cboOtherCity.Text;
            loc.State = (cboOtherState.SelectedIndex > 0) ? (state)cboOtherState.SelectedItem : null;
            loc.Zip = (cboOtherZip.SelectedIndex > 0) ? ((ZipCode)cboOtherZip.SelectedItem).Zip : "";

            string lat, lon;

            MainWindow.GetCoords(loc, out(lat), out(lon));
            txtOtherLat.Text = lat;
            txtOtherLon.Text = lon;
        }

        private void cboBusCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                city x = (city)cboBusCity.SelectedItem;
                if (x.ID > 0)
                {
                    if (x.ZipCodeList != null && x.ZipCodeList.Count > 0) cboBusZip.Items.Filter = (y) => x.ZipCodeList.IndexOf(((ZipCode)y).Zip) > -1 || ((ZipCode)y).ID == 0;
                    else cboBusZip.Items.Filter = null;
                    if (x.TownshipIDList != null && x.TownshipIDList.Count > 0) cboBusTownship.Items.Filter = (y) => x.TownshipIDList.IndexOf(((township)y).ID) > -1 || ((township)y).ID == 0;
                    else cboBusTownship.Items.Filter = null;
                    if (cboBusCounty.Text == "") cboBusCounty.SelectedValue = (object)x.DefaultCounty.ID;
                    if (cboBusTownship.Text == "") cboBusTownship.SelectedValue = (object)x.DefaultTownship.ID;
                }
                else
                {
                    cboBusZip.Items.Filter = null;
                    cboBusTownship.Items.Filter = null;
                }
            }             
        }

        private void btnFindExistingBus_Click(object sender, RoutedEventArgs e)
        {
            NonPermFacSearch search = new NonPermFacSearch(SetNonPermFacAddress);
            try { search.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); search.Close(); return; }
        }

        private void btnFindExistingOther_Click(object sender, RoutedEventArgs e)
        {
            OtherSiteSearch search = new OtherSiteSearch(this);
            try { search.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); search.Close(); return; }
        }

        private void btnCreateNewBus_Click(object sender, RoutedEventArgs e)
        { SetNonPermFacAddress(0); }

        private void btnOtherAuditor_Click(object sender, RoutedEventArgs e)
        {
            if (txtOtherAddLine1.Text != "")
            {
                AuditorSearch search = new AuditorSearch(txtOtherAddLine1.Text, this, txtOtherParcel, cboOtherZip);
                try { search.ShowDialog(); }
                catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); search.Close(); return; }
            }
        }

        private void btnCreateNewOther_Click(object sender, RoutedEventArgs e)
        { SetOtherAddress(new OtherLocation()); }

        public void SetOtherAddress(OtherLocation loc)
        {
            otherloc.LocationDescription = txtBusDesc.Text = loc.LocationDescription;
            otherloc.AddressLine1 = txtBusAddLine1.Text = loc.AddressLine1;
            otherloc.AddressLine2 = txtBusAddLine2.Text = loc.AddressLine2;
            otherloc.City = loc.City;
            if (loc.City != null && loc.City.ID != 0) cboBusCity.SelectedItem = loc.City;
            else cboBusCity.SelectedIndex = 0;
            otherloc.State = loc.State;
            if (loc.State != null && loc.State.ID != 0) cboBusState.SelectedItem = loc.State;
            else cboBusState.SelectedIndex = 0;
            otherloc.Zip = loc.Zip;
            if (loc.Zip != "")
            {
                foreach (ZipCode z in cboBusZip.Items)
                { if (z.Zip == loc.Zip) cboBusZip.SelectedItem = z; }
            }
            otherloc.County = loc.County;
            if (loc.County != null && loc.County.ID != 0) cboBusCounty.SelectedItem = loc.County;
            txtBusLat.Text = (loc.Latitude != 0) ? loc.Latitude.ToString() : "";
            otherloc.Latitude = loc.Latitude;
            txtBusLon.Text = (loc.Longitude != 0) ? loc.Longitude.ToString() : "";
            otherloc.Longitude = loc.Longitude;
            otherloc.ID = loc.ID;

            bool newenable = false;
            if (otherloc.ID != 0) newenable = true;
            btnFindExistingOther.IsEnabled = !newenable;
            btnFindExistingOther.Foreground = (btnFindExistingOther.IsEnabled) ? System.Windows.Media.Brushes.DarkRed : System.Windows.Media.Brushes.DarkGray;
            btnCreateNewOther.IsEnabled = newenable;
            btnCreateNewOther.Foreground = (btnCreateNewOther.IsEnabled) ? System.Windows.Media.Brushes.DarkRed : System.Windows.Media.Brushes.DarkGray;
        }

        public void SetNonPermFacAddress(int id)
        {
            NonPermittedFacLocation loc = new NonPermittedFacLocation();
            if (id != 0) loc.LoadNewAddress(id);

            nonpermfacloc.AddressLine1 = txtBusAddLine1.Text = loc.AddressLine1;
            nonpermfacloc.AddressLine2 = txtBusAddLine2.Text = loc.AddressLine2;
            nonpermfacloc.City = loc.City;
            if (loc.City != null && loc.City.ID != 0) cboBusCity.SelectedItem = loc.City;
            else cboBusCity.SelectedIndex = 0;
            nonpermfacloc.State = loc.State;
            if (loc.State != null && loc.State.ID != 0) cboBusState.SelectedItem = loc.State;
            else cboBusState.SelectedIndex = 0;
            nonpermfacloc.Zip = loc.Zip;
            if (loc.Zip != "")
            {
                foreach (ZipCode z in cboBusZip.Items)
                { if (z.Zip == loc.Zip) cboBusZip.SelectedItem = z; }
            }
            nonpermfacloc.County = loc.County;
            if (loc.County != null && loc.County.ID != 0) cboBusCounty.SelectedItem = loc.County;
            txtBusLat.Text = (loc.Latitude != 0) ? loc.Latitude.ToString() : "";
            nonpermfacloc.Latitude = loc.Latitude;
            txtBusLon.Text = (loc.Longitude != 0) ? loc.Longitude.ToString() : "";
            nonpermfacloc.Longitude = loc.Longitude;
            nonpermfacloc.Parcel = txtBusParcel.Text = loc.Parcel;
            nonpermfacloc.ID = loc.ID;
            nonpermfacloc.PlaceID = loc.PlaceID;

            bool newenable = false;
            if (nonpermfacloc.ID != 0) newenable = true;
            btnFindExistingBus.IsEnabled = !newenable;
            btnFindExistingBus.Foreground = (btnFindExistingBus.IsEnabled) ? System.Windows.Media.Brushes.DarkRed : System.Windows.Media.Brushes.DarkGray;
            btnCreateNewBus.IsEnabled = newenable;
            btnCreateNewBus.Foreground = (btnCreateNewBus.IsEnabled) ? System.Windows.Media.Brushes.DarkRed : System.Windows.Media.Brushes.DarkGray;
        }

        private void btnFacGetCoords_Click(object sender, RoutedEventArgs e)
        {
            Address loc = new Address();
            loc.AddressLine1 = txtFacAddLine1.Text;
            loc.AddressLine2 = "";
            loc.City = txtFacCity.Text;
            if (cboFacState.Text != "") loc.State = (state)cboFacState.SelectedItem;
            loc.Zip = txtFacZip.Text;

            string lat, lon;

            MainWindow.GetCoords(loc, out(lat), out(lon));
            txtFacLat.Text = lat;
            txtFacLon.Text = lon;
        }

        private void cboFacID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboFacID.SelectedItem != null)
            {
                facilityloc = (Facility)cboFacID.SelectedItem;
                facilityloc.UpdateSiteControlContent();                    
            }
        }

        public void SetFacility(int id)
        {
            Facility fac = new Facility();
            MainWindow.GetSingleItem<Facility>(out fac, id, MainWindow.Facilities);
            cboFacID.SelectedItem = fac;
        }

        private void btnFacSearch_Click(object sender, RoutedEventArgs e)
        {
            FacilitySearch search = new FacilitySearch(SetFacility);
            try { search.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); search.Close(); return; }
        }
    }
}
