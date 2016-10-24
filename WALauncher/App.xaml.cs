using System;
using System.Windows;
using WALauncher.Utils;

namespace WALauncher
{
    public partial class App : Application
    {
        public App() : base()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ExceptionHandling.Crash(e.ExceptionObject);
        }
    }
}
