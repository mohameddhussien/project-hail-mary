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

        // Create runtime copy
        _volume.profile = Instantiate(_volume.profile);

        _volume.profile.TryGet(out _beautify);

        // Force the effect itself active
        _beautify.active = true;

        // Force each parameter's checkbox on manually
        _beautify.blurIntensity.overrideState = true;
        _beautify.vignettingBlink.overrideState = true;

        // Add every other property you use here the same way
        _beautify.vignettingOuterRing.overrideState = true;
        _beautify.vignettingInnerRing.overrideState = true;

        _beautify.vignettingOuterRing.value = 0.8f;
        _beautify.vignettingInnerRing.value = 0.6f;

        // etc...
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