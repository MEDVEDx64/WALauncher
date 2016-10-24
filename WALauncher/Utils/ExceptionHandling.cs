using System;
using System.Threading.Tasks;
using System.Windows;
using WALauncher.Wapkg;
using WALauncher.Windows;

namespace WALauncher.Utils
{
    public static class ExceptionHandling
    {
        public static void Crash(object exceptionObject)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ExceptionWindow(exceptionObject).ShowDialog();
            });

            var process = InteractionService.Get().ServiceProcess;
            if (process != null && !process.HasExited)
            {
                process.Kill();
            }

            Environment.Exit(1);
        }

        public static void TaskCrash(Task task)
        {
            Crash(task.Exception);
        }
    }
}
