using Unity.Sentis;
using UnityEngine;
using System;
using Input = UnityEngine.Windows.Input;

public class BulletController : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyAI enemyAI = other.gameObject.GetComponent<EnemyAI>();
            enemyAI.KillEnemy();
            Destroy(other.gameObject, 0.4f);
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }
        
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
