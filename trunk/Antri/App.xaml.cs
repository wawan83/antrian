using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Application=System.Windows.Application;

namespace Antri
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //hotkey = new GlobalHotkeys();
            //hotkey.RegisterGlobalHotKey((int)Keys.F10, GlobalHotkeys.MOD_CONTROL);
        }

        private void Application_Exit(object sender, System.Windows.ExitEventArgs e)
        {
            //hotkey.Dispose();
        }
    }
}
