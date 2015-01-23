using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CID2
{
    public class Landfill : Contact
    {
        public LandfillControl thisControl { get; set; }

        public Landfill()
        {
            ID = 0;
            FName = LName = Email = Phone = City = Zip = "";
            State = new state();

            thisControl = new LandfillControl(this);
        }

        public Landfill(Complaint owner)
        { Owner = owner; }

        public Landfill(LandfillControl control, Complaint owner)
        {
            thisControl = control;
            Owner = owner;
        }

        public Landfill(int id, string firstname, string lastname, string addressline1, string addressline2,
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

            thisControl = new LandfillControl(this);
        }

        public Landfill(Contact contact)
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

            thisControl = new LandfillControl(this);
        }

        public override void UpdateContentFromControls()
        {
            FName = thisControl.txtLandfillName.Text;
            LName = thisControl.txtLandfillLName.Text;
            Email = thisControl.txtLandfillEmail.Text;
            Phone = thisControl.txtLandfillPhone.Text;
            AddressLine1 = thisControl.txtLandfillAddLine1.Text;
            AddressLine2 = thisControl.txtLandfillAddLine2.Text;
            City = thisControl.txtLandfillCity.Text;
            State = (thisControl.cboLandfillState.SelectedIndex > 0) ? (state)thisControl.cboLandfillState.SelectedItem : new state();
            Zip = thisControl.txtLandfillZip.Text;
        }

        public override void UpdateControlContent()
        {
            thisControl.txtLandfillName.Text = FName;
            thisControl.txtLandfillLName.Text = LName;
            thisControl.txtLandfillEmail.Text = Email;
            thisControl.txtLandfillPhone.Text = Phone;
            thisControl.txtLandfillAddLine1.Text = AddressLine1;
            thisControl.txtLandfillAddLine2.Text = AddressLine2;
            thisControl.txtLandfillCity.Text = City;
            thisControl.cboLandfillState.SelectedValue = (State != null) ? State.ID : 0;
            thisControl.txtLandfillZip.Text = Zip;
        }
    }
}
