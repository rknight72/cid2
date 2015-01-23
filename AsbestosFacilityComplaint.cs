﻿using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace CID2
{
    public class AsbestosFacilityComplaint : AsbestosComplaint
    {
        public SiteControlFacility FacilityControl { get; set; }
        public FacilityContactControl ContactControl { get; set; }
        public OtherLocation PortableLocation { get; set; }
        public string FacilityName { get; set; }

        public AsbestosFacilityComplaint() { }

        public AsbestosFacilityComplaint(int id, int subid, string typetable, Facility fac, FacilityContact faccontact, DemoContractor democontractor,
            AbatementContractor abatementcontractor, EntityCoordinator entitycoordinator, Landfill landfill, OtherOperator otheroperator,
            Complainant complainant, bool anonymous, string complainttext, string inspectionnotes, string othernotes, user receivedby,
            user inspector, casestatus status, DateTime datereceived, DateTime incidentdate, DateTime dateclosed, DateTime retentiondate,
            CETAtype ceta, ComplaintType type, DateTime lastmodified, receivedmethod methodreceived, DateTime investigateddate, string antsid,
            bool appendixa, bool restricted, string facilityname, OtherLocation portablelocation)
        {
            ID = id;
            SubID = subid;
            TypeTable = typetable;
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


            PortableLocation = portablelocation;
            PortableLocation.SetOwner(this);

            FacilityControl = new SiteControlFacility((Complaint)this, fac);
            ContactControl = new FacilityContactControl((Complaint)this, faccontact);
            SetContactFilter();
        }

        public AsbestosFacilityComplaint(Facility fac, Complainant complainant, string complainttext, bool anonymous)
        {
            ComplainantInfo = complainant;
            ComplainantInfo.SetOwner(this);
            ComplaintText = complainttext;
            Anonymous = anonymous;

            NewComplaint = true;
            MainWindow.GetSingleItem<casestatus>(out Status, 1, MainWindow.Statuses);
            ReceivedBy = MainWindow.thisUser;
            DateReceived = DateTime.Now;

            PortableLocation = new OtherLocation();

            FacilityControl = new SiteControlFacility((Complaint)this, fac);
            ContactControl = new FacilityContactControl((Complaint)this, new FacilityContact());
            SetContactFilter();
        }

        public AsbestosFacilityComplaint(AsbestosComplaint comp)
        {
            ID = comp.ID;
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

            PortableLocation = new OtherLocation();

            FacilityControl = new SiteControlFacility((Complaint)this, new Facility());
            ContactControl = new FacilityContactControl((Complaint)this, new FacilityContact());
            SetContactFilter();
        }

        public override string GetComplaintSpecificAddressInfo() { return FacilityName; }
        public override void SetComplaintSpecificAddressInfo(string specificstring) { FacilityName = specificstring; }

        public override Object MyClone()
        {
            AsbestosComplaint temp = (AsbestosComplaint)base.MyClone();
            AsbestosFacilityComplaint obj = new AsbestosFacilityComplaint(temp);

            if (FacilityControl != null && FacilityControl.thisFacility.ID != 0)
            {
                Facility f = new Facility();
                MainWindow.GetSingleItem<Facility>(out f, FacilityControl.thisFacility.ID, MainWindow.Facilities);
                obj.FacilityControl = FacilityControl;
            }
            else obj.FacilityControl = new SiteControlFacility(this, new Facility());

            if (ContactControl != null && ContactControl.thisContact.ID != 0)
            { obj.ContactControl = new FacilityContactControl((Complaint)this, ContactControl.thisContact.CopyItem<FacilityContact>(ContactControl.thisContact)); }
            else obj.ContactControl = new FacilityContactControl((Complaint)this, new FacilityContact());

            if (PortableLocation != null) obj.PortableLocation = PortableLocation;

            return obj;
        }

        public void RestoreAsbestosFacDataMembers(AsbestosFacilityComplaint comp)
        {
            FacilityControl.thisFacility = comp.FacilityControl.thisFacility;
            ContactControl.UpdateContact(comp.ContactControl.thisContact);

            PortableLocation = comp.PortableLocation;
            if (PortableLocation != null) PortableLocation.SetOwner(this);

            RestoreAsbestosDataMembers((AsbestosComplaint)comp);

            return;
        }

        public List<string> CompareAsbestosFacDataMembers(AsbestosFacilityComplaint comp, List<string> differenceList)
        {
            List<String> returnList = new List<string>();
            returnList.AddRange(differenceList);

            if (FacilityControl.thisFacility.ID != comp.FacilityControl.thisFacility.ID) returnList.Add("Facility");
            if (ContactControl.thisContact.ID != comp.ContactControl.thisContact.ID) returnList.Add("Facility contact");

            return CompareAsbestosDataMembers((AsbestosComplaint)comp, returnList);
        }

        public override void UpdateContentFromControls()
        {
            ComplainantInfo.UpdateContentFromControls();
            FacilityControl.thisFacility.UpdateContentFromSiteControls();
            ContactControl.thisContact.UpdateContentFromControls();
            DemoContractor.UpdateContentFromControls();
            AbatementContractor.UpdateContentFromControls();
            EntityCoordinator.UpdateContentFromControls();
            Landfill.UpdateContentFromControls();
            OtherOperator.UpdateContentFromControls();
            PortableLocation.UpdateContentFromSiteControl();

            base.UpdateContentFromControls();
        }

        public override void UpdateControlContent()
        {
            if (ComplainantInfo != null) ComplainantInfo.UpdateControlContent();
            FacilityControl.thisFacility.UpdateSiteControlContent();
            if (ContactControl != null) ContactControl.thisContact.UpdateControlContent();
            if (DemoContractor != null) DemoContractor.UpdateControlContent();
            if (AbatementContractor != null) AbatementContractor.UpdateControlContent();
            if (EntityCoordinator != null) EntityCoordinator.UpdateControlContent();
            if (Landfill != null) Landfill.UpdateControlContent();
            if (OtherOperator != null) OtherOperator.UpdateControlContent();
            PortableLocation.UpdateSiteControlContent();

            base.UpdateControlContent();
        }

        public override void SetContactFilter()
        { if (FacilityControl.thisFacility != null) ContactControl.SetContactFilter(FacilityControl.thisFacility.FacilityID); }

        public override void SaveSubComplaint(OleDbCommand cidCMD)
        {
            string strSQL = "";

            if (SubID != 0)
            {
                strSQL = "UPDATE " + CompType.ComplaintTable + " SET Site = @fid, PrimaryContact = @cid, "
                + "ANTSID = @ants, SiteName = @facname, PortableSite = @portable, pID = @pID WHERE pID = " + ID.ToString() + ";";


                ExecuteSubTypeSQL(out cidCMD, strSQL);
            }
            else
            {
                if (!NewComplaint)
                    MessageBox.Show("This is not a new complaint. A SubID should already exist. This is a problem");
                else
                {
                    strSQL = "INSERT INTO " + CompType.ComplaintTable + " (Site, PrimaryContact, ANTSID, SiteName, PortableSite, pID) "
                    + "VALUES (@fid, @cid, @ants, @facname, @portable, @pid)";

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
            cidCMD.Parameters.AddWithValue("@fid", FacilityControl.thisFacility.ID);
            cidCMD.Parameters.Add("@cid", OleDbType.Integer).Value = ((ContactControl != null && ContactControl.thisContact.ID > 0) ? ContactControl.thisContact.ID : (object)DBNull.Value);
            cidCMD.Parameters.Add("@ants", OleDbType.BSTR).Value = (strANTSID != null) ? strANTSID : "";
            cidCMD.Parameters.Add("@facname", OleDbType.BSTR).Value = (FacilityControl.txtFacName_nolock != null) ? FacilityControl.txtFacName_nolock.Text : "";
            cidCMD.Parameters.Add("@portable", OleDbType.Integer).Value = (FacilityControl.thisFacility.Portable && PortableLocation.ID > 0) ? PortableLocation.ID : (object)DBNull.Value;
            cidCMD.Parameters.AddWithValue("@pid", ID);
            cidCMD.ExecuteNonQuery();
        }

        public override void SaveComplaintSpecific(OleDbCommand cidCMD)
        {
            FacilityControl.thisFacility.SaveComplaintSite(cidCMD, CompType.SiteTable);
            if (FacilityControl.thisFacility.Portable) PortableLocation.SaveComplaintSite(cidCMD);
        }
    }

    public class AsbestosFacilityList
    {
        private static List<AsbestosFacilityComplaint> AsbestosFacList = new List<AsbestosFacilityComplaint>();
        public static ListCollectionView AsbestosFacilityCollection = new ListCollectionView(AsbestosFacList);
    }
}
