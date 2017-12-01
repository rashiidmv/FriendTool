using Friend.Infra;
using Microsoft.Practices.Prism.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System;
using System.Windows;
using System.Windows.Threading;
using System.Threading;

namespace DataLists.Content
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        private bool isOverride;

        public bool IsOverride
        {
            get { return isOverride; }
            set
            {
                isOverride = value;
                OnPropertyChanged("IsOverride");
                IsBusyIndicator = true;
                if (isOverride == true && OverrideDataListSet == null)
                {
                    MainViewModel_initialize();
                }
                else if (isOverride == true)
                {
                    OriginalDataListSet.AddRange(OverrideDataListSet);
                    IsBusyIndicator = false;
                }
                else if (OverrideDataListSet != null)
                {
                    RemoveOverrideItems();
                    IsBusyIndicator = false;
                }
                if (CanExecuteSearch())
                {
                    ExecuteSearch();
                }
            }
        }

        private void RemoveOverrideItems()
        {
            foreach (DataListResult item in OverrideDataListSet)
            {
                if (OriginalDataListSet.Contains(item))
                {
                    OriginalDataListSet.Remove(item);
                }
            }
        }

        private List<DataListResult> OriginalDataListSet;
        private List<DataListResult> OverrideDataListSet;
        private string title;

        public string Title
        {
            get { return "Data Lists"; }
            set { title = value; }
        }

        private bool isBusyIndicator;

        public bool IsBusyIndicator
        {
            get { return isBusyIndicator; }
            set
            {
                isBusyIndicator = value;
                OnPropertyChanged("IsBusyIndicator");
            }
        }
        private bool isBusyWholeContent;

        public bool IsBusyWholeContent
        {
            get { return isBusyWholeContent; }
            set
            {
                isBusyWholeContent = value;
                OnPropertyChanged("IsBusyWholeContent");
            }
        }

        private string searchText;

        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                OnPropertyChanged("SearchText");
                searchCommand.RaiseCanExecuteChanged();
                if (CanExecuteSearch())
                {
                    ExecuteSearch();
                }
                else
                {
                    DataListResultSet = new ObservableCollection<DataListResult>(OriginalDataListSet);
                }
            }
        }

        private ObservableCollection<DataListResult> dataListResultSet;

        public ObservableCollection<DataListResult> DataListResultSet
        {
            get { return dataListResultSet; }
            set
            {
                dataListResultSet = value;
                OnPropertyChanged("DataListResultSet");

            }
        }


        private DelegateCommand searchCommand;

        public DelegateCommand SearchCommand
        {
            get { return searchCommand; }
            set { searchCommand = value; }
        }


        private DelegateCommand<DataListResult> copyXMLCommand;

        public DelegateCommand<DataListResult> CopyXMLCommand
        {
            get { return copyXMLCommand; }
            set { copyXMLCommand = value; }
        }

        private DelegateCommand<DataListResult> copyTextCommand;

        public DelegateCommand<DataListResult> CopyTextCommand
        {
            get { return copyTextCommand; }
            set { copyTextCommand = value; }
        }


        public delegate void Initialize();
        public event Initialize initialize = delegate { };

        public MainViewModel()
        {
            SearchCommand = new DelegateCommand(ExecuteSearch, CanExecuteSearch);
            CopyXMLCommand = new DelegateCommand<DataListResult>(ExecuteCopyXML, CanExecuteCopyXML);
            CopyTextCommand = new DelegateCommand<DataListResult>(ExecuteCopyText, CanExecuteCopyText);
            initialize += MainViewModel_initialize;
            IsOverride = false;
            IsBusyWholeContent = true;
        }

        private async void MainViewModel_updateDataListResultSet(List<DataListResult> dataListResultSet)
        {
            await Task.Run(() =>
            {
                DataListResultSet = new ObservableCollection<DataListResult>(dataListResultSet);
                IsBusyIndicator = false;
            }
              );
        }

        private void MainViewModel_loadSearchResult()
        {
            Thread.Sleep(500);
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                DataListResultSet = new ObservableCollection<DataListResult>(temp);

            });
        }
        List<DataListResult> temp = null;
        public async void MainViewModel_initialize()
        {
            Task<List<DataListResult>> task = new Task<List<DataListResult>>(LoadFromXml);
            task.Start();
            await task;
            IsBusyWholeContent = false;
            temp = task.Result;
            Task task1 = new Task(MainViewModel_loadSearchResult);
            task1.Start();
            await task1;
            IsBusyIndicator = false;
        }

        private bool CanExecuteCopyText(DataListResult dataListResult)
        {
            return dataListResult != null;
        }

        private void ExecuteCopyText(DataListResult dataListResult)
        {
            Clipboard.SetText(dataListResult.CommandText);
        }

        private bool CanExecuteCopyXML(DataListResult dataListResult)
        {
            return dataListResult != null;
        }

        private void ExecuteCopyXML(DataListResult dataListResult)
        {
            Clipboard.SetText("rashid");
        }

        private bool CanExecuteSearch()
        {
            return SearchText != null && SearchText != String.Empty;
        }

        private async void ExecuteSearch()
        {
            List<DataListResult> searchResult = new List<DataListResult>();
            await Task.Run(() =>
            {
                foreach (DataListResult item in OriginalDataListSet)
                {
                    if (item.DataListName.ToUpper().Contains(SearchText.ToUpper()))
                    {
                        searchResult.Add(item);
                    }
                }
            }
           );
            Application.Current.Dispatcher.Invoke(() =>
            {
                DataListResultSet.Clear();
                if (searchResult.Count == 0)
                {
                    searchResult.Add(new DataListResult() { ResultStatus = "NotFound" });
                }
                DataListResultSet = new ObservableCollection<DataListResult>(searchResult);
                IsBusyIndicator = false;
            });
        }

        private List<DataListResult> LoadFromXml()
        {
            Thread.Sleep(500);
            List<DataListResult> temp = null;
            //string source = @"C:\Users\rvayalil001\Documents\Rashid\P66\ConfigurationData\System\DataListConfiguration.config";
            string source = @"C:\Users\rvayalil001\Documents\Rashid\P66\ConfigurationData\System\DataListConfiguration.config";
            if (IsOverride)
            {
                source = @"C:\Users\rvayalil001\Documents\Rashid\P66\ConfigurationData\System\Override\DataListConfiguration.config";
            }
            string dataLists = File.ReadAllText(source);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(dataLists);
            if (doc.HasChildNodes)
            {
                foreach (XmlNode item in doc.ChildNodes)
                {
                    if (item.Name.Equals("DataListConfiguration"))
                    {
                        temp = new List<DataListResult>();
                        if (item.HasChildNodes)
                        {
                            foreach (XmlNode dataListNode in item.ChildNodes)
                            {
                                if (dataListNode.Name.Equals("SqlDataList"))
                                {
                                    DataListResult dataList = new DataListResult();
                                    if (dataListNode.HasChildNodes)
                                    {
                                        foreach (XmlNode child in dataListNode.ChildNodes)
                                        {
                                            if (child.InnerText.Trim() != String.Empty)
                                            {
                                                if (child.Name.Equals("DataListName"))
                                                {
                                                    dataList.DataListName = child.InnerText.Trim();
                                                }
                                                else if (child.Name.Equals("DisplayName"))
                                                {
                                                    dataList.DisplayName = child.InnerText.Trim();
                                                }
                                                else if (child.Name.Equals("CommandText"))
                                                {
                                                    dataList.CommandText = child.InnerText.Trim();
                                                }
                                                else if (child.Name.Equals("CacheBehavior"))
                                                {
                                                    dataList.CacheBehavior = child.InnerText.Trim();
                                                }
                                                else if (child.Name.Equals("KeyColumnName"))
                                                {
                                                    dataList.KeyColumnName = child.InnerText.Trim();
                                                }
                                                else if (child.Name.Equals("DefaultDisplayColumnName"))
                                                {
                                                    dataList.DefaultDisplayColumnName = child.InnerText.Trim();
                                                }
                                            }
                                        }
                                    }
                                    temp.Add(dataList);
                                }
                            }
                        }
                    }
                }
            }
            if (temp != null && temp.Count > 0)
            {
                if (IsOverride && OverrideDataListSet == null)
                {
                    OverrideDataListSet = temp;
                    OriginalDataListSet.AddRange(OverrideDataListSet);
                }
                else
                {
                    OriginalDataListSet = temp;
                }
            }
            return temp;
        }
        private void DummyFunctionToMakeSureReferencesGetCopiedProperly_DO_NOT_DELETE_THIS_CODE()
        {
            // In order to get the required assemblies copied over, I add some dummy code here (that never
            // gets called) that references following assemblies; this will flag VS/MSBuild to copy the required assemblies over as well.
            var dummyType = typeof(Xceed.Wpf.Toolkit.BusyIndicator);
            String s = dummyType.FullName;
            var dummyType2 = typeof(Microsoft.Expression.Interactivity.Input.KeyTrigger);
            s = dummyType2.FullName;
        }
    }
}
