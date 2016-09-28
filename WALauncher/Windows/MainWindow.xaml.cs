using System;
using System.Windows;
using WALauncher.Wapkg;

namespace WALauncher.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Closed += OnClosed;
        }

        void OnLoaded(object sender, EventArgs e)
        {
            var wapkg = InteractionService.Get();

            wapkg.Subscribe();
            wapkg.RequestUpdate();
            wapkg.RequestDistributions();
            wapkg.RequestWorkingDirectory();
        }

        void OnClosed(object sender, EventArgs e)
        {
            InteractionService.Get().Shutdown();
        }
    }
}
