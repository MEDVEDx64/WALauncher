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

                foreach(var k in groups.Keys.OrderBy(x => x))
                {
                    PackageGroup dstGroup = null;
                    foreach(var x in AvailableItems)
                    {
                        var g = x as PackageGroup;
                        if(g != null && g.Name == k)
                        {
                            dstGroup = g;
                            break;
                        }
                    }

                    if(dstGroup == null)
                    {
                        dstGroup = new PackageGroup(k);
                        var list = new List<PackageGroup>();
                        foreach(var x in AvailableItems)
                        {
                            var g = x as PackageGroup;
                            if(g == null)
                            {
                                break;
                            }

                            list.Add(g);
                        }

                        if (list.Count > 0)
                        {
                            bool done = false;
                            for (int i = 1; i < list.Count; i++)
                            {
                                if (string.Compare(list[i - 1].Name, k) < 0 && string.Compare(k, list[i].Name) < 0)
                                {
                                    AvailableItems.Insert(i, dstGroup);
                                    done = true;
                                    break;
                                }
                            }

                            if (!done)
                            {
                                if (string.Compare(k, list[0].Name) < 0)
                                {
                                    AvailableItems.Insert(0, dstGroup);
                                }
                                else
                                {
                                    AvailableItems.Add(dstGroup);
                                }
                            }
                        }

                        else
                        {
                            AvailableItems.Add(dstGroup);
                        }
                    }

                    dstGroup.Push(groups[k].OrderBy(x => x.Name));
                }

                foreach(var x in AvailableItems.ToList())
                {
                    var g = x as PackageGroup;
                    if(g != null)
                    {
                        if (!groups.Keys.Contains(g.Name))
                        {
                            AvailableItems.Remove(x);
                        }

                        continue;
                    }

                    AvailableItems.Remove(x);
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
