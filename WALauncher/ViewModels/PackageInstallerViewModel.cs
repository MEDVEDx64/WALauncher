using System.Collections.ObjectModel;
using WALauncher.Utils;
using WALauncher.ViewModels.WapkgObjects;

namespace WALauncher.ViewModels
{
    public class PackageInstallerViewModel : ViewModelBase
    {
        public PackageInstallerViewModel()
        {
        }

        ObservableCollection<Distribution> items = new ObservableCollection<Distribution>();

        public ObservableCollection<Distribution> Items
        {
            get
            {
                CollectItems();
                return items;
            }
        }

        public void NotifyItemsChanged()
        {
            RaisePropertyChanged(nameof(Items));
        }

        void CollectItems()
        {
            items.Clear();
            var wapkg = new Wapkg();

            foreach (string d in wapkg.GetDists())
            {
                var dist = new Distribution()
                {
                    Name = d
                };

                var installed = wapkg.GetPackages(d);
                var available = wapkg.GetPackagesAvailable();

                foreach(var pkg in installed)
                {
                    dist.Packages.Add(new Package()
                    {
                        Name = pkg.Item1,
                        Revision = pkg.Item2,
                        IsInstalled = true
                    });
                }

                foreach(var pkg in available)
                {
                    bool alreadyInstalled = false;
                    foreach(var instPkg in installed)
                    {
                        if(pkg.Item1 == instPkg.Item1)
                        {
                            alreadyInstalled = true;
                            break;
                        }
                    }

                    if(!alreadyInstalled)
                    {
                        dist.Packages.Add(new Package()
                        {
                            Name = pkg.Item1,
                            Revision = pkg.Item2,
                            IsInstalled = false
                        });
                    }
                }

                items.Add(dist);
            }
        }
    }
}
