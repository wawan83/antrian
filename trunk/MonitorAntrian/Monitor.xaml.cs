using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using TextBox=System.Windows.Controls.TextBox;

namespace MonitorAntrian
{
    /// <summary>
    /// Interaction logic for GantiNomer.xaml
    /// </summary>
    public partial class Monitor
    {
        private readonly AntrianService antrianService;
        public Monitor()
        {
            WindowStartupLocation = WindowStartupLocation.Manual;
            InitializeComponent();
            
            antrianService = new AntrianService();
            antrianService.PropertyChanged += antrianService_PropertyChanged;
            antrianService.Start();
            DataContext = antrianService;
        }

        void antrianService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ThreadStart start = null;
            ;
            if (e.PropertyName == "Number")
            {
                start = () => Dispatcher.Invoke(DispatcherPriority.Normal,
                                                            new Action<string>(UpdateNumber),
                                                            string.Format("{0:000}", antrianService.Number));
            }
            if (e.PropertyName == "Loket")
            {

                start = () => Dispatcher.Invoke(DispatcherPriority.Normal,
                                                            new Action<string>(UpdateLoket),
                                                            antrianService.Loket.ToString());
            }
            if (start != null)
                new Thread(start).Start();
        }

        private void UpdateNumber(string number)
        {
            //numberTransitionBox.Content = null;
            var textBox = new TextBox();
            textBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            textBox.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            textBox.Text = number;
            textBox.BorderThickness = new Thickness(0.0);
            numberTransitionBox.Content = textBox;

        }
        private void UpdateLoket(string number)
        {
           var textBox = new TextBox();
            textBox.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            textBox.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            textBox.Text = number;
            textBox.BorderThickness = new Thickness(0.0);
            loketTransitionBox.Content = textBox;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowStyle = WindowStyle.None;
            Cursor = System.Windows.Input.Cursors.None;
            Topmost = true;
            int screens = SystemInformation.MonitorCount;
            System.Drawing.Rectangle workingArea = Screen.AllScreens[screens -1].WorkingArea;
            Left = workingArea.Left;
            Top = workingArea.Top;
            Width = workingArea.Width;
            Height = workingArea.Height;
            WindowState = WindowState.Maximized; 
            WindowStyle = WindowStyle.None; 
            Topmost = true;
            ShowInTaskbar = false;

            Show();
        }
    }
}
