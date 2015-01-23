using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace CID2
{
    public class Facility : DBobject
    {
        public string FacilityID { get; set; }
        public string FacName { get; set; }
        public county County { get; set; }
        public township Township { get; set; }
        public bool Portable { get; set; }
        public OperatingStatus OpStatus { get; set; }
        public permittingclassification PermitClassification { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string SortAddress { get; set; }
        public string AddressLine1 { get { return FacAddress.AddressLine1; } set { FacAddress.AddressLine1 = value; } }
        public string City { get { return FacAddress.City; } set { FacAddress.City = value; } }
        public state State { get { return FacAddress.State; } set { FacAddress.State = value; } }
        public string Zip { get { return FacAddress.Zip; } set { FacAddress.Zip = value; } }

        private Address FacAddress = new Address();
        
        public SiteControlFacility FacilityControl = null;

        public Facility()
        {
            FacilityID = FacName = "";
            Longitude = Latitude = 0;
            County = new county();
            Township = new township();
            Portable = false;
            PermitClassification = null;
        }

        public Facility(int id, string facilityid, string facname, string addressline1, string city, string zip, county county, township township,
            state state, bool portable, OperatingStatus opstatus, permittingclassification permitclassification, double longitude, double latitude)
        {
            ID = id;
            FacilityID = facilityid;
            FacName = facname;
            Township = township;
            County = county;
            Portable = portable;
            OpStatus = opstatus;
            PermitClassification = permitclassification;
            Longitude = longitude;
            Latitude = latitude;

            FacAddress = new Address(addressline1, "", city, state, zip);
            if (AddressLine1 != null & AddressLine1 != "")
            {
                string a, b;
                MainWindow.SplitAdd(AddressLine1, out a, out b);
                SortAddress = b;
            }
            else SortAddress = "";
        }

        public override string ToString()
        { return (string)FacilityID; }

        public Address GetFacilityAddress()
        { return FacAddress; }

        public void SetFacilityControl(SiteControlFacility control)
        { FacilityControl = control; }

        public void UpdateContentFromSiteControls()
        {
            FacilityControl.ControlOwner.SetComplaintSpecificAddressInfo(FacilityControl.txtFacName_nolock.Text);
            FacilityID = (FacilityControl.cboFacilityID.SelectedIndex > 0) ? ((Facility)FacilityControl.cboFacilityID.SelectedItem).FacilityID : "";
            County = (FacilityControl.cboFacCounty.SelectedIndex > 0) ? (county)FacilityControl.cboFacCounty.SelectedItem : null;
            Township = (FacilityControl.cboFacTownship.SelectedIndex > 0) ? (township)FacilityControl.cboFacTownship.SelectedItem : null;
            Latitude = (FacilityControl.txtFacLat.Text != "") ? Convert.ToDouble(FacilityControl.txtFacLat.Text) : 0;
            Longitude = (FacilityControl.txtFacLon.Text != "") ? Convert.ToDouble(FacilityControl.txtFacLon.Text) : 0;
            FacilityControl.ControlOwner.AppendixA = (bool)FacilityControl.chkAppendixA.IsChecked;
            FacilityControl.ControlOwner.Restricted = (bool)FacilityControl.chkRestricted.IsChecked;
            Portable = (bool)FacilityControl.chkPortable_nolock.IsChecked;
        }

        public void UpdateSiteControlContent()
        {
            FacilityControl.txtFacName_nolock.Text = FacilityControl.ControlOwner.GetComplaintSpecificAddressInfo();
            FacilityControl.cboFacilityID.SelectedValue = ID;
            FacilityControl.txtFacAddress_nolock.Text = AddressLine1;
            FacilityControl.txtFacCity_nolock.Text = City;
            FacilityControl.txtZipBox_nolock.Text = Zip;
            FacilityControl.cboFacCounty.SelectedValue = (County != null) ? County.ID : 0;
            FacilityControl.cboFacTownship.SelectedValue = (Township != null) ? Township.ID : 0;
            FacilityControl.txtFacLat.Text = (Latitude != 0) ? Latitude.ToString() : "";
            FacilityControl.txtFacLon.Text = (Longitude != 0) ? Longitude.ToString() : "";
            FacilityControl.chkAppendixA.IsChecked = FacilityControl.ControlOwner.AppendixA;
            FacilityControl.chkRestricted.IsChecked = FacilityControl.ControlOwner.Restricted;
            FacilityControl.chkPortable_nolock.IsChecked = Portable;
        }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            FacilityID = (dr[1] != DBNull.Value) ? dr.GetInt32(1).ToString() : "";
            FacName = (dr[2] != DBNull.Value) ? dr.GetString(2) : "";
            if (dr[3] != DBNull.Value)
            {
                AddressLine1 = dr.GetString(3);
                string a, b;
                MainWindow.SplitAdd(AddressLine1, out a, out b);
                SortAddress = b;
            }
            else AddressLine1 = SortAddress = "";
            City = (dr[4] != DBNull.Value) ? dr.GetString(4) : "";
            OperatingStatus s = null;
            if (dr[5] != DBNull.Value) MainWindow.GetSingleItem<OperatingStatus>(out s, dr.GetInt32(5), MainWindow.OperatingStatuses);
            OpStatus = s;
            permittingclassification c = null;
            if (dr[6] != DBNull.Value && (int)dr[6] != 0) MainWindow.GetSingleItem<permittingclassification>(out c, dr.GetInt32(6), MainWindow.PermittingClassifications);
            PermitClassification = c;
            Portable = (dr[7] != DBNull.Value) ? dr.GetBoolean(7) : false;
            Zip = (dr[8] != DBNull.Value) ? dr.GetInt32(8).ToString() : "";
            Latitude = (dr[9] != DBNull.Value) ? dr.GetDouble(9) : 0;
            Longitude = (dr[10] != DBNull.Value) ? dr.GetDouble(10) : 0;
            township t = null;
            if (dr[11] != DBNull.Value) MainWindow.GetSingleItem<township>(out t, dr.GetInt32(11), MainWindow.Townships);
            Township = t;
            county cc = null;
            if (dr[12] != DBNull.Value) MainWindow.GetSingleItem<county>(out cc, dr.GetInt32(12), MainWindow.Counties);
            County = cc;
            State = new state(41, "Ohio", "OH");
        }

        public override T CopyItem<T>(T item)
        {
            //Change item to the correct type of object then create a copy of it
            Facility x = item as Facility;
            Facility y = new Facility(x.ID, x.FacilityID, x.FacName, x.AddressLine1, x.City, x.Zip, x.County, x.Township, x.State, x.Portable,
                x.OpStatus, x.PermitClassification, x.Longitude, x.Latitude);

            //This is necessary because the function doesn't know in advance that T is a Facility
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            //create an object with no values
            Facility y = new Facility(0, "", "", "", "", "", new county(0, "", null, 0, 0, 0, 0), new township(0, "",false, false),
                new state(0, "", ""), false, null, null, 0, 0);

            //This is necessary because the function doesn't know in advance that T is a Facility
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public void SaveComplaintSite(OleDbCommand cidCMD, string subTable)
        {
            if (ID > 0)
            {
                string strSQL = "UPDATE @theTable SET Latitude = @lat, Longitude = @lon, Township = @township, County = @county "
                    + "WHERE ID = " + ID.ToString() + ";";

                ExecuteSiteSQL(out cidCMD, strSQL, subTable);
            }
            else MessageBox.Show("This complaint type can't be zero. This is a problem.");

            return;
        }

        public void ExecuteSiteSQL(out OleDbCommand cidCMD, string strSQL, string subTable)
        {
            int countyid, townshipid;
            countyid = (FacilityControl.thisFacility.County != null ? FacilityControl.thisFacility.County.ID : 0);
            townshipid = (FacilityControl.thisFacility.Township != null ? FacilityControl.thisFacility.Township.ID : 0);

            strSQL = strSQL.Replace("@theTable", "tbl_Facilities");
            cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
            cidCMD.Parameters.AddWithValue("@lat", FacilityControl.thisFacility.Latitude);
            cidCMD.Parameters.AddWithValue("@lon", FacilityControl.thisFacility.Longitude);
            cidCMD.Parameters.Add("@county", OleDbType.Integer).Value = (countyid > 0 ? countyid : (object)DBNull.Value);
            cidCMD.Parameters.Add("@township", OleDbType.Integer).Value = (townshipid > 0 ? townshipid : (object)DBNull.Value);
            cidCMD.ExecuteNonQuery();
        }

        public List<Control> GetRequiredControls()
        {
            List<Control> controls = new List<Control>();
            controls.Add(FacilityControl.cboFacilityID);

            return controls;
        }
    }
}