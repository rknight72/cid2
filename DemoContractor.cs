using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CID2
{
    public class DemoContractor : Contact
    {
        public DemoContractorControl thisControl { get; set; }

        public DemoContractor()
        {
            ID = 0;
            FName = LName = Email = Phone = City = Zip = "";
            State = new state();

            thisControl = new DemoContractorControl(this);
        }

        public DemoContractor(Complaint owner)
        { Owner = owner; }

        public DemoContractor(DemoContractorControl control, Complaint owner)
        {
            thisControl = control;
            Owner = owner;
        }

        public DemoContractor(int id, string firstname, string lastname, string addressline1, string addressline2,
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

            thisControl = new DemoContractorControl(this);
        }

        public DemoContractor(Contact contact)
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

            thisControl = new DemoContractorControl(this);
        }

        public override void UpdateContentFromControls()
        {
            FName = thisControl.txtDemoName.Text;
            LName = thisControl.txtDemoLName.Text;
            Email = thisControl.txtDemoEmail.Text;
            Phone = thisControl.txtDemoPhone.Text;
            AddressLine1 = thisControl.txtDemoAddLine1.Text;
            AddressLine2 = thisControl.txtDemoAddLine2.Text;
            City = thisControl.txtDemoCity.Text;
            State = (thisControl.cboDemoState.SelectedIndex > 0) ? (state)thisControl.cboDemoState.SelectedItem : new state();
            Zip = thisControl.txtDemoZip.Text;
        }

        public override void UpdateControlContent()
        {
            thisControl.txtDemoName.Text = FName;
            thisControl.txtDemoLName.Text = LName;
            thisControl.txtDemoEmail.Text = Email;
            thisControl.txtDemoPhone.Text = Phone;
            thisControl.txtDemoAddLine1.Text = AddressLine1;
            thisControl.txtDemoAddLine2.Text = AddressLine2;
            thisControl.txtDemoCity.Text = City;
            thisControl.cboDemoState.SelectedValue = (State != null) ? State.ID : 0;
            thisControl.txtDemoZip.Text = Zip;
        }
    }
}
