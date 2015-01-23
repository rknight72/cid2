using System.Collections.Generic;
using System.Windows.Data;
using System.Data.OleDb;
using System;

namespace CID2
{
    public class ParcelResult
    {
        public string strParcel { get; set; }
        public string strOwner { get; set; }
        public string strAdd { get; set; }

        public ParcelResult()
        {
            strParcel = "";
            strOwner = "";
            strAdd = "";
        }

        public ParcelResult(string parcelID, string owner, string address)
        {
            strParcel = parcelID;
            strOwner = owner;
            strAdd = address;
        }

        public override string ToString()
        { return strParcel; }
    }

    public class ParcelResultList
    {
        private static List<ParcelResult> ParcelList = new List<ParcelResult>();
        public static ListCollectionView ParcelsList = new ListCollectionView(ParcelList);
    }
}
