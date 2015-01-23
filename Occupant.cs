using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CID2
{
    public class Occupant : Contact
    {
        public OccupantControl thisControl { get; set; }

        public Occupant()
        {
            ID = 0;
            FName = "";
            LName = "";
            Email = "";
            Phone = "";

            thisControl = new OccupantControl(this);
        }

        public Occupant(Complaint owner)
        { Owner = owner; }

        public Occupant(OccupantControl control, Complaint owner)
        {
            thisControl = control;
            Owner = owner;
        }

        public Occupant(int id, string firstname, string lastname, string addressline1, string addressline2,
            string city, state state, string zip, string email, string phone)
        {
            ID = id;
            FName = firstname;
            LName = lastname;
            Email = email;
            Phone = phone;
            AddressLine1 = addressline1;
            AddressLine2 = addressline2;
            City = city;
            State = state;
            Zip = zip;

            thisControl = new OccupantControl(this);
        }

        public Occupant(Contact contact)
        {
            ID = contact.ID;
            FName = contact.FName;
            LName = contact.LName;
            Email = contact.Email;
            Phone = contact.Phone;
            AddressLine1 = contact.AddressLine1;
            AddressLine2 = contact.AddressLine2;
            City = contact.City;
            State = contact.State;
            Zip = contact.Zip;
            strContactLabel = contact.strContactLabel;
            strContactTable = contact.strContactTable;
            strParentTable = contact.strParentTable;

            thisControl = new OccupantControl(this);
        }

        public override void UpdateContentFromControls()
        {
            FName = thisControl.txtPropConFName.Text;
            LName = thisControl.txtPropConLName.Text;
            Email = thisControl.txtPropConEmail.Text;
            Phone = thisControl.txtPropConPhone.Text;
            AddressLine1 = thisControl.txtPropConAddLine1.Text;
            AddressLine2 = thisControl.txtPropConAddLine2.Text;
            City = thisControl.txtPropConCity.Text;
            State = (thisControl.cboPropConState.SelectedIndex > 0) ? (state)thisControl.cboPropConState.SelectedItem : new state();
            Zip = thisControl.txtPropConZip.Text;
        }

        public override void UpdateControlContent()
        {
            thisControl.txtPropConFName.Text = FName;
            thisControl.txtPropConLName.Text = LName;
            thisControl.txtPropConEmail.Text = Email;
            thisControl.txtPropConPhone.Text = Phone;
            thisControl.txtPropConAddLine1.Text = AddressLine1;
            thisControl.txtPropConAddLine2.Text = AddressLine2;
            thisControl.txtPropConCity.Text = City;
            thisControl.cboPropConState.SelectedValue = (State != null) ? State.ID : 0;
            thisControl.txtPropConZip.Text = Zip;
        }
    }
}
