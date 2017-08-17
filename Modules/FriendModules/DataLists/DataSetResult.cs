using Friend.Infra;

namespace DataLists
{
    public class Result : ViewModelBase
    {
        public Result()
        {
            ResultStatus = "Found";
        }
        private string resultStatus;

        public string ResultStatus
        {
            get { return resultStatus; }
            set { resultStatus = value; }
        }

    }
    public class DataListResult : Result
    {
        private string dataListName;

        public string DataListName
        {
            get { return dataListName; }
            set {
                dataListName = value;
                OnPropertyChanged("DataListName");
            }
        }

        private string displayName;

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        private string commandText;

        public string CommandText
        {
            get { return commandText; }
            set { commandText = value;
                OnPropertyChanged("CommandText");
            }
        }

        private string cacheBehavior;

        public string CacheBehavior
        {
            get { return cacheBehavior; }
            set { cacheBehavior = value;
                OnPropertyChanged("CacheBehavior");
            }
        }

        private string keyColumnName;

        public string KeyColumnName
        {
            get { return keyColumnName; }
            set { keyColumnName = value;
                OnPropertyChanged("KeyColumnName");
            }
        }

        private string defaultDisplayColumnName;

        public string DefaultDisplayColumnName
        {
            get { return defaultDisplayColumnName; }
            set { defaultDisplayColumnName = value;
                OnPropertyChanged("DefaultDisplayColumnName");
            }
        }


        public DataListResult()
        {
            DataListName = DisplayName= CommandText= CacheBehavior= KeyColumnName= DefaultDisplayColumnName="Null";
        }
    }   
}
