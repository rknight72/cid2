using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace CID2
{
    public class DustOtherComplaint : DustComplaint
    {
        public OtherLocation ComplaintAddress = new OtherLocation();

        public DustOtherComplaint() { }

        public DustOtherComplaint(int id, int subid, string typetable, OtherLocation complaintaddress, Complainant complainant, bool anonymous,
            string complainttext, string inspectionnotes, string othernotes, user receivedby, user inspector, casestatus status,
            DateTime datereceived, DateTime incidentdate, DateTime dateclosed, DateTime retentiondate, CETAtype ceta, ComplaintType type,
            DateTime lastmodified, receivedmethod methodreceived, DateTime investigateddate, bool appendixa, bool restricted)
        {
            ID = id;
            SubID = subid;
            TypeTable = typetable;
            ComplaintAddress = complaintaddress;
            ComplaintAddress.SetOwner(this);
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
            AppendixA = appendixa;
            Restricted = restricted;
        }

        public DustOtherComplaint(OtherLocation complaintaddress, Complainant complainant, string complainttext, bool anonymous)
        {
            ComplaintAddress = complaintaddress;
            ComplaintAddress.SetOwner(this);
            ComplainantInfo = complainant;
            ComplainantInfo.SetOwner(this);
            ComplaintText = complainttext;
            Anonymous = anonymous;

            NewComplaint = true;
            MainWindow.GetSingleItem<casestatus>(out Status, 1, MainWindow.Statuses);
            ReceivedBy = MainWindow.thisUser;
            DateReceived = DateTime.Now;
        }

        public DustOtherComplaint(DustComplaint comp)
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

            ComplaintAddress.SetOwner(this);
        }

        public override Object MyClone()
        {
            DustComplaint temp = (DustComplaint)base.MyClone();
            DustOtherComplaint obj = new DustOtherComplaint(temp);

            obj.ComplaintAddress.ID = ComplaintAddress.ID;
            obj.ComplaintAddress.LocationDescription = new string(ComplaintAddress.LocationDescription.ToCharArray());
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

            return obj;
        }

        public void RestoreDustOtherDataMembers(DustOtherComplaint comp)
        {
            ComplaintAddress.ID = comp.ComplaintAddress.ID;
            ComplaintAddress.LocationDescription = comp.ComplaintAddress.LocationDescription;
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

            RestoreDustDataMembers((DustComplaint)comp);

            return;
        }

        public List<string> CompareDustOtherDataMembers(DustOtherComplaint comp, List<string> differenceList)
        {
            List<String> returnList = new List<string>();
            returnList.AddRange(differenceList);

            if (ComplaintAddress.LocationDescription != comp.ComplaintAddress.LocationDescription || ComplaintAddress.AddressLine1 != comp.ComplaintAddress.AddressLine1 ||
                ComplaintAddress.AddressLine2 != comp.ComplaintAddress.AddressLine2 || ComplaintAddress.City.ID != comp.ComplaintAddress.City.ID ||
                ComplaintAddress.Zip != comp.ComplaintAddress.Zip || ComplaintAddress.State.ID != comp.ComplaintAddress.State.ID || ComplaintAddress.Parcel != comp.ComplaintAddress.Parcel ||
                ComplaintAddress.Latitude != comp.ComplaintAddress.Latitude || ComplaintAddress.Longitude != comp.ComplaintAddress.Longitude) returnList.Add("Complaint address");

            return CompareDustDataMembers((DustComplaint)comp, returnList);
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

            base.UpdateContentFromControls();
        }

        public override void UpdateControlContent()
        {
            if (ComplainantInfo != null) ComplainantInfo.UpdateControlContent();
            if (ComplaintAddress != null) ComplaintAddress.UpdateSiteControlContent();

            base.UpdateControlContent();
        }

        public override void SaveComplaintSpecific(OleDbCommand cidCMD)
        { ComplaintAddress.SaveComplaintSite(cidCMD); }
    }

    public class DustOtherList
    {
        private static List<DustOtherComplaint> DustOthList = new List<DustOtherComplaint>();
        public static ListCollectionView DustOtherCollection = new ListCollectionView(DustOthList);
    }
}
