using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Antri.Properties;

namespace Antri
{
    public partial class SettingsWindow : Form
    {

        public SettingsWindow()
        {
            InitializeComponent();
            FormClosing += SettingsWindow_FormClosing;
            propertyGrid.SelectedObject = Settings.Default;
        }

        static void SettingsWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}
