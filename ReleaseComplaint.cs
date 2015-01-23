using System;
using System.Data.OleDb;
using System.Windows.Controls;
using System.Collections.Generic;

namespace CID2
{
    public class ReleaseComplaint : Complaint
    {
        public ReleaseComplaint()
        { }

        public ReleaseComplaint(Complaint comp)
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
            AppendixA = comp.AppendixA;
            Restricted = comp.Restricted;

            ComplainantInfo.SetOwner(this);
        }

        public override Object MyClone()
        {
            Complaint temp = (Complaint)base.MyClone();
            ReleaseComplaint obj = new ReleaseComplaint(temp);

            return obj;
        }

        public void RestoreReleaseDataMembers(ReleaseComplaint comp)
        { RestoreComplaintDataMembers((ReleaseComplaint)comp); }

        public List<string> CompareReleaseDataMembers(ReleaseComplaint comp, List<string> differenceList)
        {
            List<String> returnList = new List<string>();
            returnList.AddRange(differenceList);

            return CompareDataMembers((Complaint)comp, returnList);
        }
    }
}