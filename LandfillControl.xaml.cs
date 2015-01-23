using System;
using System.Windows;
using System.Windows.Controls;

namespace CID2
{
    /// <summary>
    /// Interaction logic for LandfillControl.xaml
    /// </summary>
    public partial class LandfillControl : ContactControlBase
    {
        public new Contact ControlOwner { get; set; }

        public LandfillControl()
        {
            InitializeComponent();
            InitializeControls();
        }

        public LandfillControl(Contact owner)
        {
            ControlOwner = owner;

            InitializeComponent();
            InitializeControls();
        }

        public void InitializeControls()
        { FillCombo(cboLandfillState, MainWindow.States); }

        public void btnLandfillList_Click(object sender, RoutedEventArgs e)
        {
            ContactSearch search = new ContactSearch("tbl_Asbestos_Landfills", LoadNewContact);
            try { search.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); search.Close(); return; }
        }

        private void btnNewLandfill_Click(object sender, RoutedEventArgs e)
        {
            LoadNewContact(new Contact());
            ControlOwner.UpdateControlContent();
            btnNewLandfill.IsEnabled = false;
        }
    }
}
