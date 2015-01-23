using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace CID2
{
    public class SiteControlBase : UserControl
    {
        public ComplaintAddress ControlOwner;
        public ComplaintAddress thisAddress;

        public void txtcoords_PreviewTextInput(object sender, TextCompositionEventArgs e)
        { e.Handled = !MainWindow.IsTextAllowed(e.Text); }

        public void txtcoords_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!MainWindow.IsTextAllowed(text)) e.CancelCommand();
            }
            else e.CancelCommand();
        }

        public void FillCombo(ComboBox combo, ListCollectionView list)
        {
            combo.ItemsSource = list;
            combo.SelectedItem = null;
        }

        public void LoadNewAddress(int id)
        { thisAddress.LoadNewAddress(id); }

        public void btnGetCoords_Click(object sender, RoutedEventArgs e)
        {
            string lat, lon;
            MainWindow.GetCoords(thisAddress, out(lat), out(lon));
            thisAddress.Latitude = Convert.ToDouble(lat);
            thisAddress.Longitude = Convert.ToDouble(lon);

            thisAddress.UpdateSiteControlContent();
        }

        public void btnAuditor_Click(object sender, RoutedEventArgs e)
        { thisAddress.Owner.Form.OpenAuditor(thisAddress.AddressLine1); }

        public void btnGetParcel_Click(object sender, RoutedEventArgs e)
        { thisAddress.Owner.Form.LoadParcelPage(thisAddress.AddressLine1); }

        public void Button_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        { MainWindow.Button_IsEnabledChanged(sender, e); }

        public void btnNewProperty_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            thisAddress.BlankAddress();
            thisAddress.UpdateSiteControlContent();
            btn.IsEnabled = false;
        }

        public void btnNewProperty_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Button btn = sender as Button;
            if (IsLoaded)
            {
                if (!thisAddress.Owner.Form.IsLocked || thisAddress.ID == 0)
                {
                    btn.IsEnabled = false;
                    btn.Foreground = System.Windows.Media.Brushes.DarkGray;
                }

                if (btn.IsEnabled) btn.Foreground = System.Windows.Media.Brushes.DarkRed;
            }
        }
    }
}
