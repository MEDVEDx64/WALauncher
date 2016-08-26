using System.Collections.ObjectModel;
using WALauncher.Utils;

namespace WALauncher.ViewModels.WapkgObjects
{
    public class Distribution : ViewModelBase
    {
        public Distribution()
        {
            Packages = new ObservableCollection<Package>();
        }

        public string Name { get; set; }
        public ObservableCollection<Package> Packages { get; private set; }
    }
}
