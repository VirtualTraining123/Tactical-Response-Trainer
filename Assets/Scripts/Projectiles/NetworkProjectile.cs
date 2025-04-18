using UnityEngine;
using Fusion;

namespace Projectiles
{
    public class NetworkProjectile : BaseProjectile
    {
        [Networked] private Vector3 NetworkedVelocity { get; set; }
        [Networked] private TickTimer LifeTimer { get; set; }

        private NetworkObject _networkObject;
        private float _maxLifetime = 5.0f;
        public NetworkRunner Runner => _networkObject?.Runner;
        public NetworkObject NetworkObj => _networkObject;

        public override void Init(Weapon weapon)
        {
            base.Init(weapon);
            // Set up a lifetime for the projectile to avoid orphaned objects
            LifeTimer = TickTimer.CreateFromSeconds(Runner, _maxLifetime);
        }

        public override void Launch()
        {
            var rb = GetComponent<Rigidbody>();
            rb.linearVelocity = transform.forward * speed;
            NetworkedVelocity = rb.linearVelocity;

            // Spawn trail effect on all clients
            HandleTrail();
        }

        public void FixedUpdateNetwork()
        {
            if (LifeTimer.Expired(Runner))
            {
                Runner.Despawn(_networkObject);
                return;
            }
            transform.position += NetworkedVelocity * Runner.DeltaTime;
        }

        // TODO
        // This is laggy, maybe the trail should be local only
        // But it's a good example of how to spawn effects on all clients
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        protected override void HandleTrail()
        {
            var trail = Instantiate(bulletTrailPrefab, transform.position, Quaternion.identity);
            trail.transform.SetParent(transform);
        }

        private void Awake()
        {
            _networkObject = GetComponent<NetworkObject>();
        }
    }
}
