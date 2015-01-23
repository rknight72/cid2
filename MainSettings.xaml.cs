using System;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Data;
using Microsoft.Win32;

namespace CID2
{
    /// <summary>
    /// Interaction logic for MainSettings.xaml
    /// </summary>
    public partial class MainSettings : Window
    {
        private bool restart;

        public MainSettings()
        { InitializeComponent(); }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtCIDPath.Text = MainWindow.cidDBpath;
            //txtTasPath.Text = MainWindow.tasDBpath;

            restart = false;
        }

        private void btnCID_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".accdb";
            dlg.Filter = "Access database (.accdb)|*.accdb";

            if (dlg.ShowDialog() == true) txtCIDPath.Text = dlg.FileName;
        }

        private void btnFac_Click(object sender, RoutedEventArgs e)
        {
            /*OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".accdb";
            dlg.Filter = "Access database (.accdb)|*.accdb";

            if (dlg.ShowDialog() == true) txtFacPath.Text = dlg.FileName;*/
        }

        private void btnTas_Click(object sender, RoutedEventArgs e)
        {
            /*OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".accdb";
            dlg.Filter = "Access database (.accdb)|*.accdb";

            if (dlg.ShowDialog() == true) txtTasPath.Text = dlg.FileName;*/
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (restart == true) MessageBox.Show("You must restart CID for the changes to take effect.");
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            restart = true;

            OleDbConnection db = new OleDbConnection(MainWindow.strCon + txtCIDPath.Text + MainWindow.strCon2 + txtCIDPass.Password);
            try { db.Open(); }
            catch (Exception Exception)
            {
                MessageBox.Show("There was a problem opening your CID DB. Check the path and password.");
                System.Windows.MessageBox.Show(Exception.Message);
                restart = false;
            }

            /*db = new OleDbConnection(MainWindow.strCon + txtFacPath.Text + MainWindow.strCon2 + txtFacPass.Password);
            try { db.Open(); }
            catch (Exception Exception)
            {
                MessageBox.Show("There was a problem opening your Facs DB. Check the path and password."); 
                System.Windows.MessageBox.Show(Exception.Message);
                restart = false;
            }

            db = new OleDbConnection(MainWindow.strCon + txtTasPath.Text + MainWindow.strCon2 + txtTASPass.Password);
            try { db.Open(); }
            catch (Exception Exception)
            {
                MessageBox.Show("There was a problem opening your TAS DB. Check the path and password."); 
                System.Windows.MessageBox.Show(Exception.Message);
                restart = false;
            }*/

            db.Close();

            if (restart == true)
            {
                if (MessageBox.Show("Apply settings and close CID?","Restart Required", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                {
                    ApplySettings();
                    MainWindow.Main.Close();
                    Close();

                }
                else if (MessageBox.Show("Apply settings anyway?","Apply",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ApplySettings();
                }
            }
        }

        private void ApplySettings()
        {
            if (!MainWindow.IsSettingsDBConnected()) MainWindow.settingsDB.Open();

            string strSQL = "UPDATE tblDBpath SET Path = @path, Pass = @pass WHERE ID = ";
            OleDbCommand cmddb = new OleDbCommand(strSQL + "1;", MainWindow.settingsDB);
            cmddb.Parameters.AddWithValue("@path", txtCIDPath.Text);
            cmddb.Parameters.AddWithValue("@pass", txtCIDPass.Password);
            cmddb.ExecuteNonQuery();

            /*cmddb = new OleDbCommand(strSQL + "2;", MainWindow.settingsDB);
            cmddb.Parameters.AddWithValue("@path", txtFacPath.Text);
            cmddb.Parameters.AddWithValue("@pass", txtFacPass.Password);
            cmddb.ExecuteNonQuery();

            cmddb = new OleDbCommand(strSQL + "3;", MainWindow.settingsDB);
            cmddb.Parameters.AddWithValue("@path", txtTasPath.Text);
            cmddb.Parameters.AddWithValue("@pass", txtTASPass.Password);
            cmddb.ExecuteNonQuery();*/
        }
    }
}
