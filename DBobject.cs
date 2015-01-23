using System.Collections.Generic;
using System.Windows.Data;
using System.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class DBobject : IConvertible
    {
        public int ID { get; set; }

        public DBobject()
        { ID = 0; }

        public DBobject(int id)
        { ID = id; }

        public DBobject(OleDbDataReader dr)
        { SetMembers(dr); }

        public virtual void SetMembers<T>(T item)
        {
            OleDbDataReader dr = item as OleDbDataReader;
            ID = dr.GetInt32(0);
        }

        public virtual T CopyItem<T>(T item)
        {
            //Change item to the correct type of object then create a copy of it
            DBobject x = item as DBobject;
            DBobject y = new DBobject(x.ID);

            //This is necessary because the function doesn't know in advance that T is a
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public virtual T NullItem<T>(T item)
        {
            //create an object with no values
            DBobject y = new DBobject(0);

            //This is necessary because the function doesn't know in advance that T is a
            return (T)Convert.ChangeType(y, typeof(T));
        }

        public override string ToString()
        { return ID.ToString(); }

        public static List<T> DataReaderMapToList<T>(OleDbDataReader dr, T derp)
        {
            List<T> list = new List<T>();

            while (dr.Read())
            {
                var mi = typeof(T).GetMethod("SetMembers");
                var miref = mi.MakeGenericMethod(dr.GetType());
                miref.Invoke(derp, new object[] { dr });
                mi = typeof(T).GetMethod("CopyItem");
                miref = mi.MakeGenericMethod(derp.GetType());
                T herp = (T)miref.Invoke(derp, new object[] { derp });
                list.Add(herp);
            }
            return list;
        }

		public TypeCode GetTypeCode()
		{ return TypeCode.Object; }

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			if (ID != 0)
				return true;
			else 
				return false;
		}

		double GetDoubleValue()
		{ return ID; }

		byte IConvertible.ToByte(IFormatProvider provider)
		{ return Convert.ToByte(GetDoubleValue()); }

		char IConvertible.ToChar(IFormatProvider provider)
		{ return Convert.ToChar(GetDoubleValue()); }

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{ return Convert.ToDateTime(GetDoubleValue()); }

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{ return Convert.ToDecimal(GetDoubleValue()); }

		double IConvertible.ToDouble(IFormatProvider provider)
		{ return GetDoubleValue(); }

		short IConvertible.ToInt16(IFormatProvider provider)
		{ return Convert.ToInt16(GetDoubleValue()); }

		int IConvertible.ToInt32(IFormatProvider provider)
		{ return Convert.ToInt32(GetDoubleValue()); }

		long IConvertible.ToInt64(IFormatProvider provider)
		{ return Convert.ToInt64(GetDoubleValue()); }

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{ return Convert.ToSByte(GetDoubleValue()); }

		float IConvertible.ToSingle(IFormatProvider provider)
		{ return Convert.ToSingle(GetDoubleValue()); }

		string IConvertible.ToString(IFormatProvider provider)
		{ return ID.ToString(); }

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{ return this; }

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{ return Convert.ToUInt16(GetDoubleValue()); }

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{ return Convert.ToUInt32(GetDoubleValue()); }

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{ return Convert.ToUInt64(GetDoubleValue()); }
    }

    public class DBObjectList
    {
        private static List<DBobject> ObjectList = new List<DBobject>();
        public static ListCollectionView DBObjectCollection = new ListCollectionView(ObjectList);
    }
}