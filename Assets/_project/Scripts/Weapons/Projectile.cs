using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField][Range(5000f, 25000f)] float launchForce = 10000f;
    [SerializeField][Range(10, 1000)] int _damage = 100;
    [SerializeField][Range(2f, 10f)] float _range = 2f;
    Rigidbody _rigidbody;

    bool outOfFuel { get { duration -= Time.deltaTime; return duration <= 0f; } }

    private float duration;
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        _rigidbody.AddForce(launchForce * transform.forward);
        duration = _range;
    }

    void Update()
    {
        if (outOfFuel) Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.collider.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Vector3 hitPosition = collision.GetContact(0).point;
            damageable.TakeDamage(_damage, hitPosition);
        }
    }
}
