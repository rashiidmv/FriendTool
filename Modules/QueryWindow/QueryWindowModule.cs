using Friend.Infra;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using QueryWindow.Navigation;
using QueryWindow.Content;

namespace QueryWindow
{
    public class QueryWindowModule : ModuleBase
    {
        public QueryWindowModule(IUnityContainer uc, IRegionManager rm) : base(uc, rm)
        {

        }
        protected override void InitializeModules()
        {
            MyRegionManager.RegisterViewWithRegion(RegionNames.ToolbarRegion, typeof(Open));
            //  MyRegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(ViewA));
            MyRegionManager.RequestNavigate(RegionNames.ContentRegion, typeof(Main).FullName);
        }

        protected override void RegisterTypes()
        {
            MyUnityContainer.RegisterType<IMainViewModel, MainViewModel>();
            MyUnityContainer.RegisterType<object, Main>(typeof(Main).FullName);
        }
    }
}
