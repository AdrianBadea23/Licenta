using System;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Boss : Agent
{
    [SerializeField] private Boss boss;
    [SerializeField] private GameObject forceField;
    [SerializeField] private GameObject spinField;
    [SerializeField] private GameObject burstField;
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
    private float health = 10f;
    private int flasks = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agentRigidbody = GetComponent<Rigidbody>();
        startPos = transform.localPosition;
    }

    void FixedUpdate()
    {
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
            Debug.Log(shieldTimerDuration);
            Debug.Log(shieldActive);
        }

        if (shieldTimerDuration <= 0f)
        {
            shieldActive = false;
            shieldTimerDuration = 2f;
            Debug.Log(shieldActive);
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
            boss.AddReward(0.5f);
            EndEpisode();
        }
        
    }

    public override void OnEpisodeBegin()
    {
        health = 10f;
        transform.localPosition = startPos;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(spinTimer);
        sensor.AddObservation(areaTimer);
        sensor.AddObservation(projectileTimer);
        sensor.AddObservation(shieldTimer);
        sensor.AddObservation(shieldActive);
        sensor.AddObservation(health);
        sensor.AddObservation(flasks);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        // Map WASD keys to discrete actions and 1, 2, 3, 4 to character powers
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
        else if (Input.GetKey(KeyCode.Alpha1))
        {
            discreteActions[0] = 4; // First ability
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            discreteActions[0] = 5; //Second ability
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            discreteActions[0] = 6; //Third ability
        }
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            discreteActions[0] = 7; //Fourth ability
        }
        else if (Input.GetKey(KeyCode.R))
        {
            discreteActions[0] = 9;
        }
        else
        {
            discreteActions[0] = 8;
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
            
            Destroy(spawnedObject, 0.01f);
            spinTimer = 0.5f;
        }
        
    }

    private void AreaSlam()
    {
        if (areaTimer <= 0f)
        {
            GameObject spawnedObject = Instantiate(forceField, transform.position, Quaternion.identity);

            Collider bossCollider = GetComponent<Collider>();
            Collider forceCollider = spawnedObject.GetComponent<Collider>();

            if (forceCollider != null && bossCollider != null)
            {
                Physics.IgnoreCollision(bossCollider, forceCollider, true);
            }
            
            Destroy(spawnedObject, 0.01f);
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
            lingerTimer = 4f;
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
        int action = actions.DiscreteActions[0];
        float moveSpeed = 10f;
        Vector3 movement = Vector3.zero;
        switch (action)
        {
            case 0: // Move forward
                movement = transform.forward * moveSpeed * Time.fixedDeltaTime;
                break;

            case 1: // Move backward
                movement = -transform.forward * moveSpeed * Time.fixedDeltaTime;
                break;

            case 2: // Move right
                movement = transform.right * moveSpeed * Time.fixedDeltaTime;
                break;

            case 3: // Move left
                movement = -transform.right * moveSpeed * Time.fixedDeltaTime;
                break;
            
            case 4: // First ability
                SpinAttack();
                break;
            
            case 5: // Second ability
                AreaSlam();
                break;
            
            case 6: // Third ability
                ProjectileBurst();
                break;
            
            case 7: // Fourth ability
                ShieldMode();
                break;
            
            case 8:
                movement = Vector3.zero;
                break;
            
            case 9:
                if (flasks > 0)
                {
                    health += 2f;
                    flasks--;
                }
                Debug.Log(flasks);
                break;
            
            default:
                break;
        }

        if (movement != Vector3.zero)
        {
            _agentRigidbody.MovePosition(_agentRigidbody.position + movement);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Force"))
        {   
            Debug.Log("Applying force!");
            _agentRigidbody.AddForce(Vector3.up * 10f, ForceMode.Impulse);
        }

        if (other.gameObject.CompareTag("Spin"))
        {
            if (shieldActive)
            {
                health -= 0.5f;
            }
            else
            {
                health = 1f;
            }
            
        }

        if (other.gameObject.CompareTag("Burst"))
        {
            if (shieldActive)
            {
                health -= 0.75f;
            }
            else
            {
                health -= 1.5f;
            }
            
            Debug.Log(health);
        }
    }
}
