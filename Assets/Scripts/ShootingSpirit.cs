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
   [SerializeField] private GameObject meteorPrefab;
   [SerializeField] protected GameObject swarmPrefab;
   [SerializeField] protected GameObject ghostPrefab;
   [SerializeField] protected GameObject spinPrefab;
   [SerializeField] protected GameObject shadowPrefab;
   [SerializeField] private GameObject enumaElisPrefab;
   // [SerializeField] private GameObject cube1;
   // [SerializeField] private GameObject cube2;
   // [SerializeField] private GameObject cube3;
   // [SerializeField] private GameObject cube4;
   
   private GameObject[] enemies;
   private GameObject[] snakes;
   private GameObject[] gandalfs;
   private int enumaElisKillCount = 0;
   private float thunderBoltsTimer = -1f;
   private float enumaElisTimer = -1f;
   private float healPlayerTimer = -1f;
   private float swarmTimer = -1f;
   private float meteorTimer = -1;
   private float shadowTimer = -1f;
   private float ghostsTimer = -1f;
   private float spinTimer = -1f;
   private float aoeHealPlayerTimer = -1f;
   private float burstHealPlayerTimer = -1f;
   private bool thunderBolt = false;
   private bool enumaElis = false;
   private bool swarm = false;
   private bool meteor = false;
   private bool shadow = false;
   private bool ghost = false;
   private bool spin = false;
   private bool aoe = false;
   private bool burst = false;
   private bool healPlayer = false;
   
   private bool choseFirst = false;
   private bool choseSecond = false;
   private bool choseThird = false;

   private int first = -1;
   private int second = -1;
   private int third = -1;
   
   private float EpisodeTimer = 20f;
   
   // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
   {
      enemies = GameObject.FindGameObjectsWithTag("Enemy");
      int xRange = Random.Range(-3, 13);
      int zRange = Random.Range(-8, 8);
      // cube1.transform.localPosition = new Vector3(xRange, 0, zRange);
      // xRange = Random.Range(-3, 13);
      // zRange = Random.Range(-8, 8);
      // cube2.transform.localPosition = new Vector3(xRange, 0, zRange);
      // xRange = Random.Range(-3, 13);
      // zRange = Random.Range(-8, 8);
      // cube3.transform.localPosition = new Vector3(xRange, 0, zRange);
      // xRange = Random.Range(-3, 13);
      // zRange = Random.Range(-8, 8);
      // cube4.transform.localPosition = new Vector3(xRange, 0, zRange);
      
   }

   private void FixedUpdate()
   {
      enemies = GameObject.FindGameObjectsWithTag("Enemy");
      
      if (enumaElisTimer >= 0f)
      {
         enumaElisTimer -= Time.fixedDeltaTime;
      }
      
      if (burstHealPlayerTimer >= 0f)
      {
         burstHealPlayerTimer -= Time.fixedDeltaTime;
      }
      
      if (aoeHealPlayerTimer >= 0f)
      {
         aoeHealPlayerTimer -= Time.fixedDeltaTime;
      }
      
      if (spinTimer >= 0)
      {
         spinTimer -= Time.fixedDeltaTime;
      }

      if (thunderBoltsTimer >= 0)
      {
         thunderBoltsTimer -= Time.fixedDeltaTime;
      }
      
      if (ghostsTimer >= 0)
      {
         ghostsTimer -= Time.fixedDeltaTime;
      }
      
      if (shadowTimer >= 0)
      {
         shadowTimer -= Time.fixedDeltaTime;
      }
      
      if (meteorTimer >= 0)
      {
         meteorTimer -= Time.fixedDeltaTime;
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
      
      if (swarmTimer >= 0)
      {
         swarmTimer -= Time.fixedDeltaTime;
      }
      
      // transform.position = playerTransform.position;
      // dont forget to uncomment this
   }
   
   public override void OnEpisodeBegin()
   {
      thunderBoltsTimer = -1f;
      // enumaElisTimer = -1f;
      healPlayerTimer = -1f;
      
      int xRange = Random.Range(-3, 13);
      int zRange = Random.Range(-8, 8);
      // cube1.transform.localPosition = new Vector3(xRange, 0, zRange);
      // xRange = Random.Range(-3, 13);
      // zRange = Random.Range(-8, 8);
      // cube2.transform.localPosition = new Vector3(xRange, 0, zRange);
      // xRange = Random.Range(-3, 13);
      // zRange = Random.Range(-8, 8);
      // cube3.transform.localPosition = new Vector3(xRange, 0, zRange);
      // xRange = Random.Range(-3, 13);
      // zRange = Random.Range(-8, 8);
      // cube4.transform.localPosition = new Vector3(xRange, 0, zRange);
      
      thunderBolt = false;
      enumaElis = false;
      swarm = false;
      meteor = false;
      shadow = false;
      ghost = false;
      spin = false;
      aoe = false;
      burst = false;
      healPlayer = false;
   
      choseFirst = false;
      choseSecond = false;
      choseThird = false;
      
      first = -1;
      second = -1;
      third = -1;
   }

   public override void CollectObservations(VectorSensor sensor)
   {
      sensor.AddObservation(thunderBoltsTimer);
      sensor.AddObservation(healPlayerTimer);
      sensor.AddObservation(enumaElisTimer);
      sensor.AddObservation(swarmTimer);
      sensor.AddObservation(meteorTimer);
      sensor.AddObservation(shadowTimer);
      sensor.AddObservation(ghostsTimer);
      sensor.AddObservation(spinTimer);
      sensor.AddObservation(aoeHealPlayerTimer);
      sensor.AddObservation(burstHealPlayerTimer);
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
      
      if (Input.GetKey(KeyCode.Space))
      {
         discreteActions[2] = 0;
      }
      else
      {
         discreteActions[2] = 1;
      }

      if (Input.GetKey(KeyCode.Alpha1))
      {
         discreteActions[3] = 0;
      }
      else if (Input.GetKey(KeyCode.Alpha2))
      {
         discreteActions[3] = 1;
         
      }else if (Input.GetKey(KeyCode.Alpha3))
      {
         discreteActions[3] = 2;
         
      }else if (Input.GetKey(KeyCode.Alpha4))
      {
         discreteActions[3] = 3;
         
      }else if (Input.GetKey(KeyCode.Alpha5))
      {
         discreteActions[3] = 4;
         
      }else if (Input.GetKey(KeyCode.Alpha6))
      {
         discreteActions[3] = 5;
         
      }else if (Input.GetKey(KeyCode.Alpha7))
      {
         discreteActions[3] = 6;
         
      }else if (Input.GetKey(KeyCode.Alpha8))
      {
         discreteActions[3] = 7;
         
      }else if (Input.GetKey(KeyCode.Alpha9))
      {
         discreteActions[3] = 8;
         
      }else if (Input.GetKey(KeyCode.Alpha0))
      {
         discreteActions[3] = 9;
         
      }else
      {
         discreteActions[3] = 10;
      }
      
      if (Input.GetKey(KeyCode.Q))
      {
         discreteActions[4] = 0;
      }
      else if (Input.GetKey(KeyCode.W))
      {
         discreteActions[4] = 1;
         
      }else if (Input.GetKey(KeyCode.E))
      {
         discreteActions[4] = 2;
         
      }else if (Input.GetKey(KeyCode.R))
      {
         discreteActions[4] = 3;
         
      }else if (Input.GetKey(KeyCode.T))
      {
         discreteActions[4] = 4;
         
      }else if (Input.GetKey(KeyCode.Y))
      {
         discreteActions[4] = 5;
         
      }else if (Input.GetKey(KeyCode.U))
      {
         discreteActions[4] = 6;
         
      }else if (Input.GetKey(KeyCode.I))
      {
         discreteActions[4] = 7;
         
      }else if (Input.GetKey(KeyCode.O))
      {
         discreteActions[4] = 8;
         
      }else if (Input.GetKey(KeyCode.P))
      {
         discreteActions[4] = 9;
         
      }else
      {
         discreteActions[4] = 10;
      }
      
      if (Input.GetKey(KeyCode.A))
      {
         discreteActions[5] = 0;
      }
      else if (Input.GetKey(KeyCode.S))
      {
         discreteActions[5] = 1;
         
      }else if (Input.GetKey(KeyCode.D))
      {
         discreteActions[5] = 2;
         
      }else if (Input.GetKey(KeyCode.F))
      {
         discreteActions[5] = 3;
         
      }else if (Input.GetKey(KeyCode.G))
      {
         discreteActions[5] = 4;
         
      }else if (Input.GetKey(KeyCode.H))
      {
         discreteActions[5] = 5;
         
      }else if (Input.GetKey(KeyCode.J))
      {
         discreteActions[5] = 6;
         
      }else if (Input.GetKey(KeyCode.K))
      {
         discreteActions[5] = 7;
         
      }else if (Input.GetKey(KeyCode.L))
      {
         discreteActions[5] = 8;
         
      }else if (Input.GetKey(KeyCode.Colon))
      {
         discreteActions[5] = 9;
         
      }
      else
      {
         discreteActions[5] = 10;
      }
      
   }
   
   private void SpinAttack()
   {
      if (spinTimer <= 0f)
      {
         GameObject spawnedObject = Instantiate(spinPrefab, transform.position, Quaternion.identity);
         Destroy(spawnedObject, 1f);
         spinTimer = 1.5f;
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

   private void MeteorShower()
   {
      if (meteorTimer <= 0f)
      {
         foreach (var obj in enemies)
         {
            if (obj != null)
            {
               GameObject bmSphr = Instantiate(meteorPrefab, obj.transform.position, Quaternion.identity);
               Destroy(bmSphr, 5f);
               break;
            }
            
         }
         meteorTimer = 6f;
      }
   }

   private void ShadowGrasp()
   {
      if (shadowTimer <= 0f)
      {
         foreach (var obj in enemies)
         {
            if (obj != null)
            {
               GameObject bmSphr = Instantiate(shadowPrefab, obj.transform.position, Quaternion.identity);
               Destroy(bmSphr, 1f);
               break;
            }
            
         }
         shadowTimer = 2f;
      }
   }

   private void Swarm()
   {
      if (swarmTimer <= 0f)
      {
         foreach (var obj in enemies)
         {
            if (obj != null)
            {
               GameObject bmSphr = Instantiate(swarmPrefab, transform.position, Quaternion.identity);
               bmSphr.GetComponent<SwarmMovement>().SetTarget(obj.transform);
               Destroy(bmSphr, 5f);
            }
            
         }
         swarmTimer = 6f;
      }
   }
   
   private void Ghosts()
   {
      if (ghostsTimer <= 0f)
      {
         GameObject bmSphr = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
         GameObject bmSphr1 = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
         GameObject bmSphr2 = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
         GameObject bmSphr3 = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
         Destroy(bmSphr, 5f);
         Destroy(bmSphr1, 5f);
         Destroy(bmSphr2, 5f);
         Destroy(bmSphr3, 5f);
         ghostsTimer = 2f;
      }
   }
   
   private void EnumaElis()
   {
      if (enumaElisTimer <= 0 && enemies.Length > 0)
      {
         GameObject enumaElishInstance = Instantiate(enumaElisPrefab, new Vector3(transform.position.x , 0, transform.position.z ), Quaternion.identity);
         Destroy(enumaElishInstance, 2f);
         enumaElisTimer = 45f;
      }
   }
   
   private void BurstHeal()
   {
      if (healPlayerTimer <= 0f)
      {
         // Debug.Log("HealPlayer");
         burstHealPlayerTimer = 1f;
      }
   }
   
   private void AoeHeal()
   {
      if (healPlayerTimer <= 0f)
      {
         // Debug.Log("HealPlayer");
         aoeHealPlayerTimer = 2f;
      }
   }

   private void HealPlayer()
   {
      if (healPlayerTimer <= 0f)
      {
         // Debug.Log("HealPlayer");
         healPlayerTimer = 2f;
      }
   }

   public override void OnActionReceived(ActionBuffers actions)
   {
      int firstAbility = actions.DiscreteActions[0];
      int secondAbility = actions.DiscreteActions[1];
      int thirdAbility = actions.DiscreteActions[2];
      int selectFirstAbility = actions.DiscreteActions[3];
      int selectSecondAbility = actions.DiscreteActions[4];
      int selectThirdAbility = actions.DiscreteActions[5];
      
      Debug.Log("selectFirstAbility: " + selectFirstAbility);
      Debug.Log("firstAbility: " + firstAbility);
      
      if (choseFirst == false)
      {
         
         switch (selectFirstAbility)
         {
            case 0:
               thunderBolt = true;
               choseFirst = true;
               first = 0;
               break;
         
            case 1:
               enumaElis = true;
               choseFirst = true;
               first = 1;
               break;
            
            case 2:
               swarm = true;
               choseFirst = true;
               first = 2;
               break;
            
            case 3:
               meteor = true;
               choseFirst = true;
               first = 3;
               break;
            
            case 4:
               shadow = true;
               choseFirst = true;
               first = 4;
               break;
            
            case 5:
               ghost = true;
               choseFirst = true;
               first = 5;
               break;
            
            case 6:
               spin = true;
               choseFirst = true;
               first = 6;
               break;
            
            case 7:
               aoe = true;
               choseFirst = true;
               first = 7;
               break;
            
            case 8:
               burst = true;
               choseFirst = true;
               first = 8;
               break;
            
            case 9:
               healPlayer = true;
               choseFirst = true;
               first = 9;
               break;
            
            default:
               break;
         }
      }
      
      if (choseSecond == false)
      {
         
         switch (selectSecondAbility)
         {
            case 0:
               thunderBolt = true;
               choseSecond = true;
               second = 0;
               break;
         
            case 1:
               enumaElis = true;
               choseSecond = true;
               second = 1;
               break;
            
            case 2:
               swarm = true;
               choseSecond = true;
               second = 2;
               break;
            
            case 3:
               meteor = true;
               choseSecond = true;
               second = 3;
               break;
            
            case 4:
               shadow = true;
               choseSecond = true;
               second = 4;
               break;
            
            case 5:
               ghost = true;
               choseSecond = true;
               second = 5;
               break;
            
            case 6:
               spin = true;
               choseSecond = true;
               second = 6;
               break;
            
            case 7:
               aoe = true;
               choseSecond = true;
               second = 7;
               break;
            
            case 8:
               burst = true;
               choseSecond = true;
               second = 8;
               break;
            
            case 9:
               healPlayer = true;
               choseSecond = true;
               second = 9;
               break;
            
            default:
               break;
         }
      }
      
      if (choseThird == false)
      {
         
         switch (selectThirdAbility)
         {
            case 0:
               thunderBolt = true;
               choseThird = true;
               third = 0;
               break;
         
            case 1:
               enumaElis = true;
               choseThird = true;
               third = 1;
               break;
            
            case 2:
               swarm = true;
               choseThird = true;
               third = 2;
               break;
            
            case 3:
               meteor = true;
               choseThird = true;
               third = 3;
               break;
            
            case 4:
               shadow = true;
               choseThird = true;
               third = 4;
               break;
            
            case 5:
               ghost = true;
               choseThird = true;
               third = 5;
               break;
            
            case 6:
               spin = true;
               choseThird = true;
               third = 6;
               break;
            
            case 7:
               aoe = true;
               choseThird = true;
               third = 7;
               break;
            
            case 8:
               burst = true;
               choseThird = true;
               third = 8;
               break;
            
            case 9:
               healPlayer = true;
               choseThird = true;
               third = 9;
               break;
            
            default:
               break;
         }
      }
      
      switch (firstAbility)
      {
         case 0:
            if (thunderBolt && first == 0)
            {
               ThunderBolts();
            }

            if (enumaElis && first == 1)
            {
               EnumaElis();
            }

            if (swarm && first == 2)
            {
               Swarm();
            }

            if (meteor && first == 3)
            {
               MeteorShower();
            }

            if (shadow && first == 4)
            {
               ShadowGrasp();
            }

            if (ghost && first == 5)
            {
               Ghosts();
            }

            if (spin && first == 6)
            {
               SpinAttack();
            }

            if (aoe && first == 7)
            {
               AoeHeal();
            }

            if (burst && first == 8)
            {
               BurstHeal();
            }

            if (healPlayer && first == 9)
            {
               HealPlayer();
            }
            break;
         
         case 1:
            break;
         
         default:
            break;
      }

      switch (secondAbility)
      {
         case 0:
            if (thunderBolt && second == 0)
            {
               ThunderBolts();
            }

            if (enumaElis && second == 1)
            {
               EnumaElis();
            }

            if (swarm && second == 2)
            {
               Swarm();
            }

            if (meteor && second == 3)
            {
               MeteorShower();
            }

            if (shadow && second == 4)
            {
               ShadowGrasp();
            }

            if (ghost && second == 5)
            {
               Ghosts();
            }

            if (spin && second == 6)
            {
               SpinAttack();
            }

            if (aoe && second == 7)
            {
               AoeHeal();
            }

            if (burst && second == 8)
            {
               BurstHeal();
            }

            if (healPlayer && second == 9)
            {
               HealPlayer();
            }
            break;
         
         case 1:
            break;
         
         default:
            break;
      }
      
      switch (thirdAbility)
      {
         case 0:
            if (thunderBolt && third == 0)
            {
               ThunderBolts();
            }

            if (enumaElis && third == 1)
            {
               EnumaElis();
            }

            if (swarm && third == 2)
            {
               Swarm();
            }

            if (meteor && third == 3)
            {
               MeteorShower();
            }

            if (shadow && third == 4)
            {
               ShadowGrasp();
            }

            if (ghost && third == 5)
            {
               Ghosts();
            }

            if (spin && third == 6)
            {
               SpinAttack();
            }

            if (aoe && third == 7)
            {
               AoeHeal();
            }

            if (burst && third == 8)
            {
               BurstHeal();
            }

            if (healPlayer && third == 9)
            {
               HealPlayer();
            }
            break;
         
         case 1:
            break;
         
         default:
            break;
      }
      
   }
}
