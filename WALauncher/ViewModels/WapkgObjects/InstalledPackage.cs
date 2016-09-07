using System.Windows;
using WALauncher.Utils;
using WALauncher.Wapkg;

namespace WALauncher.ViewModels.WapkgObjects
{
    public class InstalledPackage : Package
    {
        string distro;

        public string SpoilerButtonText { get; private set; }
        public Visibility SpoilerContentVisibility
        {
            get { return SpoilerButtonText == "<" ? Visibility.Visible : Visibility.Hidden; }
            set
            {
                SpoilerButtonText = value == Visibility.Visible ? "<" : ">";
                RaisePropertyChanged(nameof(SpoilerButtonText));
            }
        }

        public bool RemoveButtonEnabled { get; private set; }
        public DelegateCommand SpoilerCommand { get; }
        public DelegateCommand RemoveCommand { get; }

        public InstalledPackage(string distro)
        {
            this.distro = distro;
            SpoilerContentVisibility = Visibility.Hidden;
            SpoilerCommand = new DelegateCommand(Spoiler);
            RemoveCommand = new DelegateCommand(Remove);
            RemoveButtonEnabled = true;
        }

        void Spoiler()
        {
            if (SpoilerContentVisibility == Visibility.Visible)
                SpoilerContentVisibility = Visibility.Hidden;
            else if (SpoilerContentVisibility == Visibility.Hidden)
                SpoilerContentVisibility = Visibility.Visible;

            RaisePropertyChanged(nameof(SpoilerContentVisibility));
        }

        void Remove()
        {
            InteractionService.Get().RemovePackage(distro, Name);
            RemoveButtonEnabled = false;
            RaisePropertyChanged(nameof(RemoveButtonEnabled));
        }
    }
}
