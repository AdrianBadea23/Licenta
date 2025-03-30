using System;
using UnityEngine;

public class GateSpawn : MonoBehaviour
{
    [SerializeField] private GameObject solidSnake;
    [SerializeField] private GameObject gandalf;
    [SerializeField] private int[] X = new int[3];
    [SerializeField] private int[] Y = new int[3];
    [SerializeField] private int[] Z = new int[3];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Instantiate(solidSnake, new Vector3(X[0], Y[0], Z[0]), Quaternion.identity);
            Instantiate(gandalf, new Vector3(X[1], Y[1], Z[1]), Quaternion.identity);
            Instantiate(solidSnake, new Vector3(X[2], Y[2], Z[2]), Quaternion.identity);
            Destroy(this.gameObject);
            
        }
    }
}
