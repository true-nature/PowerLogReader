using PowerLogReader.Modules;
using PowerLogReader.Modules.Services;
using PowerLogReader.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace PowerLogReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IPowerLogService, PowerLogReaderService>();
            containerRegistry.RegisterSingleton<IPreferenceService, PreferenceService>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<PowerLogModule>();
        }
    }
}
