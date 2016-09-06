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
        InteractionService wapkg = null;

        public MainWindowViewModel()
        {
            wapkg = InteractionService.Get();

            RecentMessage = "Ready";

            Dists = new ObservableCollection<string>();
            wapkg.DistributionsChanged += OnDistsChanged;
            wapkg.TextAccepted += OnTextAccepted;
            wapkg.IndexChanged += OnIndexChanged;

            RunCommand = new DelegateCommand(Run);
        }

        public ObservableCollection<string> Dists { get; }
        public DelegateCommand RunCommand { get; }

        public string SelectedDistro { get; set; }
        public string RecentMessage { get; set; }

        void Run()
        {
            if(SelectedDistro == null || SelectedDistro.Length == 0)
            {
                MessageBox.Show("Please select a distro.", "WALauncher", MessageBoxButton.OK, MessageBoxImage.Information);
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
