using System;
using System.ComponentModel;
using System.Data.OleDb;
using System.IO;
using System.Threading;
using ADODB;
using Antri.Properties;
using Microsoft.Win32;

namespace Antri
{
    public class NumberService : INotifyPropertyChanged
    {
        private Timer timer;
        public void Start()
        {
            ExecuteQuery(File.ReadAllText( "CreateTable.sql" ), false);

            timer = new Timer(CheckStatus, null, TimeSpan.FromMilliseconds(Settings.Default.PollingRateInMillis * 1),
                      TimeSpan.FromMilliseconds(Settings.Default.PollingRateInMillis * 1));
            Settings.Default.PropertyChanged += Default_PropertyChanged;

        }

        void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Loket")
                Loket = Settings.Default.Loket;

        }

        private bool freetoConnect = true;
        public void CheckStatus(Object stateInfo)
        {
            if (!freetoConnect)
                return;
            freetoConnect = false;

            try
            {
                var connection = new OleDbConnection(GetConnectionString(Settings.Default.ConnectionString));
                connection.Open();
                var dbCommand =
                    new OleDbCommand(
                        string.Format("SELECT NO_ANTRIAN FROM RSUD_ANTRIAN WHERE LOKET = {0}", Settings.Default.Loket),
                        connection);
                object result = dbCommand.ExecuteScalar();
                if (result == null)
                    ExecuteQuery(string.Format("INSERT INTO RSUD_ANTRIAN VALUES ({0},{1})", 0, Settings.Default.Loket));
                else
                {
                    Number = (int)result;
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(string.Format("Error ada begini, tolong panggil SIMRS: {0}", ex.Message));
            }
            freetoConnect = true;
        }

        public static string GetConnectionString(string oldString)
        {
            String connectionString = String.Empty;
            var nuansaKey = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Nuansa");
            if (nuansaKey == null)
            {
                if (!string.IsNullOrEmpty(oldString))
                    return oldString;
                MSDASC.DataLinks dataLinks = new MSDASC.DataLinksClass();
                object connection = new Connection();
                ((Connection)connection).ConnectionString = Settings.Default.ConnectionString;

                if (dataLinks.PromptEdit(ref connection))
                {
                    connectionString = ((Connection)connection).ConnectionString;
                    Settings.Default.ConnectionString = connectionString;
                }
                return connectionString;
            }

            RegistryKey key = nuansaKey.OpenSubKey("NCI Medismart").OpenSubKey("3.00").OpenSubKey("Database");
            if (key == null)
                throw new Exception("Tidak bisa buka registry NCI Medismart");

            for (int i = 0; i < 11; i++)
            {
                var value = (string)key.GetValue("key" + i);
                if (!(value.Contains("User ID") || value.Contains("Password")))
                    connectionString = connectionString + value;
            }
            connectionString += string.Format(";Password={0};User ID={1}", Settings.Default.DatabasePassword,
                                              Settings.Default.DatabaseUsername);
            return connectionString;
        }

        public int Loket
        {
            get
            {
                return Settings.Default.Loket;
            }
            set
            {
                number = value;
                OnPropertyChanged("Loket");
            }
        }


        private int? number = -1;
        public int? Number
        {
            get
            {
                if (number == -1)
                    return null;
                return number;
            }
            set
            {
                number = value;
                OnPropertyChanged("Number");
            }
        }
        public void ShowContextMenu()
        {
            OnPropertyChanged("ContextMenu");
        }
        
        public void ExecuteQuery(string query)
        {
            ExecuteQuery(query, false);
        }

        public void ExecuteQuery(string query, bool scalar)
        {
            try
            {
                var connection = new OleDbConnection(GetConnectionString(Settings.Default.ConnectionString));
                connection.Open();
                var dbCommand = new OleDbCommand(query, connection);
                
                if (scalar)
                    dbCommand.ExecuteScalar();
                else 
                    dbCommand.ExecuteNonQuery();

                connection.Close();
            }
            catch(ArgumentException)
            {
                GetConnectionString("");
            }
            catch(OleDbException)
            {
                GetConnectionString("");
            }
        }

        public void IncreaseNumber()
        {
            try
            {
                ExecuteQuery(string.Format("UPDATE RSUD_ANTRIAN SET NO_ANTRIAN = (SELECT MAX(NO_ANTRIAN) FROM  RSUD_ANTRIAN) + 1 WHERE LOKET = {0}",
                                              Settings.Default.Loket));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(string.Format("Error ada begini, tolong panggil SIMRS: {0}", ex.Message));
            }
        }
        public void ResetNumber(int newNumber)
        {
            try
            {
                ExecuteQuery(string.Format("BEGIN TRAN; DELETE FROM RSUD_ANTRIAN; INSERT INTO RSUD_ANTRIAN VALUES ({0},{1}); COMMIT;", newNumber,
                                              Settings.Default.Loket));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(string.Format("Error ada begini, tolong panggil SIMRS: {0}", ex.Message));
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
