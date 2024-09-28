using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AI {
  public abstract class AI : MonoBehaviour, ITakeDamage {
    [SerializeField] public Material deadMaterial;
    [SerializeField] public ParticleSystem bloodSplatterFX;
    [SerializeField] protected Animator animator;
    protected NavMeshAgent navigationMesh;
    [SerializeField] private float health;
    private Renderer[] renderers;
    private Collider[] colliders;
    /// <summary>
    /// Has the AI been destroyed?
    /// </summary>
    private bool isDestroyed;
    
    // TODO: Move these to a separate class
    protected const string RUN_TRIGGER = "Run";
    protected const string CROUCH_TRIGGER = "Crouch";
    protected const string SHOOT_TRIGGER = "Shoot";
    protected static readonly int Run = Animator.StringToHash(RUN_TRIGGER);
    protected static readonly int Crouch = Animator.StringToHash(CROUCH_TRIGGER);
    protected static readonly int Shoot1 = Animator.StringToHash(SHOOT_TRIGGER);
    protected Evaluator Evaluator;

    protected virtual void Awake() {
      renderers = GetComponentsInChildren<Renderer>();
      colliders = GetComponentsInChildren<Collider>();
      animator = GetComponent<Animator>();
      navigationMesh = FindObjectOfType<NavMeshAgent>();
      Evaluator = FindObjectOfType<Evaluator>();
    }

    public void TakeDamage(Weapon weapon, Projectile projectile, Vector3 contactPoint) {
      var effect = Instantiate(
        bloodSplatterFX,
        contactPoint,
        Quaternion.LookRotation(weapon.transform.position - contactPoint)
      );
      effect.Play();
      health -= weapon.GetDamage();

      if (!ShouldDie()) return;
      onDie();
      Die();
      DisableAllColliders();
      isDestroyed = true;
      StopAnimations();
    }

    protected abstract void onDie();

    private void StopAnimations() {
      navigationMesh.isStopped = true;
      animator.enabled = false;
    }

    private bool ShouldDie() {
      return deadMaterial && health <= 0 && !isDestroyed;
    }

    private void Die() {
      foreach (var renderer1 in renderers) {
        renderer1.material = deadMaterial;
      }
    }

    private void DisableAllColliders() {
      foreach (var collider1 in colliders.Where(collider1 => collider1)) {
        collider1.enabled = false;
      }
    }
  }
}