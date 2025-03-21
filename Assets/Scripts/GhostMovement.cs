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
       GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");
       int i = Random.Range(0, targets.Length);
       target = targets[i];
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
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");
            int i = Random.Range(0, targets.Length);
            target = targets[i];
        }
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == target.gameObject && hitTimer < 0)
            {
                GameObject hiting = Instantiate(hit, gameObject.transform.position, Quaternion.identity);
                Destroy(hiting, 0.3f);
                hitTimer = 0.5f;
            }
        }

        if (hitTimer >= 0)
        {
            hitTimer -= Time.deltaTime;
        }
    }
}
