using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class BossFodder : MonoBehaviour
{
    [SerializeField] Boss boss;
    private NavMeshAgent _agent;
    private Rigidbody _agentRigidbody;
    private float waitTime = 0f;
    private float timer = 0f;
    private float health = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agentRigidbody = GetComponent<Rigidbody>();
        // MoveToRandomPosition();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        // {
        //     timer += Time.deltaTime;
        //     if (timer >= waitTime)
        //     {
        //         MoveToRandomPosition();
        //         timer = 0f;
        //     }
        // }
    }

    public void SetPosition(Vector3 pos)
    {
        transform.localPosition = pos;
    }

    // void MoveToRandomPosition()
    // {
    //     Vector3 randomDirection = Random.insideUnitSphere * 5f;
    //     randomDirection.y = 0f;
    //     randomDirection += transform.position;
    //     NavMeshHit hit;
    //
    //     if (NavMesh.SamplePosition(randomDirection, out hit, 20.0f, NavMesh.AllAreas))
    //     {
    //         _agent.SetDestination(hit.position);
    //     }
    //     
    // }
    
}
