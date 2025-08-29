using UnityEngine;
using UnityEngine.EventSystems;

public class DragIconCursor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 IconCenter = Vector2.zero;
    [SerializeField] private Texture2D hoveredCursor;
    public void OnPointerDown(PointerEventData eventData)
    {
        Cursor.SetCursor(hoveredCursor, new Vector2(IconCenter.x * hoveredCursor.width, IconCenter.x * hoveredCursor.height), CursorMode.Auto);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
