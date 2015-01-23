using System;
using System.Windows;
using System.Windows.Controls;

namespace CID2
{
    /// <summary>
    /// Interaction logic for AbatementContractorControl.xaml
    /// </summary>
    public partial class AbatementContractorControl : ContactControlBase
    {
        public new Contact ControlOwner { get; set; }

        public AbatementContractorControl()
        {
            InitializeComponent();
            InitializeControls();
        }

        public AbatementContractorControl(Contact owner)
        {
            ControlOwner = owner;

            InitializeComponent();
            InitializeControls();
        }

        public void InitializeControls()
        { FillCombo(cboState, MainWindow.States); }

        public void btnList_Click(object sender, RoutedEventArgs e)
        {
            ContactSearch search = new ContactSearch("tbl_Asbestos_Abatement_Contractors", LoadNewContact);
            try { search.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); search.Close(); return; }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            LoadNewContact(new Contact());
            ControlOwner.UpdateControlContent();
            btnNew.IsEnabled = false;
        }
    }
}
