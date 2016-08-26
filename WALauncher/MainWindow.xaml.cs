using System;
using System.Windows;
using WALauncher.ViewModels;

namespace WALauncher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            distroBox.GotFocus += OnDistroBoxGotFocus;
            new Wapkg().Init();
        }

        void OnDistroBoxGotFocus(object sender, EventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;
            if(vm != null)
            {
                vm.NotifyDistroListChanged();
            }
        }
    }
}
