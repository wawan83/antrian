using System;
using System.ComponentModel;
using System.Data.OleDb;
using System.IO;
using System.Media;
using System.Threading;
using Antri;
using MonitorAntrian.Properties;

namespace MonitorAntrian
{
    class AntrianService : INotifyPropertyChanged
    {

        private Timer timer;
        private bool freetoConnect = true;

        private int number;
        public int Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
                OnPropertyChanged("Number");
            }
        }

        private int loket;
        public int Loket
        {
            get
            {
                return loket;
            }
            set
            {
                loket = value;
                OnPropertyChanged("Loket");
            }
        }
        public string TextNomer
        {
            get
            {
                return Settings.Default.TextNomer;
            }
        }
        public string TextLoket
        {
            get
            {
                return Settings.Default.TextLoket;
            }
        }
        public string TextJudul1
        {
            get
            {
                return Settings.Default.TextJudul1;
            }
        }

        public string TextJudul2
        {
            get
            {
                return Settings.Default.TextJudul2;
            }
        }


        private int previousNumber;
        public int PreviousNumber
        {
            get
            {
                return previousNumber;
            }
            set
            {
                previousNumber = value;
                OnPropertyChanged("PreviousNumber");
            }
        }

        private int previousLoket;
        public int PreviousLoket
        {
            get
            {
                return previousLoket;
            }
            set
            {
                previousLoket = value;
                OnPropertyChanged("PreviousLoket");
            }
        }

        public void Start()
        {
            timer = new Timer(CheckStatus, null, TimeSpan.FromMilliseconds(Settings.Default.PollingRateInMillis*1),
                      TimeSpan.FromMilliseconds(Settings.Default.PollingRateInMillis * 1));
        }

        public void CheckStatus(Object stateInfo)
        {
            if (!freetoConnect)
                return;
            freetoConnect = false;
            try
            {

                var connection = new OleDbConnection(NumberService.GetConnectionString());
                connection.Open();
                var dbCommand = new OleDbCommand("SELECT TOP 1 NO_ANTRIAN, LOKET FROM RSUD_ANTRIAN ORDER BY NO_ANTRIAN DESC",
                                                 connection);
                var reader = dbCommand.ExecuteReader();
                reader.Read();
                SetNewPair((int) reader["NO_ANTRIAN"], (int) reader["LOKET"]);

                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(string.Format("Error ada begini, tolong panggil SIMRS: {0}", ex.Message));
            }
            freetoConnect = true;
        }
        private void SetNewPair(int newNumber, int newLoket)
        {
            if (newNumber == Number)
                return;

            PreviousNumber = Number;
            PreviousLoket = Loket;
            Number = newNumber;
            Loket = newLoket;

            PlaySound();
        }

        private void PlaySound()
        {
            string nomorPath = Environment.CurrentDirectory + @"\nomor";
            string nomorUrutFileName = Path.Combine(nomorPath, "no-urut.wav");
            string loketFileName = Path.Combine(nomorPath, "loket-nomor.wav");
            string nomorFileName = Path.Combine(nomorPath,
                                                string.Format("{0}.wav", Number));
            string nomorLoketFileName = Path.Combine(nomorPath,
                                                     string.Format("{0}.wav", Loket));

            if (!File.Exists(nomorFileName))
                return;

            new SoundPlayer(nomorUrutFileName).PlaySync();
            new SoundPlayer(nomorFileName).PlaySync();
            new SoundPlayer(loketFileName).PlaySync();
            new SoundPlayer(nomorLoketFileName).PlaySync();
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
