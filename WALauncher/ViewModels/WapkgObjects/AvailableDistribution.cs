using System;
using System.Windows;
using WALauncher.Utils;
using WALauncher.Wapkg;

namespace WALauncher.ViewModels.WapkgObjects
{
    public class AvailableDistribution : ViewModelBase
    {
        InteractionService wapkg = InteractionService.Get();
        string actionToken = null;

        public string Name { get; }
        public string SuggestedName { get; set; }
        public bool ButtonEnabled { get; private set; }
        public bool Installing { get; private set; }

        public int CurrentProgress { get; private set; }
        public int TotalProgress { get; private set; }

        public DelegateCommand InstallCommand { get; }

        public AvailableDistribution(string name)
        {
            Name = name;
            SuggestedName = name;

            ButtonEnabled = true;
            CurrentProgress = 0;
            TotalProgress = 1;

            InstallCommand = new DelegateCommand(Install);

            wapkg.ActionProgressUpdated += OnActionProgressUpdated;
            wapkg.ActionComplete += OnActionComplete;
        }

        void Install()
        {
            ButtonEnabled = false;
            RaisePropertyChanged(nameof(ButtonEnabled));

            actionToken = Guid.NewGuid().ToString().Substring(0, 8);
            wapkg.InstallDistribution(Name, SuggestedName, actionToken);
        }

        void OnActionProgressUpdated(object sender, ServiceMessageEventArgs e)
        {
            if (actionToken != e.ActionToken) return;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                CurrentProgress = e.ActionProgressCurrent;
                RaisePropertyChanged(nameof(CurrentProgress));

                if (e.ActionProgressTotal != TotalProgress)
                {
                    TotalProgress = e.ActionProgressTotal;
                    RaisePropertyChanged(nameof(TotalProgress));
                }

                if (CurrentProgress == TotalProgress)
                {
                    Installing = true;
                    RaisePropertyChanged(nameof(Installing));
                }
            }));
        }

        void OnActionComplete(object sender, ServiceMessageEventArgs e)
        {
            if (actionToken != e.ActionToken) return;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                CurrentProgress = 0;
                TotalProgress = 1;
                SuggestedName = Name;
                ButtonEnabled = true;
                Installing = false;

                RaisePropertyChanged(nameof(CurrentProgress));
                RaisePropertyChanged(nameof(TotalProgress));
                RaisePropertyChanged(nameof(SuggestedName));
                RaisePropertyChanged(nameof(ButtonEnabled));
                RaisePropertyChanged(nameof(Installing));
            }));

            actionToken = null;
        }
    }
}
