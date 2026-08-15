using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Timers;


namespace DatabaseBackupService
{
    public partial class DatabaseBackupService : ServiceBase
    {

        string DataBaseConn = ConfigurationManager.AppSettings["ConnectionString"];
        string LogFolder= ConfigurationManager.AppSettings["LogFolder"];
        string BackupFolder= ConfigurationManager.AppSettings["BackupFolder"];
        double BackupIntervalMilliseconds =
            Convert.ToDouble(ConfigurationManager.AppSettings["BackupIntervalMinutes"])
            * 60 * 1000; 
        string DataBaseName= ConfigurationManager.AppSettings["DataBaseName"];

        Timer timer;


        public DatabaseBackupService()
        {
            InitializeComponent();
        }

        private void Timer_Init()
        {
            timer = new Timer();
            timer.Interval = BackupIntervalMilliseconds;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            timer.Start();
        }
        private void Timer_Elapsed(object Sende, ElapsedEventArgs e)
        {
            DataBase_Backup();
        }
        private void DataBase_Backup()
        {
            string NewDatabaseName = $"{DataBaseName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.bak";
            string DatabaseDestination = Path.Combine(BackupFolder, NewDatabaseName);

            string Query = $@"
                         BACKUP DATABASE [{DataBaseName}]
                         TO DISK = '{DatabaseDestination}'
                         WITH INIT;";
            try
            {
                using (SqlConnection con = new SqlConnection(DataBaseConn))
                {
                    using (SqlCommand cmd = new SqlCommand(Query, con))
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                LogServiceEvent($"Database backup successful: {DatabaseDestination}");
            }
            catch (Exception ex)
            {
                LogServiceEvent($"Error while executing backup: {ex.Message}");
            }
        }

        private void CreateDirectories()
        {
            if (!Directory.Exists(BackupFolder))
            {
                Directory.CreateDirectory(BackupFolder);
            }
            if (!Directory.Exists(LogFolder))
            {
                Directory.CreateDirectory(LogFolder);

            }
        }

        protected override void OnStart(string[] args)
        {
            CreateDirectories();
            LogServiceEvent("Service Started");
            Timer_Init();
        }

        protected override void OnStop()
        {
            LogServiceEvent("Service Stoped");
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }

        }
          private void LogServiceEvent(string message)
        {
            string logFilePath = Path.Combine(LogFolder,"LogBackup.txt");
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";
            File.AppendAllText(logFilePath, logMessage);

            // Write to console if running interactively
            if (Environment.UserInteractive)
            {
                Console.WriteLine(logMessage);
            }

        }
        public void StartInConsole()
        {
            OnStart(null); // Trigger OnStart logic
            Console.WriteLine("Press Enter to stop the service...");
            Console.ReadLine(); // Wait for user input to simulate service stopping
            OnStop(); // Trigger OnStop logic
            Console.ReadKey();

        }
    }
}
