using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace InvLock.Utilities;

public static class WindowsApi
{
    public const int SW_HIDE = 0;
    public const int SW_SHOWNORMAL = 1;
    public const int SW_NORMAL = 1;
    public const int SW_SHOWMINIMIZED = 2;
    public const int SW_SHOWMAXIMIZED = 3;
    public const int SW_MAXIMIZE = 3;
    public const int SW_SHOWNOACTIVATE = 4;
    public const int SW_SHOW = 5;
    public const int SW_MINIMIZE = 6;
    public const int SW_SHOWMINNOACTIVE = 7;
    public const int SW_SHOWNA = 8;
    public const int SW_RESTORE = 9;
    public const int SW_SHOWDEFAULT = 10;
    public const int SW_FORCEMINIMIZE = 11;

    private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

    public static List<IntPtr> GetOpenWindows()
    {
        IntPtr shellWindow = GetShellWindow();
        List<IntPtr> windows = [];

        _ = EnumWindows(delegate (IntPtr hWnd, int lParam)
        {
            StringBuilder title = new StringBuilder(200);

            _ = GetWindowText(hWnd, title, title.Capacity);

            Debug.WriteLine($"{hWnd} | {title}");

            if (hWnd == shellWindow)
            {
                return true;
            }

            if (!IsWindowVisible(hWnd) || IsIconic(hWnd))
            {
                return true;
            }

            if (GetWindowTextLength(hWnd) == 0)
            {
                return true;
            }

            windows.Add(hWnd);
            return true;
        }, 0);

        windows.Reverse();

        return windows;
    }

    public static void ShowTaskbar(bool show)
    {
        IntPtr hWnd = FindWindow("Shell_TrayWnd", null);
        _ = ShowWindow(hWnd, show ? SW_SHOW : SW_HIDE);

        // Taskbars on other monitors
        hWnd = FindWindow("Shell_SecondaryTrayWnd", null);
        _ = ShowWindow(hWnd, show ? SW_SHOW : SW_HIDE);
    }

    [DllImport("user32.dll")]
    internal static extern bool LockWorkStation();

    [DllImport("user32.dll")]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("USER32.DLL")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("USER32.DLL")]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetShellWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string? lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);
}