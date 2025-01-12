using System;
using UnityEngine;

namespace UI
{
    public class SliderValueHelper : MonoBehaviour
    {
        private TMPro.TMP_Text _text;
        private UnityEngine.UI.Slider _slider;
        [SerializeField] private Material material;
        [SerializeField] private String propertyName;

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
            
            if (material == null)
            {
                Debug.LogError("Material not set");
            }
            
            if (propertyName == null)
            {
                Debug.LogError("Property name not set");
            }
            
            //find the property
            if (material.HasProperty(propertyName))
            {
                _slider.value = material.GetFloat(propertyName);
                ShowText(_slider.value);
            }
            else
            {
                Debug.LogError("Property not found: " + propertyName);
            }
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
            ShowText(value);
            material.SetFloat(propertyName, value);
        }
        
        private void ShowText(float value)
        {
            
            if(value%1 == 0) //if the value is a whole number
            {
                _text.text = value.ToString("F0");
                return;
            }
            
            //only show 3 decimal places
            _text.text = value.ToString("F3");
        }
    }
}
