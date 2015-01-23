using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows.Controls;

namespace CID2
{
    public class OtherLocation : ComplaintAddress
    {
        public string LocationDescription { get; set; }
        public SiteControlOther SiteControl { get; set; }

        public OtherLocation()
        { SiteControl = new SiteControlOther(this); }

        public OtherLocation(int id, string locationdescription, string addressline1, string addressline2, city city, state state,
            string zip, county county, township township, double longitude, double latitude)
        {
            ID = id;
            LocationDescription = locationdescription;
            AddressLine1 = addressline1;
            AddressLine2 = addressline2;
            City = city;
            State = state;
            Zip = zip;
            County = county;
            Township = township;
            Longitude = longitude;
            Latitude = latitude;

            SiteControl = new SiteControlOther(this);
        }

        public OtherLocation(int id, Complaint owner)
        {
            ID = id;
            LocationDescription = AddressLine1 = AddressLine2 = "";
            Latitude = Longitude = 0;
            City = new city();
            State = new state();
            County = new county();
            Township = new township();
            SiteControl = new SiteControlOther(this);

            Owner = owner;
        }

        public override void BlankAddress()
        { base.BlankAddress(); }

        public ComplaintAddress GetAddress()
        { return new ComplaintAddress(AddressLine1, AddressLine2, City, State, Zip, County, Township, Parcel, Longitude, Latitude); }

        public override void UpdateSiteControlContent()
        {
            SiteControl.txtPropDesc.Text = LocationDescription;
            if(!Owner.GetType().ToString().Contains("Facility")) SiteControl.txtPropDesc.Text = Owner.GetComplaintSpecificAddressInfo();
            SiteControl.txtPropAddLine1.Text = AddressLine1;
            SiteControl.txtPropAddLine2.Text = AddressLine2;
            SiteControl.txtPropLat.Text = (Latitude != 0) ? Latitude.ToString() : "";
            SiteControl.txtPropLon.Text = (Longitude != 0) ? Longitude.ToString() : "";
            if (Zip != null)
            {
                foreach (ZipCode z in SiteControl.cboZipBox.Items)
                { if (Zip == z.Zip) SiteControl.cboZipBox.SelectedValue = z.ID; }
            }
            SiteControl.cboPropCounty.SelectedValue = (County != null) ? County.ID : 0;
            SiteControl.cboPropCity.SelectedValue = (City != null) ? City.ID : 0;
            SiteControl.cboPropTownship.SelectedValue = (Township != null) ? Township.ID : 0;
            SiteControl.cboPropState.SelectedValue = (State != null) ? State.ID : 0;

            SiteControl.chkAppendixA.IsChecked = Owner.AppendixA;
            SiteControl.chkRestricted.IsChecked = Owner.Restricted;
        }

        public override void UpdateContentFromSiteControl()
        {
            LocationDescription = SiteControl.txtPropDesc.Text;
            AddressLine1 = SiteControl.txtPropAddLine1.Text;
            AddressLine2 = SiteControl.txtPropAddLine2.Text;
            Latitude = (SiteControl.txtPropLat.Text != "") ? Convert.ToDouble(SiteControl.txtPropLat.Text) : 0;
            Longitude = (SiteControl.txtPropLon.Text != "") ? Convert.ToDouble(SiteControl.txtPropLon.Text) : 0;

            City = (SiteControl.cboPropCity.SelectedIndex > 0) ? (city)SiteControl.cboPropCity.SelectedItem : null;
            County = (SiteControl.cboPropCounty.SelectedIndex > 0) ? (county)SiteControl.cboPropCounty.SelectedItem : null;
            Township = (SiteControl.cboPropTownship.SelectedIndex > 0) ? (township)SiteControl.cboPropTownship.SelectedItem : null;
            State = (SiteControl.cboPropState.SelectedIndex > 0) ? (state)SiteControl.cboPropState.SelectedItem : null;
            Zip = (SiteControl.cboZipBox.SelectedItem != null) ? ((ZipCode)SiteControl.cboZipBox.SelectedItem).Zip : "";

            Owner.AppendixA = (bool)SiteControl.chkAppendixA.IsChecked;
            Owner.Restricted = (bool)SiteControl.chkRestricted.IsChecked;
        }

        public override List<Control> GetRequiredControls()
        {
            List<Control> controls = new List<Control>();
            controls.Add(SiteControl.cboPropCity);

            return controls;
        }

        public void SaveComplaintSite(OleDbCommand cidCMD)
        {
            string strSQL;

            if (ID > 0)
            {
                strSQL = "UPDATE @theTable SET Name = @locdesc, Address = @add1, Address2 = @add2, City = @city, "
                + "Zip = @zip, Latitude = @lat, State = @state, Longitude = @lon, County = @county, Township = @township "
                + "WHERE ID = " + ID.ToString() + ";";

                ExecuteSiteSQL(out cidCMD, strSQL);
            }
            else
            {
                strSQL = "INSERT INTO @theTable (Name, Address, Address2, City, Zip, Latitude, State, Longitude, "
                + "County, Township) VALUES (@locdesc, @add1, @add2, @city, @zip, @lat, @state, @lon, @county, @township);";

                ExecuteSiteSQL(out cidCMD, strSQL);
                cidCMD.CommandText = "SELECT @@Identity";
                int theID = (int)cidCMD.ExecuteScalar();

                string strlabel = "Site";
                if (Owner.GetType().ToString().Contains("Facility")) strlabel = "PortableSite";

                strSQL = "UPDATE " + Owner.TypeTable + " SET Site = @facID, SiteName = @bldgdesc WHERE (ID = @subid);";
                cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                cidCMD.Parameters.AddWithValue("@facID", theID);
                cidCMD.Parameters.AddWithValue("@bldgdesc", Owner.GetComplaintSpecificAddressInfo());
                cidCMD.Parameters.AddWithValue("@subid", Owner.SubID);
                cidCMD.ExecuteNonQuery();

                ID = theID;
            }
        }

        public void ExecuteSiteSQL(out OleDbCommand cidCMD, string strSQL)
        {
            int cityid = (City != null ? City.ID : 0);
            int stateid = (State != null ? State.ID : 0);
            int countyid = (County != null ? County.ID : 0);
            int townshipid = (Township != null ? Township.ID : 0);

            string strtable = Owner.CompType.SiteTable;
            if (Owner.GetType().ToString().Contains("Facility")) strtable = "tbl_Complaint_Portable_Sites";

            strSQL = strSQL.Replace("@theTable", strtable);
            cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
            cidCMD.Parameters.AddWithValue("@locdesc", LocationDescription);
            cidCMD.Parameters.AddWithValue("@add1", AddressLine1);
            cidCMD.Parameters.AddWithValue("@add2", AddressLine2);
            cidCMD.Parameters.Add("@city", OleDbType.Integer).Value = (cityid > 0 ? cityid : (object)DBNull.Value);
            cidCMD.Parameters.AddWithValue("@zip", Zip);
            cidCMD.Parameters.AddWithValue("@lat", Latitude);
            cidCMD.Parameters.Add("@state", OleDbType.Integer).Value = (stateid > 0 ? stateid : (object)DBNull.Value);
            cidCMD.Parameters.AddWithValue("@lon", Longitude);
            cidCMD.Parameters.Add("@county", OleDbType.Integer).Value = (countyid > 0 ? countyid : (object)DBNull.Value);
            cidCMD.Parameters.Add("@township", OleDbType.Integer).Value = (townshipid > 0 ? townshipid : (object)DBNull.Value);
            cidCMD.ExecuteNonQuery();
        }
    }
}
