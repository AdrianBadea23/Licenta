using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class ShootingSpirit : Agent
{
   
   // [SerializeField] private Transform playerTransform;
   [SerializeField] private GameObject thunderboltPrefab;
   [SerializeField] private GameObject enumaElisPrefab;
   [SerializeField] private GameObject cube1;
   [SerializeField] private GameObject cube2;
   [SerializeField] private GameObject cube3;
   [SerializeField] private GameObject cube4;
   
   private GameObject[] enemies;
   private GameObject[] snakes;
   private GameObject[] gandalfs;
   private int enumaElisKillCount = 0;
   private float thunderBoltsTimer = -1f;
   private float enumaElisTimer = -1f;
   private float healPlayerTimer = -1f;
   private float EpisodeTimer = 20f;
   
   // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
   {
      enemies = GameObject.FindGameObjectsWithTag("Enemy");
      int xRange = Random.Range(-3, 13);
      int zRange = Random.Range(-8, 8);
      cube1.transform.localPosition = new Vector3(xRange, 0, zRange);
      xRange = Random.Range(-3, 13);
      zRange = Random.Range(-8, 8);
      cube2.transform.localPosition = new Vector3(xRange, 0, zRange);
      xRange = Random.Range(-3, 13);
      zRange = Random.Range(-8, 8);
      cube3.transform.localPosition = new Vector3(xRange, 0, zRange);
      xRange = Random.Range(-3, 13);
      zRange = Random.Range(-8, 8);
      cube4.transform.localPosition = new Vector3(xRange, 0, zRange);
      
   }

   private void FixedUpdate()
   {
      if (enumaElisTimer >= 0f)
      {
         enumaElisTimer -= Time.fixedDeltaTime;
      }

      if (thunderBoltsTimer >= 0)
      {
         thunderBoltsTimer -= Time.fixedDeltaTime;
      }

      if (healPlayerTimer >= 0)
      {
         healPlayerTimer -= Time.fixedDeltaTime;
      }

      if (EpisodeTimer >= 0)
      {
         EpisodeTimer -= Time.fixedDeltaTime;
      }
      else
      {
         EndEpisode();
         EpisodeTimer = 20f;
      }
      
   }
   
   public override void OnEpisodeBegin()
   {
      thunderBoltsTimer = -1f;
      // enumaElisTimer = -1f;
      healPlayerTimer = -1f;
      
      int xRange = Random.Range(-3, 13);
      int zRange = Random.Range(-8, 8);
      cube1.transform.localPosition = new Vector3(xRange, 0, zRange);
      xRange = Random.Range(-3, 13);
      zRange = Random.Range(-8, 8);
      cube2.transform.localPosition = new Vector3(xRange, 0, zRange);
      xRange = Random.Range(-3, 13);
      zRange = Random.Range(-8, 8);
      cube3.transform.localPosition = new Vector3(xRange, 0, zRange);
      xRange = Random.Range(-3, 13);
      zRange = Random.Range(-8, 8);
      cube4.transform.localPosition = new Vector3(xRange, 0, zRange);
   }

   public override void CollectObservations(VectorSensor sensor)
   {
      sensor.AddObservation(thunderBoltsTimer);
      sensor.AddObservation(healPlayerTimer);
      sensor.AddObservation(enumaElisTimer);
   }
   
   public override void Heuristic(in ActionBuffers actionsOut)
   {
      ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

      if (Input.GetKey(KeyCode.Mouse0))
      {
         discreteActions[0] = 0;
      }
      else
      {
         discreteActions[0] = 1;
      }

      if (Input.GetKey(KeyCode.Mouse1))
      {
         discreteActions[1] = 0;
      }
      else
      {
         discreteActions[1] = 1;
      }
      
      if (Input.GetKey(KeyCode.E))
      {
         discreteActions[2] = 0;
      }
      else
      {
         discreteActions[2] = 1;
      }
      
   }

   private void ThunderBolts()
   {
      if (thunderBoltsTimer <= 0f)
      {
         foreach (var obj in enemies)
         {
            if (obj != null)
            {
               GameObject bmSphr = Instantiate(thunderboltPrefab, obj.transform.position, Quaternion.identity);
               Destroy(bmSphr, 1f);
            }
            
         }
         thunderBoltsTimer = 0.3f;
      }
   }

   private void EnumaElis()
   {
      if (enumaElisTimer <= 0)
      {
         GameObject enumaElishInstance = Instantiate(enumaElisPrefab, new Vector3(transform.position.x , 0, transform.position.z ), Quaternion.identity);
         Destroy(enumaElishInstance, 2f);
         enumaElisTimer = 45f;
      }
   }

   private void HealPlayer()
   {
      if (healPlayerTimer <= 0f)
      {
         Debug.Log("HealPlayer");
         AddReward(0.2f);
         healPlayerTimer = 2f;
      }
   }

   public override void OnActionReceived(ActionBuffers actions)
   {
      int thunderBolts = actions.DiscreteActions[0];
      int enumaElis = actions.DiscreteActions[1];
      int healPlayer = actions.DiscreteActions[2];

      switch (thunderBolts)
      {
         case 0:
            ThunderBolts();
            AddReward(0.7f);
            break;
         
         case 1:
            break;
         
         default:
            break;
      }

      switch (enumaElis)
      {
         case 0:
            EnumaElis();
            AddReward(1f);
            break;
         
         case 1:
            break;
         
         default:
            break;
      }
      
      switch (healPlayer)
      {
         case 0:
            HealPlayer();
            AddReward(0.5f);
            break;
         
         case 1:
            break;
         
         default:
            break;
      }
      
   }
}
