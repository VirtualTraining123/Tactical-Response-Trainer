using Fusion;
using UnityEngine;

namespace Projectiles
{
    public abstract class ProjectileSpawner {
        public abstract void SpawnProjectile(Projectile projectilePrefab, Transform bulletSpawn, Pistol owner);
    }

    public class ServerProjectileSpawner : ProjectileSpawner {
        private readonly NetworkRunner _networkRunner;

        public ServerProjectileSpawner(NetworkRunner networkRunner) {
            _networkRunner = networkRunner;
        }

        public override void SpawnProjectile(Projectile projectilePrefab, Transform bulletSpawn, Pistol owner) {
            Debug.Log("Spawning projectile on server");
            var networkObject = projectilePrefab.GetComponent<NetworkObject>();
            var projectileInstance = _networkRunner.Spawn(networkObject, bulletSpawn.position, bulletSpawn.rotation);
            var spawnedProjectile = projectileInstance.GetComponent<NetworkProjectile>();
            spawnedProjectile.Init(owner);
            spawnedProjectile.Launch();
        }
    }

    public class ClientProjectileSpawner : ProjectileSpawner {
        public override void SpawnProjectile(Projectile projectilePrefab, Transform bulletSpawn, Pistol owner) {
            var projectileInstance = Object.Instantiate(projectilePrefab, bulletSpawn.position, bulletSpawn.rotation);
            projectileInstance.Init(owner);
            projectileInstance.Launch();
        }
    }
}