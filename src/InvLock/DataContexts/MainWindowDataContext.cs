﻿using CommunityToolkit.Mvvm.Input;

using SharpHook.Native;

using System.Reflection;

namespace InvLock.DataContexts;

internal partial class MainWindowDataContext(Func<LockWindow?> lockWindowAccessor)
{
#if DEBUG
    public string Title => $"{App.AppName} - Dev {AppVersion}";
#else
    public string Title => $"{App.AppName} - {AppVersion}";
#endif

    public string AppName => App.AppName;

    public string AppVersion => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";

    public Settings Settings { get; } = Settings.Load();

    [RelayCommand]
    public async Task RecordLockShortcut()
    {
        LockWindow? lockWindow = lockWindowAccessor();

        if (lockWindow is null)
        {
            return;
        }

        HashSet<KeyCode>? lockShortcut = await lockWindow.RecordShortcut();

        if (lockShortcut is not null)
        {
            Settings.LockShortcut = lockShortcut;
        }
    }

    [RelayCommand]
    public async Task RecordUnlockShortcut()
    {
        LockWindow? lockWindow = lockWindowAccessor();

        if (lockWindow is null)
        {
            return;
        }

        HashSet<KeyCode>? unlockShortcut = await lockWindow.RecordShortcut();

        if (unlockShortcut is not null)
        {
            Settings.UnlockShortcut = unlockShortcut;
        }
    }
}