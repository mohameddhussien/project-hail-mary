using System.Runtime.CompilerServices;
using UnityEngine;

public class MatchRotation : MonoBehaviour
{
    [SerializeField]
    private Transform _transform;

    void LateUpdate()
    {
        transform.rotation = _transform.rotation;
    }

}
