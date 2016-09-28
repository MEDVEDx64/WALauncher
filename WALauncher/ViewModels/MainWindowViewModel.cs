using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WALauncher.Utils;
using WALauncher.Wapkg;

namespace WALauncher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        static readonly string distroFile = "current_distro";

        InteractionService wapkg = null;
        string selectedDistro = "";

        public MainWindowViewModel()
        {
            wapkg = InteractionService.Get();

            Dists = new ObservableCollection<string>();
            wapkg.DistributionsChanged += OnDistsChanged;
            wapkg.TextAccepted += OnTextAccepted;
            wapkg.IndexChanged += OnIndexChanged;

            RunCommand = new DelegateCommand(Run);
            OpenExplorerCommand = new DelegateCommand(OpenExplorer);
            RefreshCommand = new DelegateCommand(Refresh);

            if (File.Exists(distroFile))
            {
                try
                {
                    using (var f = new StreamReader(new FileStream(distroFile, FileMode.Open)))
                    {
                        SelectedDistro = f.ReadLine();
                    }

                    RaisePropertyChanged(nameof(SelectedDistro));
                }

                catch
                {
                }
            }
        }

        public ObservableCollection<string> Dists { get; }
        public DelegateCommand RunCommand { get; }
        public DelegateCommand OpenExplorerCommand { get; }
        public DelegateCommand RefreshCommand { get; }

        public string SelectedDistro
        {
            get { return selectedDistro; }
            set
            {
                selectedDistro = value;
                try
                {
                    using (var f = new StreamWriter(new FileStream(distroFile, FileMode.Create)))
                    {
                        f.Write(value);
                    }
                }

                catch
                {
                }
            }
        }

        public string RecentMessage { get; set; }

        void Run()
        {
            if(SelectedDistro == null || SelectedDistro.Length == 0)
            {
                MessageBox.Show("Please select a distro first.", "WALauncher", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if(wapkg.WorkingDirectory == null)
            {
                MessageBox.Show("I don't know how to access the selected distro. Try restarting WALauncher.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var p = new ProcessStartInfo()
            {
                FileName = Path.Combine(wapkg.WorkingDirectory, SelectedDistro, "WA.exe")
            };

            new Process()
            {
                StartInfo = p
            }.Start();
        }

        void OpenExplorer()
        {
            Process.Start(Path.Combine(wapkg.WorkingDirectory, SelectedDistro ?? ""));
        }

        void Refresh()
        {
            wapkg.RequestDistributions();
            wapkg.RequestUpdate();
        }

        private void OnDistsChanged(object sender, ServiceMessageEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var selected = SelectedDistro;
                Dists.Clear();
                foreach (var d in e.Distributions)
                {
                    Dists.Add(d);
                }

                if (Dists.Contains(selected))
                {
                    SelectedDistro = selected;
                    RaisePropertyChanged(nameof(SelectedDistro));
                }
            }));
        }

        private void OnTextAccepted(object sender, ServiceMessageEventArgs e)
        {
            RecentMessage = e.Text;
            RaisePropertyChanged(nameof(RecentMessage));
        }

        private void OnIndexChanged(object sender, ServiceMessageEventArgs e)
        {
            wapkg.RequestAvailableDistributions();
            wapkg.RequestAvailablePackages();
        }
    }
}
