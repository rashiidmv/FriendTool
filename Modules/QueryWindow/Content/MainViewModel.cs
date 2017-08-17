using Friend.Infra;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QueryWindow.Content
{
    //Provider=SQLOLEDB.1;Password=Lifeis4GoodGood;Persist Security Info=True;User ID = sa
    //Provider=SQLOLEDB.1;Integrated Security = SSPI; Persist Security Info=False
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private bool IsServerChanged = false;
        private String ConnectionString = String.Empty;
        private string title;

        public string Title
        {
            get { return "Query Window"; }
            set { title = value; }
        }


        private List<string> dataBaseNames;

        public List<string> DataBaseNames
        {
            get { return dataBaseNames; }
            set
            {
                dataBaseNames = value;
                OnPropertyChanged("DataBaseNames");
            }
        }

        private string serverName;

        public string ServerName
        {
            get { return serverName?.Trim(); }
            set
            {
                serverName = value;
                OnPropertyChanged("ServerName");
                GetDbNamesCommand.RaiseCanExecuteChanged();
                if (value == String.Empty)
                {
                    DataBaseNames = null;
                }
                CanSelectDB = false;
                CurrentDatabase = String.Empty;
                QueryResultStatus = String.Empty;
                BusyStatus = Visibility.Collapsed;
                QueryResultStatusToolTip = String.Empty;
                IsServerChanged = true;
            }
        }


        private DelegateCommand<object> getDbNamesCommand;

        public DelegateCommand<object> GetDbNamesCommand
        {
            get { return getDbNamesCommand; }
            set { getDbNamesCommand = value; }
        }

        private DelegateCommand executeQueryCommand;

        public DelegateCommand ExecuteQueryCommand
        {
            get { return executeQueryCommand; }
            set { executeQueryCommand = value; }
        }
        private DelegateCommand<QueryResult> deleteResultCommand;

        public DelegateCommand<QueryResult> DeleteResultCommand
        {
            get { return deleteResultCommand; }
            set { deleteResultCommand = value; }
        }
        private DelegateCommand<QueryResult> editResultCommand;

        public DelegateCommand<QueryResult> EditResultCommand
        {
            get { return editResultCommand; }
            set { editResultCommand = value; }
        }

        private string queryResultStatus;

        public string QueryResultStatus
        {
            get { return queryResultStatus; }
            set
            {
                queryResultStatus = value;
                OnPropertyChanged("QueryResultStatus");
            }
        }

        private string queryString;

        public string QueryString
        {
            get { return queryString; }
            set
            {
                previous = queryString;
                queryString = value;
                OnPropertyChanged("QueryString");
                CheckExistence(value);
                ExecuteQueryCommand.RaiseCanExecuteChanged();
            }
        }
        string previous = string.Empty;

        private async void CheckExistence(string value)
        {
            await Task.Run(() =>
            {
                SearchResults = new List<string>();
                //int spaceIndex = 0;
                //string temp = value;
                //while (temp.Contains(" "))
                //{
                //    spaceIndex = spaceIndex + temp.IndexOf(" ");
                //    if (spaceIndex > CursorPosition)
                //    {
                //        spaceIndex = spaceIndex - temp.IndexOf(" ");
                //        break;
                //    }
                //    temp = temp.Substring(spaceIndex, temp.Length);
                //}
                //string oldString = value.Substring(spaceIndex, spaceIndex + CursorPosition);
                String temp = value;


                previous = previous + "";
                int index = 0;
                if (previous.Length < temp.Length)
                    index = previous.Zip(temp, (c1, c2) => c1 == c2).TakeWhile(b => b).Count() + 1;
                else
                {
                    index = temp.Zip(previous, (c1, c2) => c1 == c2).TakeWhile(b => b).Count() + 1;
                    index = index - 1;
                }
                int spaceIndex = 0;
                string anotherTemp = string.Empty;
                if (temp != String.Empty && temp.Contains(" ") && temp.IndexOf(" ") <= index && index < previous.Length)
                {
                    anotherTemp = temp.Substring(0, index);
                    while (anotherTemp.Contains(" "))
                    {
                        spaceIndex = spaceIndex + anotherTemp.IndexOf(" ");
                        spaceIndex++;
                        if (spaceIndex > index)
                        {
                            break;
                        }
                        anotherTemp = anotherTemp.Substring(anotherTemp.IndexOf(" ") + 1);
                    }

                    anotherTemp = anotherTemp + "";
                }

                if (anotherTemp.Trim() != String.Empty)
                    temp = anotherTemp;
                else if (temp.Contains(" "))
                    temp = value.Substring(value.LastIndexOf(" "));
                foreach (string item in tableNames)
                {
                    if (temp.Trim() == String.Empty)
                        break;
                    if (item.Trim().ToUpper().StartsWith(temp.Trim().ToUpper()))
                        SearchResults.Add(item);
                }
                if (SearchResults.Count > 0)
                {
                    IsFoundInSearch = true;
                }
                else
                {
                    IsFoundInSearch = false;
                }
            });
        }

        private string queryResultStatusToolTip;

        public string QueryResultStatusToolTip
        {
            get
            {
                if (queryResultStatusToolTip != String.Empty)
                    return queryResultStatusToolTip;
                else
                    return null;
            }
            set
            {
                queryResultStatusToolTip = value;
                OnPropertyChanged("QueryResultStatusToolTip");
            }
        }



        private String currentDatabase;

        public String CurrentDatabase
        {
            get { return currentDatabase; }
            set
            {
                string previousValue = currentDatabase;
                currentDatabase = value;
                OnPropertyChanged("CurrentDatabase");
                ExecuteQueryCommand.RaiseCanExecuteChanged();
                if (currentDatabase != null && currentDatabase != String.Empty && !currentDatabase.Equals(previousValue))
                {
                    tableNames = new List<string>();
                    GetAllTables();
                }
            }
        }

        private bool canSelectDB;

        public bool CanSelectDB
        {
            get { return canSelectDB; }
            set
            {
                canSelectDB = value;
                OnPropertyChanged("CanSelectDB");
                ServerConfigHeader = "Connect to database server";
            }

        }

        private bool holdLastResult;

        public bool HoldLastResult
        {
            get { return holdLastResult; }
            set
            {
                holdLastResult = value;
                OnPropertyChanged("HoldLastResult");
            }
        }

        private ObservableCollection<QueryResult> queryAndResult;

        public ObservableCollection<QueryResult> QueryAndResult
        {
            get { return queryAndResult; }
            set
            {
                queryAndResult = value;
                OnPropertyChanged("QueryAndResult");
            }
        }

        private string queryResultStatusColor;

        public string QueryResultStatusColor
        {
            get { return queryResultStatusColor; }
            set
            {
                queryResultStatusColor = value;
                OnPropertyChanged("QueryResultStatusColor");
            }
        }

        private string serverConfigHeader;
        public string ServerConfigHeader
        {
            get
            {
                if (CanSelectDB)
                {
                    return "Connected to " + ServerName;
                }
                else
                {
                    return serverConfigHeader;
                }
            }
            set
            {
                serverConfigHeader = value;
                OnPropertyChanged("ServerConfigHeader");
            }
        }

        private bool isSqlAuthentication;

        public bool IsSqlAuthentication
        {
            get { return isSqlAuthentication; }
            set
            {
                isSqlAuthentication = value;
                OnPropertyChanged("IsSqlAuthentication");
                //DataBaseNames = null;
                if (DataBaseNames != null && DataBaseNames.Count > 0)
                {
                    CanSelectDB = false;
                }
                CurrentDatabase = null;
            }
        }

        private string userName;

        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                OnPropertyChanged("UserName");
            }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        private List<string> searchResults;

        public List<string> SearchResults
        {
            get { return searchResults; }
            set
            {
                searchResults = value;
                OnPropertyChanged("SearchResults");
            }
        }

        private bool isFoundInSearch;

        public bool IsFoundInSearch
        {
            get { return isFoundInSearch; }
            set
            {
                isFoundInSearch = value;
                OnPropertyChanged("IsFoundInSearch");
            }
        }

        private int cursorPosition;

        public int CursorPosition
        {
            get { return cursorPosition; }
            set { cursorPosition = value; }
        }

        private Visibility busyStatus;

        public Visibility BusyStatus
        {
            get { return busyStatus; }
            set {
                busyStatus = value;
                OnPropertyChanged("busyStatus");
            }
        }


        public MainViewModel()
        {
            
            GetDbNamesCommand = new DelegateCommand<object>(GetDataBaseNames, CanGetDataBaseNames);
            ExecuteQueryCommand = new DelegateCommand(ExecuteQuery, CanExecuteQuery);
            DeleteResultCommand = new DelegateCommand<QueryResult>(ExecuteDelete, CanExecuteDelete);
            EditResultCommand = new DelegateCommand<QueryResult>(ExecuteEdit, CanExecuteEdit);
            ServerConfigHeader = "Connect to database server";
            BusyStatus = Visibility.Collapsed;
            //Temporary code
            //ServerName = "rvayalil00190";
            //QueryString = "Select * from categories";
            //GetDataBaseNames();
        }


        private bool CanExecuteEdit(QueryResult parameter)
        {
            return true;
        }

        private void ExecuteEdit(QueryResult parameter)
        {
            IsSqlAuthentication = parameter.IsSQLAuthenticated;
            CurrentDatabase = parameter.Database;
            QueryString = parameter.Query;
            if (parameter.IsSQLAuthenticated)
            {
                Password = parameter.Password;
                UserName = parameter.Username;
            }
            if (DataBaseNames.Count > 0)
            {
                CanSelectDB = true;
            }
        }

        private bool CanExecuteDelete(QueryResult parameter)
        {
            return true;
        }

        private void ExecuteDelete(QueryResult parameter)
        {
            QueryAndResult.Remove(parameter);
        }

        private bool CanExecuteQuery()
        {
            return QueryString != null && QueryString != String.Empty
                && CurrentDatabase != null && CurrentDatabase != String.Empty;
        }

        private void ExecuteQuery()
        {
            if (CurrentDatabase != null && currentDatabase != String.Empty)
            {
                QueryResultStatusColor = "Blue";
                QueryResultStatus = "Executing Query..!";
                BusyStatus = Visibility.Visible;
                QueryResultStatusToolTip = QueryResultStatus;
                String connectionString = "Data Source=" + ServerName + ";Initial Catalog=" + CurrentDatabase;
                if (CreateConnectionString(connectionString))
                {
                    DataSet dbDataSet = new DataSet();
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(ConnectionString))
                        {
                            conn.Open();
                            using (SqlCommand cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = QueryString.Trim();
                                cmd.CommandType = CommandType.Text;
                                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                                {
                                    da.Fill(dbDataSet);
                                }
                            }
                        }
                        if (!HoldLastResult || QueryAndResult == null)
                        {
                            QueryAndResult = new ObservableCollection<QueryResult>();
                        }
                        QueryResult qr = new QueryResult();
                        qr.Server = ServerName;
                        qr.Database = CurrentDatabase;
                        qr.IsSQLAuthenticated = IsSqlAuthentication;
                        if (IsSqlAuthentication)
                        {
                            qr.Password = Password;
                            qr.Username = UserName;
                        }
                        qr.Query = QueryString.Trim();
                        qr.Result = dbDataSet.Tables[0].DefaultView;
                        QueryAndResult.Add(qr);
                    }
                    catch (Exception ex)
                    {
                        FormatExceptionMessage(ex);
                        return;
                    }
                    QueryResultStatusColor = "Green";
                    QueryResultStatus = "Query Executed Successfully..!";
                    BusyStatus = Visibility.Collapsed;
                    QueryResultStatusToolTip = QueryResultStatus;
                }
            }
        }

        private void FormatExceptionMessage(Exception ex)
        {
            string exceptionMessage = ex.Message.Trim();
            if (exceptionMessage.Contains(".") && exceptionMessage.Split('.').Length > 2)
            {
                QueryResultStatusToolTip = String.Empty;
                foreach (String item in exceptionMessage.Split('.'))
                {
                    if (item != String.Empty)
                        QueryResultStatusToolTip = QueryResultStatusToolTip + item + "\n";
                }
                if (QueryResultStatusToolTip.EndsWith("\n"))
                    QueryResultStatusToolTip = QueryResultStatusToolTip.Remove(QueryResultStatusToolTip.Length - 1);
            }
            else
            {
                QueryResultStatusToolTip = ex.Message;
            }
            QueryResultStatusColor = "Red";
            QueryResultStatus = ex.Message;
            BusyStatus = Visibility.Collapsed;
            return;
        }

        private bool CanGetDataBaseNames(object parameter)
        {
            return ServerName != null && ServerName != String.Empty;
        }

        private async void GetDataBaseNames(object parameter)
        {
            PasswordBox passwordBox = parameter as PasswordBox;
            if (passwordBox.Password != null && passwordBox.Password != String.Empty)
            {
                Password = passwordBox.Password;
            }
            await Task.Run(() =>
            {
                if (ServerName != null && ServerName != String.Empty)
                {
                    if (IsServerChanged || (IsSqlAuthentication && (UserName != null || Password != null)))
                    {
                        List<string> dataBaseNames = new List<string>();
                        QueryResultStatusColor = "Blue";
                        QueryResultStatus = "Connecting to server..!";
                        BusyStatus = Visibility.Visible;
                        QueryResultStatusToolTip = QueryResultStatus;
                        String connectionString = "Data Source=" + ServerName;
                        if (CreateConnectionString(connectionString))
                        {
                            DataSet dbDataSet = new DataSet();
                            try
                            {
                                using (SqlConnection conn = new SqlConnection(ConnectionString))
                                {
                                    conn.Open();
                                    using (SqlCommand cmd = conn.CreateCommand())
                                    {
                                        cmd.CommandText = "SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb')";
                                        cmd.CommandType = CommandType.Text;
                                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                                        {
                                            da.Fill(dbDataSet);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                FormatExceptionMessage(ex);
                                return;
                            }
                            QueryResultStatusColor = "Green";
                            QueryResultStatus = "Connected to server Successfully..!";
                            BusyStatus = Visibility.Collapsed;
                            QueryResultStatusToolTip = QueryResultStatus;
                            foreach (DataRow item in dbDataSet.Tables[0].Rows)
                            {
                                dataBaseNames.Add(item[0].ToString());
                            }
                            DataBaseNames = dataBaseNames;
                            IsServerChanged = false;
                        }
                    }
                    CanSelectDB = true;
                }
            });
        }

        private bool CreateConnectionString(string connectionString)
        {
            if (IsSqlAuthentication)
            {
                if (UserName == null || UserName == String.Empty ||
                    Password == null || Password == String.Empty)
                {
                    QueryResultStatusColor = "Red";
                    QueryResultStatus = "Username or Password cann't be empty..!";
                    return false;
                }
                connectionString = connectionString + ";Integrated Security=False" + ";Password=" + Password + ";User ID=" + UserName;
            }
            else
            {
                connectionString = connectionString + ";Integrated Security=True";
            }
            ConnectionString = connectionString;
            return true;
        }

        List<string> tableNames = new List<string>();
        private async void GetAllTables()
        {
            await Task.Run(() =>
            {
                if (ServerName != null && ServerName != String.Empty)
                {
                    String connectionString = "Data Source=" + ServerName + ";Initial Catalog=" + CurrentDatabase;
                    if (CreateConnectionString(connectionString))
                    {
                        DataSet dbDataSet = new DataSet();
                        try
                        {
                            using (SqlConnection conn = new SqlConnection(ConnectionString))
                            {
                                conn.Open();
                                using (SqlCommand cmd = conn.CreateCommand())
                                {
                                    cmd.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'";
                                    cmd.CommandType = CommandType.Text;
                                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                                    {
                                        da.Fill(dbDataSet);
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            return;
                        }
                        foreach (DataRow item in dbDataSet.Tables[0].Rows)
                        {
                            tableNames.Add(item[0].ToString());
                        }
                    }
                }
            });
        }

    }
}
