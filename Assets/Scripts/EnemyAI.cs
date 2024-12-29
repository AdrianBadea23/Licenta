using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;
    [SerializeField] public Animator animator;
    [SerializeField] public float engageDistance = 20f;

    public LayerMask GroundLayer, PlayerLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }
    
    private void ChasePlayer()
    {
        if (Vector3.Distance(transform.position, target.position) <= engageDistance)
        {
            animator.SetBool("EnemyRunning", true);
            agent.SetDestination(target.position - new Vector3(-1, target.position.y, -1));
        }
        
    }

    public void KillEnemy()
    {
        animator.SetBool("IsDead", true);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ChasePlayer();
    }
}
