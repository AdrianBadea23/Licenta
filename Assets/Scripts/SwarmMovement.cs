using UnityEngine;

public class SwarmMovement : MonoBehaviour
{
    public Transform target;
    public float speed = 10f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else
        {
            // GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            // int random = Random.Range(0, enemies.Length);
            // target = enemies[random].transform;
            Destroy(gameObject);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
