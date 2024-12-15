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
    private LockWindow? lockWindow;
    private bool blockWindowClosing = true;

    public MainWindow()
    {
        DataContext = new MainWindowDataContext();

        InitializeComponent();

        SystemThemeWatcher.Watch(this);
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
        Close();

        lockWindow = new LockWindow();
        lockWindow.Show();
    }

    private void NotifyIcon_LeftClick(Wpf.Ui.Tray.Controls.NotifyIcon sender, RoutedEventArgs e)
    {
        RestoreWindow();
    }

    private void FluentWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (blockWindowClosing)
        {
            e.Cancel = true;

            ShowInTaskbar = false;
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