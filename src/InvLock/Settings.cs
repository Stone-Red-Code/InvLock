using SharpHook.Native;

using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

using Wpf.Ui.Appearance;

namespace InvLock;

public class Settings : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private static readonly string settingsPath = Path.Combine(App.ApplicationDataPath, "settings.json");
    private bool useWindowsLockScreen = true;
    private bool hideWindows = false;
    private bool blurScreen = false;
    private string lockText = "Lock Screen Active";
    private string theme = "Windows default";
    private HashSet<KeyCode> lockShortcut = [KeyCode.VcLeftControl, KeyCode.VcLeftShift, KeyCode.VcL];
    private HashSet<KeyCode> unlockShortcut = [KeyCode.VcLeftControl, KeyCode.VcLeftShift, KeyCode.VcL];

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

    public bool BlurScreen
    {
        get => blurScreen;
        set
        {
            blurScreen = value;
            OnPropertyChanged();
        }
    }

    public bool UseWindowsLockScreen
    {
        get => useWindowsLockScreen;
        set
        {
            useWindowsLockScreen = value;
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

    public HashSet<KeyCode> LockShortcut
    {
        get => lockShortcut;
        set
        {
            lockShortcut = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(LockShortcutText));
        }
    }

    [JsonIgnore]
    public string LockShortcutText => string.Join(" + ", LockShortcut.Select(key => key.ToString()[2..]));

    public string UnlockText
    {
        get => unlockText;
        set
        {
            unlockText = value;
            OnPropertyChanged();
        }
    }

    public HashSet<KeyCode> UnlockShortcut
    {
        get => unlockShortcut;
        set
        {
            unlockShortcut = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(UnlockShortcutText));
        }
    }

    [JsonIgnore]
    public string UnlockShortcutText => string.Join(" + ", UnlockShortcut.Select(key => key.ToString()[2..]));

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
            try
            {
                string json = File.ReadAllText(settingsPath);
                return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
            }
            catch (JsonException)
            {
                return new Settings();
            }
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