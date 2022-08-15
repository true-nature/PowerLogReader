using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Reflection;
using System.Windows.Input;

namespace PowerLogReader.Modules.ViewModels
{
    public class AboutBoxControlViewModel : BindableBase, IDialogAware
    {
        public Version Version { get; }
        public ICommand OkCommand { get; }
        public string Copyright { get; }

        public AboutBoxControlViewModel()
        {
            var assembly = Assembly.GetEntryAssembly();
            Version = assembly.GetName().Version;
            Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright ?? string.Empty;
            OkCommand = new DelegateCommand(() =>
            {
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            });
        }

        public string Title => "About this application";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
    }
}
