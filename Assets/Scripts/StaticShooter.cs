using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class StaticShooter : Weapon
{
    [SerializeField] private Projectile bulletPrefab;
    [SerializeField] private float shootingInterval = 3f; // Intervalo de tiempo entre disparos en segundos

    protected virtual void Start()
    {
        StartCoroutine(ShootRoutine()); // Comienza la rutina de disparo
    }

    protected IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootingInterval); // Espera el intervalo de tiempo antes del próximo disparo
            Shoot(); // Realiza un disparo
        }
    }

    protected override void Shoot()
    {
        base.Shoot();
        Projectile projectileInstance = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        projectileInstance.Init(this);
        projectileInstance.Launch();
    }
}