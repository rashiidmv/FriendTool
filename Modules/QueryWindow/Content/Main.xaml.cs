using Friend.Infra;
using System.Windows;
using System.Windows.Controls;

namespace QueryWindow.Content
{
    public partial class Main : UserControl, IView
    {
        public Main(IMainViewModel vm)
        {
            InitializeComponent();
            ViewModel = vm;
        }

        public IViewModel ViewModel
        {
            get
            {
                return (IViewModel)DataContext; ;
            }

            set
            {
                DataContext = value;
            }
        }

    }
}
