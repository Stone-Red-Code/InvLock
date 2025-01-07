﻿using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;

using Wpf.Ui.Appearance;

namespace InvLock;

public class Settings : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private static readonly string settingsPath = Path.Combine(App.ApplicationDataPath, "settings.json");
    private bool hideWindows = false;
    private string lockText = "Lock Screen Active";
    private string theme = "Windows default";

    private string unlockText = "Lock Screen Inactive";

    public bool HideWindows
    {
        get => hideWindows;
        set
        {
            hideWindows = value;
            OnPropertyChanged();
        }
    }

    public string LockText
    {
        get => lockText;
        set
        {
            lockText = value;
            OnPropertyChanged();
        }
    }

    public string UnlockText
    {
        get => unlockText;
        set
        {
            unlockText = value;
            OnPropertyChanged();
        }
    }

    public string Theme
    {
        get => theme;
        set
        {
            theme = value;

            if (value == "Windows default")
            {
                ApplicationThemeManager.ApplySystemTheme();
            }
            else if (value == "Light")
            {
                ApplicationThemeManager.Apply(ApplicationTheme.Light);
            }
            else if (value == "Dark")
            {
                ApplicationThemeManager.Apply(ApplicationTheme.Dark);
            }

            OnPropertyChanged();
        }
    }

    public static Settings Load()
    {
        if (File.Exists(settingsPath))
        {
            string json = File.ReadAllText(settingsPath);
            return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
        }

        return new Settings();
    }

    public void Save()
    {
        string json = JsonSerializer.Serialize(this);
        File.WriteAllText(settingsPath, json);
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}