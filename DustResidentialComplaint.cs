using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace CID2
{
    public class DustResidentialComplaint : DustComplaint
    {
        public ResidentialLocation ComplaintAddress = new ResidentialLocation();
        public Occupant Occupant = new Occupant();
        public Owner Owner = new Owner();

        public string BuildingDescription = "";
        public TextBox BuildingDescBox { get; set; }

        public DustResidentialComplaint() { }

        public DustResidentialComplaint(int id, int subid, string typetable, ResidentialLocation complaintaddress, Occupant occupant,
            Owner owner, Complainant complainant, bool anonymous, string complainttext, string inspectionnotes, string othernotes,
            user receivedby, user inspector, casestatus status, DateTime datereceived, DateTime incidentdate, DateTime dateclosed,
            DateTime retentiondate, CETAtype ceta, ComplaintType type, DateTime lastmodified, receivedmethod methodreceived,
            DateTime investigateddate, bool appendixa, bool restricted, string buildingdescription)
        {
            ID = id;
            SubID = subid;
            TypeTable = typetable;
            ComplaintAddress = complaintaddress;
            ComplaintAddress.SetOwner(this);
            Occupant = occupant;
            Occupant.SetOwner(this);
            Owner = owner;
            Owner.SetOwner(this);
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
            BuildingDescription = buildingdescription;
        }

        public DustResidentialComplaint(string buildingdescription, ResidentialLocation complaintaddress,
            Complainant complainant, string complainttext, bool anonymous)
        {
            BuildingDescription = buildingdescription;
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

            Occupant.SetOwner(this);
            Owner.SetOwner(this);
        }

        public DustResidentialComplaint(DustComplaint comp)
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
            Occupant.SetOwner(this);
            Owner.SetOwner(this);
        }

        public override ComplaintAddress GetComplaintAddress() { return (ComplaintAddress)ComplaintAddress; }
        public override Contact GetOccupantAddress() { return (Contact)Occupant; }

        public override string GetComplaintSpecificAddressInfo() { return BuildingDescription; }
        public override void SetComplaintSpecificAddressInfo(string specificstring) { BuildingDescription = specificstring; }

        public override Object MyClone()
        {
            DustComplaint temp = (DustComplaint)base.MyClone();
            DustResidentialComplaint obj = new DustResidentialComplaint(temp);

            obj.ComplaintAddress.ID = ComplaintAddress.ID;
            obj.BuildingDescription = new string(BuildingDescription.ToCharArray());
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

            if (Occupant != null && Occupant.ID != 0)
            {
                obj.Occupant.ID = Occupant.ID;
                obj.Occupant.FName = new string(Occupant.FName.ToCharArray());
                obj.Occupant.LName = new string(Occupant.LName.ToCharArray());
                obj.Occupant.AddressLine1 = new string(Occupant.AddressLine1.ToCharArray());
                obj.Occupant.AddressLine2 = new string(Occupant.AddressLine2.ToCharArray());
                obj.Occupant.City = new string(Occupant.City.ToCharArray());
                if (Occupant.State != null && Occupant.State.ID != 0)
                {
                    state s = new state();
                    MainWindow.GetSingleItem<state>(out s, Occupant.State.ID, MainWindow.States);
                    obj.Occupant.State = s;
                }
                obj.Occupant.Zip = new string(Occupant.Zip.ToCharArray());
                obj.Occupant.Phone = new string(Occupant.Phone.ToCharArray());
                obj.Occupant.Email = new string(Occupant.Email.ToCharArray());
                obj.Occupant.SetOwner(this);
            }
            else obj.Occupant = new Occupant(obj.Occupant.thisControl, this);

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

        public void RestoreDustResDataMembers(DustResidentialComplaint comp)
        {
            ComplaintAddress.ID = comp.ComplaintAddress.ID;
            BuildingDescription = comp.BuildingDescription;
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

            Occupant.FName = comp.Occupant.FName;
            Occupant.LName = comp.Occupant.LName;
            Occupant.AddressLine1 = comp.Occupant.AddressLine1;
            Occupant.AddressLine2 = comp.Occupant.AddressLine2;
            Occupant.City = comp.Occupant.City;
            if (comp.Occupant.State.ID != 0)
            {
                state s = new state();
                MainWindow.GetSingleItem<state>(out s, comp.Occupant.State.ID, MainWindow.States);
                Occupant.State = s;
            }
            else Occupant.State = new state();
            Occupant.Zip = comp.Occupant.Zip;
            Occupant.Phone = comp.Occupant.Phone;
            Occupant.Email = comp.Occupant.Email;
            Occupant.SetOwner(this);

            Owner.FName = comp.Owner.FName;
            Owner.LName = comp.Owner.LName;
            Owner.AddressLine1 = comp.Owner.AddressLine1;
            Owner.AddressLine2 = comp.Owner.AddressLine2;
            Owner.City = comp.Owner.City;
            if (comp.Owner.State != null && comp.Owner.State.ID != 0)
            {
                state s = new state();
                MainWindow.GetSingleItem<state>(out s, comp.Owner.State.ID, MainWindow.States);
                Owner.State = s;
            }
            else Owner.State = new state();
            Owner.Zip = comp.Owner.Zip;
            Owner.Phone = comp.Owner.Phone;
            Owner.Email = comp.Owner.Email;
            Owner.SetOwner(this);

            RestoreDustDataMembers((DustComplaint)comp);

            return;
        }

        public List<string> CompareDustResDataMembers(DustResidentialComplaint comp, List<string> differenceList)
        {
            List<String> returnList = new List<string>();
            returnList.AddRange(differenceList);

            if (ComplaintAddress.AddressLine1 != comp.ComplaintAddress.AddressLine1 || ComplaintAddress.AddressLine2 != comp.ComplaintAddress.AddressLine2 ||
                ComplaintAddress.City.ID != comp.ComplaintAddress.City.ID || ComplaintAddress.Zip != comp.ComplaintAddress.Zip ||
                ComplaintAddress.State.ID != comp.ComplaintAddress.State.ID || ComplaintAddress.Parcel != comp.ComplaintAddress.Parcel ||
                ComplaintAddress.Latitude != comp.ComplaintAddress.Latitude || ComplaintAddress.Longitude != comp.ComplaintAddress.Longitude) returnList.Add("Complaint address");
            if (BuildingDescription != comp.BuildingDescription) returnList.Add("Building description");
            if (Occupant.FName != comp.Occupant.FName || Occupant.LName != comp.Occupant.LName) returnList.Add("Occupant name");
            if (Occupant.AddressLine1 != comp.Occupant.AddressLine1 || Occupant.AddressLine2 != comp.Occupant.AddressLine2 || Occupant.City != comp.Occupant.City
                || Occupant.State.ID != comp.Occupant.State.ID || Occupant.Zip != comp.Occupant.Zip) returnList.Add("Occupant address");
            if (Occupant.Phone != comp.Occupant.Phone || Occupant.Email != comp.Occupant.Email) returnList.Add("Occupant contact info");
            if (Owner.FName != comp.Owner.FName || Owner.LName != comp.Owner.LName) returnList.Add("Owner name");
            if (Owner.AddressLine1 != comp.Owner.AddressLine1 || Owner.AddressLine2 != comp.Owner.AddressLine2 || Owner.City != comp.Owner.City
                || Owner.State.ID != comp.Owner.State.ID || Owner.Zip != comp.Owner.Zip) returnList.Add("Owner address");
            if (Owner.Phone != comp.Owner.Phone || Owner.Email != comp.Owner.Email) returnList.Add("Owner contact info");

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
            if (BuildingDescBox != null) BuildingDescription = BuildingDescBox.Text;
            ComplainantInfo.UpdateContentFromControls();
            ComplaintAddress.UpdateContentFromSiteControl();
            Occupant.UpdateContentFromControls();
            Owner.UpdateContentFromControls();

            base.UpdateContentFromControls();
        }

        public override void UpdateControlContent()
        {
            if (BuildingDescBox != null) BuildingDescBox.Text = BuildingDescription;
            if (ComplainantInfo != null) ComplainantInfo.UpdateControlContent();
            if (ComplaintAddress != null) ComplaintAddress.UpdateSiteControlContent();
            if (Occupant != null) Occupant.UpdateControlContent();
            if (Owner != null) Owner.UpdateControlContent();

            base.UpdateControlContent();
        }

        public override void SaveContacts(OleDbCommand cidCMD)
        {
            Occupant.SaveContact(cidCMD, ID);
            Owner.SaveContact(cidCMD, ID);
        }

        public override void SaveComplaintSpecific(OleDbCommand cidCMD)
        { ComplaintAddress.SaveComplaintSite(cidCMD); }
    }

    public class DustResidentialList
    {
        private static List<DustResidentialComplaint> DustResList = new List<DustResidentialComplaint>();
        public static ListCollectionView DustResCollection = new ListCollectionView(DustResList);
    }
}
