using System;
using System.Runtime.InteropServices;
using UnityEngine;
using BlurBehindDemo;

public class WindowsOverlay : MonoBehaviour
{
    #region DLL Imports & Functions
    //
    // DLL Imports & Functions
    //
    [DllImport("user32.dll")]
    private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern IntPtr SetActiveWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    CompositeWindow compositeWindow;


    private struct MARGINS 
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [Flags]
    internal enum MONITOR_DEFAULTTO
    {
        NULL = 0x00000000,
        PRIMARY = 0x00000001,
        NEAREST = 0x00000002,
    }

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr MonitorFromWindow(IntPtr hwnd, MONITOR_DEFAULTTO dwFlags);

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
    private const int GWL_EXSTYLE = -20;
    private const uint WS_EX_TRANSPARENT = 0x00000020;
    private const uint WS_EX_LAYERED = 0x00080000;
    private const uint SWP_NOSIZE = 0x0001;
    private bool isClickThrough = true;
    private bool alwaysOnTop = true;
    private bool overlayEnabled = true;
    private MARGINS margins;
    private uint WS_EX_LAYERED_TRANSPARENT => isClickThrough ? (WS_EX_LAYERED | WS_EX_TRANSPARENT) : WS_EX_LAYERED;
    private IntPtr hWnd;
    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    static readonly IntPtr HWND_NORMAL = new IntPtr(0);
    private Vector2Int windowPosition = Vector2Int.zero;
    private Vector2Int windowSize => new Vector2Int(Screen.mainWindowDisplayInfo.width, Screen.mainWindowDisplayInfo.height);
    //
    // End Region
    //
    public static WindowsOverlay instance { get; private set; }
    private WindowsOverlay() { }

    #endregion
    [Header("Overlay Settings")]
    public bool isAlwaysOnTop = true;
    public bool useMouseOverDetection = true;
    public bool isOverlayEnabled = true;
    public OverlayMode overlayMode = OverlayMode.Transparent;

    [Header("Window Settings")]
    public bool startBorderless = false;
    public Vector2Int borderSize = new Vector2Int(8, 40);
    private AccentState bgTargetState;

    private void Awake()
    {
        alwaysOnTop = isAlwaysOnTop;
        overlayEnabled = isOverlayEnabled;
        instance = this;
        windowPosition = Screen.mainWindowPosition;
    }


    private void Start()
    {
        if (Application.isEditor)
            return;
        compositeWindow = new CompositeWindow();
        hWnd = GetActiveWindow();
        SetOverlayEnabled(isOverlayEnabled);
        SetAlwaysOnTop(alwaysOnTop);
        if (startBorderless)
            SetWindowBorderless();
        if (isOverlayEnabled)
        {
            switch (overlayMode)
            {
                case OverlayMode.BluryTransparent:
                    bgTargetState = AccentState.ACCENT_ENABLE_BLURBEHIND;
                    compositeWindow.SetBlurEnabled(hWnd, bgTargetState);
                    break;
                default:
                    bgTargetState = AccentState.ACCENT_ENABLE_TRANSPARENTGRADIENT;
                    //compositeWindow.SetBlurEnabled(hWnd, bgTargetState);
                    break;
            }

        }
    }

    private void SetWindowBorderless()
    {
        if (!BorderlessWindow.framed || Application.isEditor)
            return;

        BorderlessWindow.SetFramelessWindow();
        BorderlessWindow.MoveWindowPos(Vector2Int.zero, Screen.width - borderSize.x, Screen.height - borderSize.y);
    }

    private void Update()
    {
        if (!MouseOverDetection.instance && useMouseOverDetection)
        {
            Debug.LogError($"{GetType().Name} is missing a 'Mouse Over Detection' instance!");
            return;
        }

        if (!isOverlayEnabled || Application.isEditor)
            return;
        UpdateClickThroughState();
        UpdateAlwaysOnTop();
        UpdateOverlayEnabled();
    }

    private void UpdateAlwaysOnTop()
    {
        if (alwaysOnTop == isAlwaysOnTop) return;
        alwaysOnTop = isAlwaysOnTop;

        SetAlwaysOnTop(alwaysOnTop);
    }

    private void UpdateOverlayEnabled() 
    {
        if (overlayEnabled == isOverlayEnabled) return;
        overlayEnabled = isOverlayEnabled;
        SetOverlayEnabled(overlayEnabled);
    }

    private void UpdateClickThroughState() 
    {
        if (overlayEnabled && isClickThrough != !MouseOverDetection.isMouseOverAny)// If mouse is over any obstacle, set ClickThrough to FALSE.
        {
            SetClickThrough(!MouseOverDetection.isMouseOverAny);
        }
    }

    public void SetOverlayEnabled(bool enabled) 
    {
        if (Application.isEditor)
            return;
        isOverlayEnabled = enabled;
        margins = new MARGINS { cxLeftWidth = enabled ? -1 : 0 };
        DwmExtendFrameIntoClientArea(hWnd, ref margins);
        if (!isOverlayEnabled)
        {
            SetAlwaysOnTop(false);
            SetClickThrough(false);
            SetActiveWindow(hWnd);
        }
    }

    private void SetClickThrough(bool enabled) 
    {
        isClickThrough = enabled;
        SetActiveWindow(hWnd);
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED_TRANSPARENT);
    }

    public void SetAlwaysOnTop(bool isAlwaysOnTop) 
    {
        if (Application.isEditor)
            return;
        this.isAlwaysOnTop = isAlwaysOnTop;
        SetWindowPos(hWnd, (this.isAlwaysOnTop ? HWND_TOPMOST : HWND_NORMAL), windowPosition.x, windowPosition.y, windowSize.x, windowSize.y, SWP_NOSIZE);
    }

    public void MoveWindowByDelta(Vector2Int delta) 
    {
        if (Application.isEditor)
            return;
        windowPosition += delta;
        SetWindowPos(hWnd, (this.isAlwaysOnTop ? HWND_TOPMOST : HWND_NORMAL), windowPosition.x, windowPosition.y, windowSize.x, windowSize.y, SWP_NOSIZE);
    }

    public void ResetWindowPosition() => windowPosition = Screen.mainWindowPosition;

    public enum OverlayMode 
    {
        Transparent,
        BluryTransparent
    }
}
