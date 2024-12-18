using InvLock.Utilities;

using SharpHook;
using SharpHook.Native;

using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace InvLock;

/// <summary>
/// Interaction logic for LockWindow.xaml
/// </summary>
public partial class LockWindow : Window
{
    private readonly SimpleGlobalHook hook = new SimpleGlobalHook();
    private readonly Dictionary<KeyCode, DateTime> pressedKeys = [];
    private readonly Settings settings;
    private List<IntPtr> windows = [];
    private bool isOpen = true;
    private bool suppressInput = false;

    public LockWindow(Settings settings)
    {
        InitializeComponent();

        Window w = new()
        {
            Left = -100,
            Top = -100,
            Width = 0,
            Height = 0,

            WindowStyle = WindowStyle.ToolWindow,
            ShowInTaskbar = false
        };

        WindowInteropHelper helper = new WindowInteropHelper(w);
        _ = helper.EnsureHandle();

        Owner = w;

        this.settings = settings;

        hook.MouseMoved += Hook_Mouse;
        hook.MouseDragged += Hook_Mouse;
        hook.MousePressed += Hook_Mouse;
        hook.MouseReleased += Hook_Mouse;
        hook.MouseClicked += Hook_Mouse;
        hook.MouseWheel += Hook_MouseWheel;

        hook.KeyReleased += Hook_KeyReleased;
        hook.KeyTyped += Hook_Key;
        hook.KeyPressed += Hook_KeyPressed;
        Microsoft.Win32.SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;

        _ = Task.Run(hook.Run);
    }

    private void ActivateLockScreen()
    {
        if (settings.HideWindows)
        {
            MinimizeWindows();
        }

        isOpen = true;
        suppressInput = true;

        Dispatcher.Invoke(() =>
        {
            _ = Activate();

            textBlock.Text = settings.LockText;
            textBlock.Stroke = (Brush)FindResource("PaletteRedBrush");

            Animation();
        });
    }

    private void DeactivateLockScreen()
    {
        isOpen = false;
        Dispatcher.Invoke(Close);
    }

    private void Hook_KeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        foreach (KeyCode key in pressedKeys.Keys)
        {
            if (DateTime.UtcNow - pressedKeys[key] > TimeSpan.FromSeconds(1))
            {
                _ = pressedKeys.Remove(key);
            }
        }
        _ = pressedKeys[e.Data.KeyCode] = DateTime.UtcNow;

        if (pressedKeys.ContainsKey(KeyCode.VcL) && pressedKeys.ContainsKey(KeyCode.VcLeftShift) && pressedKeys.ContainsKey(KeyCode.VcLeftControl) && pressedKeys.Count == 3)
        {
            if (suppressInput)
            {
                DeactivateLockScreen();
            }
            else
            {
                ActivateLockScreen();
            }
        }

        e.SuppressEvent = suppressInput;
    }

    private void Hook_KeyReleased(object? sender, KeyboardHookEventArgs e)
    {
        _ = pressedKeys.Remove(e.Data.KeyCode);
        e.SuppressEvent = suppressInput;
    }

    private async void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e)
    {
        if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionUnlock)
        {
            if (settings.HideWindows)
            {
                RestoreWindows();
            }

            isOpen = true;

            _ = Activate();

            await Task.Delay(500);

            textBlock.Text = settings.UnlockText;
            textBlock.Stroke = (Brush)FindResource("PaletteGreenBrush");

            Animation();
        }
    }

    private void MinimizeWindows()
    {
        windows = WindowsApi.GetOpenWindows();

        WindowsApi.ShowTaskbar(false);

        IntPtr thisWindowHandle = Dispatcher.Invoke(() => new WindowInteropHelper(this).Handle);

        foreach (IntPtr handle in windows)
        {
            if (handle == thisWindowHandle)
            {
                continue;
            }

            _ = WindowsApi.ShowWindow(handle, WindowsApi.SW_MINIMIZE);
        }
    }

    private void RestoreWindows()
    {
        WindowsApi.ShowTaskbar(true);

        foreach (IntPtr handle in windows)
        {
            _ = WindowsApi.ShowWindow(handle, WindowsApi.SW_RESTORE);
        }
    }

    private void Animation()
    {
        Storyboard storyboard = new Storyboard();
        TimeSpan duration = TimeSpan.FromMilliseconds(500);

        DoubleAnimation fadeInAnimation = new DoubleAnimation()
        { From = 0.0, To = 1.0, Duration = new Duration(duration) };

        DoubleAnimation fadeOutAnimation = new DoubleAnimation
        {
            From = 1.0,
            To = 0.0,
            Duration = new Duration(duration),
            BeginTime = TimeSpan.FromSeconds(3)
        };

        Storyboard.SetTargetName(fadeInAnimation, name: textBlock.Name);
        Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity", 1));
        storyboard.Children.Add(fadeInAnimation);
        storyboard.Begin(textBlock);

        Storyboard.SetTargetName(fadeOutAnimation, textBlock.Name);
        Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity", 0));
        storyboard.Children.Add(fadeOutAnimation);
        storyboard.Begin(textBlock);
    }

    private void Hook_MouseWheel(object? sender, MouseWheelHookEventArgs e)
    {
        e.SuppressEvent = suppressInput;
    }

    private void Hook_Mouse(object? sender, MouseHookEventArgs e)
    {
        e.SuppressEvent = suppressInput;
    }

    private void Hook_Key(object? sender, KeyboardHookEventArgs e)
    {
        e.SuppressEvent = suppressInput;
    }

    private void Window_Deactivated(object sender, EventArgs e)
    {
        if (isOpen)
        {
            isOpen = false;
            Close();
        }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (suppressInput)
        {
            _ = WindowsApi.LockWorkStation();
            suppressInput = false;
        }

        e.Cancel = true;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _ = Activate();
    }
}