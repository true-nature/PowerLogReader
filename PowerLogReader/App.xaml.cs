using PowerLogReader.Modules;
using PowerLogReader.Modules.Services;
using PowerLogReader.Views;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Reflection;
using System.Threading;
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
            CreateLock();
            if (LockObject == null)
            {
                return null;
            }
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

        private Mutex LockObject;

        private void CreateLock()
        {
            var created = false;
            try
            {
                var name = Assembly.GetEntryAssembly().GetName().Name;
                LockObject = new Mutex(true, name, out created);
            }
            catch (Exception)
            {
                created = false;
            }
            if (!created)
            {
                LockObject?.Close();
                LockObject = null;
                Application.Current.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            LockObject?.ReleaseMutex();
            LockObject?.Close();
            LockObject = null;
        }
    }
}
