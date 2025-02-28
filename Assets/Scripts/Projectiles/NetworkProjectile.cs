using UnityEngine;
using Fusion;

namespace Projectiles
{
    public class NetworkProjectile : Projectile {
        [Networked] private Vector3 NetworkedVelocity { get; set; }
        [Networked] private TickTimer LifeTimer { get; set; }

        private NetworkObject _networkObject;
        private float _maxLifetime = 5.0f;
        public NetworkRunner Runner => _networkObject?.Runner;
        public NetworkObject NetworkObj => _networkObject;

        private void Awake() {
            _networkObject = GetComponent<NetworkObject>();
            Debug.Log("NetworkProjectile is here??" + _networkObject);
        }

        public override void Init(Weapon weapon) {
            base.Init(weapon);

            if (Runner.IsServer) {
                // Set up a lifetime for the projectile to avoid orphaned objects
                LifeTimer = TickTimer.CreateFromSeconds(Runner, _maxLifetime);
            }
        }

        public override void Launch() {
            if (Runner.IsServer) {
                var rb = GetComponent<Rigidbody>();
                rb.linearVelocity = transform.forward * speed;
                NetworkedVelocity = rb.linearVelocity;

                // Spawn trail effect on all clients
                RPC_SpawnTrail();
            }
        }

        public override void FixedUpdateNetwork() {
            if (LifeTimer.Expired(Runner)) {
                Runner.Despawn(_networkObject);
                return;
            }

            // Non-authoritative clients should update position based on networked velocity
            if (!Runner.IsServer) {
                transform.position += NetworkedVelocity * Runner.DeltaTime;
            }
        }

        protected override void HandleHit(ContactPoint contact, Transform hitTransform) {
            base.HandleHit(contact, hitTransform);

            // Despawn the networked object when it hits something (only on server)
            if (Runner.IsServer) {
                Runner.Despawn(_networkObject);
            }
        }
        // TODO
        // This is laggy, maybe the trail should be local only
        // But it's a good example of how to spawn effects on all clients
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_SpawnTrail(){
            if (bulletTrailPrefab == null) return;
            var trail = Instantiate(bulletTrailPrefab, transform.position, Quaternion.identity);
            trail.transform.SetParent(transform);
        }
    }
}
