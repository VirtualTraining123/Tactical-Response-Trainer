using System;
using System.Collections.Generic;
using System.Linq;
using Projectiles;
using UnityEngine;
using UnityEngine.AI;

namespace AI {
  public abstract class AI : BodyMasterDamageable {
    [SerializeField] public Material deadMaterial;
    [SerializeField] public ParticleSystem bloodSplatterFX;
    protected Animator Animator;
    protected NavMeshAgent NavigationMesh;
    private Dictionary<string, AudioSource> audioSources = new();

    [SerializeField] private float health;
    [SerializeField] private bool updateAI;
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
    private State state = State.Running;

    protected virtual void Awake() {
      renderers = GetComponentsInChildren<Renderer>();
      colliders = GetComponentsInChildren<Collider>();
      Animator = GetComponent<Animator>();
      NavigationMesh = FindFirstObjectByType<NavMeshAgent>();
      Evaluator = FindFirstObjectByType<Evaluator>(FindObjectsInactive.Include);
      foreach (var source in GetComponentsInChildren<AudioSource>()) {
        if (!audioSources.TryAdd(source.clip.name, source)) {
          Debug.LogWarning($"Audio clip name conflict: {source.clip.name} already exists.");
        }
      }
    }

    public override void TakeDamage(Weapon weapon, Projectile projectile, Vector3 contactPoint, BodyPart part) {
      Debug.Log($"Hit on {part}");
      var effect = Instantiate(
        bloodSplatterFX,
        contactPoint,
        Quaternion.LookRotation(weapon.transform.position - contactPoint)
      );
      effect.Play();
      health -= weapon.GetDamage();

      if (!ShouldDie()) return;
      ConvertToDeadMaterial();
      DisableAllColliders();
      isDestroyed = true;
      StopAnimations();
      OnDie(part);
      ToState(State.Dead);
    }

    protected abstract void OnDie(BodyPart finalHit);

    private void StopAnimations() {
      NavigationMesh.isStopped = true;
      Animator.enabled = false;
    }

    private bool ShouldDie() {
      return deadMaterial && health <= 0 && !isDestroyed;
    }

    private void ConvertToDeadMaterial() {
      foreach (var renderer1 in renderers) {
        renderer1.material = deadMaterial;
      }
    }

    private void DisableAllColliders() {
      foreach (var collider1 in colliders.Where(collider1 => collider1)) {
        collider1.enabled = false;
      }
    }

    protected void ToState(State newState) {
      state = newState;
      NavigationMesh.isStopped = state != State.Running;
      switch (state) {
        case State.Running:
          Animator.SetTrigger(Run);
          Animator.ResetTrigger(Crouch);
          Animator.ResetTrigger(Shoot1);
          break;
        case State.Crouching:
          Animator.ResetTrigger(Run);
          Animator.SetTrigger(Crouch);
          Animator.ResetTrigger(Shoot1);
          break;
        case State.Shooting:
          Animator.ResetTrigger(Run);
          Animator.ResetTrigger(Crouch);
          Animator.SetTrigger(Shoot1);
          break;
        case State.Dead:
          enabled = false;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void Update() {
      if (!updateAI) return;
      switch (state) {
        case State.Running:
          UpdateRunning();
          break;
        case State.Crouching:
          UpdateCrouching();
          break;
        case State.Shooting:
          UpdateShooting();
          break;
        case State.Dead:
          UpdateDead();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    protected void PlaySound(string clipName) {
      if (audioSources.TryGetValue(clipName, out var source)) {
        source.Play();
      } else {
        Debug.LogWarning($"Audio clip '{clipName}' not found on {gameObject.name}");
      }
    }

    protected abstract void UpdateRunning();
    protected abstract void UpdateDead();
    protected abstract void UpdateCrouching();
    protected abstract void UpdateShooting();
  }


}
