using UnityEngine;

public class SetCameraQuad : MonoBehaviour
{
    public Camera overlayCam;
    void Start()
    {
        overlayCam.rect = new Rect(new Vector2(0.1f, 0.1f), new Vector2(0.8f, 0.8f));
    }
}
