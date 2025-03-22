using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class BossFodder : MonoBehaviour
{
    // [SerializeField] Boss boss;
    // private NavMeshAgent _agent;
    // private Rigidbody _agentRigidbody;
    private float waitTime = 0f;
    private float timer = 0f;
    public float health = 2f;
    private GameObject player;
    private float speed = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }

        if (health <= 0f)
        {
            Destroy(this.gameObject);
            player.GetComponent<ShootingSpirit>().AddReward(0.02f);
        }
    }

    public void SetPosition(Vector3 pos)
    {
        transform.localPosition = pos;
    }

    public void SetHealth(float health)
    {
        this.health = health;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Thunderbolt"))
        {
            health -= 0.3f;
            Debug.Log("Thunderbolt hit " + health);
        }
        
        if (other.CompareTag("EnumaElis"))
        {
            health = -1f;
        }

        if (other.CompareTag("Spin"))
        {
            health -= 1f;
        }

        if (other.CompareTag("Shadow"))
        {
            speed = 2;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Meteor"))
        {
            health -= 0.007f;
        }
        
        if (other.CompareTag("GhostMinions"))
        {
            health -= 0.007f;
        }

        if (other.CompareTag("Swarm"))
        {
            health = -0.002f;
        }
    }
}
