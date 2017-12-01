namespace FriendExtension
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
            DummyFunctionToMakeSureReferencesGetCopiedProperly_DO_NOT_DELETE_THIS_CODE();
            InitializeComponent();
            //var myResourceDictionary = new ResourceDictionary();
            //myResourceDictionary.Source = new Uri("pack://application:,,,/Friend.Infra;Component/FriendResources.xaml", UriKind.Absolute);
            //Resources.MergedDictionaries.Add(myResourceDictionary);
            ViewModel = vm;
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
