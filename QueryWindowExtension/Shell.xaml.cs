namespace QueryWindowExtension
{
    using Friend.Infra;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Resources;
    public partial class Shell : UserControl, IView
    {
        public Shell(IShellViewModel vm)
        {
            InitializeComponent();
            //var myResourceDictionary = new ResourceDictionary();
            //myResourceDictionary.Source = new Uri("pack://application:,,,/Friend.Infra;Component/FriendResources.xaml", UriKind.Absolute);
            //Resources.MergedDictionaries.Add(myResourceDictionary);
            ViewModel = vm;
            ViewMode = FlowDocumentReaderViewingMode.Scroll;

        }

        private FlowDocumentReaderViewingMode viewMode;

        public FlowDocumentReaderViewingMode ViewMode
        {
            get { return viewMode; }
            set
            {
                viewMode = value;
            }
        }

        public IViewModel ViewModel
        {
            get
            {
                return (IViewModel)DataContext;
            }

            set
            {
                DataContext = value;
            }
        }
    }
}
