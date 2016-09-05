using System.Collections.ObjectModel;
using WALauncher.Utils;
using WALauncher.ViewModels.WapkgObjects;
using WALauncher.Wapkg;

namespace WALauncher.ViewModels
{
    public class PackageInstallerViewModel : ViewModelBase
    {
        public PackageInstallerViewModel()
        {
            Items = new ObservableCollection<Distribution>();
        }

        public ObservableCollection<Distribution> Items { get; private set; }

        public void NotifyItemsChanged()
        {
            RaisePropertyChanged(nameof(Items));
        }
    }
}
