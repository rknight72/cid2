using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Data.OleDb;
//using System.Windows.Data;
//using System.ComponentModel;

namespace CID2
{
    public static class InspectionDetails
    {
        public static Grid gridCurrent { get; set; }
        public static int intInspectionType { get; set; }
        public static Grid gridLocation { get; set; }
        public static int intLocationType { get; set; }
        public static Grid gridContact { get; set; }
        public static Grid gridOwner { get; set; }
    }

    /// <summary>
    /// Interaction logic for InspectionWizard.xaml
    /// </summary>
    public partial class InspectionWizard : Window
    {
        public InspectionWizard()
        {
            InitializeComponent();

            InspectionDetails.intInspectionType = 0;
            InspectionDetails.intLocationType = 0;
            InspectionDetails.gridCurrent = grid01;
            SwitchGridButtons(true, false);

            FillCityCombos();
        }
        
        private void FillCityCombos()
        {
            //CityList.CityCollection = MainWindow.Cities;
            cboPropCity.ItemsSource = MainWindow.Cities;
            cboPropCity.SelectedIndex = 0;
            cboFacCity.ItemsSource = MainWindow.Cities;
            cboFacCity.SelectedIndex = 0;
            cboBusCity.ItemsSource = MainWindow.Cities;
            cboBusCity.SelectedIndex = 0;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            GetNextGrid();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            GetPrevGrid();
        }

        private void SwitchGridButtons(bool bNext, bool bPrev)
        {
            switch(bNext)
            {
                case true:
                    btnNext.Visibility = System.Windows.Visibility.Visible;
                    break;
                case false:
                    btnNext.Visibility = System.Windows.Visibility.Hidden;
                    break;
                default:
                    btnNext.Visibility = System.Windows.Visibility.Visible;
                    break;
            }

            switch (bPrev)
            {
                case true:
                    btnBack.Visibility = System.Windows.Visibility.Visible;
                    break;
                case false:
                    btnBack.Visibility = System.Windows.Visibility.Hidden;
                    break;
                default:
                    btnBack.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }

        private void GetNextGrid()
        {
            switch (InspectionDetails.gridCurrent.Name)
            {
                case "grid01":
                    if (InspectionDetails.intInspectionType != 0)
                    {
                        if (InspectionDetails.intLocationType != 0)
                        {
                            InspectionDetails.intLocationType = 0;
                            btnLoc1.IsChecked = false;
                            btnLoc2.IsChecked = false;
                            btnLoc3.IsChecked = false;
                            btnLoc4.IsChecked = false;
                        }
                        grid01.Visibility = System.Windows.Visibility.Hidden;
                        grid02.Visibility = System.Windows.Visibility.Visible;
                        InspectionDetails.gridCurrent = grid02;
                        SwitchGridButtons(true, true);
                    }
                    break;
                case "grid02":
                    if (InspectionDetails.intLocationType != 0)
                    {
                        grid02.Visibility = System.Windows.Visibility.Hidden;
                        switch (InspectionDetails.intLocationType)
                        {
                            case 1:
                                grid03a.Visibility = System.Windows.Visibility.Visible;
                                InspectionDetails.gridCurrent = grid03a;
                                break;
                            case 2:
                                grid03b.Visibility = System.Windows.Visibility.Visible;
                                InspectionDetails.gridCurrent = grid03b;
                                break;
                            default:
                                grid03c.Visibility = System.Windows.Visibility.Visible;
                                InspectionDetails.gridCurrent = grid03c;
                                break;
                        }
                        SwitchGridButtons(true, true);
                    }
                    this.Width = 600;
                    this.Height = 370;
                    break;
                case "grid03a":
                    grid03a.Visibility = System.Windows.Visibility.Hidden;
                    grid04a.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid04a;
                    SwitchGridButtons(true, true);
                    break;
                case "grid03b":
                    grid03b.Visibility = System.Windows.Visibility.Hidden;
                    grid04b.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid04b;
                    SwitchGridButtons(true, true);
                    break;
                case "grid03c":
                    grid03c.Visibility = System.Windows.Visibility.Hidden;
                    grid04c.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid04c;
                    SwitchGridButtons(true, true);
                    break;
                case "grid04a":
                    grid04a.Visibility = System.Windows.Visibility.Hidden;
                    grid05a.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid05a;
                    SwitchGridButtons(false, true);
                    break;
                case "grid04b":
                    grid04b.Visibility = System.Windows.Visibility.Hidden;
                    grid05b.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid05b;
                    SwitchGridButtons(false, true);
                    break;
                case "grid04c":
                    grid04c.Visibility = System.Windows.Visibility.Hidden;
                    grid05c.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid05c;
                    SwitchGridButtons(false, true);
                    break;
                default:
                    break;
            }
        }

        private void GetPrevGrid()
        {
            switch (InspectionDetails.gridCurrent.Name)
            {
                case "grid02":
                    grid02.Visibility = System.Windows.Visibility.Hidden;
                    grid01.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid01;
                    SwitchGridButtons(true, false);
                    break;
                case "grid03a":
                    grid03a.Visibility = System.Windows.Visibility.Hidden;
                    grid02.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid02;
                    SwitchGridButtons(true, true);
                    this.Width = 400;
                    this.Height = 300;
                    break;
                case "grid03b":
                    grid03b.Visibility = System.Windows.Visibility.Hidden;
                    grid02.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid02;
                    SwitchGridButtons(true, true);
                    this.Width = 400;
                    this.Height = 300;
                    break;
                case "grid03c":
                    grid03c.Visibility = System.Windows.Visibility.Hidden;
                    grid02.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid02;
                    SwitchGridButtons(true, true);
                    this.Width = 400;
                    this.Height = 300;
                    break;
                case "grid04a":
                    grid04a.Visibility = System.Windows.Visibility.Hidden;
                    grid03a.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid03a;
                    SwitchGridButtons(true, true);
                    break;
                case "grid04b":
                    grid04b.Visibility = System.Windows.Visibility.Hidden;
                    grid03b.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid03b;
                    SwitchGridButtons(true, true);
                    break;
                case "grid04c":
                    grid04c.Visibility = System.Windows.Visibility.Hidden;
                    grid03c.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid03c;
                    SwitchGridButtons(true, true);
                    break;
                case "grid05a":
                    grid05a.Visibility = System.Windows.Visibility.Hidden;
                    grid04a.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid04a;
                    SwitchGridButtons(true, true);
                    break;
                case "grid05b":
                    grid05b.Visibility = System.Windows.Visibility.Hidden;
                    grid04b.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid04b;
                    SwitchGridButtons(true, true);
                    break;
                case "grid05c":
                    grid05c.Visibility = System.Windows.Visibility.Hidden;
                    grid04c.Visibility = System.Windows.Visibility.Visible;
                    InspectionDetails.gridCurrent = grid04c;
                    SwitchGridButtons(true, true);
                    break;
                default:
                    break;
            }
        }

        private void InspectionButton1_Click(object sender, RoutedEventArgs e)
        {
            InspectionDetails.intInspectionType = 1;
        }

        private void InspectionButton2_Click(object sender, RoutedEventArgs e)
        {
            InspectionDetails.intInspectionType = 2;
        }

        private void InspectionButton3_Click(object sender, RoutedEventArgs e)
        {
            InspectionDetails.intInspectionType = 3;
        }

        private void InspectionButton4_Click(object sender, RoutedEventArgs e)
        {
            InspectionDetails.intInspectionType = 4;
        }

        private void InspectionButton5_Click(object sender, RoutedEventArgs e)
        {
            InspectionDetails.intInspectionType = 5;
        }

        private void InspectionButton6_Click(object sender, RoutedEventArgs e)
        {
            InspectionDetails.intInspectionType = 6;
        }

        private void LocationButton1_Click(object sender, RoutedEventArgs e)
        {
            InspectionDetails.intLocationType = 1;
        }

        private void LocationButton2_Click(object sender, RoutedEventArgs e)
        {
            InspectionDetails.intLocationType = 2;
        }

        private void LocationButton3_Click(object sender, RoutedEventArgs e)
        {
            InspectionDetails.intLocationType = 3;
        }

        private void LocationButton4_Click(object sender, RoutedEventArgs e)
        {
            InspectionDetails.intLocationType = 4;
        }
    }
}
