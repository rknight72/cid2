using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CID2
{
    public class Owner : Contact
    {
        public OwnerControl thisControl { get; set; }

        public Owner()
        {
            ID = 0;
            FName = "";
            LName = "";
            Email = "";
            Phone = "";

            thisControl = new OwnerControl(this);
        }

        public Owner(Complaint owner)
        { Owner = owner; }

        public Owner(OwnerControl control, Complaint owner)
        {
            thisControl = control;
            Owner = owner;
        }

        public Owner(int id, string firstname, string lastname, string addressline1, string addressline2,
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

            thisControl = new OwnerControl(this);
        }

        public Owner(Contact contact)
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

            thisControl = new OwnerControl(this);
        }

        public override void UpdateContentFromControls()
        {
            FName = thisControl.txtPropOwnFName.Text;
            LName = thisControl.txtPropOwnLName.Text;
            Email = thisControl.txtPropOwnEmail.Text;
            Phone = thisControl.txtPropOwnPhone.Text;
            AddressLine1 = thisControl.txtPropOwnAddLine1.Text;
            AddressLine2 = thisControl.txtPropOwnAddLine2.Text;
            City = thisControl.txtPropOwnCity.Text;
            State = (thisControl.cboPropOwnState.SelectedIndex > 0) ? (state)thisControl.cboPropOwnState.SelectedItem : new state();
            Zip = thisControl.txtPropOwnZip.Text;
        }

        public override void UpdateControlContent()
        {
            thisControl.txtPropOwnFName.Text = FName;
            thisControl.txtPropOwnLName.Text = LName;
            thisControl.txtPropOwnEmail.Text = Email;
            thisControl.txtPropOwnPhone.Text = Phone;
            thisControl.txtPropOwnAddLine1.Text = AddressLine1;
            thisControl.txtPropOwnAddLine2.Text = AddressLine2;
            thisControl.txtPropOwnCity.Text = City;
            thisControl.cboPropOwnState.SelectedValue = (State != null) ? State.ID : 0;
            thisControl.txtPropOwnZip.Text = Zip;
        }
    }
}
