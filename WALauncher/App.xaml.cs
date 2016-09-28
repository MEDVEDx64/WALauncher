using System;
using System.Windows;
using WALauncher.Wapkg;
using WALauncher.Windows;

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
            var ex = e.ExceptionObject as Exception;
            new ExceptionWindow(e.ExceptionObject).ShowDialog();

            var process = InteractionService.Get().ServiceProcess;
            if(process != null && !process.HasExited)
            {
                process.Kill();
            }

            Environment.Exit(1);
        }
    }
}
