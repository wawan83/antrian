using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Antri.Properties;

namespace Antri
{
    public static class Logger
    {
        private static readonly StreamWriter logFile =
            new StreamWriter(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                             Settings.Default.LogFileName), true);

        public static void WriteLog(string line)
        {
            logFile.WriteLine(DateTime.Now.ToUniversalTime() + " " + line);
            logFile.Flush();
        }
    }
}
