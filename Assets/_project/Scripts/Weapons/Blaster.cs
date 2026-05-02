using UnityEngine;

public class Blaster : MonoBehaviour
{
    [SerializeField] Projectile _projectilePrefab;
    [SerializeField] Transform _muzzleTransform;
    [SerializeField][Range(0f, 5f)] float _coolDownTime = 0.25f;
    float _coolDown;

    bool canFire { get { _coolDown -= Time.deltaTime; return _coolDown <= 0f; } }
    // Update is called once per frame
    void Update()
    {
        if (canFire && Input.GetMouseButton(0)) Fire();
    }

    void Fire()
    {
        _coolDown = _coolDownTime;
        Instantiate(_projectilePrefab, _muzzleTransform.position, transform.rotation);
    }
}
