using System;
using System.Collections.ObjectModel;
using System.Windows;
using WALauncher.Utils;
using WALauncher.ViewModels.WapkgObjects;
using WALauncher.Wapkg;

namespace WALauncher.ViewModels
{
    public class DistributionInstallerViewModel : ViewModelBase
    {
        InteractionService wapkg = InteractionService.Get();

        public ObservableCollection<AvailableDistribution> Items { get; }

        public DistributionInstallerViewModel()
        {
            Items = new ObservableCollection<AvailableDistribution>();
            wapkg.AvailableDistributionsAccepted += OnAvailableDistributionsAccepted;
        }

        void OnAvailableDistributionsAccepted(object sender, ServiceMessageEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Items.Clear();
                foreach (var dist in e.Distributions)
                {
                    Items.Add(new AvailableDistribution(dist));
                }
            }));
        }
    }
}
