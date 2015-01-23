using System;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;


namespace CID2
{
    public class AsbestosComplaint : Complaint
    {
        public string strANTSID = "";
        public DemoContractor DemoContractor = new DemoContractor();
        public AbatementContractor AbatementContractor = new AbatementContractor();
        public EntityCoordinator EntityCoordinator = new EntityCoordinator();
        public Landfill Landfill = new Landfill();
        public OtherOperator OtherOperator = new OtherOperator();

        public TextBox ANTSIDBox = new TextBox();

        public AsbestosComplaint()
        { }

        public AsbestosComplaint(Complaint comp)
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
            ReceivedBy = comp.ReceivedBy;
            Inspector = comp.Inspector;
            CETA = comp.CETA;
            AppendixA = comp.AppendixA;
            Restricted = comp.Restricted;

            ComplainantInfo.SetOwner(this);
            DemoContractor.SetOwner(this);
            AbatementContractor.SetOwner(this);
            EntityCoordinator.SetOwner(this);
            Landfill.SetOwner(this);
            OtherOperator.SetOwner(this);
        }

        public override Object MyClone()
        {
            Complaint temp = (Complaint)base.MyClone();
            AsbestosComplaint obj = new AsbestosComplaint(temp);

            if (strANTSID != null) obj.strANTSID = new string(strANTSID.ToCharArray());
            if (DemoContractor != null && DemoContractor.ID != 0)
            {
                obj.DemoContractor.ID = DemoContractor.ID;
                obj.DemoContractor.FName = new string(DemoContractor.FName.ToCharArray());
                obj.DemoContractor.LName = new string(DemoContractor.LName.ToCharArray());
                obj.DemoContractor.AddressLine1 = new string(DemoContractor.AddressLine1.ToCharArray());
                obj.DemoContractor.AddressLine2 = new string(DemoContractor.AddressLine2.ToCharArray());
                obj.DemoContractor.City = new string(DemoContractor.City.ToCharArray());
                if (DemoContractor.State != null && DemoContractor.State.ID != 0)
                {
                    state s = new state();
                    MainWindow.GetSingleItem<state>(out s, DemoContractor.State.ID, MainWindow.States);
                    obj.DemoContractor.State = s;
                }
                obj.DemoContractor.Zip = new string(DemoContractor.Zip.ToCharArray());
                obj.DemoContractor.Phone = new string(DemoContractor.Phone.ToCharArray());
                obj.DemoContractor.Email = new string(DemoContractor.Email.ToCharArray());
                obj.DemoContractor.SetOwner(this);
            }
            else obj.DemoContractor = new DemoContractor(obj.DemoContractor.thisControl, this);

            if (AbatementContractor != null && AbatementContractor.ID != 0)
            {
                obj.AbatementContractor.ID = AbatementContractor.ID;
                obj.AbatementContractor.FName = new string(AbatementContractor.FName.ToCharArray());
                obj.AbatementContractor.LName = new string(AbatementContractor.LName.ToCharArray());
                obj.AbatementContractor.AddressLine1 = new string(AbatementContractor.AddressLine1.ToCharArray());
                obj.AbatementContractor.AddressLine2 = new string(AbatementContractor.AddressLine2.ToCharArray());
                obj.AbatementContractor.City = new string(AbatementContractor.City.ToCharArray());
                if (AbatementContractor.State != null && AbatementContractor.State.ID != 0)
                {
                    state s = new state();
                    MainWindow.GetSingleItem<state>(out s, AbatementContractor.State.ID, MainWindow.States);
                    obj.AbatementContractor.State = s;
                }
                obj.AbatementContractor.Zip = new string(AbatementContractor.Zip.ToCharArray());
                obj.AbatementContractor.Phone = new string(AbatementContractor.Phone.ToCharArray());
                obj.AbatementContractor.Email = new string(AbatementContractor.Email.ToCharArray());
                obj.AbatementContractor.SetOwner(this);
            }
            else obj.AbatementContractor = new AbatementContractor(obj.AbatementContractor.thisControl, this);

            if (EntityCoordinator != null && EntityCoordinator.ID != 0)
            {
                obj.EntityCoordinator.ID = EntityCoordinator.ID;
                obj.EntityCoordinator.FName = new string(EntityCoordinator.FName.ToCharArray());
                obj.EntityCoordinator.LName = new string(EntityCoordinator.LName.ToCharArray());
                obj.EntityCoordinator.AddressLine1 = new string(EntityCoordinator.AddressLine1.ToCharArray());
                obj.EntityCoordinator.AddressLine2 = new string(EntityCoordinator.AddressLine2.ToCharArray());
                obj.EntityCoordinator.City = new string(EntityCoordinator.City.ToCharArray());
                if (EntityCoordinator.State != null && EntityCoordinator.State.ID != 0)
                {
                    state s = new state();
                    MainWindow.GetSingleItem<state>(out s, EntityCoordinator.State.ID, MainWindow.States);
                    obj.EntityCoordinator.State = s;
                }
                obj.EntityCoordinator.Zip = new string(EntityCoordinator.Zip.ToCharArray());
                obj.EntityCoordinator.Phone = new string(EntityCoordinator.Phone.ToCharArray());
                obj.EntityCoordinator.Email = new string(EntityCoordinator.Email.ToCharArray());
                obj.EntityCoordinator.SetOwner(this);
            }
            else obj.EntityCoordinator = new EntityCoordinator(obj.EntityCoordinator.thisControl, this);

            if (Landfill != null && Landfill.ID != 0)
            {
                obj.Landfill.ID = Landfill.ID;
                obj.Landfill.FName = new string(Landfill.FName.ToCharArray());
                obj.Landfill.LName = new string(Landfill.LName.ToCharArray());
                obj.Landfill.AddressLine1 = new string(Landfill.AddressLine1.ToCharArray());
                obj.Landfill.AddressLine2 = new string(Landfill.AddressLine2.ToCharArray());
                obj.Landfill.City = new string(Landfill.City.ToCharArray());
                if (Landfill.State != null && Landfill.State.ID != 0)
                {
                    state s = new state();
                    MainWindow.GetSingleItem<state>(out s, Landfill.State.ID, MainWindow.States);
                    obj.Landfill.State = s;
                }
                obj.Landfill.Zip = new string(Landfill.Zip.ToCharArray());
                obj.Landfill.Phone = new string(Landfill.Phone.ToCharArray());
                obj.Landfill.Email = new string(Landfill.Email.ToCharArray());
                obj.Landfill.SetOwner(this);
            }
            else obj.Landfill = new Landfill(obj.Landfill.thisControl, this);

            if (OtherOperator != null && OtherOperator.ID != 0)
            {
                obj.OtherOperator.ID = OtherOperator.ID;
                obj.OtherOperator.FName = new string(OtherOperator.FName.ToCharArray());
                obj.OtherOperator.LName = new string(OtherOperator.LName.ToCharArray());
                obj.OtherOperator.AddressLine1 = new string(OtherOperator.AddressLine1.ToCharArray());
                obj.OtherOperator.AddressLine2 = new string(OtherOperator.AddressLine2.ToCharArray());
                obj.OtherOperator.City = new string(OtherOperator.City.ToCharArray());
                if (OtherOperator.State != null && OtherOperator.State.ID != 0)
                {
                    state s = new state();
                    MainWindow.GetSingleItem<state>(out s, OtherOperator.State.ID, MainWindow.States);
                    obj.OtherOperator.State = s;
                }
                obj.OtherOperator.Zip = new string(OtherOperator.Zip.ToCharArray());
                obj.OtherOperator.Phone = new string(OtherOperator.Phone.ToCharArray());
                obj.OtherOperator.Email = new string(OtherOperator.Email.ToCharArray());
                obj.OtherOperator.SetOwner(this);
            }
            else obj.OtherOperator = new OtherOperator(obj.OtherOperator.thisControl, this);

            return obj;
        }

        public void RestoreAsbestosDataMembers(AsbestosComplaint comp)
        {
            strANTSID = comp.strANTSID;

            DemoContractor.ID = comp.DemoContractor.ID;
            DemoContractor.FName = new string(comp.DemoContractor.FName.ToCharArray());
            DemoContractor.LName = new string(comp.DemoContractor.LName.ToCharArray());
            DemoContractor.AddressLine1 = new string(comp.DemoContractor.AddressLine1.ToCharArray());
            DemoContractor.AddressLine2 = new string(comp.DemoContractor.AddressLine2.ToCharArray());
            DemoContractor.City = new string(comp.DemoContractor.City.ToCharArray());
            if (comp.DemoContractor.State != null && comp.DemoContractor.State.ID != 0)
            {
                state s = new state();
                MainWindow.GetSingleItem<state>(out s, comp.DemoContractor.State.ID, MainWindow.States);
                DemoContractor.State = s;
            }
            DemoContractor.Zip = new string(comp.DemoContractor.Zip.ToCharArray());
            DemoContractor.Phone = new string(comp.DemoContractor.Phone.ToCharArray());
            DemoContractor.Email = new string(comp.DemoContractor.Email.ToCharArray());
            DemoContractor.SetOwner(this);

            AbatementContractor.ID = comp.AbatementContractor.ID;
            AbatementContractor.FName = new string(comp.AbatementContractor.FName.ToCharArray());
            AbatementContractor.LName = new string(comp.AbatementContractor.LName.ToCharArray());
            AbatementContractor.AddressLine1 = new string(comp.AbatementContractor.AddressLine1.ToCharArray());
            AbatementContractor.AddressLine2 = new string(comp.AbatementContractor.AddressLine2.ToCharArray());
            AbatementContractor.City = new string(comp.AbatementContractor.City.ToCharArray());
            if (comp.AbatementContractor.State != null && comp.AbatementContractor.State.ID != 0)
            {
                state s = new state();
                MainWindow.GetSingleItem<state>(out s, comp.AbatementContractor.State.ID, MainWindow.States);
                AbatementContractor.State = s;
            }
            AbatementContractor.Zip = new string(comp.AbatementContractor.Zip.ToCharArray());
            AbatementContractor.Phone = new string(comp.AbatementContractor.Phone.ToCharArray());
            AbatementContractor.Email = new string(comp.AbatementContractor.Email.ToCharArray());
            AbatementContractor.SetOwner(this);

            EntityCoordinator.ID = comp.EntityCoordinator.ID;
            EntityCoordinator.FName = new string(comp.EntityCoordinator.FName.ToCharArray());
            EntityCoordinator.LName = new string(comp.EntityCoordinator.LName.ToCharArray());
            EntityCoordinator.AddressLine1 = new string(comp.EntityCoordinator.AddressLine1.ToCharArray());
            EntityCoordinator.AddressLine2 = new string(comp.EntityCoordinator.AddressLine2.ToCharArray());
            EntityCoordinator.City = new string(comp.EntityCoordinator.City.ToCharArray());
            if (comp.EntityCoordinator.State != null && comp.EntityCoordinator.State.ID != 0)
            {
                state s = new state();
                MainWindow.GetSingleItem<state>(out s, comp.EntityCoordinator.State.ID, MainWindow.States);
                EntityCoordinator.State = s;
            }
            EntityCoordinator.Zip = new string(comp.EntityCoordinator.Zip.ToCharArray());
            EntityCoordinator.Phone = new string(comp.EntityCoordinator.Phone.ToCharArray());
            EntityCoordinator.Email = new string(comp.EntityCoordinator.Email.ToCharArray());
            EntityCoordinator.SetOwner(this);

            Landfill.ID = comp.Landfill.ID;
            Landfill.FName = new string(comp.Landfill.FName.ToCharArray());
            Landfill.LName = new string(comp.Landfill.LName.ToCharArray());
            Landfill.AddressLine1 = new string(comp.Landfill.AddressLine1.ToCharArray());
            Landfill.AddressLine2 = new string(comp.Landfill.AddressLine2.ToCharArray());
            Landfill.City = new string(comp.Landfill.City.ToCharArray());
            if (comp.Landfill.State != null && comp.Landfill.State.ID != 0)
            {
                state s = new state();
                MainWindow.GetSingleItem<state>(out s, comp.Landfill.State.ID, MainWindow.States);
                Landfill.State = s;
            }
            Landfill.Zip = new string(comp.Landfill.Zip.ToCharArray());
            Landfill.Phone = new string(comp.Landfill.Phone.ToCharArray());
            Landfill.Email = new string(comp.Landfill.Email.ToCharArray());
            Landfill.SetOwner(this);

            OtherOperator.ID = comp.OtherOperator.ID;
            OtherOperator.FName = new string(comp.OtherOperator.FName.ToCharArray());
            OtherOperator.LName = new string(comp.OtherOperator.LName.ToCharArray());
            OtherOperator.AddressLine1 = new string(comp.OtherOperator.AddressLine1.ToCharArray());
            OtherOperator.AddressLine2 = new string(comp.OtherOperator.AddressLine2.ToCharArray());
            OtherOperator.City = new string(comp.OtherOperator.City.ToCharArray());
            if (comp.OtherOperator.State != null && comp.OtherOperator.State.ID != 0)
            {
                state s = new state();
                MainWindow.GetSingleItem<state>(out s, comp.OtherOperator.State.ID, MainWindow.States);
                OtherOperator.State = s;
            }
            OtherOperator.Zip = new string(comp.OtherOperator.Zip.ToCharArray());
            OtherOperator.Phone = new string(comp.OtherOperator.Phone.ToCharArray());
            OtherOperator.Email = new string(comp.OtherOperator.Email.ToCharArray());
            OtherOperator.SetOwner(this);

            RestoreComplaintDataMembers((Complaint)comp);
        }

        public List<string> CompareAsbestosDataMembers(AsbestosComplaint comp, List<string> differenceList)
        {
            List<String> returnList = new List<string>();
            returnList.AddRange(differenceList);

            if (strANTSID != comp.strANTSID) returnList.Add("ANTS ID");
            if (DemoContractor.FName != comp.DemoContractor.FName || DemoContractor.LName != comp.DemoContractor.LName) returnList.Add("Demo Contractor name");
            if (DemoContractor.AddressLine1 != comp.DemoContractor.AddressLine1 || DemoContractor.AddressLine2 != comp.DemoContractor.AddressLine2 || DemoContractor.City != comp.DemoContractor.City
                || DemoContractor.State.ID != comp.DemoContractor.State.ID || DemoContractor.Zip != comp.DemoContractor.Zip) returnList.Add("Demo Contractor address");
            if (DemoContractor.Phone != comp.DemoContractor.Phone || DemoContractor.Email != comp.DemoContractor.Email) returnList.Add("Demo Contractor contact info");

            if (AbatementContractor.FName != comp.AbatementContractor.FName || AbatementContractor.LName != comp.AbatementContractor.LName) returnList.Add("Abatement Contractor name");
            if (AbatementContractor.AddressLine1 != comp.AbatementContractor.AddressLine1 || AbatementContractor.AddressLine2 != comp.AbatementContractor.AddressLine2 || AbatementContractor.City != comp.AbatementContractor.City
                || AbatementContractor.State.ID != comp.AbatementContractor.State.ID || AbatementContractor.Zip != comp.AbatementContractor.Zip) returnList.Add("batement Contractor address");
            if (AbatementContractor.Phone != comp.AbatementContractor.Phone || AbatementContractor.Email != comp.AbatementContractor.Email) returnList.Add("batement Contractor contact info");

            if (EntityCoordinator.FName != comp.EntityCoordinator.FName || EntityCoordinator.LName != comp.EntityCoordinator.LName) returnList.Add("Entity Coordinator name");
            if (EntityCoordinator.AddressLine1 != comp.EntityCoordinator.AddressLine1 || EntityCoordinator.AddressLine2 != comp.EntityCoordinator.AddressLine2 || EntityCoordinator.City != comp.EntityCoordinator.City
                || EntityCoordinator.State.ID != comp.EntityCoordinator.State.ID || EntityCoordinator.Zip != comp.EntityCoordinator.Zip) returnList.Add("Entity Coordinator address");
            if (EntityCoordinator.Phone != comp.EntityCoordinator.Phone || EntityCoordinator.Email != comp.EntityCoordinator.Email) returnList.Add("Entity Coordinator contact info");

            if (Landfill.FName != comp.Landfill.FName || Landfill.LName != comp.Landfill.LName) returnList.Add("Landfill name");
            if (Landfill.AddressLine1 != comp.Landfill.AddressLine1 || Landfill.AddressLine2 != comp.Landfill.AddressLine2 || Landfill.City != comp.Landfill.City
                || Landfill.State.ID != comp.Landfill.State.ID || Landfill.Zip != comp.Landfill.Zip) returnList.Add("Landfill address");
            if (Landfill.Phone != comp.Landfill.Phone || Landfill.Email != comp.Landfill.Email) returnList.Add("Landfill contact info");

            if (OtherOperator.FName != comp.OtherOperator.FName || OtherOperator.LName != comp.OtherOperator.LName) returnList.Add("Other Operator name");
            if (OtherOperator.AddressLine1 != comp.OtherOperator.AddressLine1 || OtherOperator.AddressLine2 != comp.OtherOperator.AddressLine2 || OtherOperator.City != comp.OtherOperator.City
                || OtherOperator.State.ID != comp.OtherOperator.State.ID || OtherOperator.Zip != comp.OtherOperator.Zip) returnList.Add("Other Operator address");
            if (OtherOperator.Phone != comp.OtherOperator.Phone || OtherOperator.Email != comp.OtherOperator.Email) returnList.Add("Other Operator contact info");

            return CompareDataMembers((Complaint)comp, returnList);
        }

        public override void SaveContacts(OleDbCommand cidCMD)
        {
            DemoContractor.SaveContact(cidCMD, ID);
            AbatementContractor.SaveContact(cidCMD, ID);
            EntityCoordinator.SaveContact(cidCMD, ID);
            Landfill.SaveContact(cidCMD, ID);
            OtherOperator.SaveContact(cidCMD, ID);
        }

        public override void UpdateContentFromControls()
        {
            strANTSID = ANTSIDBox.Text;

            base.UpdateContentFromControls();
        }

        public override void UpdateControlContent()
        {
            ANTSIDBox.Text = strANTSID;

            base.UpdateControlContent();
        }

        public virtual void SetControls(TextBlock idblock, TextBox complainttextbox, TextBox inspectionnotesbox, TextBox othernotesbox,
            ComboBox receivedbybox, ComboBox inspectorbox, ComboBox statusbox, DatePicker datereceivedbox, DatePicker incidentdatebox,
            DatePicker dateclosedbox, DatePicker retentionbox, ComboBox cetabox, ComboBox methodreceivedbox,
            DatePicker investigatedbox, TextBox antsidbox)
        {
            ANTSIDBox = antsidbox;
            base.SetControls(idblock, complainttextbox, inspectionnotesbox, othernotesbox, receivedbybox, inspectorbox, statusbox,
                datereceivedbox, incidentdatebox, dateclosedbox, retentionbox, cetabox, methodreceivedbox, investigatedbox);
        }
    }
}