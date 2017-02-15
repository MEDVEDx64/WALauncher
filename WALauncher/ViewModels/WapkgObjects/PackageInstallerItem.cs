using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        public IReadOnlyList<Tuple<string, uint?, string>> InstalledPackages { get; set; }

        public PackageInstallerItem(string targetDistro)
        {
            this.targetDistro = targetDistro;
            AvailableItems = new ObservableCollection<object>();
            wapkg.AvailablePackagesAccepted += OnAvailablePackagesAccepted;
        }

        void OnAvailablePackagesAccepted(object sender, ServiceMessageEventArgs e)
        {
            if(e.RelatedDistribution == null || e.RelatedDistribution != targetDistro)
            {
                return;
            }

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var nogroup = new List<AvailablePackage>();
                var groups = new Dictionary<string, IList<AvailablePackage>>();

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

                    ICollection<AvailablePackage> dst = nogroup;
                    if(pkg.Item3 != null)
                    {
                        if(!groups.ContainsKey(pkg.Item3))
                        {
                            groups.Add(pkg.Item3, new List<AvailablePackage>());
                        }

                        dst = groups[pkg.Item3];
                    }

                    dst.Add(new AvailablePackage(targetDistro)
                    {
                        Name = pkg.Item1,
                        Revision = pkg.Item2 == null ? "virtual" : "r" + pkg.Item2.ToString()
                    });
                }

                AvailableItems.Clear();

                foreach(var k in groups.Keys.OrderBy(x => x))
                {
                    AvailableItems.Add(new PackageGroup(k, groups[k].OrderBy(x => x.Name)));
                }

                foreach(var pkg in nogroup.OrderBy(x => x.Name))
                {
                    AvailableItems.Add(pkg);
                }

                AvailableItems.Add(new PackageFileInstallerItem(targetDistro));
            }));
        }
    }
}
