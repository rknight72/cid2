using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CID2
{
    public class AbatementContractor : Contact
    {
        public AbatementContractorControl thisControl { get; set; }

        public AbatementContractor()
        {
            ID = 0;
            FName = LName = Email = Phone = City = Zip = "";
            State = new state();

            thisControl = new AbatementContractorControl(this);
        }

        public AbatementContractor(Complaint owner)
        { Owner = owner; }

        public AbatementContractor(AbatementContractorControl control, Complaint owner)
        {
            thisControl = control;
            Owner = owner;
        }

        public AbatementContractor(int id, string firstname, string lastname, string addressline1, string addressline2,
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

            thisControl = new AbatementContractorControl(this);
        }

        public AbatementContractor(Contact contact)
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

            thisControl = new AbatementContractorControl(this);
        }

        public override void UpdateContentFromControls()
        {
            FName = thisControl.txtName.Text;
            LName = thisControl.txtLName.Text;
            Email = thisControl.txtEmail.Text;
            Phone = thisControl.txtPhone.Text;
            AddressLine1 = thisControl.txtAddLine1.Text;
            AddressLine2 = thisControl.txtAddLine2.Text;
            City = thisControl.txtCity.Text;
            State = (thisControl.cboState.SelectedIndex > 0) ? (state)thisControl.cboState.SelectedItem : new state();
            Zip = thisControl.txtZip.Text;
        }

        public override void UpdateControlContent()
        {
            thisControl.txtName.Text = FName;
            thisControl.txtLName.Text = LName;
            thisControl.txtEmail.Text = Email;
            thisControl.txtPhone.Text = Phone;
            thisControl.txtAddLine1.Text = AddressLine1;
            thisControl.txtAddLine2.Text = AddressLine2;
            thisControl.txtCity.Text = City;
            thisControl.cboState.SelectedValue = (State != null) ? State.ID : 0;
            thisControl.txtZip.Text = Zip;
        }
    }
}
