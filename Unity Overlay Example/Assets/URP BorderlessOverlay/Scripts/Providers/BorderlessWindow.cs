using System;
using UnityEngine;
using Unity.Mathematics;
using System.Runtime.InteropServices;

public class BorderlessWindow
{
    public static bool framed = true;
    private static Vector2Int wSize;
    private static Vector2Int wPosition;

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll")]
    private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);
    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hwnd, out WinRect lpRect);

    private struct WinRect { public int left, top, right, bottom; }

    private const int GWL_STYLE = -16;

    private const int SW_MINIMIZE = 6;
    private const int SW_MAXIMIZE = 3;
    private const int SW_RESTORE = 9;

    private const uint WS_VISIBLE = 0x10000000;    
    private const uint WS_POPUP = 0x80000000;
    private const uint WS_BORDER = 0x00800000;
    private const uint WS_OVERLAPPED = 0x00000000;
    private const uint WS_CAPTION = 0x00C00000;
    private const uint WS_SYSMENU = 0x00080000;
    private const uint WS_THICKFRAME = 0x00040000; // WS_SIZEBOX
    private const uint WS_MINIMIZEBOX = 0x00020000;
    private const uint WS_MAXIMIZEBOX = 0x00010000;
    private const uint WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;


    // This attribute will make the method execute on game launch, after the Unity Logo Splash Screen.
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeOnLoad()
    {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN   // Dont do this while on Unity Editor!
        SetFramelessWindow();
#endif
    }

    public static void SetTransparentWindow()
    {
        var hwnd = GetActiveWindow();
        SetWindowLong(hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);
        framed = false;
    }

    public static void SetFramelessWindow()
    {
        var hwnd = GetActiveWindow();
        SetWindowLong(hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);
        framed = false;
    }

    public static void SetFramedWindow()
    {
        var hwnd = GetActiveWindow();
        SetWindowLong(hwnd, GWL_STYLE, WS_OVERLAPPEDWINDOW | WS_VISIBLE);
        framed = true;
    }

    public static void MinimizeWindow()
    {
        var hwnd = GetActiveWindow();
        ShowWindow(hwnd, SW_MINIMIZE);
    }

    public static void MaximizeWindow()
    {
        var hwnd = GetActiveWindow();
        ShowWindow(hwnd, SW_MAXIMIZE);
    }

    public static void RestoreWindow()
    {
        var hwnd = GetActiveWindow();
        ShowWindow(hwnd, SW_RESTORE);
    }

    public static void MoveWindowPos(Vector2 posDelta, int newWidth, int newHeight)
    {
        var hwnd = GetActiveWindow();

        var windowRect = GetWindowRect(hwnd, out WinRect winRect);

        var x = winRect.left + (int)posDelta.x;
        var y = winRect.top - (int)posDelta.y;
        MoveWindow(hwnd, x, y, newWidth, newHeight, false);
    }

    public static void MoveWindowPos(Vector2 posDelta)
    {
        var hwnd = GetActiveWindow();
        var windowRect = GetWindowRect(hwnd, out WinRect winRect);
        wSize = new Vector2Int(math.abs(winRect.right - winRect.left), math.abs(winRect.top - winRect.bottom));
        wPosition = new Vector2Int(winRect.left, winRect.top);
        var x = winRect.left + (int)posDelta.x;
        var y = winRect.top - (int)posDelta.y;
        MoveWindow(hwnd, x, y, wSize.x, wSize.y, false);
    }

    public static Vector2Int GetWindowSize() 
    {
        var hwnd = GetActiveWindow();
        GetWindowRect(hwnd, out WinRect winRect);
        wSize = new Vector2Int(math.abs(winRect.right - winRect.left), math.abs(winRect.top - winRect.bottom));
        return wSize;
    }

    public static void MoveWindowBorders(float left, float right, float top, float bottom)
    {
        var hwnd = GetActiveWindow();
        var windowRect = GetWindowRect(hwnd, out WinRect winRect);
        wPosition = new Vector2Int(winRect.left, winRect.top);
        winRect.left += (int)left;
        winRect.right += (int)right;
        winRect.top -= (int)top;
        winRect.bottom -= (int)bottom;
        wSize = new Vector2Int(math.abs(winRect.right - winRect.left), math.abs(winRect.top - winRect.bottom));
        wPosition += new Vector2Int((int)left, -(int)top);
        MoveWindow(hwnd, wPosition.x, wPosition.y, wSize.x, wSize.y, false);
    }

    public static void GetWindowData(out Rect rect)
    {
        var hwnd = GetActiveWindow();

        var windowRect = GetWindowRect(hwnd, out WinRect winRect);
        rect = new Rect(new Vector2(winRect.left, winRect.right), new Vector2(winRect.top, winRect.bottom));
    }
}

