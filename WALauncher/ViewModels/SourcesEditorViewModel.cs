using System;
using System.Collections.ObjectModel;
using System.Windows;
using WALauncher.Utils;
using WALauncher.ViewModels.Sources;
using WALauncher.Wapkg;

namespace WALauncher.ViewModels
{
    public class SourcesEditorViewModel : ViewModelBase
    {
        InteractionService wapkg = InteractionService.Get();

        public ObservableCollection<SourceDefinition> Sources { get; }
        public DelegateCommand<string> AddSourceCommand { get; }

        public Visibility ListViewVisibility
        {
            get { return Sources.Count == 0 ? Visibility.Hidden : Visibility.Visible; }
        }

        public SourcesEditorViewModel()
        {
            Sources = new ObservableCollection<SourceDefinition>();
            AddSourceCommand = new DelegateCommand<string>(AddSource);
            wapkg.SourcesChanged += OnSourcesChanged;
        }

        private void OnSourcesChanged(object sender, ServiceMessageEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Sources.Clear();
                foreach (var src in e.Sources)
                {
                    Sources.Add(new SourceDefinition(src));
                }

                foreach(var sd in Sources)
                {
                    sd.ApplySources(Sources);
                }

                RaisePropertyChanged(nameof(Sources));
                RaisePropertyChanged(nameof(ListViewVisibility));
            });
        }

        void AddSource(string link)
        {
            try
            {
                var uri = new Uri(link).ToString();
                if(uri[uri.Length - 1] != '/')
                {
                    uri = uri + '/';
                }

                foreach(var src in Sources)
                {
                    if(uri == src.ToString())
                    {
                        MessageBox.Show("The given URI is already exist.", "Can't add source",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var sd = new SourceDefinition(uri);
                Sources.Add(sd);
                sd.ApplySources(Sources);
                wapkg.PushSources(Sources);
            }

            catch(UriFormatException)
            {
                MessageBox.Show("Invalid URI.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
