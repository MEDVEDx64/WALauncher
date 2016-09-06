using WALauncher.Utils;
using WALauncher.Wapkg;

namespace WALauncher.ViewModels.WapkgObjects
{
    public class AvailablePackage : Package
    {
        string targetDistro;

        public DelegateCommand InstallCommand { get; }

        public AvailablePackage(string targetDistro)
        {
            this.targetDistro = targetDistro;
            InstallCommand = new DelegateCommand(Install);
        }

        void Install()
        {
            InteractionService.Get().InstallPackage(targetDistro, Name);
        }
    }
}
