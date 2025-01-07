using System.Reflection;

namespace InvLock.DataContexts;

internal class MainWindowDataContext
{
#if DEBUG
    public string Title => $"{App.AppName} - Dev {AppVersion}";
#else

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title => $"{App.AppName} - {AppVersion}";
#endif

    public string AppName => App.AppName;

    public string AppVersion => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";

    public Settings Settings { get; } = Settings.Load();
}