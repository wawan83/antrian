using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Antri.Properties;

namespace Antri
{
    /// <summary>
    /// Interaction logic for NomorWindow.xaml
    /// </summary>
    public partial class NomorWindow
    {
        private readonly NumberService service;
        private HotKey hotkey;

        public NomorWindow()
        {
            InitializeComponent();
            Background = null;
            service = new NumberService();
            DataContext = service;
            service.Start();
            Loaded += (s, e) =>
                          {
                             hotkey = new HotKey(Keys.F10, this);
                             hotkey.HotKeyPressed += k => MoveNext();
                          };
            Loaded += NomorWindow_Loaded;
        }

        void NomorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var workingArea = new Rectangle((int)Left, (int)Top, (int)ActualWidth, (int)ActualHeight);
            workingArea = Screen.GetWorkingArea(workingArea);

            // Initialize the window location to the bottom right corner.
            Left = workingArea.Right - ActualWidth;
            Top = workingArea.Bottom - ActualHeight + ActualHeight / 4;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MoveNext();
        }

        private void MoveNext()
        {
            service.IncreaseNumber();
            Activate();
        }
    

        private void MenuItem_GantiNomor_Click(object sender, RoutedEventArgs e)
        {
            var gantiNomor = new GantiNomer(service);
            gantiNomor.Show();
        }

        private void MenuItem_Settings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().Show();
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            Close();
        }
    }
}
