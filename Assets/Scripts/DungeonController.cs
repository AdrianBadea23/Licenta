using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(Wait());
        navMeshSurface = GetComponent<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
