using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox=System.Windows.MessageBox;

namespace Antri
{
    /// <summary>
    /// Interaction logic for GantiNomer.xaml
    /// </summary>
    public partial class GantiNomer
    {
        private readonly NumberService service;
        private HotKey hotkey;

        public GantiNomer(NumberService service)
        {
            this.service = service;
            InitializeComponent();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            int newNumber;
            if (int.TryParse(nomorTextBox.Text, out newNumber))
            {
                service.ResetNumber(newNumber);
                Close();
            }
            else
                nomorTextBox.Text = "0";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var workingArea = new Rectangle((int)Left, (int)Top, (int)ActualWidth, (int)ActualHeight);
            workingArea = Screen.GetWorkingArea(workingArea);

            // Initialize the window location to the bottom right corner.
            Left = workingArea.Right - ActualWidth;
            Top = workingArea.Bottom - ActualHeight;
        }

    }
}
