using System.Collections.ObjectModel;
using WALauncher.Utils;

namespace WALauncher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
        }

        ObservableCollection<string> dists = new ObservableCollection<string>();

        public string SelectedDistro { get; set; }

        public ObservableCollection<string> Dists
        {
            get
            {
                dists.Clear();
                foreach(var d in new Wapkg().GetDists())
                {
                    dists.Add(d);
                }
                return dists;
            }
        }

        public void NotifyDistroListChanged()
        {
            RaisePropertyChanged(nameof(Dists));
        }
    }
}
