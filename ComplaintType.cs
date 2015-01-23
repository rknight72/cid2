using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace CID2
{
    public class ComplaintType : DBobject
    {
        public string Label { get; set; }
        public string ComplaintTable { get; set; }
        public string SiteTable { get; set; }
        public string PrimaryContactTable { get; set; }
        public string SecondaryContactTable { get; set; }
        public string TertiaryContactTable { get; set; }
        public string QuaternaryContactTable { get; set; }
        public string QuinaryContactTable { get; set; }
        public string SenaryContactTable { get; set; }
        public List<string> SearchTerms { get; set; }
        public CETAtype defaultType { get; set; }
        public ListCollectionView cetaTypes { get; set; }
        public int RetentionMonths { get; set; }
        public ComplaintCategory Category { get; set; }
        public ComplaintLocation Location { get; set; }

        public ComplaintType()
        {
            Label = ComplaintTable = SiteTable = PrimaryContactTable = SecondaryContactTable = TertiaryContactTable =
                QuaternaryContactTable = QuinaryContactTable = SenaryContactTable = "";
            SearchTerms = new List<string>();
            defaultType = new CETAtype();
            cetaTypes = new ListCollectionView(SearchTerms);
            RetentionMonths = 0;
            Category = new ComplaintCategory();
            Location = new ComplaintLocation();
        }

        public ComplaintType(int id, string label, string complainttable, string sitetable, string primarycontacttable, string secondarycontacttable,
            string tertiarycontacttable, string quaternarycontacttable, string quinarycontacttable, string senarycontacttable,
            List<string> searchterms, CETAtype defaulttype, ListCollectionView cetatypes, int retentionmonths,
            ComplaintCategory category, ComplaintLocation location, List<string> contacttables)
        {
            ID = id;
            Label = label;
            ComplaintTable = complainttable;
            SiteTable = sitetable;
            PrimaryContactTable = primarycontacttable;
            SecondaryContactTable = secondarycontacttable;
            TertiaryContactTable = tertiarycontacttable;
            QuaternaryContactTable = quaternarycontacttable;
            QuinaryContactTable = quinarycontacttable;
            SenaryContactTable = senarycontacttable;
            SearchTerms = searchterms;
            defaultType = defaulttype;
            cetaTypes = cetatypes;
            RetentionMonths = retentionmonths;
            Category = category;
            Location = location;
        }

        public ComplaintType(ComplaintType comptype)
        {
            ID = comptype.ID;
            Label = comptype.Label;
            ComplaintTable = comptype.ComplaintTable;
            SiteTable = comptype.SiteTable;
            PrimaryContactTable = comptype.PrimaryContactTable;
            SecondaryContactTable = comptype.SecondaryContactTable;
            TertiaryContactTable = comptype.TertiaryContactTable;
            QuaternaryContactTable = comptype.QuaternaryContactTable;
            QuinaryContactTable = comptype.QuinaryContactTable;
            SenaryContactTable = comptype.SenaryContactTable;
            SearchTerms = comptype.SearchTerms;
            defaultType = comptype.defaultType;
            cetaTypes = comptype.cetaTypes;
            RetentionMonths = comptype.RetentionMonths;
            Category = comptype.Category;
            Location = comptype.Location;
        }

        public override string ToString()
        { return Label; }

        public override void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
            ComplaintTable = (dr[1] != DBNull.Value) ? dr.GetString(1) : "";
            Label = (dr[2] != DBNull.Value) ? dr.GetString(2) : "";
            SiteTable = (dr[3] != DBNull.Value) ? dr.GetString(3) : "";
            PrimaryContactTable = (dr[4] != DBNull.Value) ? dr.GetString(4) : "";
            SecondaryContactTable = (dr[5] != DBNull.Value) ? dr.GetString(5) : "";
            TertiaryContactTable = (dr[6] != DBNull.Value) ? dr.GetString(6) : "";
            QuaternaryContactTable = (dr[7] != DBNull.Value) ? dr.GetString(7) : "";
            QuinaryContactTable = (dr[8] != DBNull.Value) ? dr.GetString(8) : "";
            SenaryContactTable = (dr[9] != DBNull.Value) ? dr.GetString(9) : "";
            if (dr[10] != DBNull.Value)
            {
                string x = dr.GetString(10);
                string[] y = x.Split(',');
                SearchTerms = y.ToList<string>();
                for (int z = 0; z < SearchTerms.Count; z++)
                {
                    SearchTerms[z] = SearchTerms[z].TrimStart(' ');
                    SearchTerms[z] = SearchTerms[z].Substring(1, (SearchTerms[z].Length - 2));
                }
            }
            else SearchTerms = null;
            CETAtype type = null;
            if (dr[11] != DBNull.Value) MainWindow.GetSingleItem<CETAtype>(out type, dr.GetInt32(11), MainWindow.CETATypes);
            defaultType = type;
            if (dr[12] != DBNull.Value)
            {
                List<CETAtype> typeList = new List<CETAtype>();
                string raw = dr.GetString(12);
                List<int> list = raw.Split(',').Select(int.Parse).ToList();
                foreach (int x in list)
                {
                    CETAtype ctype = new CETAtype();
                    MainWindow.GetSingleItem<CETAtype>(out ctype, x, MainWindow.CETATypes);
                    typeList.Add(ctype);
                }
                cetaTypes = new ListCollectionView(typeList);
                cetaTypes.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }
            else cetaTypes = null;
            RetentionMonths = (dr[13] != DBNull.Value) ? dr.GetInt32(13) : 9999;
            ComplaintCategory cat = null;
            if (dr[14] != DBNull.Value) MainWindow.GetSingleItem<ComplaintCategory>(out cat, dr.GetInt32(14), MainWindow.ComplaintCategories);
            Category = cat;
            ComplaintLocation loc = null;
            if (dr[15] != DBNull.Value) MainWindow.GetSingleItem<ComplaintLocation>(out loc, dr.GetInt32(15), MainWindow.ComplaintLocations);
            Location = loc;
        }

        public override T CopyItem<T>(T item)
        {
            ComplaintType x = item as ComplaintType;
            ComplaintType y = new ComplaintType(x);
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override T NullItem<T>(T item)
        {
            ComplaintType y = new ComplaintType(0, "", "", "", "", "", "", "", "", "", new List<string>(), null, null, 0, null, null, null);
            return (T)Convert.ChangeType(y, typeof(T));
        }
    }
}