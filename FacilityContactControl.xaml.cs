using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace CID2
{
    /// <summary>
    /// Interaction logic for FacilityContactControl.xaml
    /// </summary>
    public partial class FacilityContactControl : UserControl
    {
        public Complaint ControlOwner { get; set; }
        public FacilityContact thisContact { get; set; }
        public string ContactNameOverride { get; set; }
        public ListCollectionView ContactList;

        public FacilityContactControl()
        {
            InitializeComponent();
            InitializeControls();
        }

        public FacilityContactControl(Complaint owner, FacilityContact contact)
        {
            ControlOwner = owner;
            thisContact = contact;
            thisContact.SetContactControl(this, owner.TypeTable, "PrimaryContact");

            InitializeComponent();
            InitializeControls();
        }

        public void InitializeControls()
        {
            List<FacilityContact> contlist = new List<FacilityContact>();
            foreach (object z in MainWindow.FacilityContacts)
            { contlist.Add((FacilityContact)z); }
            ContactList = new ListCollectionView(contlist);
            ContactList.SortDescriptions.Add(new SortDescription("ContactType", ListSortDirection.Ascending));
            cboFacContactTypes.ItemsSource = ContactList;
            cboFacContactTypes.SelectedItem = null;
        }

        public void cboFacContactTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;

            if (IsLoaded && combo.SelectedValue != null && combo.SelectedIndex > 0) SetFacilityContact((int)combo.SelectedValue);
        }

        public void SetFacilityContact(int id)
        {
            if (id > 0)
            {
                int tableid = (thisContact != null && thisContact.ID != 0) ? thisContact.ID : 0;
                FacilityContact contact = new FacilityContact();
                MainWindow.GetSingleItem<FacilityContact>(out contact, id, MainWindow.FacilityContacts);
                contact.ID = tableid;
                thisContact = contact;
                thisContact.SetContactControl(this, ControlOwner.TypeTable, "PrimaryContact");

                thisContact.UpdateControlContent();
            }
        }

        public void UpdateContact(FacilityContact contact)
        {
            thisContact = contact;
            thisContact.SetContactControl(this, ControlOwner.TypeTable, "PrimaryContact");
            thisContact.UpdateControlContent();
        }

        public void SetContactFilter(string facilityid)
        { ContactList.Filter = (y) => ((FacilityContact)y).FacID == facilityid || ((FacilityContact)y).ID == 0; }
    }
}
