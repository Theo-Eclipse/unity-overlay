using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class MouseOverDetection : MonoBehaviour
{
    #region Singleton
    public static MouseOverDetection instance { get; private set; }
    private MouseOverDetection() { }// Since this is a snigleton, we want to prevent new instances.
    [Header("Singleton Settings"), Tooltip("If enabled, each new object created with this component, will overwrite the previous singleton if it exists.")]
    [SerializeField] private bool allowSingletonOverwrite = false;
    #endregion

    [Header("Raycast Camera")]
    [SerializeField] private Camera targetCamera;

    [Space, Space, Header("Collision Settings")]
    [SerializeField] private LayerMask CollisionLayers;
    [Space]
    [SerializeField] private bool is3DClickable = true;
    [SerializeField] private bool is2DClickable = true;
    [SerializeField] private bool isUIClickable = true;

    private PointerEventData pointerData;
    private List<RaycastResult> results = new List<RaycastResult>(10);

    // Reset is called once the component added to an object, or when using Reset button in inspector.
    private void Reset() => targetCamera = GetComponent<Camera>();

    // Awake is called before the Start methods and the first frame update
    void Awake()
    {
        if (!targetCamera) Reset();
        instance = allowSingletonOverwrite ? this : (instance != null ? instance : this);
        pointerData = new PointerEventData(EventSystem.current);
    }
    public bool getMouseOver2D() => is2DClickable ? Physics2D.OverlapPoint(targetCamera.ScreenToWorldPoint(Input.mousePosition), CollisionLayers) : false;
    public bool getMouseOver3D() => is3DClickable ? Physics.Raycast(targetCamera.ScreenPointToRay(Input.mousePosition), CollisionLayers) : false;
    public bool getMouseOverUi()
    {
        if (!isUIClickable) return false;
        pointerData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(pointerData, results);
        return results.Count > 0;
    }
    public static bool isMouseOver2D => instance.getMouseOver2D();
    public static bool isMouseOver3D => instance.getMouseOver3D();
    public static bool isMouseOverUi => instance.getMouseOverUi();
    public static bool isMouseOverAny => instance.getMouseOver2D() || instance.getMouseOver3D() || instance.getMouseOverUi();
}
