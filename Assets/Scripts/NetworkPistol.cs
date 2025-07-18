using Projectiles;
using UnityEngine;
using Fusion;
using System;

public class NetworkPistol : Pistol
{
    [SerializeField] private NetworkRunner networkRunner;
    [SerializeField] public bool DoShoot;

    private void Update() {
        if (DoShoot) {
            DoShoot = false;
            Shoot();
        }
    }

    protected override void SpawnProjectile()
    {
        Debug.Log("Spawning projectile on network");
        var networkObject = projectilePrefab.GetComponent<NetworkObject>();
        var projectileInstance = networkRunner.Spawn(networkObject, bulletSpawn.position, bulletSpawn.rotation);
        var spawnedProjectile = projectileInstance.GetComponent<NetworkProjectile>();
        spawnedProjectile.Init(this);
        spawnedProjectile.Launch();
    }

}