using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CID2
{
    public class ContactControlBase : UserControl
    {
        public Contact ControlOwner;

        public void FillCombo(ComboBox combo, ListCollectionView list)
        {
            combo.ItemsSource = list;
            combo.SelectedItem = null;
        }

        public void Button_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        { MainWindow.Button_IsEnabledChanged(sender, e); }

        public void btnSameAsSite_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult rslt = MessageBoxResult.Yes;
            if (ControlOwner.AddressLine1 != "" || ControlOwner.AddressLine2 != "" || ControlOwner.City != "" || ControlOwner.Zip != "")
                rslt = MessageBox.Show("This will replace the existing information. Are you sure you want to continue?", "Continue?", MessageBoxButton.YesNo);
            if (rslt == MessageBoxResult.Yes) ControlOwner.SameAsSiteAdress(ControlOwner.Owner.GetComplaintAddress());
        }

        public void LoadNewContact(Contact contact)
        {
            ControlOwner.SameAsContact(contact);
            ControlOwner.ID = contact.ID;
            ControlOwner.UpdateControlContent();
        }


        public void btnNewContact_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Button btn = sender as Button;
            if (IsLoaded)
            {
                if (!ControlOwner.Owner.Form.IsLocked || ControlOwner.ID == 0)
                {
                    btn.IsEnabled = false;
                    btn.Foreground = System.Windows.Media.Brushes.DarkGray;
                }

                if (btn.IsEnabled) btn.Foreground = System.Windows.Media.Brushes.DarkRed;
            }
        }
    }
}
