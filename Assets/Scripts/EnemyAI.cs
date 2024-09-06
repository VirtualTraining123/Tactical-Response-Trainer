using System.Collections;
using System.Collections.Generic; // Necesario para usar List<>
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour, ITakeDamage
{
    private static readonly int Run = Animator.StringToHash(RUN_TRIGGER);
    private static readonly int Crouch = Animator.StringToHash(CROUCH_TRIGGER);
    private static readonly int Shoot1 = Animator.StringToHash(SHOOT_TRIGGER);

    // ReSharper disable once Unity.IncorrectMonoBehaviourInstantiation, genera un error de sintaxis la solucion sugerida
    readonly EnemyAudioScript enemyAudioScript = new EnemyAudioScript();
    const string RUN_TRIGGER = "Run";
    const string CROUCH_TRIGGER = "Crouch";
    const string SHOOT_TRIGGER = "Shoot";

    [SerializeField] private Material deadMaterial; // Material para el estado destruido
    [SerializeField] private List<Collider> enemyColliders; // Lista de Colliders del enemigo
    [SerializeField] private List<Renderer> enemyRenderers; // Lista de Renderers del enemigo
    [SerializeField] private float startingHealth;
    [SerializeField] private float minTimeUnderCover;
    [SerializeField] private float maxTimeUnderCover;
    [SerializeField] private int minShotsToTake;
    [SerializeField] private int maxShotsToTake;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float damage;
    [Range(0, 100)] [SerializeField] private float shootingAccuracy;

    [SerializeField] private Transform shootingPosition;
    [SerializeField] private ParticleSystem bloodSplatterFX;

    [Header("Haptic Vest Events")] [SerializeField]
    private UnityEvent hapticEvent1;

    [SerializeField] private UnityEvent hapticEvent2;

    [Header("Scene Transition")] [SerializeField]
    private string nextSceneName; // Nombre de la siguiente escena

    private int remainingEnemies;
    private bool isShooting;
    private int currentShotsTaken;
    private int currentMaxShotsToTake;
    private NavMeshAgent agent;
    private Player player;
    private Transform occupiedCoverSpot;
    private Animator animator;
    [SerializeField] private AudioClip[] audios;

    private AudioSource controlAudio;

    //   private Renderer enemyRenderer; // Renderer del modelo del enemigo
    private bool isDestroyed; // Flag para verificar si el enemigo ha sido destruido

    // Contadores

    private float health;
    private Evaluator evaluator;

    private float Health
    {
        get => health;
        set => health = Mathf.Clamp(value, 0, startingHealth);
    }

    private void Awake()
    {
        enemyRenderers = new List<Renderer>(GetComponentsInChildren<Renderer>());

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        animator.SetTrigger(Run);
        health = startingHealth;
        controlAudio = GetComponent<AudioSource>();
        evaluator = FindObjectOfType<Evaluator>(); // Get the Evaluator instance
    }

    public void Init(Player parameterPlayer, Transform coverSpot)
    {
        occupiedCoverSpot = coverSpot;
        this.player = parameterPlayer;
        GetToCover();
    }

    private void GetToCover()
    {
        agent.isStopped = false;
        agent.SetDestination(occupiedCoverSpot.position);
        SeleccionAudio(1, 0.2f);
    }

    private void Update()
    {
        if (agent.isStopped == false && (transform.position - occupiedCoverSpot.position).sqrMagnitude <= 0.1f)
        {
            agent.isStopped = true;
            //detenemos el la reproduccion de sonido por unica vez
            controlAudio.Stop();
            StartCoroutine(InitializeShootingCo());
        }

        if (isShooting)
        {
            RotateTowardsPlayer();
        }
    }

    private IEnumerator InitializeShootingCo()
    {
        HideBehindCover();
        yield return new WaitForSeconds(Random.Range(minTimeUnderCover, maxTimeUnderCover));
        StartShooting();
    }

    private void HideBehindCover()
    {
        animator.SetTrigger(Crouch);
    }

    private void StartShooting()
    {
        isShooting = true;
        currentMaxShotsToTake = Random.Range(minShotsToTake, maxShotsToTake);
        currentShotsTaken = 0;
        animator.SetTrigger(Shoot1);
        //SeleccionAudio(0, 0.5f);
    }

    private void SeleccionAudio(int indice, float volumen)
    {
        controlAudio.PlayOneShot(audios[indice], volumen);
    }

    // ReSharper disable once UnusedMember.Global
    public void Shoot() //wtf? como que unused? rider?
    {
        var direction = player.GetHeadPosition() - shootingPosition.position;
        if (Physics.Raycast(shootingPosition.position, direction, out var hit))
        {
            enemyAudioScript.PlayShootSound();

            Debug.DrawRay(shootingPosition.position, direction, Color.green, 2.0f);
            //tomamos el componente Player del objeto que colisiono con el rayo, pero en su padre Complete XR
            Player componentInParent = hit.collider.GetComponentInParent<Player>();
            //Player player = hit.collider.GetComponentInParent<Player>();


            if (componentInParent)
            {
                SeleccionAudio(0, 0.5f);

                //probabilidad del 50% de que el enemigo acierte
                if (Random.Range(0, 100) < shootingAccuracy)
                {
                    componentInParent.TakeDamage(damage);
                }
            }
            else
            {
                Debug.LogWarning("Ray hit something, but it's not the player.");
            }
        }
        else
        {
            Debug.DrawRay(shootingPosition.position, direction, Color.red, 2.0f);
            Debug.LogWarning("Ray did not hit anything.");
        }


        currentShotsTaken++;
        if (currentShotsTaken >= currentMaxShotsToTake)
        {
            StartCoroutine(InitializeShootingCo());
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = player.GetHeadPosition() - transform.position;
        direction.y = 0;
        Quaternion rotation = Quaternion.LookRotation(direction);
        rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        transform.rotation = rotation;
    }

    public void TakeDamage(Weapon weapon, Projectile projectile, Vector3 contactPoint)
    {
        ParticleSystem effect = Instantiate(bloodSplatterFX, contactPoint,
            Quaternion.LookRotation(weapon.transform.position - contactPoint));
        effect.Play();
        Health -= weapon.GetDamage();
        if (Health <= 0 && !isDestroyed)
        {
            if (deadMaterial != null && enemyRenderers != null)
            {
                foreach (Renderer renderer1 in enemyRenderers)
                {
                    renderer1.material = deadMaterial;
                }
            }

            // Desactivar todos los colliders del enemigo
            foreach (Collider collider1 in enemyColliders)
            {
                if (collider1 != null)
                {
                    collider1.enabled = false;
                }
            }

            if (projectile != null)
            {
                Destroy(projectile.gameObject);
            }

            // Establecer el flag de destruido a true
            isDestroyed = true;

            // Detener el comportamiento del enemigo (pausarlo)
            agent.isStopped = true;
            animator.enabled = false;

            //   NotifyEnemyDestroyed();
            evaluator.EnemyNeutralized();
        }
        else
        {
            //agregar 1 flag con un if aca para que solo se active 1 vez si se desea implementarlo
        }
    }
}