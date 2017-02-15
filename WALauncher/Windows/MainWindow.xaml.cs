using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using WALauncher.Wapkg;

namespace WALauncher.Windows
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        const int GWL_STYLE = -16;
        const int WS_MAXIMIZEBOX = 0x10000;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Closed += OnClosed;
            StateChanged += OnWindowStateChanged;
        }

        void OnLoaded(object sender, EventArgs e)
        {
            // Disabling maximize button
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_MAXIMIZEBOX);

            var wapkg = InteractionService.Get();

            wapkg.Subscribe();
            wapkg.RequestUpdate();
            wapkg.RequestDistributions();
            wapkg.RequestWorkingDirectory();
            wapkg.RequestSources();
        }

        void OnClosed(object sender, EventArgs e)
        {
            InteractionService.Get().Shutdown();
        }

        private void OnWindowStateChanged(object sender, EventArgs e)
        {
            if(WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
        }
    }
}
