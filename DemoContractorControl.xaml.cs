using System;
using System.Windows;
using System.Windows.Controls;

namespace CID2
{
    /// <summary>
    /// Interaction logic for DemoContractorControl.xaml
    /// </summary>
    public partial class DemoContractorControl : ContactControlBase
    {
        public new Contact ControlOwner { get; set; }

        public DemoContractorControl()
        {
            InitializeComponent();
            InitializeControls();
        }

        public DemoContractorControl(Contact owner)
        {
            ControlOwner = owner;

            InitializeComponent();
            InitializeControls();
        }

        public void InitializeControls()
        { FillCombo(cboDemoState, MainWindow.States); }

        public void btnDemoList_Click(object sender, RoutedEventArgs e)
        {
            ContactSearch search = new ContactSearch("tbl_Asbestos_Demolition_Contractors", LoadNewContact);
            try { search.ShowDialog(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); search.Close(); return; }
        }

        private void btnNewDemo_Click(object sender, RoutedEventArgs e)
        {
            LoadNewContact(new Contact());
            ControlOwner.UpdateControlContent();
            btnNewDemo.IsEnabled = false;
        }
    }
}
