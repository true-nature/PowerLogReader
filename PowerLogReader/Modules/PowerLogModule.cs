using PowerLogReader.Core;
using PowerLogReader.Modules.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace PowerLogReader.Modules
{
    public class PowerLogModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public PowerLogModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.MenuRegion, nameof(MenuControl));
            _regionManager.RequestNavigate(RegionNames.PowerLogRegion, nameof(PowerLogControl));
            _regionManager.RequestNavigate(RegionNames.CalendarRegion, nameof(CalendarControl));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MenuControl>();
            containerRegistry.RegisterForNavigation<CalendarControl>();
            containerRegistry.RegisterForNavigation<PowerLogControl>();
            containerRegistry.RegisterDialog<SettingsDialog>(nameof(SettingsDialog));
            containerRegistry.RegisterDialog<AboutBoxControl>(nameof(AboutBoxControl));
        }
    }
}