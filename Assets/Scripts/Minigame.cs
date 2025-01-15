using UnityEngine;

public class Minigame : MonoBehaviour
{
    [SerializeField] private GameObject Goal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void RandomSpawn()
    {
        float x = Random.Range(-8f, 8f);
        float z = Random.Range(-8.5f, 8.8f);
        Goal.transform.position = new Vector3(x, 0, z);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
