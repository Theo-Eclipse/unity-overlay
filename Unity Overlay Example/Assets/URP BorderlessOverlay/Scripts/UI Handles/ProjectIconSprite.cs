using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Image))]
#if UNITY_EDITOR
    [ExecuteAlways]
#endif
    public class ProjectIconSprite : MonoBehaviour
    {
#if UNITY_EDITOR
        private Image _icon;
        private Image Icon
        {
            get
            {
                if (!_icon)
                    _icon = GetComponent<Image>();
                return _icon;
            }
        }

        private void OnValidate()
        {
            var textures = PlayerSettings.GetIcons(NamedBuildTarget.Unknown, IconKind.Any);
            if(textures == null || textures.Length <= 0)
                return;
            Icon.sprite = Sprite.Create(textures[0], new Rect(Vector2.zero, textures[0].Size()), Vector2.one * 0.5f); 
        }
#endif
    }
}

