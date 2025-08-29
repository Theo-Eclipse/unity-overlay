using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class WindowMoveHandle : MonoBehaviour, IDragHandler
{
    private Vector2 _deltaValue = Vector2.zero;
    
    public void OnDrag(PointerEventData data)
    {
        _deltaValue += data.delta;
        if (data.dragging)
        {
            BorderlessWindow.MoveWindowPos(_deltaValue);
        }
    }
}
