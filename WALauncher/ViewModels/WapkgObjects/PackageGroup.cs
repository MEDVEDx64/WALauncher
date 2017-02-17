using System.Collections;
using System.Collections.ObjectModel;
using WALauncher.Utils;

namespace WALauncher.ViewModels.WapkgObjects
{
    public class PackageGroup : ViewModelBase
    {
        public string Name { get; }
        public ObservableCollection<AvailablePackage> Packages { get; }

        public PackageGroup(string name)
        {
            Name = name;
            Packages = new ObservableCollection<AvailablePackage>();
        }

        public void Push(IEnumerable pkgs)
        {
            if(pkgs == null)
            {
                return;
            }

            Packages.Clear();
            foreach (var x in pkgs)
            {
                var pkg = x as AvailablePackage;
                if (pkg != null)
                {
                    Packages.Add(pkg);
                }
            }
        }
    }
}
