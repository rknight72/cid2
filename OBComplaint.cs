using System;
using System.Data.OleDb;
using System.Windows.Controls;
using System.Collections.Generic;

namespace CID2
{
    public class OBComplaint : Complaint
    {
        public FireDepartment FDinfo = new FireDepartment();
        public bool FDGenerated {get; set;}

        public OBComplaint()
        {
            FDinfo = new FireDepartment();
            FDGenerated = false;
        }

        public OBComplaint(Complaint comp)
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
            ReceivedBy = comp.ReceivedBy;
            Inspector = comp.Inspector;
            CETA = comp.CETA;
            AppendixA = comp.AppendixA;
            Restricted = comp.Restricted;

            FDinfo = new FireDepartment();
            ComplainantInfo.SetOwner(this);
        }

        public override Object MyClone()
        {
            Complaint temp = (Complaint)base.MyClone();
            OBComplaint obj = new OBComplaint(temp);

            if (FDinfo != null && FDinfo.ID != 0)
            {
                FireDepartment f = new FireDepartment();
                MainWindow.GetSingleItem<FireDepartment>(out f, FDinfo.ID, MainWindow.FireDepartments);
                obj.FDinfo = f;
            }
            else obj.FDinfo = new FireDepartment();
            obj.FDGenerated = (FDGenerated == true);

            return obj;
        }

        public void RestoreOBDataMembers(OBComplaint comp)
        {
            FDGenerated = comp.FDGenerated;
            if (comp.FDinfo != null && comp.FDinfo.ID != 0)
            {
                FireDepartment f = new FireDepartment();
                MainWindow.GetSingleItem<FireDepartment>(out f, comp.FDinfo.ID, MainWindow.FireDepartments);
                FDinfo = f;
            }
            else FDinfo = new FireDepartment();

            RestoreComplaintDataMembers((Complaint)comp);

            return;
        }

        public List<string> CompareOBDataMembers(OBComplaint comp, List<string> differenceList)
        {
            List<String> returnList = new List<string>();
            returnList.AddRange(differenceList);

            if (FDGenerated != comp.FDGenerated) returnList.Add("Fire Dept generated changed to " + FDGenerated.ToString());
            if (FDinfo.ID != comp.FDinfo.ID) returnList.Add("Fire Dept changed to " + FDinfo.FDName);

            return CompareDataMembers((Complaint)comp, returnList);
        }
    }
}
