using Fusion;
using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Networking;

namespace Projectiles
{


    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(XRGrabInteractable))]
    public class Weapon_ProjectileDataBuffer_Hitscan : WeaponBase
    {
        // PRIVATE MEMBERS

        [SerializeField]
        private LayerMask _hitMask;
        [SerializeField]
        private float _hitImpulse = 50f;
        [SerializeField]
        private DummyFlyingProjectile _dummyProjectilePrefab;
        [Networked]
        private int _fireCount { get; set; }
        [Networked, Capacity(32)]
        private NetworkArray<ProjectileData> _projectileData { get; }
        private XRGrabInteractable interactableWeapon;
        private int _visibleFireCount;

        public void Awake()
        {
            interactableWeapon = GetComponent<XRGrabInteractable>();
            interactableWeapon.selectEntered.AddListener(OnWeaponSelected);
            interactableWeapon.selectExited.AddListener(OnWeaponDeselected);
        }

        private void OnWeaponSelected(SelectEnterEventArgs args)
        {
            Debug.Log("Weapon selected by: " + args.interactorObject.transform.name);
            // Find the NetworkedPlayer component on the interactor's hierarchy
            var player = args.interactorObject.transform.GetComponentInParent<NetworkedPlayer>();
            Debug.Log("Player found: " + player);
            if (player != null)
            {
                player.SetCurrentWeapon(this);
            }
            else
            {
                Debug.LogWarning("Weapon selected, but couldn't find NetworkedPlayer on interactor's hierarchy.", args.interactorObject.transform.gameObject);
            }
        }

        private void OnWeaponDeselected(SelectExitEventArgs args)
        {
            var player = args.interactorObject.transform.GetComponentInParent<NetworkedPlayer>();
            if (player != null)
            {
                // If this was the player's current weapon, clear it
                if (player.CurrentWeapon == this)
                {
                    player.SetCurrentWeapon(null);
                }
            }
            else
            {
                // This might happen if the object is destroyed while selected, etc.
            }
        }



        public override void Fire()
        {
            var hitPosition = Vector3.zero;
            if (Runner.LagCompensation == null)
            {
                Debug.LogError("Runner.LagCompensation is null! Lag compensation might not be initialized or available.", this);
                // FallbackFireMethod(); 
                return;
            }

            var hitOptions = HitOptions.IncludePhysX | HitOptions.IgnoreInputAuthority;
            // Whole projectile path and effects are immediately processed (= hitscan projectile)
            if (Runner.LagCompensation.Raycast(FireTransform.position, FireTransform.forward, 100f,
                    Object.InputAuthority, out var hit, _hitMask, hitOptions) == true)
            {
                if (hit.Collider != null && hit.Collider.attachedRigidbody != null)
                {
                    hit.Collider.attachedRigidbody.AddForce(FireTransform.forward * _hitImpulse, ForceMode.Impulse);
                }

                hitPosition = hit.Point;
            }

            // As opposed to Example 03, with projectile data buffer it would be possible to fire
            // multiple projectiles at once (e.g. shotgun)
            _projectileData.Set(_fireCount % _projectileData.Length, new ProjectileData()
            {
                HitPosition = hitPosition,
            });

            _fireCount++;
        }

        public override void Spawned()
        {
            _visibleFireCount = _fireCount;
        }

        public override void Render()
        {
            if (_visibleFireCount < _fireCount)
            {
                PlayFireEffect();
            }

            if (_dummyProjectilePrefab != null)
            {
                // As opposed to Example 03, all missing projectiles are instantiated here
                for (int i = _visibleFireCount; i < _fireCount; i++)
                {
                    var data = _projectileData[i % _projectileData.Length];
                    Debug.Log("Firing projectile at position: " + FireTransform.position.ToString());
                    var dummyProjectile = Instantiate(_dummyProjectilePrefab, FireTransform.position, FireTransform.rotation);
                    dummyProjectile.SetHitPosition(data.HitPosition);

                    // When using multipeer, move to correct scene and disable renderers for other clients. Can be omitted otherwise.
                    if (Runner.Config.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
                    {
                        Runner.MoveToRunnerScene(dummyProjectile);
                        Runner.AddVisibilityNodes(dummyProjectile.gameObject);
                    }
                }
            }

            _visibleFireCount = _fireCount;
        }

        private struct ProjectileData : INetworkStruct
        {
            public Vector3 HitPosition;

            // ProjectileData struct can be expanded with additional data
            // like ImpactNormal, ImpactType to better reconstruct projectile effects on all clients
            // See ProjectileManager in the Projectiles Advanced.
            // It is however best practice to keep the ProjectileData struct as small as possible.
        }
    }
}
