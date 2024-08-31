using General;
using UnityEngine;

public class InterlaceSlider : MonoBehaviour
{
    [SerializeField] private ShaderController _controller;
    private TMPro.TMP_Text _text;
    private UnityEngine.UI.Slider _slider;
    public Material crtScanlinesMat;
    private static readonly int InterlaceFrequency = Shader.PropertyToID("_InterlaceFrequency");
    // Start is called before the first frame update
    void Awake()
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
        
        if(_controller == null)
        {
            //try finding the controller on the main camera
            _controller = Camera.main.GetComponent<ShaderController>();

            if (_controller == null)
            {
                Debug.LogError("ShaderController reference not set in InterlaceSlider");
            }
        }
        
        if(crtScanlinesMat == null)
        {
            Debug.LogError("CRT Scanlines material not set in InterlaceSlider");
        }
        
        _slider.value = crtScanlinesMat.GetFloat(InterlaceFrequency);
        ShowText(_slider.value);
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
        crtScanlinesMat.SetFloat(InterlaceFrequency, value);
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
