using System;
using System.Windows;
using System.Windows.Controls;

namespace CID2
{
    /// <summary>
    /// Interaction logic for EntityCoordinatorControl.xaml
    /// </summary>
    public partial class EntityCoordinatorControl : ContactControlBase
    {
        public new Contact ControlOwner { get; set; }

        public EntityCoordinatorControl()
        {
            InitializeComponent();
            InitializeControls();
        }

        public EntityCoordinatorControl(Contact owner)
        {
            ControlOwner = owner;

            InitializeComponent();
            InitializeControls();
        }

        public void InitializeControls()
        { FillCombo(cboState, MainWindow.States); }

        public void btnList_Click(object sender, RoutedEventArgs e)
        {
            ContactSearch search = new ContactSearch("tbl_Asbestos_Entity_Coordinators", LoadNewContact);
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
