using mshtml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Data.Sql;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace CID2
{
    /// <summary>
    /// Interaction logic for AuditorSearch.xaml
    /// </summary>
    public partial class AuditorSearch : Window
    {
        public ComplaintWizard Wizard { get; set; }
        public TextBox ParcelBox { get; set; }
        public ComboBox ZipCombo { get; set; }

        public AuditorSearch(string add, ComplaintWizard wizard, TextBox parcelbox, ComboBox zipcombo)
        {
            InitializeComponent();

            string strURL = "";
            if (add != null)
            {
                strURL = MainWindow.GetAuditorURL(add);
                if (strURL != null) ParcelBrowser.Navigate(strURL);
            }

            Wizard = wizard;
            ParcelBox = parcelbox;
            ZipCombo = zipcombo;
        }

        private void ParcelBrowse_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            string strResults = "";
            HTMLDocument doc = (HTMLDocument)ParcelBrowser.Document;

            if (doc != null)
            {
                if (doc.url.Contains("http://maps"))
                {
                    MessageBox.Show(doc.url);
                }
                else
                {
                    if (doc.body.innerHTML != null)
                    {
                        strResults = MainWindow.ParceParcel(doc.body.innerHTML);
                        if (MainWindow.IsNumeric(strResults))
                            MainWindow.ParceResults((IHTMLTable)doc.getElementById("ctl00_ContentPlaceHolder1_gvSearchResults"), listAddresses);
                        else
                        {
                            MessageBox.Show("There were no matches.");
                            if (MessageBox.Show("Show the auditors page? You may need to accept the Terms of Service and try again.", "No matches", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            { ParcelBrowser.Visibility = System.Windows.Visibility.Visible; }
                        }
                    }
                }
            }
        }

        private void listAddresses_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            
            if (row.Item != null)
            {
                ParcelResult parcel = (ParcelResult)row.Item;

                int pos = parcel.strAdd.LastIndexOf(" ") + 1;
                string zip = parcel.strAdd.Substring(pos, 5);

                ParcelConfirm confirm = new ParcelConfirm(parcel.strParcel, zip, parcel.strOwner, ParcelBox, ZipCombo, null);
                bool? x = confirm.ShowDialog();
                if (x == true) Close();
            }
        }
    }
}
