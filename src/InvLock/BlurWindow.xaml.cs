using InvLock.Utilities;

using System.Windows;

namespace InvLock;

/// <summary>
/// Interaction logic for BlurWindow.xaml
/// </summary>
public partial class BlurWindow : Window
{
    public BlurWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        WindowsApi.EnableBlur(this);
    }

    private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        DragMove();
    }
}