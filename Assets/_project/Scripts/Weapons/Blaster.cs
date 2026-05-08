using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Blaster : MonoBehaviour
{
    [SerializeField] Projectile _projectilePrefab;
    [SerializeField] AudioClip _fireSound;
    [SerializeField] Transform _muzzle;

    float _coolDownTime;
    int _launchForce, _damage;
    float _duration;
    IWeaponControls _weaponInput;
    float _coolDown;
    Rigidbody _rigidBody;
    AudioSource _audioSource;

    bool CanFire
    {
        get
        {
            _coolDown -= Time.deltaTime;
            return _coolDown <= 0f;
        }
    }

    void Awake()
    {
        _audioSource = SoundManager.Configure3DAudioSource(GetComponent<AudioSource>());
    }

    void Update()
    {
        if (_weaponInput == null) return;
        if (CanFire && _weaponInput.PrimaryFired)
        {
            FireProjectile();
        }
    }

    public void Init(IWeaponControls weaponInput, float coolDown, int launchForce, float duration, int damage, Rigidbody rigidBody)
    {
        _weaponInput = weaponInput;
        _coolDownTime = coolDown;
        _launchForce = launchForce;
        _duration = duration;
        _damage = damage;
        _rigidBody = rigidBody;
    }

    void FireProjectile()
    {
        if (_fireSound)
        {
            _audioSource.PlayOneShot(_fireSound);
        }
        _coolDown = _coolDownTime;
        if (_projectilePrefab == null || _muzzle == null) return;
        
        Projectile projectile = Instantiate(_projectilePrefab, _muzzle.position, transform.rotation);
        projectile.gameObject.SetActive(false);
        projectile.Init(_launchForce, _damage, _duration, _rigidBody.linearVelocity, _rigidBody.angularVelocity);
        projectile.gameObject.SetActive(true);
    }

}