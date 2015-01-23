using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.Data.OleDb;
using System;
using System.Windows;


namespace CID2
{
    public class FacilityContact : DBobject
    {
        public string FacID { get; set; }
        public String ContactType { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string strParentTable { get; set; }
        public string strContactLabel { get; set; }

        public FacilityContactControl thisControl = null;

        public FacilityContact()
        {
            FacID = FName = LName = Email = Phone = "";
            ContactType = "";
        }

        public FacilityContact(int id, string facilityid, string contacttype, string firstname, string lastname,
            string phone, string email, string parenttable, string contactlabel)
        {
            ID = id;
            FacID = facilityid;
            ContactType = contacttype;
            FName = firstname;
            LName = lastname;
            Email = email;
            Phone = phone;
            strParentTable = parenttable;
            strContactLabel = contactlabel;
        }

        public void SetContactControl(FacilityContactControl control, string parenttable, string contactlabel)
        {
            thisControl = control;
            strParentTable = parenttable;
            strContactLabel = contactlabel;
        }

        public bool IsMinimum()
        { return true; }

        public void EnableControls(bool enabled)
        { }

        public void UpdateContentFromControls()
        {
            FName = thisControl.txtFacConFName_nolock.Text;
            LName = thisControl.txtFacConLName_nolock.Text;
            Email = thisControl.txtFacConEmail_nolock.Text;
            Phone = thisControl.txtFacConPhone_nolock.Text;
        }

        public void UpdateControlContent()
        {
            thisControl.txtFacConFName_nolock.Text = FName;
            thisControl.txtFacConLName_nolock.Text = LName;
            thisControl.txtFacConEmail_nolock.Text = Email;
            thisControl.txtFacConPhone_nolock.Text = Phone;
            thisControl.cboFacContactTypes.SelectedValue = 0;
        }

        public override string ToString()
        { return LName + ", " + FName; }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            FacID = (dr[1] != DBNull.Value) ? dr.GetInt32(1).ToString() : "";
            ContactType = (dr[2] != DBNull.Value) ? dr.GetString(2) : "";
            FName = (dr[3] != DBNull.Value) ? dr.GetString(3) : "";
            LName = (dr[4] != DBNull.Value) ? dr.GetString(4) : "";
            Phone = (dr[5] != DBNull.Value) ? dr.GetDouble(5).ToString() : "";
            Email = (dr[6] != DBNull.Value) ? dr.GetString(6) : "";
            strParentTable = "";
            strContactLabel = "";
        }

        public override T CopyItem<T>(T item)
        {
            //Change item to the correct type of object then create a copy of it
            FacilityContact x = item as FacilityContact;
            FacilityContact y = new FacilityContact(x.ID, x.FacID, x.ContactType, x.FName, x.LName, x.Phone, x.Email, x.strParentTable, x.strContactLabel);

            //This is necessary because the function doesn't know in advance that T is a FacilityContact
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            //create an object with no values
            FacilityContact y = new FacilityContact(0, "", "", "", "", "", "", "", "");

            //This is necessary because the function doesn't know in advance that T is a FacilityContact
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public virtual void SaveContact(OleDbCommand cidCMD, int parentID)
        {
            string strSQL = "";

            if (strParentTable != "" && strContactLabel != "")
            {
                if (ID > 0)
                {
                    strSQL = "UPDATE tbl_Complaint_Facility_Contacts SET First_Name = @fname, Last_Name = @lname, "
                    + "Phone = @phone, Email = @email WHERE ID = " + ID.ToString() + ";";

                    ExecuteContactSQL(out cidCMD, strSQL);
                }
                else
                {
                    strSQL = "INSERT INTO tbl_Complaint_Facility_Contacts (First_Name, Last_Name, Phone, Email) VALUES ("
                    + "@fname, @lname, @phone, @email);";

                    if (ExecuteContactSQL(out cidCMD, strSQL))
                    {
                        cidCMD.CommandText = "SELECT @@Identity";
                        int theID = (int)cidCMD.ExecuteScalar();

                        strSQL = "UPDATE " + strParentTable + " SET " + strContactLabel + " = @compID WHERE ID = " + thisControl.ControlOwner.SubID.ToString() + ";";
                        cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                        cidCMD.Parameters.AddWithValue("@compID", theID);
                        cidCMD.Parameters.AddWithValue("@thisID", parentID);
                        cidCMD.ExecuteNonQuery();

                        ID = theID;
                    }
                }
            }
        }

        public bool ExecuteContactSQL(out OleDbCommand cidCMD, string strSQL)
        {
            cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
            cidCMD.Parameters.AddWithValue("@fname", FName);
            cidCMD.Parameters.AddWithValue("@lname", LName);
            cidCMD.Parameters.AddWithValue("@phone", Phone);
            cidCMD.Parameters.AddWithValue("@email", Email);
            cidCMD.ExecuteNonQuery();
            return true;
        }
    }
}

