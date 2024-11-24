using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;

    public LayerMask GroundLayer, PlayerLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }
    
    private void ChasePlayer()
    {
        agent.SetDestination(target.position - new Vector3(-1, target.position.y, -1));
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
