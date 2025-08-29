namespace UnityEngine.UI
{
    [RequireComponent(typeof(Text))]
    public class ProjectNameText : MonoBehaviour
    {
        #if UNITY_EDITOR
        private Text _text;
        private Text Text
        {
            get
            {
                if (!_text)
                    _text = GetComponent<Text>();
                return _text;
            }
        }
        private void OnValidate() 
        {
            Text.text = Application.productName;
        }
#endif
    }
}