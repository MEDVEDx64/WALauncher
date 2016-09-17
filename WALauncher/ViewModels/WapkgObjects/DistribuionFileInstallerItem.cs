using Microsoft.Win32;
using WALauncher.Utils;
using WALauncher.Wapkg;

namespace WALauncher.ViewModels.WapkgObjects
{
    public class DistribuionFileInstallerItem : ViewModelBase
    {
        public string SuggestedName { get; set; }
        public DelegateCommand OpenFileCommand { get; }

        public DistribuionFileInstallerItem()
        {
            OpenFileCommand = new DelegateCommand(OpenFile);
            SuggestedName = "";
        }

        void OpenFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Worms Armageddon distros (*.wadist)|*.wadist|All files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                InteractionService.Get().InstallDistribution(dialog.FileName, SuggestedName.Length == 0 ? null : SuggestedName);
            }
        }
    }
}
