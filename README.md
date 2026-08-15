# Database Backup Windows Service

A .NET Framework Windows Service that performs automated SQL Server database backups on a scheduled interval.

The service connects to a configured SQL Server database, creates full database backups with timestamped filenames, stores them in a designated backup folder, and logs all service activities and backup operations.

The service also supports console-mode execution for easier development, debugging, and testing.

## Features

- Perform automated full SQL Server database backups.
- Schedule backups using a configurable Timer.
- Generate unique backup filenames using timestamps.
- Store backup files in a configurable backup folder.
- Configure database connection settings through `App.config`.
- Configure backup intervals dynamically without changing the source code.
- Create backup and log directories automatically if they do not exist.
- Log service start and stop events.
- Log successful database backups with generated backup file paths.
- Log backup errors and SQL Server connection failures.
- Support console mode for easier debugging and testing.
- Configure Windows Service startup type as Automatic.
- Configure service dependencies for reliable operation.
- Properly dispose of Timer resources when the service stops.

## Technologies

- C#
- .NET Framework
- Windows Services
- ADO.NET
- SQL Server
- `SqlConnection`
- `SqlCommand`
- `System.Timers.Timer`
- `App.config`
- `InstallUtil`
- Visual Studio

## How It Works

1. The service starts and loads configuration settings from `App.config`.
2. Required backup and log directories are created if they do not exist.
3. The Timer is initialized using the configured backup interval.
4. When the Timer interval is reached, the backup process is triggered.
5. The service creates a timestamp-based backup filename.
6. A SQL Server `BACKUP DATABASE` command is executed using ADO.NET.
7. The generated `.bak` file is stored in the configured backup folder.
8. The operation result is written to the log file.
9. Any connection or backup errors are captured and logged.
10. The Timer and resources are properly released when the service stops.

## Configuration

Database connection, backup location, log location, and backup interval are configured through `App.config`:

```xml
<appSettings>
    <add key="ConnectionString" 
         value="Server=YOUR_SERVER;Database=YOUR_DATABASE;Integrated Security=True;" />

    <add key="DataBaseName" 
         value="YOUR_DATABASE" />

    <add key="BackupFolder" 
         value="C:\DatabaseBackups" />

    <add key="LogFolder" 
         value="C:\DatabaseBackups\Logs" />

    <add key="BackupIntervalMinutes" 
         value="60" />
</appSettings>
