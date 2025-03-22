using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GhostMovement : MonoBehaviour
{
    GameObject target;
    [SerializeField] GameObject hit;

    private float hitTimer = -1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       target = GameObject.FindGameObjectWithTag("Enemy");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 10 * Time.deltaTime);
        }
        else
        {
            // GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");
            // int i = Random.Range(0, targets.Length - 1);
            // target = targets[i];
            Destroy(gameObject);
        }
        
    }
    
    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }
}
