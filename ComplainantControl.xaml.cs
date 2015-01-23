using System;
using System.Windows;
using System.Windows.Controls;

namespace CID2
{
    /// <summary>
    /// Interaction logic for ComplainantControl.xaml
    /// </summary>
    public partial class ComplainantControl : ContactControlBase
    {
        public ComplainantControl()
        {
            InitializeComponent();
        }

        public ComplainantControl(Contact controlowner)
        {
            InitializeComponent();
            ControlOwner = controlowner;
            FillCombo(cboCompState, MainWindow.States);
            FillCombo(cboFireDepts, MainWindow.FireDepartments);
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (ControlOwner.Owner.CompType.Category.ID != 5)
            {
                lblCompFD.Visibility = System.Windows.Visibility.Hidden;
                chkCompFD.Visibility = System.Windows.Visibility.Hidden;
                txtCompAddLine1.SetValue(Grid.ColumnSpanProperty, 2);
                txtCompAddLine1.Margin = new Thickness(10, 20, 10, 0);

                cboFireDepts.Visibility = System.Windows.Visibility.Hidden;
                lblFireDepts.Visibility = System.Windows.Visibility.Hidden;
                txtCompAddLine2.SetValue(Grid.ColumnSpanProperty, 2);
                txtCompAddLine2.Margin = new Thickness(10, 20, 10, 0);
            }

            EnableControls(!(bool)chkCompAnon.IsChecked);
        }

        public void chkCompAnon_Checked(object sender, RoutedEventArgs e)
        {
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBoxResult.Yes;
            if (IsLoaded) result = MessageBox.Show("All complainant info will be erased. Are you sure you want to continue?", "Confirm", button);
            if (result == MessageBoxResult.Yes)
            {
                ControlOwner.Owner.Anonymous = true;
                EnableControls(false);
                chkCompFD.IsEnabled = false;
                chkCompFD.IsChecked = false;
                cboFireDepts.IsEnabled = false;
            }
        }

        public virtual void chkCompAnon_Unchecked(object sender, RoutedEventArgs e)
        {
            ControlOwner.Owner.Anonymous = false;
            EnableControls(true);
            chkCompFD.IsEnabled = true;
        }

        public void CheckControls()
        {
            bool enabled = (!(bool)chkCompAnon.IsChecked) && (!(bool)chkCompFD.IsChecked);
            txtCompFName.IsEnabled = enabled;
            txtCompLName.IsEnabled = enabled;
            txtCompAddLine1.IsEnabled = enabled;
            txtCompAddLine2.IsEnabled = enabled;
            txtCompCity.IsEnabled = enabled;
            txtCompZip.IsEnabled = enabled;
            txtCompEmail.IsEnabled = enabled;
            txtCompPhone.IsEnabled = enabled;
            cboCompState.IsEnabled = enabled;
        }

        private void EnableControls(bool enabled)
        {
            if (ControlOwner.Owner.Form.IsLocked)
            {
                txtCompFName.IsEnabled = enabled;
                txtCompLName.IsEnabled = enabled;
                txtCompAddLine1.IsEnabled = enabled;
                txtCompAddLine2.IsEnabled = enabled;
                txtCompCity.IsEnabled = enabled;
                txtCompZip.IsEnabled = enabled;
                txtCompEmail.IsEnabled = enabled;
                txtCompPhone.IsEnabled = enabled;
                cboCompState.IsEnabled = enabled;

                if (!enabled)
                {
                    ControlOwner.FName = ControlOwner.LName = ControlOwner.AddressLine1 = ControlOwner.AddressLine2 = ControlOwner.City = ControlOwner.Zip = ControlOwner.Email = ControlOwner.Phone = "";
                    ControlOwner.State = new state();
                    ControlOwner.UpdateControlContent();
                }
            }
        }

        private void chkCompFD_Checked(object sender, RoutedEventArgs e)
        {
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxResult result = MessageBoxResult.Yes;
            if (IsLoaded) result = MessageBox.Show("All complainant info will be erased. Are you sure you want to continue?", "Confirm", button);
            if (result == MessageBoxResult.Yes)
            {
                ((OBComplaint)ControlOwner.Owner).FDGenerated = true;
                EnableControls(false);
                chkCompAnon.IsEnabled = false;
                chkCompAnon.IsChecked = false;
                cboFireDepts.IsEnabled = true;
            }
        }

        private void chkCompFD_Unchecked(object sender, RoutedEventArgs e)
        {
            EnableControls(true);
            chkCompAnon.IsEnabled = true;
            cboFireDepts.IsEnabled = false;
            ((OBComplaint)(ControlOwner.Owner)).FDGenerated = false;
            ((OBComplaint)(ControlOwner.Owner)).FDinfo = null;
            cboFireDepts.SelectedIndex = 0;
        }

        private void cboFireDepts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((cboFireDepts.IsInitialized == true) && (chkCompFD.IsChecked == true))
            {
                ControlOwner.SameAsContact(((FireDepartment)cboFireDepts.SelectedItem).FDinfo);
                ((OBComplaint)(ControlOwner.Owner)).FDGenerated = true;
                ((OBComplaint)(ControlOwner.Owner)).FDinfo = (FireDepartment)cboFireDepts.SelectedItem;
                ControlOwner.UpdateControlContent();
            }
        }
    }
}