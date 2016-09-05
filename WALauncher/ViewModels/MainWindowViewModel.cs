using System;
using System.Collections.ObjectModel;
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

            Dists = new ObservableCollection<string>();
            wapkg.DistributionsChanged += OnDistsChanged;

            RunCommand = new DelegateCommand(Run);
        }

        public ObservableCollection<string> Dists { get; }
        public DelegateCommand RunCommand { get; }

        public string SelectedDistro { get; set; }

        void Run()
        {
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

        public void NotifyDistroListChanged()
        {
            RaisePropertyChanged(nameof(Dists));
        }
    }
}
