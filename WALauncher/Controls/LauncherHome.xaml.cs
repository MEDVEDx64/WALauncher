using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WALauncher.Controls
{
    public partial class LauncherHome : UserControl
    {
        public LauncherHome()
        {
            InitializeComponent();
        }

        private void OnHomepageLinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }
    }
}
