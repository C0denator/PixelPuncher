using UnityEngine;

namespace UI
{
    public class SliderValueHelper : MonoBehaviour
    {
        private TMPro.TMP_Text _text;
        private UnityEngine.UI.Slider _slider;

        private void Awake()
        {
            _text = GetComponent<TMPro.TMP_Text>();
            _slider = transform.parent.GetComponent<UnityEngine.UI.Slider>();
            
            if (_text == null)
            {
                Debug.LogError("Text component not found");
            }
            
            if (_slider == null)
            {
                Debug.LogError("Slider component not found");
            }
            
            _text.text = _slider.value.ToString();
        }
        
        private void OnEnable()
        {
            _slider.onValueChanged.AddListener(OnValueChanged);
        }
        
        private void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(OnValueChanged);
        }
        
        private void OnValueChanged(float value)
        {
            _text.text = value.ToString();
        }
    }
}
