using UnityEngine;
using UnityEngine.Rendering;
using Beautify; 

[RequireComponent(typeof(Volume))]
public class BeautifyTimelineProxy : MonoBehaviour
{
    Volume _volume;
    Beautify.Universal.Beautify _beautify;

    [Range(0f, 1f)] public float blurIntensity;
    [Range(0f, 1f)] public float blinkIntensity;

    void Awake()
    {
        _volume = GetComponent<Volume>();
        _volume.profile.TryGet(out _beautify);
    }


    void Update()
    {
        ApplyValues();
    }

    // This fires in the Editor when values change
    void OnValidate()
    {
        if (_beautify == null)
        {
            GetComponent<Volume>().profile.TryGet(out _beautify);
        }
        ApplyValues();
    }

    void ApplyValues()
    {
        if (_beautify == null) return;
        _beautify.blurIntensity.value = blurIntensity;  
        _beautify.vignettingBlink.value = blinkIntensity;
    }
}