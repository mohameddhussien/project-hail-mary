using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Skybox))]
public class SkyBoxSetter : MonoBehaviour
{
    [SerializeField] List<Material> _skyboxMaterials;

    Skybox _skybox;
    // Update is called once per frame
    void Awake()
    {
        _skybox = GetComponent<Skybox>();
    }
    
    void OnEnable()
    {
        ChangeSkybox(0);
    }

    void ChangeSkybox(int skybox)
    {
        if (_skybox != null && skybox >= 0 && skybox < _skyboxMaterials.Count)
        {
            _skybox.material = _skyboxMaterials[skybox];
        }
    }
}
