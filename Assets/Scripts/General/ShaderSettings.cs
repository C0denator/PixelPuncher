using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ShaderSettings", menuName = "ScriptableObjects/ShaderSettings", order = 1)]
public class ShaderSettings : ScriptableObject
{
    [Header("Vignette Settings")]
    [SerializeField] private bool vignetteEffect = true;
    [SerializeField] [Range(0f,4f)] private float vignetteExponent = 1.0f;
    [SerializeField] [Range(0f,2f)] private float vignetteFactor = 1f;
    [Header("Scanline Settings")]
    [SerializeField] private bool scanlineEffect = true;
    [SerializeField] private bool interlaceEffect = true;
    [SerializeField] [Range(0f, 60f)] private float interlacingPerSecond = 30f;
    
    public static event Action OnSettingsChanged;
    
    //Properties
    public bool VignetteEffect
    {
        get => vignetteEffect;
        set
        {
            vignetteEffect = value;
            OnSettingsChanged?.Invoke();
        }
    }
    public float VignetteFactor
    {
        get
        {
            if (vignetteEffect)
            {
                return vignetteFactor;
            }
            else
            {
                return 0f;
            }
        }
        set
        {
            vignetteFactor = value;
            OnSettingsChanged?.Invoke();
        }
    }
    public float VignetteExponent
    {
        get
        {
            if (vignetteEffect)
            {
                return vignetteExponent;
            }
            else
            {
                return 0f;
            }
        }
        set
        { 
            vignetteExponent = value;
            OnSettingsChanged?.Invoke();
        }
    }

    public bool ScanlineEffect
    { 
        get => scanlineEffect;
        set
        {
            scanlineEffect = value;
            OnSettingsChanged?.Invoke();
        }
    }
    
    public bool InterlaceEffect
    {
        get => interlaceEffect;
        set
        {
            interlaceEffect = value;
            OnSettingsChanged?.Invoke();
        }
    }
    
    public float InterlacingPerSecond
    {
        get
        {
            if (scanlineEffect && interlaceEffect)
            {
                return interlacingPerSecond;
            }
            else
            {
                return 0f;
            }
        }
        set
        {
            interlacingPerSecond = value;
            OnSettingsChanged?.Invoke();
        }
    }

    private void OnValidate()
    {
        OnSettingsChanged?.Invoke();
    }
}
