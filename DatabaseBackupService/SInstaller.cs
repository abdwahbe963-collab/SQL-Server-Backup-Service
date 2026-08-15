using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace DatabaseBackupService
{
    [RunInstaller(true)]
    public partial class SInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller processInstaller;
        private ServiceInstaller serviceInstaller;
        public SInstaller()
        {
            InitializeComponent();
            // Initialize ServiceProcessInstaller
            processInstaller = new ServiceProcessInstaller
            {
                // Run the service under the local system account
                Account = ServiceAccount.LocalSystem
            };

            // Initialize ServiceInstaller
            serviceInstaller = new ServiceInstaller
            {
                // Set the name of the service
                ServiceName = "DatabaseBackupService",
                DisplayName = "myDatabaseBackupService",
                Description = "A Windows Service that performs scheduled SQL Server database backups."  ,
                StartType = ServiceStartMode.Automatic, // Automatically starts the service on system boot
                ServicesDependedOn = new string[] { "MSSQLSERVER", "RpcSs", "EventLog" } // Dependencies

            };

            // Add both installers to the Installers collection
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
