using System;
using System.Data.OleDb;
using System.Windows.Controls;
using System.Collections.Generic;

namespace CID2
{
    public class OdorComplaint : Complaint
    {
        public OdorComplaint()
        {}

        public OdorComplaint(Complaint comp)
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
        }

        public override Object MyClone()
        {
            Complaint temp = (Complaint)base.MyClone();
            OdorComplaint obj = new OdorComplaint(temp);

            return obj;
        }

        public void RestoreOdorDataMembers(OdorComplaint comp)
        { RestoreComplaintDataMembers((Complaint)comp); }

        public List<string> CompareOdorDataMembers(OdorComplaint comp, List<string> differenceList)
        {
            List<String> returnList = new List<string>();
            returnList.AddRange(differenceList);

            return CompareDataMembers((Complaint)comp, returnList);
        }
    }
}