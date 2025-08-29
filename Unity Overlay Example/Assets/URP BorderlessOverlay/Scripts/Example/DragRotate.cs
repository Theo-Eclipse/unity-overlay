using UnityEngine;
using UnityEngine.EventSystems;

public class DragRotate : MonoBehaviour, IDragHandler
{
    [SerializeField] private Vector2 _deltaValue = Vector2.zero;
    public float maxSpeed = 100;
    public float sensetivity = 100;
    // Start is called before the first frame update
    public void OnDrag(PointerEventData data)
    {
        _deltaValue += data.delta * Time.deltaTime * sensetivity;
        _deltaValue = new Vector2(Mathf.Clamp(_deltaValue.x, -maxSpeed, maxSpeed), Mathf.Clamp(_deltaValue.y, -maxSpeed, maxSpeed));
    }

    void Update()
    {
        _deltaValue = Vector2.Lerp(_deltaValue, Vector2.zero, Time.deltaTime);
        transform.RotateAround(Vector3.zero, transform.parent.up, _deltaValue.x);
        transform.RotateAround(Vector3.zero, Vector3.left, _deltaValue.y);
    }
}
