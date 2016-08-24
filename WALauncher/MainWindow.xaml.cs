using System.Windows;

namespace WALauncher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            new Wapkg().Init();
        }
    }
}
