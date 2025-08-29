using UnityEngine;
using UnityEngine.EventSystems;

public class ApplicationWindowFunctions : MonoBehaviour
{
    private Vector2Int _defaultWindowSize = new Vector2Int(800, 600);
    private bool _maximized;
    private void Start()
    {
        if(Application.isEditor)
            return;
        _defaultWindowSize = new Vector2Int(PlayerPrefs.GetInt("DEFAULT_WINDOW_WIDTH", Screen.width), PlayerPrefs.GetInt("DEFAULT_WINDOW_HEIGHT", Screen.height));
        ResetWindowSize();
    }
    public void ApplicationQuit() => Application.Quit(0);

    private void ResetWindowSize()
    {
        BorderlessWindow.MoveWindowPos(Vector2Int.zero, _defaultWindowSize.x, _defaultWindowSize.y);
    }

    private void SetOnFirstLaunch()
    {
        if(PlayerPrefs.HasKey("DEFAULT_WINDOW_WIDTH") && PlayerPrefs.HasKey("DEFAULT_WINDOW_HEIGHT"))
            return;
        PlayerPrefs.SetInt("DEFAULT_WINDOW_WIDTH", Screen.width);
        PlayerPrefs.SetInt("DEFAULT_WINDOW_HEIGHT", Screen.height);
    }

    private void Awake()
    {
        if(Application.isEditor)
            return;
        SetOnFirstLaunch();
    }

    public void OnCloseBtnClick()
    {
        if(Application.isEditor)
            return;
        _defaultWindowSize = BorderlessWindow.GetWindowSize();
        PlayerPrefs.SetInt("DEFAULT_WINDOW_WIDTH", _defaultWindowSize.x);
        PlayerPrefs.SetInt("DEFAULT_WINDOW_HEIGHT", _defaultWindowSize.y);
        EventSystem.current.SetSelectedGameObject(null);
        Application.Quit();
    }

    public void OnMinimizeBtnClick()
    {
        if(Application.isEditor)
            return;
        EventSystem.current.SetSelectedGameObject(null);
        BorderlessWindow.MinimizeWindow();
    }

    public void OnMaximizeBtnClick()
    {
        if(Application.isEditor)
            return;
        EventSystem.current.SetSelectedGameObject(null);

        if (_maximized)
            BorderlessWindow.RestoreWindow();
        else
            BorderlessWindow.MaximizeWindow();

        _maximized = !_maximized;
    }


}
