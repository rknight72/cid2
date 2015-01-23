using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.Data.OleDb;
using System;
using System.Windows;

namespace CID2
{
    public class Contact : Address
    {
        public int ID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string strContactTable { get; set; }
        public string strContactLabel { get; set; }
        public string strParentTable { get; set; }

        public Contact()
        {
            ID = 0;
            FName = LName = Email = Phone = City = Zip = strContactTable = strContactLabel = strParentTable = "";
            State = new state();
        }

        public Contact(int id, string firstname, string lastname, string addressline1, string addressline2, string city,
            state state, string zip, string email, string phone, string table, string tablelabel, string parenttable)
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
            strContactTable = table;
            strContactLabel = tablelabel;
            strParentTable = parenttable;
        }

        public virtual void UpdateContentFromControls() { }
        public virtual void UpdateControlContent() { }

        public bool IsMinimum()
        {
            if ((FName != "") || (AddressLine1 != "")  || (Phone != "") || (Email != "")) return true;
            else return false;
        }

        public override string ToString()
        { return LName + ", " + FName; }

        public void SameAsContact(Contact contact)
        {
            FName = contact.FName;
            LName = contact.LName;
            Email = contact.Email;
            Phone = contact.Phone;
            AddressLine1 = contact.AddressLine1;
            AddressLine2 = contact.AddressLine2;
            City = contact.City;
            State = contact.State;
            Zip = contact.Zip;
        }

        public void SameAsSiteAdress(ComplaintAddress address)
        {
            AddressLine1 = address.AddressLine1;
            AddressLine2 = address.AddressLine2;
            City = address.City.ToString();
            Zip = address.Zip;
            State = address.State;   
        }

        public virtual void SaveContact(OleDbCommand cidCMD, int parentID)
        {
            string strSQL = "";

            if (ID > 0)
            {
                strSQL = "UPDATE @theTable SET Last_Name = @lname, First_Name = @fname, Address = @add1, Address2 = @add2, City = @city,"
                + " State = @state, Zip = @zip, Email = @email, Phone = @phone WHERE ID = " + ID.ToString() + ";";

                ExecuteContactSQL(out cidCMD, strSQL, parentID);
            }
            else
            {
                strSQL = "INSERT INTO @theTable (Last_Name, First_Name, Address, Address2, City, State, Zip, Email, Phone, pID) VALUES ("
                + "@lname, @fname, @add1, @add2, @city, @state, @zip, @email, @phone, @pid);";

                if (ExecuteContactSQL(out cidCMD, strSQL, parentID))
                {
                    cidCMD.CommandText = "SELECT @@Identity";
                    int theID = (int)cidCMD.ExecuteScalar();

                    strSQL = "UPDATE " + strParentTable + " SET " + strContactLabel + " = @compID WHERE (ID = @thisID);";
                    cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                    cidCMD.Parameters.AddWithValue("@compID", theID);
                    cidCMD.Parameters.AddWithValue("@thisID", parentID);
                    cidCMD.ExecuteNonQuery();

                    ID = theID;
                }
            }
        }

        public bool ExecuteContactSQL(out OleDbCommand cidCMD, string strSQL, int parentID)
        {
            if (IsMinimum())
            {
                strSQL = strSQL.Replace("@theTable", strContactTable);
                cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                cidCMD.Parameters.AddWithValue("@lname", LName);
                cidCMD.Parameters.AddWithValue("@fname", FName);
                cidCMD.Parameters.AddWithValue("@add1", AddressLine1);
                cidCMD.Parameters.AddWithValue("@add2", AddressLine2);
                cidCMD.Parameters.AddWithValue("@city", City);
                cidCMD.Parameters.Add("@state", OleDbType.Integer).Value = (State.ID > 0 ? State.ID : (object)DBNull.Value);
                cidCMD.Parameters.AddWithValue("@zip", Zip);
                cidCMD.Parameters.AddWithValue("@email", Email);
                cidCMD.Parameters.AddWithValue("@phone", Phone);
                cidCMD.Parameters.AddWithValue("@pid", parentID);
                cidCMD.ExecuteNonQuery();
                return true;
            }
            else
            {
                cidCMD = new OleDbCommand();
                return false;
            }
        }
    }
}
