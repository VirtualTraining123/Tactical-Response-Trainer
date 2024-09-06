using System.Collections;
using System.Collections.Generic; // Necesario para usar List<>
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CivilAI : MonoBehaviour, ITakeDamage
{
    const string RUN_TRIGGER = "Run";
    const string CROUCH_TRIGGER = "Crouch";
    [SerializeField] private Material deadMaterial; // Material para el estado destruido
    [SerializeField] private List<Collider>civilCollider; // Collider del civil
    [SerializeField] private List<Renderer> civilRenderers; // Lista de Renderers del enemigo

    [SerializeField] private ParticleSystem bloodSplatterFX;
    [SerializeField] private float startingHealth;
    [SerializeField] private float minTimeUnderCover;
    [SerializeField] private float maxTimeUnderCover;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float damage;
    [Range(0, 100)]
    


    private NavMeshAgent agent;
    private Player player;
    private Transform occupiedCoverSpot;
    private Animator animator;
    // ReSharper disable once InconsistentNaming,hay un par de conflictos que no entiendo
    private float _health;

  //  private Renderer civilRenderer; // Renderer del modelo del civil
    private bool isDestroyed; // Flag para verificar si el civil ha sido destruido
    // Contadores
    public static int CivildeadCount;
    private static readonly int Run = Animator.StringToHash(RUN_TRIGGER);
    private static readonly int Crouch = Animator.StringToHash(CROUCH_TRIGGER);

    private Evaluator evaluator;

    [SerializeField] private AudioClip[] audios;
    private AudioSource controlAudio;

    private float Health
    {
        get => _health;
        set => _health = Mathf.Clamp(value, 0, startingHealth);
    }

    private void Awake()
    {
        civilRenderers = new List<Renderer>(GetComponentsInChildren<Renderer>()); 
        Initialize();
        controlAudio = GetComponent<AudioSource>();
        
    }

    private void Initialize()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        Health = startingHealth;
        // Otras inicializaciones si es necesario
        animator.SetTrigger(Run);
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
        SeleccionAudio(0, 0.2f);
    }

    private void Update()
    {
        RotateTowardsPlayer();

        if (agent.isStopped == false && (transform.position - occupiedCoverSpot.position).sqrMagnitude <= 0.1f)
        {
            agent.isStopped = true;
            controlAudio.Stop();
            HideBehindCover();
        }
    }
    private void SeleccionAudio(int indice, float volumen)
    {
        controlAudio.PlayOneShot(audios[indice], volumen);
    }
    private void HideBehindCover()
    {
        animator.SetTrigger(Crouch);
        StartCoroutine(StayUnderCover());
    }

    private IEnumerator StayUnderCover()
    {
        float timeUnderCover = Random.Range(minTimeUnderCover, maxTimeUnderCover);
        yield return new WaitForSeconds(timeUnderCover);
        // Puedes agregar lógica adicional aquí si es necesario
        GetToCover();
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
        ParticleSystem effect = Instantiate(bloodSplatterFX, contactPoint, Quaternion.LookRotation(weapon.transform.position - contactPoint));
        effect.Play();
        Health -= weapon.GetDamage();
       
        if (Health <= 0 && !isDestroyed)
        {
            evaluator.CivilianHit();
            if (deadMaterial != null && civilRenderers != null)
            {
                foreach (Renderer renderer1 in civilRenderers)
                {
                    renderer1.material = deadMaterial;
                }
            }
            // Desactivar el collider del enemigo
            foreach (Collider collider1 in civilCollider)
            {
                if (collider1 != null)
                {
                    collider1.enabled = false;
                }
            }
            
            if (projectile != null)
            {
               // Destroy(projectile.gameObject);
            }

            // Establecer el flag de destruido a true
            isDestroyed = true;

            // Detener el comportamiento del enemigo (pausarlo)
            agent.isStopped = true;
            animator.enabled = false;
            Debug.Log("Civil destroyed!");
            CivildeadCount++;
            
            
        }
        else
        { //agregar 1 flag con un if aca para que solo se active 1 vez si se desea implementarlo
        }

           
    }
}
