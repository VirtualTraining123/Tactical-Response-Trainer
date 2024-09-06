using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsProjectile : Projectile
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float gravity = 9.81f; // Gravedad aplicada a la bala
    private float currentGravity;
    private Rigidbody rigidBody;
    public GameObject bulletHolePrefab;
    public GameObject bulletTrailPrefab;  // Prefab del rastro de bala
    private GameObject bulletTrail;       // Instancia del rastro de bala
    private bool destroying;      // Indica si la bala se está destruyendo

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public override void Init(Weapon pWeapon)
    {
        base.Init(pWeapon);
        Destroy(gameObject, lifeTime);
    }

    public override void Launch()
    {
        base.Launch();
        rigidBody.AddRelativeForce(Vector3.forward * Weapon.GetShootingForce(), ForceMode.Impulse);

        // Instanciar el rastro de bala
        if (bulletTrailPrefab != null)
        {
            bulletTrail = Instantiate(bulletTrailPrefab, transform.position, Quaternion.identity);
            bulletTrail.transform.SetParent(transform);  // Hacer que el rastro siga la bala
        }
    }

    private void FixedUpdate()
    {
        if (destroying)
        {
            return;
        }
        currentGravity -= gravity * Time.deltaTime;
        Vector3 moveVector = transform.forward * (Weapon.GetShootingForce() * Time.deltaTime);
        moveVector += Vector3.up * currentGravity;

        if (Physics.Raycast(transform.position, moveVector, out var hit, moveVector.magnitude))
        {
            destroying = true;
            if (bulletHolePrefab != null)
            {
                GameObject exp = Instantiate(bulletHolePrefab, hit.point - moveVector.normalized * .01f, Quaternion.LookRotation(hit.normal));
                exp.transform.SetParent(hit.transform);
            }
            OnBulletHit(hit);
        }
        else
        {
            transform.position += moveVector;
        }
    }

    private void OnBulletHit(RaycastHit hit)
    {
        // Infligir daño a los objetos en las cercanías (si los hay)
        Vector3 contactPoint = hit.point;
        ITakeDamage[] damageTakers = hit.collider.GetComponentsInParent<ITakeDamage>();
        foreach (var taker in damageTakers)
        {
            taker.TakeDamage(Weapon, this, contactPoint);
        }

        // Destruir la bala
        Destroy(gameObject);
    }
}
