using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows.Controls;

namespace CID2
{
    public class ResidentialLocation : ComplaintAddress
    {
        public string PlaceID { get; set; }
        public SiteControlResidential SiteControl { get; set; }

        public ResidentialLocation()
        {
            PlaceID = "";
            SiteControl = new SiteControlResidential(this);
        }

        public ResidentialLocation(int id, string placeid, string addressline1, string addressline2, city city,
            state state, string zip, county county, township township, string parcel, double longitude, double latitude)
        {
            ID = id;
            PlaceID = placeid;
            AddressLine1 = addressline1;
            AddressLine2 = addressline2;
            City = city;
            State = state;
            Zip = zip;
            County = county;
            Township = township;
            Parcel = parcel;
            Longitude = longitude;
            Latitude = latitude;

            SiteControl = new SiteControlResidential(this);
        }

        public override void BlankAddress()
        {
            base.BlankAddress();

            PlaceID = "";
        }

        public ComplaintAddress GetAddress()
        { return new ComplaintAddress(AddressLine1, AddressLine2, City, State, Zip, County, Township, Parcel, Longitude, Latitude); }

        public override void LoadNewAddress(int id)
        {
            MainWindow.OpenMainDBConnection();

            string strSQL = "SELECT * FROM tbl_Complaint_NonPermFac_Sites WHERE ID = " + id.ToString();

            OleDbCommand cidSQL = new OleDbCommand(strSQL, MainWindow.cidDB);
            OleDbDataReader cidReader = cidSQL.ExecuteReader();


            if (cidReader.Read())
            {
                ID = id;
                PlaceID = (cidReader[1] != DBNull.Value) ? cidReader.GetString(1) : "";
                AddressLine1 = (cidReader[2] != DBNull.Value) ? cidReader.GetString(2) : "";
                AddressLine2 = (cidReader[3] != DBNull.Value) ? cidReader.GetString(3) : "";
                Parcel = (cidReader[4] != DBNull.Value) ? cidReader.GetString(4) : "";
                city c = new city();
                if (cidReader[5] != DBNull.Value) MainWindow.GetSingleItem<city>(out c, cidReader.GetInt32(5), MainWindow.Cities);
                City = c;
                Zip = (cidReader[6] != DBNull.Value) ? cidReader.GetString(6) : "";
                Latitude = (cidReader[7] != DBNull.Value) ? cidReader.GetDouble(7) : 0;
                state s = new state();
                if (cidReader[8] != DBNull.Value) MainWindow.GetSingleItem<state>(out s, cidReader.GetInt32(8), MainWindow.States);
                State = s;
                Longitude = (cidReader[9] != DBNull.Value) ? cidReader.GetDouble(9) : 0;
                county acounty = new county();
                if (cidReader[10] != DBNull.Value) MainWindow.GetSingleItem<county>(out acounty, cidReader.GetInt32(10), MainWindow.Counties);
                County = acounty;
                township t = new township();
                if (cidReader[11] != DBNull.Value) MainWindow.GetSingleItem<township>(out t, cidReader.GetInt32(11), MainWindow.Townships);
                Township = t;
                if (Owner != null) Owner.SetComplaintSpecificAddressInfo((cidReader[12] != DBNull.Value) ? cidReader.GetString(12) : "");
            }

            cidReader.Close();
            MainWindow.CloseMainDBConnection();

            if (Owner != null) UpdateSiteControlContent();
        }

        public override void UpdateSiteControlContent()
        {
            SiteControl.txtPropDesc.Text = Owner.GetComplaintSpecificAddressInfo();
            SiteControl.txtPropAddLine1.Text = AddressLine1;
            SiteControl.txtPropAddLine2.Text = AddressLine2;
            SiteControl.txtPropLat.Text = (Latitude != 0) ? Latitude.ToString() : "";
            SiteControl.txtPropLon.Text = (Longitude != 0) ? Longitude.ToString() : "";
            SiteControl.txtPropParcel.Text = Parcel;
            SiteControl.txtPropPlaceID.Text = PlaceID;
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
            if (Owner != null) Owner.SetComplaintSpecificAddressInfo(SiteControl.txtPropDesc.Text);
            AddressLine1 = SiteControl.txtPropAddLine1.Text;
            AddressLine2 = SiteControl.txtPropAddLine2.Text;
            Latitude = (SiteControl.txtPropLat.Text != "") ? Convert.ToDouble(SiteControl.txtPropLat.Text) : 0;
            Longitude = (SiteControl.txtPropLon.Text != "") ? Convert.ToDouble(SiteControl.txtPropLon.Text) : 0;
            Parcel = SiteControl.txtPropParcel.Text;
            PlaceID = SiteControl.txtPropPlaceID.Text;

            City = (SiteControl.cboPropCity.SelectedIndex > 0) ? (city)SiteControl.cboPropCity.SelectedItem : null;
            County = (SiteControl.cboPropCounty.SelectedIndex > 0) ? (county)SiteControl.cboPropCounty.SelectedItem : null;
            Township = (SiteControl.cboPropTownship.SelectedIndex > 0) ? (township)SiteControl.cboPropTownship.SelectedItem : null;
            State = (SiteControl.cboPropState.SelectedIndex > 0) ? (state)SiteControl.cboPropState.SelectedItem : null;
            Zip = (SiteControl.cboZipBox.SelectedItem != null) ? ((ZipCode)SiteControl.cboZipBox.SelectedItem).Zip : "";

            if (Owner != null) Owner.AppendixA = (bool)SiteControl.chkAppendixA.IsChecked;
            if (Owner != null) Owner.Restricted = (bool)SiteControl.chkRestricted.IsChecked;
        }

        public override List<Control> GetRequiredControls()
        {
            List<Control> controls = new List<Control>();
            controls.Add(SiteControl.txtPropAddLine1);
            controls.Add(SiteControl.cboPropCity);

            return controls;
        }

        public void SaveComplaintSite(OleDbCommand cidCMD)
        {
            string strSQL;

            if (ID > 0)
            {
                strSQL = "UPDATE @theTable SET Facility_ID = @pid, Address = @add1, Address2 = @add2, Parcel = @parcel, City = @city, "
                + "Zip = @zip, Latitude = @lat, State = @state, Longitude = @lon, County = @county, Township = @township , Name = @bldgdesc"
                + "WHERE ID = " + ID.ToString() + ";";

                ExecuteSiteSQL(out cidCMD, strSQL);
            }
            else
            {
                strSQL = "INSERT INTO @theTable (Facility_ID, Address, Address2, Parcel, City, Zip, Latitude, State, Longitude, County, Township, Name) "
                + "VALUES (@pid, @add1, @add2, @parcel, @city, @zip, @lat, @state, @lon, @county, @township, @bldgdesc);";

                ExecuteSiteSQL(out cidCMD, strSQL);
                cidCMD.CommandText = "SELECT @@Identity";
                int theID = (int)cidCMD.ExecuteScalar();

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

            strSQL = strSQL.Replace("@theTable", Owner.CompType.SiteTable);
            cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
            cidCMD.Parameters.AddWithValue("@pid", PlaceID);
            cidCMD.Parameters.AddWithValue("@add1", AddressLine1);
            cidCMD.Parameters.AddWithValue("@add2", AddressLine2);
            cidCMD.Parameters.AddWithValue("@parcel", Parcel);
            cidCMD.Parameters.Add("@city", OleDbType.Integer).Value = (cityid > 0 ? cityid : (object)DBNull.Value);
            cidCMD.Parameters.AddWithValue("@zip", Zip);
            cidCMD.Parameters.AddWithValue("@lat", Latitude);
            cidCMD.Parameters.Add("@state", OleDbType.Integer).Value = (stateid > 0 ? stateid : (object)DBNull.Value);
            cidCMD.Parameters.AddWithValue("@lon", Longitude);
            cidCMD.Parameters.Add("@county", OleDbType.Integer).Value = (countyid > 0 ? countyid : (object)DBNull.Value);
            cidCMD.Parameters.Add("@township", OleDbType.Integer).Value = (townshipid > 0 ? townshipid : (object)DBNull.Value);
            cidCMD.Parameters.AddWithValue("@bldgdesc", Owner.GetComplaintSpecificAddressInfo());
            cidCMD.ExecuteNonQuery();
        }
    }
}
