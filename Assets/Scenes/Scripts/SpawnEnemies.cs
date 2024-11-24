using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] ShootingSpirit agent;
    public List<GameObject> enemies = new List<GameObject>();

    private int position = 0;
    private Vector3 spawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnTwelveEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnTwelveEnemies()
    {
        for (int i = 0; i < 12; i++)
        {
            position = Random.Range(0, 4);
            switch (position)
            {
                case 0:
                    spawnPosition = new Vector3(18, 3, -22);
                    break;
                case 1:
                    spawnPosition = new Vector3(-29, 3, -22);
                    break;
                case 2:
                    spawnPosition = new Vector3(-29, 3, 25);
                    break;
                case 3:
                    spawnPosition = new Vector3(17, 3, -25);
                    break;
                default:
                    spawnPosition = new Vector3(17, 3, -25);
                    break;
            }
            
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemies.Add(enemy);
        }
    }

    public void KillEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        
        enemies.Clear();
    }
    
}
