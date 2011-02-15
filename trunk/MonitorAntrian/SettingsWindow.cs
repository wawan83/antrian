using System.Windows.Forms;
using MonitorAntrian.Properties;

namespace MonitorAntrian
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();
            propertyGrid.SelectedObject = Settings.Default;
        }
    }
}
