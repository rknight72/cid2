using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Data.Sql;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace CID2
{
    /// <summary>
    /// Interaction logic for RelatedSearch.xaml
    /// </summary>
    public partial class RelatedSearch : Window
    {
        public int pID { get; set; }

        private List<string> ListSearchTerms;
        private List<ComplaintType> ListTypes;
        private List<city> ListCities;
        private List<county> ListCounties;
        private List<township> ListTownships;
        private List<FireDepartment> ListFireDepts;
        private List<Facility> ListFacilities;
        private List<user> ListUsers;
        private List<string> lstCompSQL;

        public ListCollectionView arg { get; set; }

        public ListCollectionView ComplaintsView { get; set; }
        public List<RankedComplaint> ListComplaints { get; set; }
        public Dictionary<int, int> DictRanked;

        public List<RelatedComplaint> ExistingRelated;

        public struct RankedComplaint
        {
            public int ID { get; set; }
            public int Status { get; set; }
            public int Rank { get; set; }
            public int SubID { get; set; }
            public DateTime DateReceived { get; set; }
            public ComplaintType Type { get; set; }
            public string FacName { get; set; }
            public Address SiteInfo { get; set; }
            public string ComplaintNotes { get; set; }
            public bool Related { get; set; }
        }

        public RelatedSearch()
        {
            InitializeComponent();

            ExistingRelated = null;
            pID = 0;
            MarkedColumn.Visibility = System.Windows.Visibility.Hidden;
            AddressColumn.Width = new DataGridLength(235);
        }

        public RelatedSearch(int parent_id, ItemCollection list)
        {
            InitializeComponent();

            ExistingRelated = new List<RelatedComplaint>();
            foreach(RelatedComplaint x in list)
            { ExistingRelated.Add(x); }

            pID = parent_id;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //Change to busy cursor
            Mouse.OverrideCursor = Cursors.Wait;

            DateTime start = DateTime.Now;
            
            if (txtTerms.Text != "")
            {
                ListSearchTerms = new List<string>();
                ListTypes = new List<ComplaintType>();
                ListCities = new List<city>();
                ListCounties = new List<county>();
                ListTownships = new List<township>();
                ListFireDepts = new List<FireDepartment>();
                ListFacilities = new List<Facility>();
                ListUsers = new List<user>();
                ListComplaints = new List<RankedComplaint>();
                lstCompSQL = new List<string>();
                DictRanked = new Dictionary<int, int>();

                List<string> lstResults = ParseSearchTerms(txtTerms.Text);
                for (int x = 0; x < lstResults.Count; x++)
                {
                    DateTime dtThis;
                    if (DateTime.TryParse(lstResults[x], out dtThis)) { GetDateRanks(dtThis); }
                    else { GetStringRanks(lstResults[x]); }
                }

                if (ListFireDepts.Count > 0) GetFDRanks();
                if (ListCities.Count > 0) GetCityRanks();
                if (ListCounties.Count > 0) GetCountyRanks();
                if (ListFacilities.Count > 0) GetFacilityRanks();
                if (ListTypes.Count > 0) GetTypeRanks();
                
                if (DictRanked.Count > 0)
                {
                    GetRankedComplaints();
                    ComplaintsView = new ListCollectionView(ListComplaints);
                    ComplaintsView.SortDescriptions.Add(new SortDescription("Rank", ListSortDirection.Descending));
                    listComplaints.ItemsSource = ComplaintsView;

                    TimeSpan time = DateTime.Now - start;
                    MessageBox.Show(DictRanked.Count.ToString() + " results in " + time.Seconds.ToString() + "." + time.Milliseconds.ToString() + " seconds. The most relevant results are shown first.");
                }
                else
                {
                    MessageBox.Show("No matches found.");
                    listComplaints.ItemsSource = null;
                }
            }

            //Change back to normal cursor
            Mouse.OverrideCursor = null;
        }

        private void GetRankedComplaints()
        {
            MainWindow.OpenMainDBConnection();

            foreach (KeyValuePair<int, int> x in DictRanked)
            {
                string strSQL = "SELECT * FROM tbl_Complaints WHERE ID = " + x.Key.ToString() + ";";
                OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                OleDbDataReader cidReader = cidCMD.ExecuteReader();

                while(cidReader.Read())
                {
                    RankedComplaint thisComp = new RankedComplaint();
                    thisComp.ID = x.Key;
                    ComplaintType type = new ComplaintType();
                    MainWindow.GetSingleItem<ComplaintType>(out type, cidReader.GetInt32(1), MainWindow.ComplaintTypes);
                    thisComp.Type = type;
                    thisComp.SubID = cidReader.GetInt32(2);
                    thisComp.Status = cidReader.GetInt32(3);
                    if (cidReader[6] != DBNull.Value) thisComp.DateReceived = cidReader.GetDateTime(6);
                    if (cidReader[12] != DBNull.Value) thisComp.ComplaintNotes = cidReader.GetString(12);
                    thisComp.Rank = x.Value;
                    string fname = "";
                    thisComp.SiteInfo = MainWindow.GetAddress(thisComp.Type, out fname, thisComp.SubID);
                    thisComp.FacName = fname;
                    if (ExistingRelated != null) thisComp.Related = ExistingRelated.Exists(y => y.ID == x.Key);
                    ListComplaints.Add(thisComp);
                }

                cidReader.Close();
            }

            MainWindow.CloseMainDBConnection();
        }

        public void GetDateRanks(DateTime date)
        {
            foreach (SearchLocation x in MainWindow.SearchList)
            {
                MainWindow.OpenMainDBConnection();

                foreach (ColumnDetail y in x.DateColumns)
                {
                    string strSQL = "SELECT b.ID FROM ((((" + x.CompType.ComplaintTable + " AS a LEFT JOIN tbl_Complaints AS b ON b.Type_SubID = a.ID) LEFT JOIN "
                        + x.CompType.SiteTable + " AS c ON c.ID = a.Site) LEFT JOIN " + x.CompType.PrimaryContactTable + " AS d ON d.ID = a.Occupant) LEFT JOIN "
                        + x.CompType.SecondaryContactTable + " AS e ON e.ID = a.Owner) LEFT JOIN tbl_Complainants AS f ON f.ID = b.Complainant WHERE " + y.ColumnName + " = @terms;";
                    OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                    cidCMD.Parameters.AddWithValue("@terms", date);
                    OleDbDataReader cidReader = cidCMD.ExecuteReader();

                    int score = 50;

                    while (cidReader.Read())
                    {
                        int thisID = cidReader.GetInt32(0);
                        if (thisID != pID)
                            if (DictRanked.ContainsKey(thisID)) { DictRanked[thisID] += score; }
                            else { DictRanked.Add(thisID, score); }
                    }

                    cidReader.Close();
                }

                MainWindow.CloseMainDBConnection();
            }
        }

        private Complainant GetComplainant(int id)
        {
            string strSQL = "SELECT * FROM tbl_Complainants WHERE ID = " + id.ToString() + ";";
            OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
            OleDbDataReader cidReader = cidCMD.ExecuteReader();
            
            Complainant thisComplainer = new Complainant();
            while(cidReader.Read())
            {
                thisComplainer.ID = cidReader.GetInt32(0);
                thisComplainer.LName = cidReader.GetString(1);
                thisComplainer.FName = cidReader.GetString(2);
                thisComplainer.AddressLine1 = cidReader.GetString(3);
                thisComplainer.AddressLine2 = cidReader.GetString(4);
                thisComplainer.City = cidReader.GetString(5);
                thisComplainer.State = GetState(cidReader.GetInt32(6));
                thisComplainer.Zip = cidReader.GetString(7);
                thisComplainer.Email = cidReader.GetString(8);
                thisComplainer.Phone = cidReader.GetString(9);
            }

            cidReader.Close();

            return thisComplainer;
        }

        private state GetState(int id)
        {
            var source = MainWindow.States.Cast<state>();
            IEnumerable<state> query = (from state in source where state.ID == id select state);
            return (state)query.First();
        }

        private void GetStringRanks(string searchterm)
        {
            MainWindow.OpenMainDBConnection();

            foreach (SearchLocation x in MainWindow.SearchList)
            {
                foreach (ColumnDetail y in x.StringColumns)
                {

                    string strSQLa = "SELECT b.ID FROM ";
                    string strSQLb = "((";
                    string strSQLc = x.CompType.ComplaintTable + " AS a LEFT JOIN tbl_Complaints AS b ON b.Type_SubID = a.ID) LEFT JOIN "
                        + x.CompType.SiteTable + " AS c ON c.ID = a.Site)";
                    if (x.CompType.PrimaryContactTable != "null")
                    {
                        strSQLb += "(";
                        strSQLc += " LEFT JOIN " + x.CompType.PrimaryContactTable + " AS d ON d.ID = a.PrimaryContact)";
                    }
                    if (x.CompType.SecondaryContactTable != "null")
                    {
                        strSQLb += "(";
                        strSQLc += " LEFT JOIN " + x.CompType.SecondaryContactTable + " AS e ON e.ID = a.SecondaryContact)";
                    }
                    if (x.CompType.TertiaryContactTable != "null")
                    {
                        strSQLb += "(";
                        strSQLc += " LEFT JOIN " + x.CompType.TertiaryContactTable + " AS f ON f.ID = a.TertiaryContact)";
                    }
                    if (x.CompType.QuaternaryContactTable != "null")
                    {
                        strSQLb += "(";
                        strSQLc += " LEFT JOIN " + x.CompType.QuaternaryContactTable + " AS g ON g.ID = a.QuaternaryContact)";
                    }
                    if (x.CompType.QuinaryContactTable != "null")
                    {
                        strSQLb += "(";
                        strSQLc += " LEFT JOIN " + x.CompType.QuinaryContactTable + " AS h ON h.ID = a.QuinaryContact)";
                    }
                    if (x.CompType.SenaryContactTable != "null")
                    {
                        strSQLb += "(";
                        strSQLc += " LEFT JOIN " + x.CompType.SenaryContactTable + " AS i ON i.ID = a.SenaryContact)";
                    }
                    strSQLc += " LEFT JOIN tbl_Complainants AS j ON j.ID = b.Complainant WHERE " + y.TableName + "." + y.ColumnName + " LIKE @terms;";
                    strSQLa += strSQLb + strSQLc;
                    OleDbCommand cidCMD = new OleDbCommand(strSQLa, MainWindow.cidDB);
                    cidCMD.Parameters.AddWithValue("@terms", "%" + searchterm + "%");
                    OleDbDataReader cidReader = cidCMD.ExecuteReader();

                    int score = 20;
                    if (y.ColumnName.Contains("Address")) score = 50;
                    else if (y.ColumnName.Contains("Name")) score = 25;
                    else if (y.ColumnName.Contains("Zip")) score = 30;
                    else if (y.ColumnName.Contains("Email")) score = 35;
                    else if (y.ColumnName.Contains("Phone")) score = 35;

                    while(cidReader.Read())
                    {
                        int thisID = cidReader.GetInt32(0);
                        if (thisID != pID)
                            if (DictRanked.ContainsKey(thisID)) { DictRanked[thisID] += score; }
                            else { DictRanked.Add(thisID, score); }
                    }

                    cidReader.Close();
                }
            }
            MainWindow.CloseMainDBConnection();
        }

        private void GetTypeRanks()
        {
            MainWindow.OpenMainDBConnection();

            foreach (ComplaintType z in ListTypes)
            {
                string strSQL = "SELECT ID FROM tbl_Complaints WHERE Complaint_Type = @typeid;";
                OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                cidCMD.Parameters.AddWithValue("@typeid", z.ID);
                OleDbDataReader cidReader = cidCMD.ExecuteReader();

                while (cidReader.Read())
                {
                    int thisID = cidReader.GetInt32(0);
                    if (thisID != pID)
                        if (DictRanked.ContainsKey(thisID)) DictRanked[thisID] *= 2;
                        else { DictRanked.Add(thisID, 50); }
                }
                cidReader.Close();
            }

            MainWindow.CloseMainDBConnection();
        }

        private void GetCityRanks()
        {
            MainWindow.OpenMainDBConnection();

            foreach (city x in ListCities)
            {
                foreach (ComplaintType y in MainWindow.ComplaintTypes)
                {
                    if (y.Label != "temp")
                    {
                        string strSQL = "SELECT a.pID FROM " + y.ComplaintTable + " AS a INNER JOIN " + y.SiteTable + " AS b ON a.Site = b.ID WHERE City = @cityid;";
                        OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                        cidCMD.Parameters.AddWithValue("@cityid", x.ID);
                        OleDbDataReader cidReader = cidCMD.ExecuteReader();

                        while (cidReader.Read())
                        {
                            int thisID = cidReader.GetInt32(0);
                            if (thisID != pID) 
                                if (DictRanked.ContainsKey(thisID)) { DictRanked[thisID] += 35; }
                                else { DictRanked.Add(thisID, 20); }
                        }
                        cidReader.Close();
                    }
                }
            }
            MainWindow.CloseMainDBConnection();
        }

        private void GetCountyRanks()
        {
            MainWindow.OpenMainDBConnection();

            foreach (county x in ListCounties)
            {
                foreach (ComplaintType y in MainWindow.ComplaintTypes)
                {
                    if (y.Label != "temp")
                    {
                        string strSQL = "SELECT a.pID FROM " + y.ComplaintTable + " AS a INNER JOIN " + y.SiteTable + " AS b ON a.Site = b.ID WHERE County = @countyid;";
                        OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                        cidCMD.Parameters.AddWithValue("@countyid", x.ID);
                        OleDbDataReader cidReader = cidCMD.ExecuteReader();

                        while (cidReader.Read())
                        {
                            int thisID = cidReader.GetInt32(0);
                            if (thisID != pID) 
                                if (DictRanked.ContainsKey(thisID)) { DictRanked[thisID] += 35; }
                                else { DictRanked.Add(thisID, 20); }
                        }

                        cidReader.Close();
                    }
                }
            }

            MainWindow.CloseMainDBConnection();
        }

        private void GetFacilityRanks()
        {
            //foreach (Facility x in ListFacilities)
            //{ MessageBox.Show(x.FacName); }
        }

        private void GetFDRanks()
        {
            MainWindow.OpenMainDBConnection();

            foreach (ComplaintType x in MainWindow.ComplaintTypes)
            {
                if (x.SearchTerms.Contains("open burn"))
                {
                    foreach (FireDepartment y in ListFireDepts)
                    {
                        string strSQL = "SELECT pID FROM " + x.ComplaintTable + " WHERE Fire_Department = @fd;";
                        OleDbCommand cidCMD = new OleDbCommand(strSQL, MainWindow.cidDB);
                        cidCMD.Parameters.AddWithValue("@fd", y.ID);
                        OleDbDataReader cidReader = cidCMD.ExecuteReader();

                        while (cidReader.Read())
                        {
                            int thisID = cidReader.GetInt32(0);
                            if (thisID != pID) 
                                if (DictRanked.ContainsKey(thisID)) { DictRanked[thisID] += 20; }
                                else { DictRanked.Add(thisID, 20); }
                        }

                        cidReader.Close();
                    }
                }
            }

            MainWindow.CloseMainDBConnection();
        }

        private void GetCities(string searchterms)
        {
            searchterms = searchterms.Replace("*", ".*");
            Regex regEx = new Regex(@searchterms, RegexOptions.IgnoreCase);

            var source = MainWindow.Cities.Cast<city>();
            IEnumerable<city> query;
            query = (from city in source .Where(county => regEx.IsMatch(county.Name)) orderby city.Name select city);
            ListCities.AddRange(query);
        }

        private void GetCounties(string searchterms)
        {
            searchterms = searchterms.Replace("*", ".*");
            Regex regEx = new Regex(@searchterms, RegexOptions.IgnoreCase);

            var source = MainWindow.Counties.Cast<county>();
            IEnumerable<county> query;
            query = (from county in source .Where(county => regEx.IsMatch(county.Name)) orderby county.Name select county);
            ListCounties.AddRange(query);
        }

        private void GetFireDepts(string searchterms)
        {
            searchterms = searchterms.Replace("*", ".*");
            Regex regEx = new Regex(@searchterms, RegexOptions.IgnoreCase);

            var source = MainWindow.FireDepartments.Cast<FireDepartment>();
            IEnumerable<FireDepartment> query;
            query = (from FireDepartment in source .Where(FireDepartment => regEx.IsMatch(FireDepartment.FDName)) orderby FireDepartment.FDName select FireDepartment);
            ListFireDepts.AddRange(query);
        }

        private void GetFacilities(string searchterms)
        {
            string temp = searchterms.Trim('*');
            bool isID = (MainWindow.IsNumeric(temp) && (temp.Length > 5));
            searchterms = searchterms.Replace("*", ".*");
            Regex regEx = new Regex(@searchterms, RegexOptions.IgnoreCase);

            var source = MainWindow.Facilities.Cast<Facility>();
            IEnumerable<Facility> query;

            if (isID) { query = (from Facility in source .Where(Facility => regEx.IsMatch(Facility.FacilityID)) orderby Facility.FacName select Facility); }
            else { query = (from Facility in source .Where(Facility => regEx.IsMatch(Facility.FacName)) orderby Facility.FacName select Facility); }

            ListFacilities.AddRange(query);
        }

        private void GetUsers(string searchterms)
        {
            searchterms = searchterms.Replace("*", ".*");
            Regex regEx = new Regex(@searchterms, RegexOptions.IgnoreCase);

            var source = MainWindow.Users.Cast<user>();
            IEnumerable<user> query;
            query = (from user in source .Where(user => regEx.IsMatch(user.Inits)) orderby user.Inits select user);
            ListUsers.AddRange(query);
        }

        private void GetTypes(string searchterms)
        {
            searchterms = searchterms.Replace("*", ".*");
            Regex regEx = new Regex(@searchterms, RegexOptions.IgnoreCase);

            var source = MainWindow.ComplaintTypes.Cast<ComplaintType>();
            IEnumerable<ComplaintType> query;
            query = (from ComplaintType in source where ComplaintType.SearchTerms.Any(x => regEx.IsMatch(x)) orderby ComplaintType.Label select ComplaintType);

            ListTypes.AddRange(query);
        }

        private List<string> ParseSearchTerms(string searchterms)
        {
            searchterms = searchterms.ToLower();

            string[] separators = {","};
            string[] strParsed = searchterms.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            List<string> lstParsed = strParsed.ToList<string>();

            for (int x = 0; x < lstParsed.Count; x++)
            {
                lstParsed[x] = lstParsed[x].Trim();
                GetTypes(lstParsed[x]);
                GetFireDepts(lstParsed[x]);
                GetFacilities(lstParsed[x]);
                GetCities(lstParsed[x]);
                GetCounties(lstParsed[x]);                
            }

            return lstParsed;
        }

        private void listComplaints_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RankedComplaint comp;
            DataGridRow row = sender as DataGridRow;

            if (row.Item != null)
            {
                comp = ((RankedComplaint)row.Item);

                if (comp.ID > 0)
                {
                    casestatus aStatus = new casestatus();
                    MainWindow.GetSingleItem<casestatus>(out aStatus, comp.Status, MainWindow.Statuses);

                    MiniComplaint minicomp = new MiniComplaint(pID, comp.ID, comp.DateReceived, aStatus, comp.Type, comp.ComplaintNotes, comp.FacName, comp.SiteInfo, true);
                    try { minicomp.ShowDialog(); }
                    catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); return; }
                }
            }
        }
    }
}

