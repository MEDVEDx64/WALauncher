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
        string targetDistro = null;

        public ObservableCollection<object> AvailableItems { get; }
        public IReadOnlyList<Tuple<string, uint?>> InstalledPackages { get; set; }

        public PackageInstallerItem(string targetDistro)
        {
            this.targetDistro = targetDistro;
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
                    if(InstalledPackages != null)
                    {
                        bool flag = false;
                        foreach(var installed in InstalledPackages)
                        {
                            if(pkg.Item1 == installed.Item1 && pkg.Item2 <= installed.Item2)
                            {
                                flag = true;
                                break;
                            }
                        }

                        if (flag) continue;
                    }

                    AvailableItems.Add(new AvailablePackage(targetDistro)
                    {
                        Name = pkg.Item1,
                        Revision = pkg.Item2 == null ? "virtual" : "r" + pkg.Item2.ToString()
                    });
                }
            }));
        }
    }
}
