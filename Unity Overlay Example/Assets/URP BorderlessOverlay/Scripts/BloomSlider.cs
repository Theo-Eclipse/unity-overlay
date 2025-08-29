using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class BloomSlider : MonoBehaviour
{
    public Vector2 minMaxIntensity = new Vector2(0.0f, 5.0f);
    [SerializeField]private Volume targetVolume;
    Bloom bloom;
    Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        Bloom tmp;
        if (targetVolume.profile.TryGet(out tmp))
        {
            bloom = tmp;
        }
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(UpdateBloom);
    }

    // Update is called once per frame
    private void UpdateBloom(float value)
    {
        bloom.intensity.value = Mathf.Lerp(minMaxIntensity.x, minMaxIntensity.y, value);
    }
}
