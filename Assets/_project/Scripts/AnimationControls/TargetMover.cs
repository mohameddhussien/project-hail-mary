using UnityEngine;

public class TargetMover : MonoBehaviour
{
    [SerializeField] private float _horizontalRange, _verticalRange, _frequency;

    Transform _transform;
    Vector3 _movement, _startPosition;
    
    void Start()
    {
        _transform = transform;
        _movement = Vector3.zero;
        _startPosition = _transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float sin = Mathf.Sin(Time.time * _frequency);
        _movement.x = sin * _horizontalRange;
        _movement.y = sin * _verticalRange;
        _transform.position = _startPosition + _movement;
    }
}