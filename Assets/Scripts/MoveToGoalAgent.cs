using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MoveToGoalAgent : Agent
{
    [SerializeField]private Transform targetTransform;
    private string targetTag = "FinalGoal";
    private Rigidbody _agentRigidbody;
    private float episodeTime;
    private Vector3 lastPosition;
    private Vector3 startingPosition;
    private Vector3 previousPosition; // Track the agent's position
    private float stuckTime; // Time the agent has been "stuck"
    private const float stuckThreshold = 0.1f; // Minimum movement distance to not be "stuck"
    private const float maxStuckDuration = 20.0f; // Maximum time the agent can be stuck
    EnvironmentParameters m_ResetParams;
    private float[] ballPositions = new float[2];
    private Quaternion startingRotation;
    private float ballReward = 5f;
    public bool isShootPressed = false;
    //[SerializeField] private Animator animator;
    private bool isRunning = false;
    //[SerializeField] private Transform modelTransform;
    private Vector3 lastAgentPosition;
    private Vector3 goalPosition;
    private float previousDistanceToGoal;
    private bool hasHit = false;
    // private GameObject[] sideObjectives;
    // private Vector3[] sideObjectivePositions;
    // private GameObject[] Gates;
    // private Vector3[] GatesPositions;
    // private Quaternion[] GatesRotation;
    Vector3 randomPos = new Vector3(0, 0, 0);
    private float previousDistanceToSpawn = 0;
    [SerializeField] private DungeonController dungeonController;
    
    public void Start()
    {
        // sideObjectives = GameObject.FindGameObjectsWithTag("SideObj");
        // sideObjectivePositions = new Vector3[sideObjectives.Length];
        // for (int i = 0; i < sideObjectives.Length; i++)
        // {
        //     sideObjectivePositions[i] = sideObjectives[i].transform.position;
        // }
        
        // Gates = GameObject.FindGameObjectsWithTag("Gate");
        // GatesPositions = new Vector3[Gates.Length];
        // GatesRotation = new Quaternion[Gates.Length];
        // for (int i = 0; i < Gates.Length; i++)
        // {
        //     GatesPositions[i] = Gates[i].transform.position;
        //     GatesRotation[i] = Gates[i].transform.rotation;
        // }
        
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        _agentRigidbody = GetComponent<Rigidbody>();
        //episodeTime = 0f;
        float x = transform.localPosition.x;
        float z = transform.localPosition.z;
        startingPosition = transform.localPosition;
        previousPosition = startingPosition;
        startingRotation.eulerAngles = transform.localRotation.eulerAngles;
        ballPositions[0] = m_ResetParams.GetWithDefault("ballPositionX", 129.3359f);
        ballPositions[1] = m_ResetParams.GetWithDefault("ballPositionZ", -18.94571f);
        GameObject targetObject = GameObject.Find(targetTag);
        // if (targetObject != null)
        // {
        //     targetTransform = targetObject.transform;
        // }
        // else
        // {
        //     // Debug.LogWarning("No target found");
        // }
        
    }
    
    public void LateUpdate()
    {
        // GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
        // if (targetObject != null)
        // {
        //     targetTransform = targetObject.transform;
        // }
        // else
        // {
        //     // Debug.LogWarning("No target found");
        // }
    }

    void RandomSpawn()
    {
        int randomCase = Random.Range(0, 7);

        switch (randomCase)
        {
            case 0:
                randomPos = new Vector3(0, 1.2f, 5);
                break;
            case 1:
                randomPos = new Vector3(121, 1.2f, 5);
                break;
            case 2:
                randomPos = new Vector3(15, 1.2f, 0);
                break;
            case 3:
                randomPos = new Vector3(-56, 1.2f, 39);
                break;
            case 4:
                randomPos = new Vector3(4, 1.2f, 87);
                break;
            case 5:
                randomPos = new Vector3(85, 1.2f, 66);
                break;
            case 6:
                randomPos = new Vector3(34, 1.2f, 46);
                break;
            // case 7:
            //     randomPos = new Vector3(69, 1.2f, 19);
            //     break;
            // case 8:
            //     randomPos = new Vector3(120, 1.2f, -27);
            //     break;
            // case 9:
            //     randomPos = new Vector3(94, 1.2f, -24);
            //     break;
            default:
                randomPos = new Vector3(0, 1.2f, 5);
                break;
        }
        
    }
    public override void OnEpisodeBegin()
    {   
        RandomSpawn();
        transform.localPosition = startingPosition;
        goalPosition = targetTransform.position;
        previousDistanceToGoal = Vector3.Distance(transform.localPosition, goalPosition);

        // Reset Rigidbody
        _agentRigidbody.linearVelocity = Vector3.zero;
        _agentRigidbody.angularVelocity = Vector3.zero;

        // Reset other elements
        transform.localRotation = startingRotation;
        dungeonController.RespawnObjectives();
        lastAgentPosition = transform.localPosition;
        
        // Update target reference
        // GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
        // if (targetObject != null)
        // {
        //     targetTransform = targetObject.transform;
        // }
        // else
        // {
        //     // Debug.LogWarning("No target found");
        // }
    }
    
    private void PenalizeProximityToWalls()
    {
        RaycastHit hit;
        float wallDetectionRange = 1.0f;
        float maxWallPenaltyRange = 1.0f;
        // Check for walls around the agent
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, wallDetectionRange) && hit.collider.CompareTag("Wall") ||
            Physics.Raycast(transform.position, Vector3.back, out hit, wallDetectionRange) && hit.collider.CompareTag("Wall") ||
            Physics.Raycast(transform.position, Vector3.left, out hit, wallDetectionRange) && hit.collider.CompareTag("Wall") ||
            Physics.Raycast(transform.position, Vector3.right, out hit, wallDetectionRange) && hit.collider.CompareTag("Wall"))
        {
            float distance = hit.distance; // Distance to the wall
            float penalty = Mathf.Lerp(-0.1f, 0f, distance / maxWallPenaltyRange); // Closer = higher penalty
            AddReward(penalty);
        }
    }
    
    private void EncourageMovement()
    {
        float distanceMoved = Vector3.Distance(lastPosition, transform.position);

        // Reward for moving away from the last position
        if (distanceMoved > 5f)
        {
            AddReward(0.01f); // Small positive reward for movement
        }
        else
        {
            AddReward(-0.01f);
        }
        lastPosition = transform.position;
        
        distanceMoved = Vector3.Distance(startingPosition, transform.localPosition);

        // Reward for moving away from the start position
        if (distanceMoved > 5f)
        {
            AddReward(0.05f); // Small positive reward for movement
        }
        else
        {
            AddReward(-0.01f);
        }
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        if (targetTransform.localPosition != null)
        {
            sensor.AddObservation(targetTransform.localPosition);
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
        }
        

        // foreach (var sideObjective in sideObjectives)
        // {
        //     if (sideObjective != null)
        //     {
        //         sensor.AddObservation(sideObjective.transform.localPosition);
        //     }
        //     
        // }
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
        // Get the discrete actions buffer
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
        
        // ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        // continuousActions[0] = Input.GetAxis("Horizontal");
        // continuousActions[1] = Input.GetAxis("Vertical");
        // isRunning = continuousActions[1] != 0;
        // animator.SetBool("Running", isRunning);
        
    }
    
    private void RandomNudgeWhenStuck()
    {
        float randomForce = 0.5f;
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        _agentRigidbody.AddForce(randomDirection * randomForce, ForceMode.Impulse);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // float distanceToGoal = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
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
            case 4:
                movement = Vector3.zero;
                break;
            default:
                break;
        }

        if (movement != Vector3.zero)
        {
            _agentRigidbody.MovePosition(_agentRigidbody.position + movement);
        }
        
        // float currentDistanceToGoal = Vector3.Distance(transform.localPosition, goalPosition);
        // if (currentDistanceToGoal < previousDistanceToGoal)
        // {
        //     AddReward(0.0001f);
        // } 
        
        AddReward(-0.0001f); // speed up the agent
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Goal") && other.gameObject.name != targetTag)
        {
            SetReward(0.5f);
            Destroy(other.gameObject);
        }
        
        if (other.gameObject.name.Contains("FinalGoal"))
        {
            SetReward(1f);
            _agentRigidbody.linearVelocity = Vector3.zero;
            _agentRigidbody.angularVelocity = Vector3.zero;
            Debug.Log("Won");
            EndEpisode();
        }
        
        if (other.gameObject.CompareTag("Gate"))
        {
            AddReward(0.5f);
            Destroy(other.gameObject);
        }
        
        if (other.gameObject.CompareTag("SideObj"))
        {
            AddReward(0.25f);
            Destroy(other.gameObject);
        }
        
        if (other.gameObject.CompareTag("KillZone"))
        {
            SetReward(-5f);
            EndEpisode();
        }
        
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
             AddReward(-0.001f);
            _agentRigidbody.linearVelocity = Vector3.zero;
            _agentRigidbody.angularVelocity = Vector3.zero;
            // EndEpisode();
        }
        
    }
}
