using System;
using System.Windows;


namespace CID2
{
    /// <summary>
    /// Interaction logic for OwnerControl.xaml
    /// </summary>
    public partial class OwnerControl : ContactControlBase
    {
        public OwnerControl()
        {
            InitializeComponent();
        }

        public OwnerControl(Contact controlowner)
        {
            InitializeComponent();
            ControlOwner = controlowner;
            FillCombo(cboPropOwnState, MainWindow.States);
        }

        public void btnSameAsOcc_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult rslt = MessageBoxResult.Yes;
            if (ControlOwner.AddressLine1 != "" || ControlOwner.AddressLine2 != "" || ControlOwner.City != "" || ControlOwner.Zip != "")
                rslt = MessageBox.Show("This will replace the existing information. Are you sure you want to continue?", "Continue?", MessageBoxButton.YesNo);
            if (rslt == MessageBoxResult.Yes) ControlOwner.SameAsContact(ControlOwner.Owner.GetOccupantAddress());
        }
    }
}
