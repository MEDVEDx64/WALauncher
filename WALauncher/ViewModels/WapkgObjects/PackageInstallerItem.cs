using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using WALauncher.Utils;
using WALauncher.Wapkg;

namespace WALauncher.ViewModels.WapkgObjects
{
    public class PackageInstallerItem : ViewModelBase
    {
        InteractionService wapkg = InteractionService.Get();
        IReadOnlyList<Tuple<string, uint?>> packages = null;

        public ObservableCollection<object> AvailableItems { get; }

        public PackageInstallerItem(IReadOnlyList<Tuple<string, uint?>> packages)
        {
            this.packages = packages;
            AvailableItems = new ObservableCollection<object>();

            wapkg.AvailablePackagesAccepted += OnAvailablePackagesAccepted;
        }

        void OnAvailablePackagesAccepted(object sender, ServiceMessageEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                AvailableItems.Clear();
                foreach (var pkg in e.Packages)
                {
                    if(packages != null)
                    {
                        bool flag = false;
                        foreach(var installed in packages)
                        {
                            if(pkg.Item1 == installed.Item1 && pkg.Item2 <= installed.Item2)
                            {
                                flag = true;
                                break;
                            }
                        }

                        if (flag) continue;
                    }

                    AvailableItems.Add(new AvailablePackage()
                    {
                        Name = pkg.Item1,
                        Revision = pkg.Item2 == null ? "virtual" : pkg.Item2.ToString()
                    });
                }
            }));

            RaisePropertyChanged(nameof(AvailableItems));
        }
    }
}
