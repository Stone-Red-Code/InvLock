using InvLock.DataContexts;

using System.Windows;

using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace InvLock;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : FluentWindow
{
    private readonly MainWindowDataContext mainWindowDataContext;
    private LockWindow? lockWindow;
    private bool blockWindowClosing = true;

    public MainWindow()
    {
        SystemThemeWatcher.Watch(this);

        mainWindowDataContext = new MainWindowDataContext(() => lockWindow);
        DataContext = mainWindowDataContext;

        InitializeComponent();
    }

    internal void RestoreWindow()
    {
        for (int i = 0; i < 10; i++)
        {
            ShowInTaskbar = true;
            Visibility = Visibility.Visible;
            SystemCommands.RestoreWindow(this);
            Topmost = true;
            _ = Activate();
            Topmost = false;
        }
    }

    private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        RestoreWindow();
    }

    private void QuitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Quit();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        lockWindow = new LockWindow(mainWindowDataContext.Settings);
        lockWindow.Show();

        if (mainWindowDataContext.Settings.FirstRun)
        {
            mainWindowDataContext.Settings.FirstRun = false;
            RestoreWindow();
        }
        else
        {
            ShowInTaskbar = false;
            WindowState = WindowState.Minimized;
            Visibility = Visibility.Collapsed;
        }
    }

    private void NotifyIcon_LeftClick(Wpf.Ui.Tray.Controls.NotifyIcon sender, RoutedEventArgs e)
    {
        RestoreWindow();
    }

    private void FluentWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        mainWindowDataContext.Settings.Save();

        if (blockWindowClosing)
        {
            e.Cancel = true;

            ShowInTaskbar = false;
            WindowState = WindowState.Minimized;
            Visibility = Visibility.Collapsed;
        }
    }

    private void Quit()
    {
        blockWindowClosing = false;
        Close();
    }

    private void FluentWindow_Closed(object sender, EventArgs e)
    {
        lockWindow?.Close();
        Environment.Exit(0);
    }
}