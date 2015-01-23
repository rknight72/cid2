using System;

namespace CID2
{
    public class ColumnDetail
    {
        public string ColumnName;
        public string TableName;
        public Type ColumnType;

        public ColumnDetail()
        {
            ColumnName = "";
            TableName = "";
            ColumnType = null;
        }

        public ColumnDetail(string columnname, string tablename, Type columntype)
        {
            ColumnName = columnname;
            TableName = tablename;
            ColumnType = columntype;
        }
    }
}
