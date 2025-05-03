using System;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss : Agent
{
    private GameObject bossFodder;
    [SerializeField] private GameObject forceField;
    [SerializeField] private GameObject spinField;
    [SerializeField] private GameObject burstField;
    [SerializeField] private GameObject deathParticle;
    [SerializeField] private GameObject chara;
    // [SerializeField] private ShootingSpirit shootingSpirit;
    private Vector3 startPos;
    private Rigidbody _agentRigidbody;
    private float spinTimer = 0f;
    private float areaTimer = 0f;
    private float projectileTimer = 0f;
    private float shieldTimer = 0f;
    private bool shieldActive = false;
    private float shieldTimerDuration = 1f;
    private List<GameObject> hotZone = new List<GameObject>();
    private float lingerTimer = 4f;
    private bool lingerActive = false;
    public float health = 10f;
    private int flasks = 3;

    private  float moveSpeed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agentRigidbody = GetComponent<Rigidbody>();
        // startPos = transform.localPosition;
        bossFodder = GameObject.FindWithTag("Player");
    }

    void FixedUpdate()
    {
        if (bossFodder == null)
        {
            bossFodder = GameObject.FindWithTag("Player");
        }
        
        if (spinTimer > 0f)
        {
            spinTimer -= Time.fixedDeltaTime;
        }

        if (areaTimer > 0f)
        {
            areaTimer -= Time.fixedDeltaTime;
        }

        if (projectileTimer > 0f)
        {
            projectileTimer -= Time.fixedDeltaTime;
        }

        if (shieldTimer > 0f)
        {
            shieldTimer -= Time.fixedDeltaTime;
        }

        if (shieldActive)
        {
            shieldTimerDuration -= Time.fixedDeltaTime;
            // Debug.Log(shieldTimerDuration);
            // Debug.Log(shieldActive);
        }

        if (shieldTimerDuration <= 0f)
        {
            shieldActive = false;
            shieldTimerDuration = 2f;
            // Debug.Log(shieldActive);
        }

        if (lingerActive)
        {
            lingerTimer -= Time.fixedDeltaTime;
        }

        if (lingerTimer <= 0)
        {
            for(int i = 0; i < hotZone.Count; i++)
            {
                Destroy(hotZone[i]);
            }
            
            hotZone.Clear();
            lingerTimer = 0f;
        }

        if (health <= 0f)
        {
            // SetReward(-0.5f);
            // bossFodder.SetReward(0.5f);
            // EndEpisode();
            // bossFodder.EndEpisode();
            // Debug.Log("SolidSnake Lost");
            
            Destroy(this.gameObject);
            // shootingSpirit.numberOfDeaths += 1;
            Debug.Log("Boss Death");
            
            GameObject[] booms = GameObject.FindGameObjectsWithTag("Spin");
            GameObject[] ultimateBooms = GameObject.FindGameObjectsWithTag("Burst");
            GameObject[] forceFields = GameObject.FindGameObjectsWithTag("Force");
            
            foreach (var boom in booms)
            {
                Destroy(boom);
            }
            
            foreach (var ultimate in ultimateBooms)
            {
                Destroy(ultimate);
            }

            foreach (var field in forceFields)
            {
                Destroy(field);
            }
            
            GameObject deathPart = Instantiate(deathParticle, transform.position, Quaternion.identity);
            Destroy(deathPart, 2f);
        }
        
    }

    public override void OnEpisodeBegin()
    {
        health = 10f;
        // transform.localPosition = startPos;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(bossFodder.transform.position);
        sensor.AddObservation(spinTimer);
        sensor.AddObservation(areaTimer);
        sensor.AddObservation(projectileTimer);
        sensor.AddObservation(shieldTimer);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        // Map WASD keys to discrete actions
        if (Input.GetKey(KeyCode.W))
        {
            discreteActions[0] = 0; // Forward
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActions[0] = 1; // Backward
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActions[0] = 2; // Right
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActions[0] = 3; // Left
        }
        else
        {
            discreteActions[0] = 4;
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            discreteActions[1] = 0;
        }else if(Input.GetKey(KeyCode.Alpha2))
        {
            discreteActions[1] = 1;
        }else if (Input.GetKey(KeyCode.Alpha3))
        {
            discreteActions[1] = 2;
        }else if (Input.GetKey(KeyCode.Alpha4))
        {
            discreteActions[1] = 3;
        }
        else
        {
            discreteActions[1] = 4;
        }
        
    }

    private void SpinAttack()
    {
        if (spinTimer <= 0f)
        {
            GameObject spawnedObject = Instantiate(spinField, transform.position, Quaternion.identity);

            Collider bossCollider = GetComponent<Collider>();
            Collider spinCollider = spawnedObject.GetComponent<Collider>();

            if (spinCollider != null && bossCollider != null)
            {
                Physics.IgnoreCollision(bossCollider, spinCollider, true);
            }
            
            Destroy(spawnedObject, 1f);
            spinTimer = 1.5f;
        }
        
    }

    private void AreaSlam()
    {
        if (areaTimer <= 0f)
        {
            GameObject spawnedObject = Instantiate(forceField, bossFodder.transform.position, Quaternion.identity);

            Collider bossCollider = GetComponent<Collider>();
            Collider forceCollider = spawnedObject.GetComponent<Collider>();

            if (forceCollider != null && bossCollider != null)
            {
                Physics.IgnoreCollision(bossCollider, forceCollider, true);
            }
            
            Destroy(spawnedObject, 0.1f);
            areaTimer = 1f;
        }
        
    }

    private void ProjectileBurst()
    {
        if (projectileTimer <= 0f)
        {
            int totalObjects = 10;
            for (int i = 0; i < totalObjects; i++)
            {
                float angle = i * 36;
                float radians = angle * Mathf.Deg2Rad;
                Vector3 player = transform.position;
                Vector3 spawnPosition = new Vector3(
                    player.x + Mathf.Cos(radians) * 5,
                    player.y,
                    player.z + Mathf.Sin(radians) * 5
                );
                GameObject spawnedObject = Instantiate(burstField, spawnPosition, Quaternion.identity);
                Collider bossCollider = GetComponent<Collider>();
                Collider burstCollider = spawnedObject.GetComponent<Collider>();

                if (burstCollider != null && bossCollider != null)
                {
                    Physics.IgnoreCollision(bossCollider, burstCollider, true);
                }
                hotZone.Add(spawnedObject);
            }
            projectileTimer = 6f;
            lingerTimer = 0.7f;
            lingerActive = true;
        }
        
    }

    private void ShieldMode()
    {
        
        if (shieldTimer <= 0f)
        {
            shieldActive = true;
            shieldTimer = 4f;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int movementAction = actions.DiscreteActions[0];
        int spellAction = actions.DiscreteActions[1];
        moveSpeed = 10f;
        Vector3 movement = Vector3.zero;
        switch (movementAction)
        {
            case 0: // Move forward
                movement = transform.forward * moveSpeed * Time.fixedDeltaTime;
                chara.transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
                break;

            case 1: // Move backward
                movement = -transform.forward * moveSpeed * Time.fixedDeltaTime;
                chara.transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
                break;

            case 2: // Move right
                movement = transform.right * moveSpeed * Time.fixedDeltaTime;
                chara.transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
                break;

            case 3: // Move left
                movement = -transform.right * moveSpeed * Time.fixedDeltaTime;
                chara.transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
                break;
            
            case 4:
                movement = Vector3.zero;
                break;
            
            default:
                break;
        }

        switch (spellAction)
        {
            case 0:
                SpinAttack();
                break;
            
            case 1:
                AreaSlam();
                break;
            
            case 2:
                ProjectileBurst();
                break;
            
            case 3:
                ShieldMode();
                break;
            
            case 4:
                break;
            
            default:
                break;
        }

        float distance = Vector3.Distance(transform.position, bossFodder.transform.position);
        if (distance <= 2f && distance >= 0.2f)
        {
            AddReward(0.1f);
            // bossFodder.AddReward(-0.1f);
        }
        
        if (movement != Vector3.zero)
        {
            _agentRigidbody.MovePosition(_agentRigidbody.position + movement);
            // AddReward(0.0001f);
        }
        else
        {
            // AddReward(-0.0005f);
        }
        
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            // EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (other.gameObject.CompareTag("Force"))
        // {   
        //     // _agentRigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        //     health -= 0.2f;
        //     bossFodder.AddReward(0.2f);
        //     AddReward(-0.2f);
        //     
        // }
        //
        // if (other.gameObject.CompareTag("Player"))
        // {
        //     health -= 0.2f;
        //     SetReward(-0.1f);
        // }
        //
        // if (other.gameObject.CompareTag("Spin"))
        // {
        //     if (shieldActive)
        //     {
        //         health -= 0.5f;
        //     }
        //     else
        //     {
        //         health -= 1f;
        //     }
        //     bossFodder.AddReward(0.2f);
        //     AddReward(-0.2f);
        // }
        //
        // if (other.gameObject.CompareTag("Burst"))
        // {
        //     if (shieldActive)
        //     {
        //         health -= 0.75f;
        //     }
        //     else
        //     {
        //         health -= 1.5f;
        //     }
        //     bossFodder.AddReward(0.2f);
        //     AddReward(-0.2f);
        // }
        //
        // if (other.gameObject.CompareTag("KillZone"))
        // {
        //     EndEpisode();
        // }
        
        if (other.CompareTag("Thunderbolt"))
        {
            health -= 0.7f;
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
            moveSpeed = 2f;
        }
    }
    

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Meteor"))
        {
            health -= 0.07f;
        }
        
        if (other.CompareTag("GhostMinions"))
        {
            health -= 0.07f;
        }

        if (other.CompareTag("Swarm"))
        {
            health = -0.02f;
        }
    }
}
