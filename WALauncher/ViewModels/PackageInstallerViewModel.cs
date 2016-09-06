using System;
using System.Collections.ObjectModel;
using System.Windows;
using WALauncher.Utils;
using WALauncher.ViewModels.WapkgObjects;
using WALauncher.Wapkg;

namespace WALauncher.ViewModels
{
    public class PackageInstallerViewModel : ViewModelBase
    {
        InteractionService wapkg = InteractionService.Get();

        public PackageInstallerViewModel()
        {
            Items = new ObservableCollection<Distribution>();

            wapkg.DistributionsChanged += OnDistributionsChanged;
            wapkg.PackagesChanged += OnPackagesChanged;
            wapkg.AvailablePackagesAccepted += OnAvailablePackagesAccepted;
        }

        public ObservableCollection<Distribution> Items { get; }

        void OnDistributionsChanged(object sender, ServiceMessageEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Items.Clear();

                foreach (var d in e.Distributions)
                {
                    Items.Add(new Distribution()
                    {
                        Name = d
                    });
                    wapkg.RequestPackages(d);
                }

                wapkg.RequestAvailablePackages();
            }));
        }

        void OnPackagesChanged(object sender, ServiceMessageEventArgs e)
        {
            foreach(var i in Items)
            {
                if(i.Name == e.RelatedDistribution)
                {
                    i.Packages.Clear();
                    foreach(var pkg in e.Packages)
                    {
                        i.Packages.Add(new InstalledPackage()
                        {
                            Name = pkg.Item1,
                            Revision = pkg.Item2.ToString()
                        });
                    }

                    i.Packages.Add(new PackageInstallerItem());
                    break;
                }
            }
        }

        void OnAvailablePackagesAccepted(object sender, ServiceMessageEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                PackageInstallerItem.AvailableItems.Clear();
                foreach (var pkg in e.Packages)
                {
                    PackageInstallerItem.AvailableItems.Add(new AvailablePackage()
                    {
                        Name = pkg.Item1,
                        Revision = pkg.Item2 == null ? "virtual" : pkg.Item2.ToString()
                    });
                }
            }));
        }
    }
}
