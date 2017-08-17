using Friend.Infra;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Regions;
using DataLists.Content;

namespace DataLists
{
    public class DataListModule : ModuleBase
    {
        public DataListModule(IUnityContainer uc, IRegionManager rm) : base(uc, rm)
        {
        }
        protected override void InitializeModules()
        {
            MyRegionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(Main));
   //         MyRegionManager.RequestNavigate(RegionNames.ContentRegion, typeof(Main).FullName);
        }

        protected override void RegisterTypes()
        {
            MyUnityContainer.RegisterType<IMainViewModel, MainViewModel>();
            MyUnityContainer.RegisterType<object, Main>(typeof(Main).FullName);
        }
        
    }
}
