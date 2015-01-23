using System;
using System.Data.OleDb;
using System.Windows.Controls;
using System.Collections.Generic;

namespace CID2
{
    public class DustComplaint : Complaint
    {
        public DustComplaint()
        { }

        public DustComplaint(Complaint comp)
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
            AppendixA = comp.AppendixA;
            Restricted = comp.Restricted;
        }

        public override Object MyClone()
        {
            Complaint temp = (Complaint)base.MyClone();
            DustComplaint obj = new DustComplaint(temp);

            return obj;
        }

        public void RestoreDustDataMembers(DustComplaint comp)
        { RestoreComplaintDataMembers((Complaint)comp); }

        public List<string> CompareDustDataMembers(DustComplaint comp, List<string> differenceList)
        {
            List<String> returnList = new List<string>();
            returnList.AddRange(differenceList);

            return CompareDataMembers((Complaint)comp, returnList);
        }
    }
}