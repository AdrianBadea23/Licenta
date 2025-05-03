using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnLevel2 : MonoBehaviour
{
    [SerializeField] private GameObject spawnObjective;
    [SerializeField] private GameObject nextObjective;
    private Vector3 spawnPos;
    private GameObject enemy1;
    private GameObject enemy2;
    private bool spawned = false;
    private float counter = 30f;

    private int end = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawned)
        {
            enemy1 = Instantiate(spawnObjective, new Vector3(89, 3, 13), Quaternion.identity);
            enemy1.transform.localScale = new Vector3(3f, 3f, 3f);
            enemy2 = Instantiate(spawnObjective, new Vector3(57, 3, 2), Quaternion.identity);
            enemy2.transform.localScale = new Vector3(3f, 3f, 3f);
            spawned = true;
        }
        
        if ((enemy1 == null) && (enemy2 == null) && spawned)
        {
            Destroy(this.gameObject);
            Instantiate(nextObjective, spawnPos, Quaternion.identity);
        }
    }
}
