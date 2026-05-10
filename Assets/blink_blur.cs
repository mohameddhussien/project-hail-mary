using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class BeautifyTimelineProxy : MonoBehaviour
{
    Volume _volume;
    Beautify.Universal.Beautify _beautify;

    [Range(0f, 1f)] public float blurIntensity;
    [Range(0f, 1f)] public float blinkIntensity;
    [Range(0f, 1f)] public float Outer_Ring;
    [Range(0f, 1f)] public float InnerRing;
    [Range(0f, 1f)] public float Fade;

    void Awake()
    {
        _volume = GetComponent<Volume>();

        if (_volume == null || _volume.profile == null) return;

        _volume.profile = Instantiate(_volume.profile);
        bool found = _volume.profile.TryGet(out _beautify);

        if (!found || _beautify == null)
        {
            Debug.LogError("[Beautify Proxy] Beautify effect not found in Volume profile.", this);
            return;
        }

        _beautify.active = true;
        _beautify.blurIntensity.overrideState = true;
        _beautify.vignettingBlink.overrideState = true;
        _beautify.vignettingOuterRing.overrideState = true;
        _beautify.vignettingInnerRing.overrideState = true;
        _beautify.vignettingFade.overrideState = true;

        // Read existing profile values so we don't wipe them on start
        blurIntensity = _beautify.blurIntensity.value;
        blinkIntensity = _beautify.vignettingBlink.value;
        Outer_Ring = _beautify.vignettingOuterRing.value;
        InnerRing = _beautify.vignettingInnerRing.value;
        Fade = _beautify.vignettingFade.value;
    }
    void Update()
    {
        ApplyValues();
    }

    void OnValidate()
    {
        if (_volume == null)
            _volume = GetComponent<Volume>();

        if (_volume != null && _beautify == null)
            _volume.profile.TryGet(out _beautify);

        ApplyValues();
    }

    void ApplyValues()
    {
        if (_beautify == null) return;

        _beautify.blurIntensity.value = blurIntensity;
        _beautify.vignettingBlink.value = blinkIntensity;
        _beautify.vignettingOuterRing.value = Outer_Ring;
        _beautify.vignettingInnerRing.value = InnerRing;
        _beautify.vignettingFade.value = Fade;
    }
}