using System.Runtime.InteropServices;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger), typeof(RectTransform))]
public class UiDragHandle : MonoBehaviour
{
    #region DLL Imports & Functions
    //
    // DLL Imports & Functions
    //
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public static implicit operator Point(POINT point)
        {
            return new Point(point.X, point.Y);
        }
    }

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out POINT lpPoint);
    public static Vector2Int GetWindowsCursorPosition()
    {
        POINT lpPoint;
        GetCursorPos(out lpPoint);
        return new Vector2Int(lpPoint.X, lpPoint.Y);
    }
    //
    // End Region
    //
    #endregion

    [Header("Move Anchors")]
    public bool topLeftCorner = true;
    public bool bottomRightCorner = true;

    [Header("Main")]
    [SerializeField] private RectTransform targetTransform;

    public bool useMoveMainWindow = false;
    private bool moveMainWindow = false;

    //[Space, Space, Header("Events")]
    private Camera targetCamera;
    private EventTrigger eventTrigger;
    private EventTrigger.Entry onPointerDown, onPointerUp;
    private bool isHeld = false;
    private Vector2Int pointerDelta = Vector2Int.zero;

    void Start()
    {
        targetCamera = Camera.main;
        SetupTriggers();
    }

    // Update is called once per frame
    void Update()
    {
        if (isHeld)
        {
            moveMainWindow = useMoveMainWindow ? Input.GetKey(KeyCode.LeftControl) : false;
            UpdateMouseDelta();
            if (!Input.GetMouseButton(0))
                isHeld = false;
        }
    }

    private void SetupTriggers() 
    {
        if(!eventTrigger) eventTrigger = GetComponent<EventTrigger>();
        onPointerDown = new EventTrigger.Entry();
        onPointerUp = new EventTrigger.Entry();
        onPointerDown.eventID = EventTriggerType.PointerDown;
        onPointerUp.eventID = EventTriggerType.PointerUp;
        onPointerDown.callback.AddListener((eventData) => OnPointerDown());
        onPointerUp.callback.AddListener((eventData) => OnPointerUp());
        eventTrigger.triggers.Add(onPointerDown);
        eventTrigger.triggers.Add(onPointerUp);
    }

    private void OnPointerDown() 
    {
        pointerDelta = GetWindowsCursorPosition();
        Debug.Log("On Pointer Down " + gameObject.name);
        isHeld = true;
    }
    private void OnPointerUp()
    {
        Debug.Log("On Pointer Up " + gameObject.name);
        isHeld = false;
    }

    private void UpdateMouseDelta() 
    {
        if (pointerDelta != GetWindowsCursorPosition()) 
        {
            OnPositionChanged(GetWindowsCursorPosition() - pointerDelta);
            pointerDelta = GetWindowsCursorPosition();
        }
    }

    private void OnPositionChanged(Vector2Int delta)
    {
        if (targetTransform)
        {
            delta = new Vector2Int(delta.x * 1, delta.y * - 1);
            if (topLeftCorner && !bottomRightCorner)
            {
                targetTransform.offsetMin = new Vector2(targetTransform.offsetMin.x + delta.x, targetTransform.offsetMin.y);
                targetTransform.offsetMax = new Vector2(targetTransform.offsetMax.x, targetTransform.offsetMax.y + delta.y);
            }
            else if (!topLeftCorner && bottomRightCorner)
            {
                targetTransform.offsetMin = new Vector2(targetTransform.offsetMin.x, targetTransform.offsetMin.y + delta.y);
                targetTransform.offsetMax = new Vector2(targetTransform.offsetMax.x + delta.x, targetTransform.offsetMax.y);
            }
            else 
            {
                targetTransform.offsetMin += delta;
                targetTransform.offsetMax += delta;
            }

        }
    }
}
