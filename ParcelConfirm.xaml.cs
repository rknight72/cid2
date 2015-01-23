using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CID2
{
    /// <summary>
    /// Interaction logic for ParcelConfirm.xaml
    /// </summary>
    /// 

    public partial class ParcelConfirm : Window
    {
        private static string strParcel;
        private static string strZip;
        private static string strOwner;
        private static TextBox ParcelBox;
        private static ComboBox ZipBox;
        private static TextBox OwnerBox;
        private static Complaint Comp;

        public ParcelConfirm(string parcel, string zip, string owner, TextBox parcelbox, ComboBox zipbox, TextBox ownerbox)
        {
            InitializeComponent();

            strParcel = parcel;
            strZip = zip;
            strOwner = owner;
            ParcelBox = parcelbox;
            ZipBox = zipbox;
            OwnerBox = ownerbox;
        }

        public ParcelConfirm(ParcelResult result, Complaint complaint)
        {
            InitializeComponent();

            int pos = result.strAdd.LastIndexOf(" ") + 1;
            string zip = result.strAdd.Substring(pos, 5);

            strOwner = result.strOwner;
            strParcel = result.strParcel;
            Comp = complaint;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ParcelID.Text = strParcel;
            ParcelZip.Text = strZip;
            if (strOwner != "") ParcelOwner.Text = strOwner;
            else
            {
                Ownerchk.IsEnabled = false;
                Ownerchk.Visibility = System.Windows.Visibility.Hidden;
                ParcelOwner.IsEnabled = false;
                ParcelOwner.Visibility = System.Windows.Visibility.Hidden;
                txtParcelChkLable.Visibility = System.Windows.Visibility.Hidden;
                txtParcelOwnerLabel.Visibility = System.Windows.Visibility.Hidden;
                Width = 300;
                Height = 250;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Comp.SetParcelInfo(((bool)IDchk.IsChecked) ? strParcel : "", ((bool)Ownerchk.IsChecked) ? strOwner : "", ((bool)Zipchk.IsChecked) ? strZip : "");
            DialogResult = true;
            Close();
        }
    }
}
