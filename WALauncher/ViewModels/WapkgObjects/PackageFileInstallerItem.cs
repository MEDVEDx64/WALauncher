using Microsoft.Win32;
using WALauncher.Utils;
using WALauncher.Wapkg;

namespace WALauncher.ViewModels.WapkgObjects
{
    public class PackageFileInstallerItem : ViewModelBase
    {
        string targetDistro;

        public DelegateCommand OpenFileCommand { get; }

        public PackageFileInstallerItem(string targetDistro)
        {
            this.targetDistro = targetDistro;
            OpenFileCommand = new DelegateCommand(OpenFile);
        }

        void OpenFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Worms Armageddon packages (*.wapkg)|*.wapkg|All files (*.*)|*.*";
            if(dialog.ShowDialog() == true)
            {
                InteractionService.Get().InstallPackage(targetDistro, dialog.FileName);
            }
        }
    }
}
