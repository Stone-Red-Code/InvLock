using CuteUtils.Logging;

using System.IO;
using System.Windows;

namespace InvLock;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public const string AppGuid = "4B2411E5-1AE9-450C-AF2F-9A515ECBB9DF";

    public const string AppName = "InvLock";

    // This is the previous name of the application, used to migrate the application data folder
    private const string PreviousAppName = "InvLock";

    private static readonly string logFilePath = Path.Combine(ApplicationDataPath, $"{AppName}.log");
    private readonly Thread? eventThread;
    private readonly EventWaitHandle eventWaitHandle;
    public static string ApplicationDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StoneRed", AppName);

    public static Logger Logger { get; } = new Logger()
    {
        Config = new()
        {
            FatalConfig = new OutputConfig()
            {
                ConsoleColor = ConsoleColor.DarkRed,
                LogTarget = LogTarget.DebugConsole | LogTarget.File,
                FilePath = logFilePath
            },
            ErrorConfig = new OutputConfig()
            {
                ConsoleColor = ConsoleColor.Red,
                LogTarget = LogTarget.DebugConsole | LogTarget.File,
                FilePath = logFilePath
            },
            WarnConfig = new OutputConfig()
            {
                ConsoleColor = ConsoleColor.Yellow,
                LogTarget = LogTarget.DebugConsole | LogTarget.File,
                FilePath = logFilePath
            },
            InfoConfig = new OutputConfig()
            {
                ConsoleColor = ConsoleColor.White,
                LogTarget = LogTarget.DebugConsole | LogTarget.File,
                FilePath = logFilePath
            },
            DebugConfig = new OutputConfig()
            {
                ConsoleColor = ConsoleColor.Gray,
                LogTarget = LogTarget.DebugConsole,
            },
            FormatConfig = new FormatConfig()
            {
                DebugConsoleFormat = $"> {{{LogFormatType.DateTime}:HH:mm:ss}} | {{{LogFormatType.LogSeverity},-5}} | {{{LogFormatType.Message}}}\nat {{{LogFormatType.LineNumber}}} | {{{LogFormatType.FilePath}}}"
            }
        }
    };

    private static string PreviousApplicationDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StoneRed", PreviousAppName);

    public App()
    {
        // Setup global event handler
        eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, AppGuid, out bool createdNew);

        // Check if creating new was successful
        if (!createdNew)
        {
            Setup(false);
            Logger.LogWarn("Shutting down because other instance already running.", source: "Setup");
            // Shutdown Application
            _ = eventWaitHandle.Set();
            Current.Shutdown();
        }
        else
        {
            eventThread = new Thread(() =>
            {
                while (eventWaitHandle.WaitOne())
                {
                    _ = Current.Dispatcher.BeginInvoke(() => ((MainWindow)Current.MainWindow).RestoreWindow());
                }
            });
            eventThread.Start();

            Setup(true);
            Exit += CloseHandler;
        }
    }

    protected void CloseHandler(object sender, EventArgs e)
    {
        eventWaitHandle.Close();
        eventThread?.Interrupt();
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception exception = (Exception)e.ExceptionObject;
        Logger.LogFatal(exception + (e.IsTerminating ? "\t Process terminating!" : ""), source: exception.Source ?? "Unknown");
    }

    private static void Setup(bool clearLogFile)
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        try
        {
            if (Directory.Exists(PreviousApplicationDataPath) && !Directory.Exists(ApplicationDataPath))
            {
                Directory.Move(PreviousApplicationDataPath, ApplicationDataPath);
                Logger.LogInfo("Migrated ApplicationData folder", source: "Setup");
            }

            if (!Directory.Exists(ApplicationDataPath))
            {
                _ = Directory.CreateDirectory(ApplicationDataPath);
                Logger.LogInfo("Created ApplicationData folder", source: "Setup");
            }
        }
        catch (Exception ex)
        {
            _ = MessageBox.Show(ex.ToString(), AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            Logger.Log(ex.Message, "Setup", LogSeverity.Error);
        }

        if (clearLogFile)
        {
            Logger.ClearLogFile(LogSeverity.Info);
        }

        Logger.LogInfo("Setup complete", source: "Setup");
    }
}