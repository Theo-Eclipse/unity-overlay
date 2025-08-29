using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class WindowSizeHandle : MonoBehaviour, IDragHandler
{
    [Header("Border Move Axis")]
    public float left = 0;
    public float right = 0;
    public float top = 0;
    public float bottom = 0;
    private Vector2 _deltaValue = Vector2.zero;
    // Start is called before the first frame update
    public void OnDrag(PointerEventData data)
    {
        _deltaValue += data.delta;
        if (data.dragging)
        {
            BorderlessWindow.MoveWindowBorders(_deltaValue.x * left, data.delta.x * right, data.delta.y * top, _deltaValue.y * bottom);
        }
    }
}
