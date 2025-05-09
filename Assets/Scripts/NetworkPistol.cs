using Projectiles;
using UnityEngine;
using Fusion;

public class NetworkPistol : Pistol
{
    [SerializeField] private NetworkRunner networkRunner;

    protected override void SpawnProjectile()
    {
        var networkObject = projectilePrefab.GetComponent<NetworkObject>();
        var projectileInstance = networkRunner.Spawn(networkObject, bulletSpawn.position, bulletSpawn.rotation);
        var spawnedProjectile = projectileInstance.GetComponent<NetworkProjectile>();
        spawnedProjectile.Init(this);
        spawnedProjectile.Launch();
    }

}