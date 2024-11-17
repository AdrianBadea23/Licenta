using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private RayPerceptionSensorComponent3D raySensorWalls;
    [SerializeField] private RayPerceptionSensorComponent3D raySensorGates;
    [SerializeField] private RayPerceptionSensorComponent3D raySensorBall;
    [SerializeField] private RayPerceptionSensorComponent3D raySensorSideObj;
    private Rigidbody _agentRigidbody;
    private float episodeTime;
    private Vector3 lastPosition;
    private Vector3 startingPosition;

    public void Start()
    {
        _agentRigidbody = GetComponent<Rigidbody>();
        episodeTime = 0f;
        startingPosition = transform.position;
    }


    public override void OnEpisodeBegin()
    {
        targetTransform.localPosition = new Vector3(-320f, 3f, -352f);
        transform.position = startingPosition;
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
        if (distanceMoved > 0.1f)
        {
            AddReward(0.01f); // Small positive reward for movement
        }
        lastPosition = transform.position;
    }
    
    private void FixedUpdate()
    { 
        episodeTime += Time.fixedDeltaTime;

        if (episodeTime >= 60f)
        {
            SetReward(-1f);
            episodeTime = 0;
            EndEpisode();
        }
    }
    
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
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        //float moveSpeed = 10f;
        //Vector3 movement = new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
        //_agentRigidbody.MovePosition(_agentRigidbody.position + movement);
        
        float moveSpeed = 10f;
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        float moveDistance = moveSpeed * Time.fixedDeltaTime;

        // Raycast to detect obstacles and check if the hit object is not tagged as "Gate"
        RaycastHit hit;
        if (Physics.Raycast(transform.position, moveDirection, out hit, moveDistance))
        {
            // If the hit object is not a gate, prevent movement
            if (!hit.collider.CompareTag("Gate"))
            {
                float proximityPenalty = 1f - (hit.distance / moveDistance); // Closer = higher penalty
                AddReward(-proximityPenalty * 0.01f); // Penalize based on closeness to the wall
                //moveDirection = Vector3.Reflect(moveDirection, hit.normal);
                EndEpisode();
            }
            //_agentRigidbody.MovePosition(_agentRigidbody.position + moveDirection * moveDistance);
        }
    
        // If no obstacle or only a gate is detected, move the agent
        _agentRigidbody.MovePosition(_agentRigidbody.position + moveDirection * moveDistance);
        PenalizeProximityToWalls();
        EncourageMovement();
        float distanceToGoalAfter = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        //float distanceReward = distanceToGoal - distanceToGoalAfter;
        //AddReward(distanceReward * 0.01f);
        //AddReward(-0.01f);
        
        if (distanceToGoalAfter < distanceToGoal)
        {
            AddReward(0.1f); // Small positive reward for moving closer
        }
        else
        {
            AddReward(-0.01f); // Small penalty for moving away or staying idle
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            AddReward(10f);
            EndEpisode();
        }
        
        if (other.TryGetComponent<Gate>(out Gate gate))
        {
            AddReward(2f);
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.5f);
            EndEpisode();
        }
        
        /*
         * if (other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-5f);
            //floorMeshRenderer.material = lossMaterial;
            //EndEpisode();
        }
         */
        
        
    }
}
