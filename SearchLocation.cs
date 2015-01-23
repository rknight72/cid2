using System;
using System.Collections.Generic;
using System.Windows;

namespace CID2
{
    public class SearchLocation
    {
        public ComplaintType CompType { get; set; }
        public List<ColumnDetail> StringColumns;
        public List<ColumnDetail> IntColumns;
        public List<ColumnDetail> BooleanColumns;
        public List<ColumnDetail> DateColumns;


        public SearchLocation()
        {
            CompType = null;
            StringColumns = new List<ColumnDetail>();
            IntColumns = new List<ColumnDetail>();
            BooleanColumns = new List<ColumnDetail>();
            DateColumns = new List<ColumnDetail>();
        }

        public SearchLocation(ComplaintType comptype)
        {
            CompType = comptype;
            StringColumns = new List<ColumnDetail>();
            IntColumns = new List<ColumnDetail>();
            BooleanColumns = new List<ColumnDetail>();
            DateColumns = new List<ColumnDetail>();
        }

        public void AddColumn(string columnname, string tablename, Type columntype)
        {
            switch(Type.GetTypeCode(columntype))
            {
                case TypeCode.Int32:
                    IntColumns.Add(new ColumnDetail(columnname, tablename, columntype));
                    break;
                case TypeCode.Boolean:
                    BooleanColumns.Add(new ColumnDetail(columnname, tablename, columntype));
                    break;
                case TypeCode.String:
                    StringColumns.Add(new ColumnDetail(columnname, tablename, columntype));
                    break;
                case TypeCode.DateTime:
                    DateColumns.Add(new ColumnDetail(columnname, tablename, columntype));
                    break;
                case TypeCode.Double:
                    break;
                default:
                    MessageBox.Show("The column named " + columnname + " in the table " + tablename + " has not been added as a location in the list of search items."
                    + " The database designer did not anticipate this type of data should be included in the complaint data.");
                    break;
            }
        }
    }
}
