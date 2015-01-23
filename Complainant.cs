using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows.Controls;

namespace CID2
{
    public class Complainant : Contact
    {
        public ComplainantControl thisControl { get; set; }

        public Complainant()
        {
            ID = 0;
            FName = "";
            LName = "";
            Email = "";
            Phone = "";
            strContactTable = "tbl_Complainants";
            strContactLabel = "Complainant";
            strParentTable = "tbl_Complaints";

            thisControl = new ComplainantControl(this);
        }

        public Complainant(Complaint owner)
        { Owner = owner; }

        public Complainant(ComplainantControl control, Complaint owner)
        {
            thisControl = control;
            Owner = owner;

            strContactTable = "tbl_Complainants";
            strContactLabel = "Complainant";
            strParentTable = "tbl_Complaints";
        }

        public Complainant(int id, string firstname, string lastname, string addressline1, string addressline2,
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
            strContactTable = "tbl_Complainants";
            strContactLabel = "Complainant";
            strParentTable = "tbl_Complaints";

            thisControl = new ComplainantControl(this);
        }

        public Complainant(Contact contact)
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
            strContactTable = "tbl_Complainants";
            strContactLabel = "Complainant";
            strParentTable = "tbl_Complaints";

            thisControl = new ComplainantControl(this);
        }

        public override void UpdateContentFromControls()
        {
            FName = thisControl.txtCompFName.Text;
            LName = thisControl.txtCompLName.Text;
            Email = thisControl.txtCompEmail.Text;
            Phone = thisControl.txtCompPhone.Text;
            AddressLine1 = thisControl.txtCompAddLine1.Text;
            AddressLine2 = thisControl.txtCompAddLine2.Text;
            City = thisControl.txtCompCity.Text;
            State = (thisControl.cboCompState.SelectedIndex > 0) ? (state)thisControl.cboCompState.SelectedItem : new state();
            Zip = thisControl.txtCompZip.Text;
            Owner.Anonymous = (bool)thisControl.chkCompAnon.IsChecked;
            if (Owner.CompType.Category.ID == 5)
            {
                ((OBComplaint)Owner).FDGenerated = (bool)thisControl.chkCompFD.IsChecked;
                ((OBComplaint)Owner).FDinfo = (thisControl.cboFireDepts.SelectedIndex > 0) ? (FireDepartment)thisControl.cboFireDepts.SelectedItem : new FireDepartment();
                if (thisControl.cboFireDepts.SelectedIndex > 0) SameAsContact(((OBComplaint)Owner).FDinfo.FDinfo);
            }
        }

        public override void UpdateControlContent()
        {
            thisControl.txtCompFName.Text = FName;
            thisControl.txtCompLName.Text = LName;
            thisControl.txtCompEmail.Text = Email;
            thisControl.txtCompPhone.Text = Phone;
            thisControl.txtCompAddLine1.Text = AddressLine1;
            thisControl.txtCompAddLine2.Text = AddressLine2;
            thisControl.txtCompCity.Text = City;
            thisControl.cboCompState.SelectedValue = (State != null) ? State.ID : 0;
            thisControl.txtCompZip.Text = Zip;
            thisControl.chkCompAnon.IsChecked = Owner.Anonymous;
            if (Owner.CompType.Category.ID == 5)
            {
                thisControl.chkCompFD.IsChecked = ((OBComplaint)Owner).FDGenerated;
                thisControl.cboFireDepts.SelectedValue = (((OBComplaint)Owner).FDinfo != null) ? ((OBComplaint)Owner).FDinfo.ID : 0;
                if (thisControl.cboFireDepts.SelectedIndex > 0) SameAsContact(((OBComplaint)Owner).FDinfo.FDinfo);
            }            
        }

        public override List<Control> GetRequiredControls()
        {
            List<Control> list = new List<Control>();
            if (!Owner.Anonymous) list.Add(thisControl.txtCompFName);

            return list;
        }

        public override void SaveContact(OleDbCommand cidCMD, int parentID)
        {
            if (!Owner.Anonymous && Owner.CompType.Category.ID != 5) base.SaveContact(cidCMD, parentID);
            else if (Owner.CompType.Category.ID == 5 && !Owner.Anonymous && !((OBComplaint)Owner).FDGenerated) base.SaveContact(cidCMD, parentID);
        }
    }
}
