using System;
using System.Data.OleDb;
using System.Windows;

namespace CID2
{
    /// <summary>
    /// Interaction logic for MiniComplaint.xaml
    /// </summary>
    public partial class MiniComplaint : Window
    {
        public int pID { get; set; }
        public int ID { get; set; }
        public DateTime DateReceived { get; set; }
        public casestatus Status { get; set; }
        public ComplaintType Type { get; set; }
        public string Complaint { get; set; }
        public string FacName { get; set; }
        public Address ComplaintSite { get; set; }
        public bool ShowMarkButtons { get; set; }

        public MiniComplaint()
        { InitializeComponent(); }

        public MiniComplaint(int pid, int id, DateTime datereceived, casestatus status, ComplaintType type, string complaint,
            string facname_bldgdesc, Address complaintsite, bool showmarkbuttons)
        {
            InitializeComponent();

            pID = pid;
            ID = id;
            DateReceived = datereceived;
            Status = status;
            Type = type;
            Complaint = complaint;
            FacName = facname_bldgdesc;
            ComplaintSite = complaintsite;
            ShowMarkButtons = showmarkbuttons;
        }

        public MiniComplaint(GenericComplaint comp)
        {
            InitializeComponent();

            pID = 0;
            ShowMarkButtons = false;

            ID = comp.ID;
            DateReceived = comp.DateReceived;
            Status = comp.Status;
            Type = comp.Type;
            Complaint = comp.ComplaintNotes;
            FacName = comp.FacName;
            ComplaintSite = new ComplaintAddress(comp.SiteAddress1, "", comp.SiteCity, null, "", null, comp.Township, comp.Parcel, 0, 0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtID.Text += ID.ToString();
            dtRcvd.Text += DateReceived.ToShortDateString();
            txtStatus.Text += Status.ToString();
            txtCompType.Text += Type.ToString();
            txtComplaint.Text += Complaint;
            txtBldg.Text += FacName;
            txtAddress.Text = ComplaintSite.AddressLine1;
            txtCity.Text = (ComplaintSite.City != null) ? ComplaintSite.City : "";
            txtState.Text = (ComplaintSite.State != null) ? ComplaintSite.State.ToString() : "";
            txtZip.Text = ComplaintSite.Zip;

            if (ShowMarkButtons & pID != 0)
            {
                switch (ComplaintForm.AlreadyRelated(ID, pID))
                {
                    case 0:
                        btnMarkClose.IsEnabled = btnMark.IsEnabled = true;
                        btnMarkClose.Visibility = btnMark.Visibility = Visibility.Visible;
                        btnRemoveClose.IsEnabled = btnUnMark.IsEnabled = false;
                        btnRemoveClose.Visibility = btnUnMark.Visibility = Visibility.Hidden;
                        break;
                    case 1:
                        btnMarkClose.IsEnabled = btnMark.IsEnabled = false;
                        btnMarkClose.Visibility = btnMark.Visibility = Visibility.Hidden;
                        btnRemoveClose.IsEnabled = btnUnMark.IsEnabled = true;
                        btnRemoveClose.Visibility = btnUnMark.Visibility = Visibility.Visible;
                        break;
                    default:
                        MessageBox.Show("There was an error opening the database.");
                        break;
                }
            }
            else
            {
                btnMark.Visibility = System.Windows.Visibility.Hidden;
                btnMarkClose.Visibility = System.Windows.Visibility.Hidden;
                btnRemoveClose.Visibility = System.Windows.Visibility.Hidden;
                btnUnMark.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void btnMark_Click(object sender, RoutedEventArgs e)
        {
            switch(ComplaintForm.AlreadyRelated(ID, pID))
            {
                case 0:
                    AddRelation();
                    break;
                case 1:
                    MessageBox.Show("This complaint is already related.");
                    break;
                default:
                    MessageBox.Show("There was an error opening the database.");
                    break;
            }
        }

        private void AddRelation()
        {
            MainWindow.OpenMainDBConnection();

            string strSQL = "INSERT INTO tbl_Related_Complaints (Complaint_ID, Related_ID) VALUES (@pid, @id);";
            OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
            cidCMD.Parameters.AddWithValue("@pid", pID);
            cidCMD.Parameters.AddWithValue("@id", ID);
            cidCMD.ExecuteNonQuery();
            MainWindow.CloseMainDBConnection();

            btnMarkClose.IsEnabled = btnMark.IsEnabled = false;
            btnMarkClose.Visibility = btnMark.Visibility = Visibility.Hidden;
            btnRemoveClose.IsEnabled = btnUnMark.IsEnabled = true;
            btnRemoveClose.Visibility = btnUnMark.Visibility = Visibility.Visible;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        { Close(); }

        private void btnUnMark_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this relationship?", "Confirm Removal", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                MainWindow.OpenMainDBConnection();

                string strSQL = "DELETE * FROM tbl_Related_Complaints WHERE ((Complaint_ID = " + pID.ToString() + ") AND (Related_ID = " + ID.ToString()
                    + ")) OR ((Complaint_ID = " + ID.ToString() + ") AND ( Related_ID = " + pID.ToString() + "));";
                OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                cidCMD.ExecuteNonQuery();
                MainWindow.CloseMainDBConnection();

                btnMarkClose.IsEnabled = btnMark.IsEnabled = true;
                btnMarkClose.Visibility = btnMark.Visibility = Visibility.Visible;
                btnRemoveClose.IsEnabled = btnUnMark.IsEnabled = false;
                btnRemoveClose.Visibility = btnUnMark.Visibility = Visibility.Hidden;
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            switch (Type.ID)
            {
                case 1:
                    AsbestosOtherCompForm form1 = new AsbestosOtherCompForm(MainWindow.Main.CreateAsbestosOther(ID), false);
                    try { form1.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form1.Close(); return; }
                    break;
                case 2:
                    AsbestosFacilityCompForm form2 = new AsbestosFacilityCompForm(MainWindow.Main.CreateAsbestosFacility(ID), false);
                    try { form2.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form2.Close(); return; }
                    break;
                case 3:
                    AsbestosNonPermFacForm form3 = new AsbestosNonPermFacForm(MainWindow.Main.CreateAsbestosNonPermFac(ID), false);
                    try { form3.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form3.Close(); return; }
                    break;
                case 4:
                    OBOtherForm form4 = new OBOtherForm(MainWindow.Main.CreateOBOther(ID), false);
                    try { form4.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form4.Close(); return; }
                    break;
                case 5:
                    OBFacCompForm form5 = new OBFacCompForm(MainWindow.Main.CreateOBFacility(ID), false);
                    try { form5.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form5.Close(); return; }
                    break;
                case 6: //Open Burn Non-permitted Facility
                    OBNonPermFacForm form6 = new OBNonPermFacForm(MainWindow.Main.CreateOBNonFac(ID), false);
                    try { form6.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form6.Close(); return; }
                    break;
                case 7: //Open Burn Residential
                    OBResCompForm form7 = new OBResCompForm(MainWindow.Main.CreateOBResidential(ID), false);
                    try { form7.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form7.Close(); return; }
                    break;
                case 8:
                    OdorOtherCompForm form8 = new OdorOtherCompForm(MainWindow.Main.CreateOdorOther(ID), false);
                    try { form8.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form8.Close(); return; }
                    break;
                case 9:
                    OdorFacCompForm form9 = new OdorFacCompForm(MainWindow.Main.CreateOdorFacility(ID), false);
                    try { form9.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form9.Close(); return; }
                    break;
                case 10:
                    OdorNonPermFacCompForm form10 = new OdorNonPermFacCompForm(MainWindow.Main.CreateOdorNonPermFac(ID), false);
                    try { form10.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form10.Close(); return; }
                    break;
                case 11://Odor Residential
                    OdorResCompForm form11 = new OdorResCompForm(MainWindow.Main.CreateOdorResidential(ID), false);
                    try { form11.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form11.Close(); return; }
                    break;
                case 12:
                    OtherOtherCompForm form12 = new OtherOtherCompForm(MainWindow.Main.CreateOtherOther(ID), false);
                    try { form12.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form12.Close(); return; }
                    break;
                case 13:
                    OtherFacCompForm form13 = new OtherFacCompForm(MainWindow.Main.CreateOtherFacility(ID), false);
                    try { form13.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form13.Close(); return; }
                    break;
                case 14:
                    OtherNonPermFacCompForm form14 = new OtherNonPermFacCompForm(MainWindow.Main.CreateOtherNonPermFac(ID), false);
                    try { form14.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form14.Close(); return; }
                    break;
                case 15:
                    OtherResCompForm form15 = new OtherResCompForm(MainWindow.Main.CreateOtherResidential(ID), false);
                    try { form15.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form15.Close(); return; }
                    break;
                case 16:
                    DustOtherCompForm form16 = new DustOtherCompForm(MainWindow.Main.CreateDustOther(ID), false);
                    try { form16.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form16.Close(); return; }
                    break;
                case 17:
                    DustFacilityCompForm form17 = new DustFacilityCompForm(MainWindow.Main.CreateDustFacility(ID), false);
                    try { form17.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form17.Close(); return; }
                    break;
                case 18:
                    DustNonPermFacCompForm form18 = new DustNonPermFacCompForm(MainWindow.Main.CreateDustNonPermFac(ID), false);
                    try { form18.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form18.Close(); return; }
                    break;
                case 19:
                    DustResCompForm form19 = new DustResCompForm(MainWindow.Main.CreateDustResidential(ID), false);
                    try { form19.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form19.Close(); return; }
                    break;
                case 20:
                    ReleaseOtherCompForm form20 = new ReleaseOtherCompForm(MainWindow.Main.CreateReleaseOther(ID), false);
                    try { form20.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form20.Close(); return; }
                    break;
                case 21:
                    ReleaseFacCompForm form21 = new ReleaseFacCompForm(MainWindow.Main.CreateReleaseFacility(ID), false);
                    try { form21.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form21.Close(); return; }
                    break;
                case 22:
                    ReleaseNonPermFacCompForm form22 = new ReleaseNonPermFacCompForm(MainWindow.Main.CreateReleaseNonPermFac(ID), false);
                    try { form22.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form22.Close(); return; }
                    break;
                case 23:
                    ReleaseResCompForm form23 = new ReleaseResCompForm(MainWindow.Main.CreateReleaseResidential(ID), false);
                    try { form23.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form23.Close(); return; }
                    break;
                case 24:
                    AsbestosResCompForm form24 = new AsbestosResCompForm(MainWindow.Main.CreateAsbestosResidential(ID), false);
                    try { form24.Show(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); form24.Close(); return; }
                    break;
                default:
                    break;
            }
        }

        private void btnMarkClose_Click(object sender, RoutedEventArgs e)
        {
            btnMark_Click(sender, e);
            Close();
        }

        private void btnRemoveClose_Click(object sender, RoutedEventArgs e)
        {
            btnUnMark_Click(sender, e);
            Close();
        }
    }
}
