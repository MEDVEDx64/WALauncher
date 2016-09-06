using System.Collections.ObjectModel;
using WALauncher.Utils;

namespace WALauncher.ViewModels.WapkgObjects
{
    public class PackageInstallerItem : ViewModelBase
    {
        public static ObservableCollection<object> AvailableItems { get; }

        static PackageInstallerItem()
        {
            AvailableItems = new ObservableCollection<object>();
        }
    }
}
