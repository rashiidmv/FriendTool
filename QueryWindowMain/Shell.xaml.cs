using Friend.Infra;
using System;
using System.Windows;

namespace QueryWindowMain
{

    public partial class Shell : Window, IView
    {
        public Shell(IShellViewModel vm)
        {
            InitializeComponent();
            //var myResourceDictionary = new ResourceDictionary();
            //myResourceDictionary.Source = new Uri("pack://application:,,,/Friend.Infra;Component/FriendResources.xaml", UriKind.Absolute);
            //Resources.MergedDictionaries.Add(myResourceDictionary);
            ViewModel = vm;
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
