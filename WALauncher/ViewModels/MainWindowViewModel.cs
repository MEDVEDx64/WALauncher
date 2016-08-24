using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WALauncher.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        ObservableCollection<string> dists = new ObservableCollection<string>();

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

        public string SelectedDistro { get; set; }
        public object ActiveItem { get; set; }
    }
}
