using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace CID2
{
    public class AsbestosNonPermFacComplaint : AsbestosComplaint
    {
        public NonPermittedFacLocation ComplaintAddress = new NonPermittedFacLocation();
        public Owner Owner = new Owner();

        public string FacilityName = "";

        public AsbestosNonPermFacComplaint() { }

        public AsbestosNonPermFacComplaint(int id, int subid, string typetable, NonPermittedFacLocation complaintaddress, Owner owner,
            DemoContractor democontractor, AbatementContractor abatementcontractor, EntityCoordinator entitycoordinator, Landfill landfill,
            OtherOperator otheroperator, Complainant complainant, bool anonymous, string complainttext, string inspectionnotes, string othernotes,
            user receivedby, user inspector, casestatus status, DateTime datereceived, DateTime incidentdate, DateTime dateclosed, DateTime retentiondate,
            CETAtype ceta, ComplaintType type, DateTime lastmodified, receivedmethod methodreceived, DateTime investigateddate, string antsid,
            bool appendixa, bool restricted, string facilityname)
        {
            ID = id;
            SubID = subid;
            TypeTable = typetable;
            ComplaintAddress = complaintaddress;
            ComplaintAddress.SetOwner(this);
            Owner = owner;
            Owner.SetOwner(this);
            DemoContractor = democontractor;
            DemoContractor.SetOwner(this);
            AbatementContractor = abatementcontractor;
            AbatementContractor.SetOwner(this);
            EntityCoordinator = entitycoordinator;
            EntityCoordinator.SetOwner(this);
            Landfill = landfill;
            Landfill.SetOwner(this);
            OtherOperator = otheroperator;
            OtherOperator.SetOwner(this);
            ComplainantInfo = complainant;
            ComplainantInfo.SetOwner(this);
            ComplaintText = complainttext;
            InspectionNotes = inspectionnotes;
            OtherNotes = othernotes;
            ReceivedBy = receivedby;
            Inspector = inspector;
            Status = status;
            DateReceived = datereceived;
            IncidentDate = incidentdate;
            DateClosed = dateclosed;
            RetentionDate = retentiondate;
            Anonymous = anonymous;
            CETA = ceta;
            CompType = type;
            NewComplaint = false;
            LastModified = lastmodified;
            MethodReceived = methodreceived;
            InvestigatedDate = investigateddate;
            strANTSID = antsid;
            AppendixA = appendixa;
            Restricted = restricted;
            FacilityName = facilityname;
        }

        public AsbestosNonPermFacComplaint(string facilityname, NonPermittedFacLocation complaintaddress, Complainant complainant,
            string complainttext, bool anonymous)
        {
            FacilityName = facilityname;
            ComplaintAddress = complaintaddress;
            ComplainantInfo.SetOwner(this);
            ComplainantInfo = complainant;
            ComplainantInfo.SetOwner(this);
            ComplaintText = complainttext;
            Anonymous = anonymous;

            NewComplaint = true;
            MainWindow.GetSingleItem<casestatus>(out Status, 1, MainWindow.Statuses);
            ReceivedBy = MainWindow.thisUser;
            DateReceived = DateTime.Now;

            Owner.SetOwner(this);
        }

        public AsbestosNonPermFacComplaint(AsbestosComplaint comp)
        {
            ComplaintText = comp.ComplaintText;
            InspectionNotes = comp.InspectionNotes;
            OtherNotes = comp.OtherNotes;
            DateReceived = comp.DateReceived;
            IncidentDate = comp.IncidentDate;
            DateClosed = comp.DateClosed;
            RetentionDate = comp.RetentionDate;
            Anonymous = comp.Anonymous;
            Status = comp.Status;
            ComplainantInfo = comp.ComplainantInfo;
            ComplainantInfo.SetOwner(this);
            ReceivedBy = comp.ReceivedBy;
            Inspector = comp.Inspector;
            CETA = comp.CETA;
            strANTSID = comp.strANTSID;
            DemoContractor = comp.DemoContractor;
            DemoContractor.SetOwner(this);
            AbatementContractor = comp.AbatementContractor;
            AbatementContractor.SetOwner(this);
            EntityCoordinator = comp.EntityCoordinator;
            EntityCoordinator.SetOwner(this);
            Landfill = comp.Landfill;
            Landfill.SetOwner(this);
            OtherOperator = comp.OtherOperator;
            OtherOperator.SetOwner(this);
            AppendixA = comp.AppendixA;
            Restricted = comp.Restricted;

            Owner.SetOwner(this);
        }

        public override ComplaintAddress GetComplaintAddress() { return (ComplaintAddress)ComplaintAddress; }
        public override Contact GetOccupantAddress() { return (Contact)Owner; }

        public override string GetComplaintSpecificAddressInfo() { return FacilityName; }
        public override void SetComplaintSpecificAddressInfo(string specificstring) { FacilityName = specificstring; }

        public override Object MyClone()
        {
            AsbestosComplaint temp = (AsbestosComplaint)base.MyClone();
            AsbestosNonPermFacComplaint obj = new AsbestosNonPermFacComplaint(temp);

            obj.FacilityName = new string(FacilityName.ToCharArray());
            obj.ComplaintAddress.ID = ComplaintAddress.ID;
            obj.ComplaintAddress.AddressLine1 = new string(ComplaintAddress.AddressLine1.ToCharArray());
            obj.ComplaintAddress.AddressLine2 = new string(ComplaintAddress.AddressLine2.ToCharArray());
            if (ComplaintAddress.City != null && ComplaintAddress.City.ID != 0)
            {
                city c = new city();
                MainWindow.GetSingleItem<city>(out c, ComplaintAddress.City.ID, MainWindow.Cities);
                obj.ComplaintAddress.City = c;
            }
            else obj.ComplaintAddress.City = new city();
            if (ComplaintAddress.State != null && ComplaintAddress.State.ID != 0)
            {
                state s = new state();
                MainWindow.GetSingleItem<state>(out s, ComplaintAddress.State.ID, MainWindow.States);
                obj.ComplaintAddress.State = s;
            }
            else obj.ComplaintAddress.State = new state();
            if (ComplaintAddress.County != null && ComplaintAddress.County.ID != 0)
            {
                county c = new county();
                MainWindow.GetSingleItem<county>(out c, ComplaintAddress.County.ID, MainWindow.Counties);
                obj.ComplaintAddress.County = c;
            }
            else obj.ComplaintAddress.County = new county();
            obj.ComplaintAddress.Zip = new string(ComplaintAddress.Zip.ToCharArray());
            obj.ComplaintAddress.Parcel = new string(ComplaintAddress.Parcel.ToCharArray());
            obj.ComplaintAddress.Latitude = ComplaintAddress.Latitude;
            obj.ComplaintAddress.Longitude = ComplaintAddress.Longitude;
            obj.ComplaintAddress.SetOwner(this);

            if (Owner != null && Owner.ID != 0)
            {
                obj.Owner.ID = Owner.ID;
                obj.Owner.FName = new string(Owner.FName.ToCharArray());
                obj.Owner.LName = new string(Owner.LName.ToCharArray());
                obj.Owner.AddressLine1 = new string(Owner.AddressLine1.ToCharArray());
                obj.Owner.AddressLine2 = new string(Owner.AddressLine2.ToCharArray());
                obj.Owner.City = new string(Owner.City.ToCharArray());
                if (Owner.State != null && Owner.State.ID != 0)
                {
                    state s = new state();
                    MainWindow.GetSingleItem<state>(out s, Owner.State.ID, MainWindow.States);
                    obj.Owner.State = s;
                }
                obj.Owner.Zip = new string(Owner.Zip.ToCharArray());
                obj.Owner.Phone = new string(Owner.Phone.ToCharArray());
                obj.Owner.Email = new string(Owner.Email.ToCharArray());
                obj.Owner.SetOwner(this);
            }
            else obj.Owner = new Owner(obj.Owner.thisControl, this);

            return obj;
        }

        public void RestoreAsbestosNonPermFacDataMembers(AsbestosNonPermFacComplaint comp)
        {
            FacilityName = comp.FacilityName;
            ComplaintAddress.ID = comp.ComplaintAddress.ID;
            ComplaintAddress.AddressLine1 = comp.ComplaintAddress.AddressLine1;
            ComplaintAddress.AddressLine2 = comp.ComplaintAddress.AddressLine2;
            if (comp.ComplaintAddress.City != null && comp.ComplaintAddress.City.ID != 0)
            {
                city c = new city();
                MainWindow.GetSingleItem<city>(out c, comp.ComplaintAddress.City.ID, MainWindow.Cities);
                ComplaintAddress.City = c;
            }
            else ComplaintAddress.City = new city();
            if (comp.ComplaintAddress.State != null && comp.ComplaintAddress.State.ID != 0)
            {
                state s = new state();
                MainWindow.GetSingleItem<state>(out s, comp.ComplaintAddress.State.ID, MainWindow.States);
                ComplaintAddress.State = s;
            }
            else ComplaintAddress.State = new state();
            if (comp.ComplaintAddress.County != null && comp.ComplaintAddress.County.ID != 0)
            {
                county c = new county();
                MainWindow.GetSingleItem<county>(out c, comp.ComplaintAddress.County.ID, MainWindow.Counties);
                ComplaintAddress.County = c;
            }
            else ComplaintAddress.County = new county();
            ComplaintAddress.Zip = comp.ComplaintAddress.Zip;
            ComplaintAddress.Parcel = comp.ComplaintAddress.Parcel;
            ComplaintAddress.Latitude = comp.ComplaintAddress.Latitude;
            ComplaintAddress.Longitude = comp.ComplaintAddress.Longitude;
            ComplaintAddress.SetOwner(this);

            Owner.ID = comp.Owner.ID;
            Owner.FName = new string(comp.Owner.FName.ToCharArray());
            Owner.LName = new string(comp.Owner.LName.ToCharArray());
            Owner.AddressLine1 = new string(comp.Owner.AddressLine1.ToCharArray());
            Owner.AddressLine2 = new string(comp.Owner.AddressLine2.ToCharArray());
            Owner.City = new string(comp.Owner.City.ToCharArray());
            if (comp.Owner.State != null && comp.Owner.State.ID != 0)
            {
                state s = new state();
                MainWindow.GetSingleItem<state>(out s, comp.Owner.State.ID, MainWindow.States);
                Owner.State = s;
            }
            Owner.Zip = new string(comp.Owner.Zip.ToCharArray());
            Owner.Phone = new string(comp.Owner.Phone.ToCharArray());
            Owner.Email = new string(comp.Owner.Email.ToCharArray());
            Owner.SetOwner(this);

            RestoreAsbestosDataMembers((AsbestosComplaint)comp);

            return;
        }

        public List<string> CompareAsbestosNonPermFacDataMembers(AsbestosNonPermFacComplaint comp, List<string> differenceList)
        {
            List<String> returnList = new List<string>();
            returnList.AddRange(differenceList);

            if (ComplaintAddress.AddressLine1 != comp.ComplaintAddress.AddressLine1 || ComplaintAddress.AddressLine2 != comp.ComplaintAddress.AddressLine2
                || ComplaintAddress.City.ID != comp.ComplaintAddress.City.ID || ComplaintAddress.Zip != comp.ComplaintAddress.Zip ||
                ComplaintAddress.State.ID != comp.ComplaintAddress.State.ID || ComplaintAddress.Parcel != comp.ComplaintAddress.Parcel ||
                ComplaintAddress.Latitude != comp.ComplaintAddress.Latitude || ComplaintAddress.Longitude != comp.ComplaintAddress.Longitude) returnList.Add("Complaint address");
            if (FacilityName != comp.FacilityName) returnList.Add("Facility name");
            if (Owner.FName != comp.Owner.FName || Owner.LName != comp.Owner.LName) returnList.Add("Owner name");
            if (Owner.AddressLine1 != comp.Owner.AddressLine1 || Owner.AddressLine2 != comp.Owner.AddressLine2 || Owner.City != comp.Owner.City
                || Owner.State.ID != comp.Owner.State.ID || Owner.Zip != comp.Owner.Zip) returnList.Add("Owner address");
            if (Owner.Phone != comp.Owner.Phone || Owner.Email != comp.Owner.Email) returnList.Add("Owner contact info");

            return CompareAsbestosDataMembers((AsbestosComplaint)comp, returnList);
        }

        public override List<Control> GetRequiredControls()
        {
            List<Control> controls = base.GetRequiredControls();
            List<Control> loccontrols = ComplaintAddress.GetRequiredControls();
            //combine the lists
            foreach (Control control in loccontrols)
            { controls.Add(control); }

            return controls;
        }

        public override void UpdateContentFromControls()
        {
            ComplainantInfo.UpdateContentFromControls();
            ComplaintAddress.UpdateContentFromSiteControl();
            Owner.UpdateContentFromControls();
            DemoContractor.UpdateContentFromControls();
            AbatementContractor.UpdateContentFromControls();
            EntityCoordinator.UpdateContentFromControls();
            Landfill.UpdateContentFromControls();
            OtherOperator.UpdateContentFromControls();

            base.UpdateContentFromControls();
        }

        public override void UpdateControlContent()
        {
            if (ComplainantInfo != null) ComplainantInfo.UpdateControlContent();
            if (ComplaintAddress != null) ComplaintAddress.UpdateSiteControlContent();
            if (Owner != null) Owner.UpdateControlContent();
            if (DemoContractor != null) DemoContractor.UpdateControlContent();
            if (AbatementContractor != null) AbatementContractor.UpdateControlContent();
            if (EntityCoordinator != null) EntityCoordinator.UpdateControlContent();
            if (Landfill != null) Landfill.UpdateControlContent();
            if (OtherOperator != null) OtherOperator.UpdateControlContent();

            base.UpdateControlContent();
        }

        public override void SaveSubComplaint(OleDbCommand cidCMD)
        {
            string strSQL = "";

            if (SubID != 0)
            {
                strSQL = "UPDATE " + CompType.ComplaintTable + " SET pID = @pID, ANTSID = @ants, Site = @site, SiteName = @facname "
                + "WHERE pID = " + ID.ToString() + ";";

                ExecuteSubTypeSQL(out cidCMD, strSQL);
            }
            else
            {
                if (!NewComplaint)
                    MessageBox.Show("This is not a new complaint. A SubID should already exist. This is a problem");
                else
                {
                    strSQL = "INSERT INTO " + CompType.ComplaintTable + " (pID, ANTSID, Site, SiteName) VALUES (@pid, @ants, @site, @facname)";

                    ExecuteSubTypeSQL(out cidCMD, strSQL);
                    cidCMD.CommandText = "SELECT @@Identity";
                    SubID = (int)cidCMD.ExecuteScalar();

                    strSQL = "UPDATE tbl_Complaints SET Type_SubID = @subid WHERE ID = " + ID.ToString() + ";";
                    cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                    cidCMD.Parameters.AddWithValue("@subid", SubID);
                    cidCMD.ExecuteNonQuery();
                }
            }
        }

        public override void ExecuteSubTypeSQL(out OleDbCommand cidCMD, string strSQL)
        {
            cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
            cidCMD.Parameters.AddWithValue("@pid", ID);
            cidCMD.Parameters.Add("@ants", OleDbType.BSTR).Value = (strANTSID != null) ? strANTSID : "";
            cidCMD.Parameters.Add("@site", OleDbType.Integer).Value = ((ComplaintAddress.ID != 0) ? ComplaintAddress.ID : (object)DBNull.Value);
            cidCMD.Parameters.Add("@facname", OleDbType.BSTR).Value = (FacilityName != null) ? FacilityName : "";
            cidCMD.ExecuteNonQuery();
        }

        public override void SaveContacts(OleDbCommand cidCMD)
        { Owner.SaveContact(cidCMD, ID); }

        public override void SaveComplaintSpecific(OleDbCommand cidCMD)
        { ComplaintAddress.SaveComplaintSite(cidCMD); }
    }

    public class AsbestosNonPermFacList
    {
        private static List<AsbestosNonPermFacComplaint> AsbestosNonPerFacList = new List<AsbestosNonPermFacComplaint>();
        public static ListCollectionView AsbestosNonPermFacCollection = new ListCollectionView(AsbestosNonPerFacList);
    }
}
