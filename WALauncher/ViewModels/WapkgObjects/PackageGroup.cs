using System.Collections;
using System.Collections.Generic;
using WALauncher.Utils;

namespace WALauncher.ViewModels.WapkgObjects
{
    public class PackageGroup : ViewModelBase
    {
        public string Name { get; }
        public IEnumerable<AvailablePackage> Packages { get; }

        public PackageGroup(string name, IEnumerable pkgs)
        {
            Name = name;
            var packages = new List<AvailablePackage>();

            foreach(var x in pkgs)
            {
                var pkg = x as AvailablePackage;
                if(pkg != null)
                {
                    packages.Add(pkg);
                }
            }

            Packages = packages;
        }
    }
}
