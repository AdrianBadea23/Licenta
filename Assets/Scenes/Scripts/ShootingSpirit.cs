using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem;
using Input = UnityEngine.Windows.Input;


public class ShootingSpirit : Agent
{
   [SerializeField] private GameObject bulletPrefab;
   //[SerializeField] private MoveToGoalAgent moveToGoalAgent;
   [SerializeField] private Transform playerTransform;
   //[SerializeField] private SpawnEnemies spawnEnemies;
   //private float _rotationSpeed = 5f;
   private Vector3 enemyLocation;
   private float timer = 0, randomTimer = 15f;
   // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
   {
      transform.position = playerTransform.transform.position;
   }

   private void FixedUpdate()
   {
      timer += Time.fixedDeltaTime;
   }

   // Update is called once per frame
   
   void Update()
   {  
      if (timer >= randomTimer)
      {
         //spawnEnemies.SpawnTwelveEnemies();
         randomTimer = Random.Range(10f, 60f);
         timer = 0;
      }
      
      transform.position = playerTransform.transform.position;
      // foreach (GameObject enemy in spawnEnemies.enemies)
      // {
      //    if (enemy != null)
      //    {
      //       float distanceToPlayer = Vector3.Distance(transform.position, enemy.transform.position);
      //       //Debug.Log(distanceToPlayer);
      //       if (distanceToPlayer < 5f)
      //       {
      //          Debug.Log("Enemy is close ending episode");
      //          SetReward(-1f);
      //          //spawnEnemies.KillEnemies();
      //          //EndEpisode();
      //          break;
      //       }
      //    }
      // }
   }

   private void StartShooting()
   {
      int rays = 100;
      float radius = 15f;
      LayerMask mask = LayerMask.GetMask("Enemy");

      for (int i = 0; i < rays; i++)
      {
         float angle = (360f / rays) * i;
         Vector3 pos = Quaternion.Euler(0, angle, 0) * transform.forward;
         
         Debug.DrawRay(transform.position, pos * radius, Color.red);
         
         if (Physics.Raycast(transform.position, pos, out RaycastHit hit, radius, mask))
         {
            GameObject bullet = Instantiate(bulletPrefab, hit.point, Quaternion.identity);
            Debug.DrawRay(transform.position, pos * radius, Color.green);
            AddReward(0.01f);
         }
      }
   }
   
   public override void OnEpisodeBegin()
   {
      //spawnEnemies.SpawnTwelveEnemies();
   }

   public override void CollectObservations(VectorSensor sensor)
   {
      // Raycast 3D sensor adds the observations automatically
   }

   public override void OnActionReceived(ActionBuffers actions)
   {
      float shootTrigger = actions.ContinuousActions[0];
      //Debug.Log(shootTrigger);
      if (shootTrigger > 0.5f)
      {
         StartShooting();
         AddReward(-0.01f);
      }
   }

   public override void Heuristic(in ActionBuffers actionsOut)
   {
      ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
      /*
      if (moveToGoalAgent.isShootPressed)
      {
         continuousActions[0] = 1f;
      }
      else
      {
         continuousActions[0] = -1f;
      }
      */
   }
}
