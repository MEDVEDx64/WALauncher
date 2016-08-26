using System;
using System.Windows.Controls;
using WALauncher.ViewModels;

namespace WALauncher.Controls
{
    public partial class PackageInstaller : UserControl
    {
        public PackageInstaller()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, EventArgs e)
        {
            var vm = DataContext as PackageInstallerViewModel;
            if(vm != null)
            {
                vm.NotifyItemsChanged();
            }
        }
    }
}
