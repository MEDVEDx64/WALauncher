using WALauncher.Utils;
using WALauncher.Wapkg;

namespace WALauncher.ViewModels.WapkgObjects
{
    public class AvailablePackage : Package
    {
        string targetDistro;

        public DelegateCommand InstallCommand { get; }
        public bool ButtonEnabled { get; private set; }

        public AvailablePackage(string targetDistro)
        {
            this.targetDistro = targetDistro;
            InstallCommand = new DelegateCommand(Install);
            ButtonEnabled = true;
        }

        void Install()
        {
            InteractionService.Get().InstallPackage(targetDistro, Name);
            ButtonEnabled = false;
            RaisePropertyChanged(nameof(ButtonEnabled));
        }
    }
}
