using System.Collections.ObjectModel;
using WALauncher.Utils;

namespace WALauncher.ViewModels.WapkgObjects
{
    public class Distribution : ViewModelBase
    {
        public Distribution()
        {
            Packages = new ObservableCollection<object>();
        }

        public string Name { get; set; }
        public ObservableCollection<object> Packages { get; private set; }
    }
}
