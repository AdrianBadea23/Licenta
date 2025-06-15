using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Gandalf : Agent
{
    private GameObject _gandalf;
    [SerializeField] private GameObject _boomSphere;
    [SerializeField] private GameObject _ultimateBoom;
    [SerializeField] private GameObject forceField;
    [SerializeField] private GameObject chara;
    [SerializeField] private GameObject deathParticle;
    // [SerializeField] private ShootingSpirit shootingSpirit;
    
    private Rigidbody _agentRigidBody;
    private Vector3 _playerStartPosition;
    
    private float boomTimer = 0f;
    private bool _boomCD = true;
    private float ultimateTimer = -1f;
    private bool _ultimateBoomCd = true;
    private float health = 100f;
    private float areaTimer = 1f;
    public float moveSpeed;
    
    private Quaternion rotation90 = Quaternion.Euler(0, 90, 0);   // Rotates 90 degrees around Y-axis
    private Quaternion rotation45 = Quaternion.Euler(0, 45, 0);   // Rotates 45 degrees around Y-axis
    private Quaternion rotationMinus45 = Quaternion.Euler(0, -45, 0); // Rotates -45 degrees around Y-axis
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agentRigidBody = GetComponent<Rigidbody>();
        // _playerStartPosition = transform.localPosition;
        _gandalf = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_gandalf == null)
        {
            _gandalf = GameObject.FindWithTag("Player");
        }
        
        if (ultimateTimer > 0f)
        {
            ultimateTimer -= Time.fixedDeltaTime;
        }
        else
        {
            _ultimateBoomCd = true;
        }

        if (boomTimer > 0f)
        {
            boomTimer -= Time.fixedDeltaTime;
        }
        else
        {
            _boomCD = true;
        }
        
        if (areaTimer > 0f)
        {
            areaTimer -= Time.fixedDeltaTime;
        }

        if (health <= 0f)
        {
            // SetReward(-0.5f);
            // bossFodder.SetReward(0.5f);
            // EndEpisode();
            // bossFodder.EndEpisode();
            Destroy(this.gameObject);
            // shootingSpirit.numberOfDeaths += 1;
            Debug.Log("Gandalf Death");
            GameObject[] booms = GameObject.FindGameObjectsWithTag("Boom");
            GameObject[] ultimateBooms = GameObject.FindGameObjectsWithTag("Ultimate");
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
            Destroy(deathPart, 1f);
        }
    }
    
    public override void OnEpisodeBegin()
    {
        // transform.localPosition = _playerStartPosition;
        health = 100f;
        ultimateTimer = -1f;
        _boomCD = true;
        _ultimateBoomCd = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position); // 3
        sensor.AddObservation(_gandalf.transform.position); // 3
        sensor.AddObservation(boomTimer); // 1
        sensor.AddObservation(ultimateTimer); // 1
        sensor.AddObservation(health); // 1
        sensor.AddObservation(_boomCD); // 1
        sensor.AddObservation(_ultimateBoomCd); // 1
        sensor.AddObservation(areaTimer);
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

        if (Input.GetKey(KeyCode.Space))
        {
            discreteActions[1] = 0;
        }
        else
        {
            discreteActions[1] = 1;
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            discreteActions[2] = 0;
        }else if (Input.GetKey(KeyCode.Alpha2))
        {
            discreteActions[2] = 1;
        }else if (Input.GetKey(KeyCode.Alpha3))
        {
            discreteActions[2] = 2;
        }else if (Input.GetKey(KeyCode.Alpha4))
        {
            discreteActions[2] = 3;
        }else if (Input.GetKey(KeyCode.Q))
        {
            discreteActions[2] = 4;
        }else if (Input.GetKey(KeyCode.E))
        {
            discreteActions[2] = 5;
        }else if (Input.GetKey(KeyCode.C))
        {
            discreteActions[2] = 6;
        }else if (Input.GetKey(KeyCode.X))
        {
            discreteActions[2] = 7;
        }else
        {
            discreteActions[2] = 8;
        }

        if (Input.GetKey(KeyCode.P))
        {
            discreteActions[3] = 0;
        }
        else
        {
            discreteActions[3] = 1;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int movementAction = actions.DiscreteActions[0];
        int spellCastAction = actions.DiscreteActions[1];
        int wallOfFireAction = actions.DiscreteActions[2];
        int pokeMagicAction = actions.DiscreteActions[3];
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

        switch (pokeMagicAction)
        {
            case 0:
                AreaSlam();
                break;
            
            case 1:
                break;
            
            default:
                break;
        }

        switch (spellCastAction)
        {
            case 0:
                AreaOfEffect();
                break;
            
            case 1:
                break;
            
            default:
                break;
        }

        switch (wallOfFireAction)
        {
            case 0:
                WallOfFire(0.2f,0, rotation90);
                break;
            case 1:
                WallOfFire(0,0.2f, Quaternion.identity);
                break;
            
            case 2:
                WallOfFire(-0.2f,0, rotation90);
                break;
            
            case 3:
                WallOfFire(0,-0.2f, Quaternion.identity);
                break;
            
            case 4:
                WallOfFire(0.2f,0.2f, rotation45);
                break;
            
            case 5:
                WallOfFire(0.2f,-0.2f, rotationMinus45);
                break;
            
            case 6:
                WallOfFire(-0.2f,0.2f, rotationMinus45);
                break;
            
            case 7:
                WallOfFire(-0.2f,-0.2f, rotation45);
                break;
            
            case 8:
                break;
            
            default:
                break;
        }

        // float distance = Vector3.Distance(transform.position, _gandalf.transform.position);
        // if (distance > 0 && distance < 1)
        // {
        //     AddReward(0.1f);
        // }
        
        if (movement != Vector3.zero)
        {
            _agentRigidBody.MovePosition(_agentRigidBody.position + movement);
            // AddReward(0.0001f);
        }
        else
        {
            // AddReward(-0.0005f);
        }
    }

    private void WallOfFire(float sphereX, float sphereZ, Quaternion rotation)
    {
        
        if (boomTimer <= 0f)
        {
            GameObject bmSphr = Instantiate(_boomSphere, new Vector3(transform.position.x + sphereX * 10, 0, transform.position.z + sphereZ * 10), rotation);
            Collider gandalfCollider = GetComponent<Collider>();
            Collider sphereCollider = bmSphr.GetComponent<Collider>();

            if (sphereCollider != null && gandalfCollider != null)
            {
                Physics.IgnoreCollision(gandalfCollider, sphereCollider, true);
            }
            Destroy(bmSphr, 1f);
            boomTimer = 5f;
            _boomCD = false;
        }
    }
    
    private void AreaSlam()
    {
        if (areaTimer <= 0f)
        {
            GameObject spawnedObject = Instantiate(forceField, _gandalf.transform.position, Quaternion.identity);

            Collider bossCollider = GetComponent<Collider>();
            Collider forceCollider = spawnedObject.GetComponent<Collider>();

            if (forceCollider != null && bossCollider != null)
            {
                Physics.IgnoreCollision(bossCollider, forceCollider, true);
            }
            
            Destroy(spawnedObject, 2f);
            areaTimer = 5f;
        }
        
    }

    private void AreaOfEffect()
    {
        if (ultimateTimer <= 0f)
        {
            GameObject bmSphr = Instantiate(_ultimateBoom, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            Collider gandalfCollider = GetComponent<Collider>();
            Collider sphereCollider = bmSphr.GetComponent<Collider>();

            if (sphereCollider != null && gandalfCollider != null)
            {
                Physics.IgnoreCollision(gandalfCollider, sphereCollider, true);
            }
            Destroy(bmSphr, 1f);
            ultimateTimer = 15f;
            _ultimateBoomCd = false;
        }
        

    }

    private void OnTriggerEnter(Collider other)
    {
        // if (other.CompareTag("Ultimate"))
        // {
        //     health -= 0.1f;
        //     _gandalf.SetReward(0.5f);
        //     SetReward(-0.5f);
        // }
        //
        // if (other.CompareTag("Boom"))
        // {
        //     health -= 0.75f;
        //     _gandalf.AddReward(0.1f);
        //     AddReward(-0.1f);
        // }
        //
        // if (other.CompareTag("Player"))
        // {
        //     health -= 0.1f;
        //     AddReward(0.1f);
        //     _gandalf.AddReward(-0.1f);
        // }
        //
        // if (other.CompareTag("KillZone"))
        // {
        //     EndEpisode();
        //     _gandalf.EndEpisode();
        // }
        //
        // if (other.gameObject.CompareTag("Force"))
        // {   
        //     health -= 0.2f;
        //     _gandalf.AddReward(0.2f);
        //     AddReward(-0.2f);
        //     
        // }

        if (other.CompareTag("Thunderbolt"))
        {
            health -= 5f;
            Debug.Log("Thunderbolt hit " + health);
        }
        
        if (other.CompareTag("EnumaElis"))
        {
            health -= 1f;
        }

        if (other.CompareTag("Spin"))
        {
            health -= 1f;
        }

        if (other.CompareTag("Shadow"))
        {
            if (tag == "Player")
            {
                
            }
            else
            {
                moveSpeed = 7f;
            }
            
        }
        
        if (other.CompareTag("Swarm"))
        {
            health = -2f;
            Destroy(other.gameObject);
        }
        
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject hitSmokePrefab = Resources.Load<GameObject>("VFX_TorchLight_Green");

            if (hitSmokePrefab != null)
            {
                GameObject instance = Instantiate(hitSmokePrefab, transform.position, Quaternion.identity);
                Destroy(instance, 3f);
            }
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
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject hitSmokePrefab = Resources.Load<GameObject>("VFX_TorchLight_Green");

            if (hitSmokePrefab != null)
            {
                GameObject instance = Instantiate(hitSmokePrefab, transform.position, Quaternion.identity);
                Destroy(instance, 3f);
            }
        }
    }

    public float GetHealth()
    {
        return health;
    }
}
