using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
            Items.CollectionChanged += OnItemsChanged;

            wapkg.DistributionsChanged += OnDistributionsChanged;
            wapkg.PackagesChanged += OnPackagesChanged;
        }

        public ObservableCollection<Distribution> Items { get; }

        public Visibility TreeViewVisibility
        {
            get { return Items.Count == 0 ? Visibility.Hidden : Visibility.Visible; }
        }

        private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(TreeViewVisibility));
        }

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
            }));
        }

        void OnPackagesChanged(object sender, ServiceMessageEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var i in Items)
                {
                    if (i.Name == e.RelatedDistribution)
                    {
                        int count = i.Packages.Count - 1;
                        for (int x = 0; x < count; x++)
                        {
                            i.Packages.RemoveAt(0);
                        }

                        if (i.Packages.Count == 0)
                        {
                            i.Packages.Add(new PackageInstallerItem(e.RelatedDistribution));
                        }

                        foreach (var pkg in e.Packages)
                        {
                            i.Packages.Add(new InstalledPackage(e.RelatedDistribution)
                            {
                                Name = pkg.Item1,
                                Revision = "r" + pkg.Item2.ToString()
                            });
                        }

                        ((PackageInstallerItem)i.Packages[0]).InstalledPackages = e.Packages;
                        i.Packages.Move(0, i.Packages.Count - 1);
                        wapkg.RequestAvailablePackages(i.Name);
                        break;
                    }
                }
            }));
        }
    }
}
