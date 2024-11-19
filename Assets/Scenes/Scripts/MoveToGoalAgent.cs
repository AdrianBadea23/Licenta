using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
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

    public void Start()
    {
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        _agentRigidbody = GetComponent<Rigidbody>();
        //episodeTime = 0f;
        float x = transform.localPosition.x;
        float z = transform.localPosition.z;
        startingPosition = new Vector3(x, 3, z);
        previousPosition = startingPosition;
        startingRotation.eulerAngles = transform.localRotation.eulerAngles;
        ballPositions[0] = m_ResetParams.GetWithDefault("ballPositionX", 129.3359f);
        ballPositions[1] = m_ResetParams.GetWithDefault("ballPositionZ", -18.94571f);
    }


    public override void OnEpisodeBegin()
    {
        targetTransform.localPosition = new Vector3(ballPositions[0], 3.14f, ballPositions[1]);
        transform.localPosition = new Vector3(25.6f, 3.5f, 8.6f);
        transform.localRotation = startingRotation;
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
            //AddReward(penalty);
        }
    }
    
    private void EncourageMovement()
    {
        float distanceMoved = Vector3.Distance(lastPosition, transform.position);

        // Reward for moving away from the last position
        if (distanceMoved > 0.1f)
        {
            AddReward(0.01f); // Small positive reward for movement
        }
        lastPosition = transform.position;
    }
    
    /*
     private void FixedUpdate()
    { 
        episodeTime += Time.fixedDeltaTime;

        // Check if the agent is stuck
        if (Vector3.Distance(transform.localPosition, previousPosition) < stuckThreshold)
        {
            stuckTime += Time.fixedDeltaTime;
        }
        else
        {
            stuckTime = 0f; // Reset if the agent moved
        }

        previousPosition = transform.localPosition;

        // If the agent has been stuck for too long, reset the episode
        if (stuckTime >= maxStuckDuration)
        {
            SetReward(-1f); // Penalize the agent for being stuck
            EndEpisode();
        }
    }
     */
    
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }
    
    private void RandomNudgeWhenStuck()
    {
        float randomForce = 0.5f;
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        _agentRigidbody.AddForce(randomDirection * randomForce, ForceMode.Impulse);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float distanceToGoal = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        float turn = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];
        
        float moveSpeed = 10f;
        float turnSpeed = 100f;
        Vector3 moveDirection = transform.forward * moveForward;
        float moveDistance = moveSpeed * Time.fixedDeltaTime;
        
        Vector3 newPosition = _agentRigidbody.position + moveDirection * moveDistance;
        _agentRigidbody.MovePosition(newPosition);
        
        float turnAmount = turn * turnSpeed * Time.fixedDeltaTime;
        Quaternion newRotation = Quaternion.Euler(0, transform.eulerAngles.y + turnAmount, 0);
        _agentRigidbody.MoveRotation(newRotation);

        // Raycast to detect obstacles and check if the hit object is not tagged as "Gate"/"Goal"/"SideObj"
        RaycastHit hit;
        if (Physics.Raycast(transform.localPosition, moveDirection, out hit, moveDistance))
        {
            if (!hit.collider.CompareTag("Gate") && !hit.collider.CompareTag("Goal") && !hit.collider.CompareTag("SideObj"))
            {
                float proximityPenalty = 1f - (hit.distance / moveDistance); 
                AddReward(-proximityPenalty * 0.001f); 
                EndEpisode();
            }
        }
        
        PenalizeProximityToWalls();
        EncourageMovement();
        float distanceToGoalAfter = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        if (distanceToGoalAfter < distanceToGoal)
        {
            AddReward(0.002f); // Small positive reward for moving closer
        }
        else
        {
            AddReward(-0.001f); // Small penalty for moving away or staying idle
        }
        
        AddReward(-0.0001f); // Speed the agent a bit.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(ballReward);
            ballReward += 0.25f;
            //Debug.Log("Done");
            EndEpisode();
        }
        
        if (other.TryGetComponent<Gate>(out Gate gate))
        {
            AddReward(1f);
        }
        
        if (other.gameObject.CompareTag("SideObj"))
        {
            AddReward(0.2f);
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
            SetReward(-2.5f);
            ballReward -= 0.01f;
            EndEpisode();
                
        }
    }
}
