using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public float lookRadius = 8f;
    [SerializeField] Transform target;

    NavMeshAgent agent;
    AudioSource groan;
    Animator animator;
    private GameObject canvas;
    private HUD hud;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        groan = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();

        // Make sure we don't try to create the agent off-mesh
        if (agent.enabled) agent.enabled = false;
    }

    void Start()
    {
        // Snap transform onto any nearby NavMesh BEFORE enabling agent
        // Increase radius if needed (try 20–50f if your scene is big)
        if (NavMesh.SamplePosition(transform.position, out var hit, 30f, NavMesh.AllAreas))
        {
            transform.position = hit.position;  // move the transform first
            agent.enabled = true;               // NOW it's close enough to enable
        }
        else
        {
            Debug.LogError($"{name}: No NavMesh found near {transform.position}. Check bake/layers/agent type.");
        }

        canvas = GameObject.FindWithTag("Canvas");
        hud = canvas.GetComponent<HUD>();

    }

    void Update()
    {
        // If agent still isn't enabled (no mesh), idle and bail
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            if (animator) animator.SetFloat("forward", 0f);
            return;
        }

        if (!target || !animator) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // Drive animation by speed (0..1)
        float speed01 = (agent.hasPath && !agent.pathPending)
            ? Mathf.Clamp01(agent.velocity.magnitude / Mathf.Max(0.01f, agent.speed))
            : 0f;
        animator.SetFloat("forward", speed01, 0.1f, Time.deltaTime);

        if (distance <= lookRadius)
        {
            // Safe to call now (we're on-mesh)
            agent.SetDestination(target.position);
            if (groan && !groan.isPlaying) groan.Play();
        }
        else
        {
            agent.ResetPath();
            if (groan && groan.isPlaying) groan.Stop();
        }

        if (distance <= agent.stoppingDistance)
        {
            animator.SetFloat("forward", 0f, 0.1f, Time.deltaTime);
            FaceTarget();
            if (!hud.Infected) 
            {
                hud.collisionWithZombie();
            }
        }
    }

    void FaceTarget()
    {
        if (!target) return;
        Vector3 dir = (target.position - transform.position).normalized;
        var look = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 5f);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
