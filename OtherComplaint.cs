using System;
using System.Data.OleDb;
using System.Windows.Controls;
using System.Collections.Generic;

namespace CID2
{
    public class OtherComplaint : Complaint
    {
        public OtherComplaint()
        { }

        public OtherComplaint(Complaint comp)
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
        }

        public override Object MyClone()
        {
            Complaint temp = (Complaint)base.MyClone();
            OtherComplaint obj = new OtherComplaint(temp);

            return obj;
        }

        public void RestoreOtherDataMembers(OtherComplaint comp)
        { RestoreComplaintDataMembers((Complaint)comp); }

        public List<string> CompareOtherDataMembers(OtherComplaint comp, List<string> differenceList)
        {
            List<String> returnList = new List<string>();
            returnList.AddRange(differenceList);

            return CompareDataMembers((Complaint)comp, returnList);
        }
    }
}