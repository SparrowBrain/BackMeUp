using System.Threading.Tasks;
using System.Windows;

namespace BackMeUp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var backupProcess = new BackupProcess();
            Task.Run(async()=>
            {
                await backupProcess.Run();
            });
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
