using UnityEngine;
using UnityEngine.EventSystems;

public class HoverIconCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2 IconCenter = Vector2.one;
    [SerializeField] private Texture2D hoveredCursor;
    public void OnPointerEnter(PointerEventData data)
    {
        Cursor.SetCursor(hoveredCursor, new Vector2(IconCenter.x * hoveredCursor.width, IconCenter.x * hoveredCursor.height), CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData data)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
