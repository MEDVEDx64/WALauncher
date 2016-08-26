using WALauncher.Utils;

namespace WALauncher.ViewModels.WapkgObjects
{
    public class Package : ViewModelBase
    {
        public string Name { get; set; }
        public uint Revision { get; set; }
        public bool IsInstalled { get; set; }
    }
}
