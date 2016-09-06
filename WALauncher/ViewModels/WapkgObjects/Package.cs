using WALauncher.Utils;

namespace WALauncher.ViewModels.WapkgObjects
{
    public abstract class Package : ViewModelBase
    {
        public string Name { get; set; }
        public string Revision { get; set; }
    }
}
