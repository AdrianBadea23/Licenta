using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    private string gate = "Goal";
    private string sideObj = "SideObj";
    private string goalObj = "Goal2";
    private GameObject Goal;
    private Vector3 GoalPos;
    private GameObject[] Gates;
    private Vector3[] GatesPositions;
    private Quaternion[] GatesRotation;
    private GameObject[] sideObjectives;
    private Vector3[] sideObjectivePositions;
    private string tileName;

    void Start()
    {
        
        tileName = gameObject.name;
        Debug.Log(tileName);
        // Get all children in the dungeon
        Transform[] children = GetComponentsInChildren<Transform>();
        
        // Initialize arrays based on the number of gates and side objectives found
        int gateCount = 0;
        int sideObjCount = 0;
        
        // First pass to count gates and side objectives
        foreach (var child in children)
        {
            if (child.gameObject.name.Contains(gate))
                gateCount++;
            if (child.gameObject.name.Contains(sideObj))
                sideObjCount++;
            if (child.gameObject.name.Contains(goalObj))
            {
                Goal = child.gameObject;
            }
        }

        // Initialize arrays
        Gates = new GameObject[gateCount];
        GatesPositions = new Vector3[gateCount];
        GatesRotation = new Quaternion[gateCount];
        sideObjectives = new GameObject[sideObjCount];
        sideObjectivePositions = new Vector3[sideObjCount];

        // Second pass to assign gates and side objectives
        int gateIndex = 0;
        int sideObjIndex = 0;

        foreach (var child in children)
        {
            if (child.gameObject.name.Contains(gate))
            {
                Gates[gateIndex] = child.gameObject;
                GatesPositions[gateIndex] = child.transform.position;
                GatesRotation[gateIndex] = child.transform.rotation;
                gateIndex++;
            }

            if (child.gameObject.name.Contains(sideObj))
            {
                sideObjectives[sideObjIndex] = child.gameObject;
                sideObjectivePositions[sideObjIndex] = child.transform.position;
                sideObjIndex++;
            }
        }

        foreach (var obj in Gates)
        {
            var gateMeshRenderer = obj.GetComponent<MeshRenderer>();
            
            if (obj.name.Contains("FinalGoal"))
            {
                continue;
            }
            
            if (gateMeshRenderer != null)
            {
                gateMeshRenderer.enabled = false;
            }
        }
    }

    public void RespawnObjectives()
    {
        for (int i = 0; i < sideObjectives.Length; i++)
        {
            if (sideObjectives[i] == null)
            {
                sideObjectives[i] = Instantiate(Resources.Load<GameObject>("SideObj"), sideObjectivePositions[i], Quaternion.identity);
                sideObjectives[i].tag = "SideObj";
            }
            else
            {
                sideObjectives[i].transform.position = sideObjectivePositions[i];
            }
        }
        
        for (int i = 0; i < Gates.Length; i++)
        {
            if (Gates[i] == null)
            {
                Gates[i] = Instantiate(Resources.Load<GameObject>("Goal"), GatesPositions[i], GatesRotation[i]);
                Gates[i].tag = "Goal";
            }
            else
            {
                Gates[i].transform.position = GatesPositions[i];
            }
        }
        
        foreach (var obj in Gates)
        {
            var gateMeshRenderer = obj.GetComponent<MeshRenderer>();
            
            if (obj.name.Contains("FinalGoal"))
            {
                continue;
            }
            
            if (gateMeshRenderer != null)
            {
                gateMeshRenderer.enabled = false;
            }
        }
        
        if (tileName.Contains("Mini-Boss"))
        {
            int rand = UnityEngine.Random.Range(0, 4);
            switch (rand)
            {
                case 0:
                    GoalPos = new Vector3(-4, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 1:
                    GoalPos = new Vector3(14, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 2:
                    GoalPos = new Vector3(5, 0, -9);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 3:
                    GoalPos = new Vector3(5, 0, 9);
                    Goal.transform.localPosition = GoalPos;
                    break;
                default:
                    GoalPos = new Vector3(-4, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
            }
            
        }

        if (tileName.Contains("Start Room"))
        {
            int rand = UnityEngine.Random.Range(0, 4);
            switch (rand)
            {
                case 0:
                    GoalPos = new Vector3(0, 0, 8.8f);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 1:
                    GoalPos = new Vector3(-9.2f, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 2:
                    GoalPos = new Vector3(9f, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 3:
                    GoalPos = new Vector3(-9f, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                default:
                    GoalPos = new Vector3(0, 0, 8.8f);
                    Goal.transform.localPosition = GoalPos;
                    break;
            }
            
        }
        
        if (tileName.Contains("Special"))
        {
            int rand = UnityEngine.Random.Range(0, 4);
            switch (rand)
            {
                case 0:
                    GoalPos = new Vector3(0, 0, 9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 1:
                    GoalPos = new Vector3(-9f, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 2:
                    GoalPos = new Vector3(9f, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 3:
                    GoalPos = new Vector3(0, 0, -9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
                default:
                    GoalPos = new Vector3(0, 0, 9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
            }
            
        }
        
        if (tileName.Contains("BranchCap"))
        {
            int rand = UnityEngine.Random.Range(0, 4);
            switch (rand)
            {
                case 0:
                    GoalPos = new Vector3(0, 0, 9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 1:
                    GoalPos = new Vector3(-9f, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 2:
                    GoalPos = new Vector3(9f, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 3:
                    GoalPos = new Vector3(0, 0, -9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
                default:
                    GoalPos = new Vector3(0, 0, 9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
            }
            
        }
        
        if (tileName.Contains("Boss Room"))
        {
            int rand = UnityEngine.Random.Range(0, 4);
            switch (rand)
            {
                case 0:
                    GoalPos = new Vector3(0, 0, 9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 1:
                    GoalPos = new Vector3(-9f, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 2:
                    GoalPos = new Vector3(9f, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 3:
                    GoalPos = new Vector3(0, 0, -9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
                default:
                    GoalPos = new Vector3(0, 0, 9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
            }
            
        }
        
        if (tileName.Contains("Corridor"))
        {
            int rand = UnityEngine.Random.Range(0, 3);
            switch (rand)
            {
                case 0:
                    GoalPos = new Vector3(0, 0, 14);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 1:
                    GoalPos = new Vector3(0, 0, -4);
                    Goal.transform.localPosition = GoalPos;
                    break;
                default:
                    GoalPos = new Vector3(0, 0, 14);
                    Goal.transform.localPosition = GoalPos;
                    break;
            }
            
        }
        
        if (tileName.Contains("L-Room"))
        {
            int rand = UnityEngine.Random.Range(0, 4);
            switch (rand)
            {
                case 0:
                    GoalPos = new Vector3(0, 0, -4);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 1:
                    GoalPos = new Vector3(0, 0, 14);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 2:
                    GoalPos = new Vector3(4, 0, 10);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 3:
                    GoalPos = new Vector3(-14, 0, 10);
                    Goal.transform.localPosition = GoalPos;
                    break;
                default:
                    GoalPos = new Vector3(0, 0, -4);
                    Goal.transform.localPosition = GoalPos;
                    break;
            }
            
        }
        
        if (tileName.Contains("Square Room"))
        {
            int rand = UnityEngine.Random.Range(0, 4);
            switch (rand)
            {
                case 0:
                    GoalPos = new Vector3(0, 0, 9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 1:
                    GoalPos = new Vector3(-9f, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 2:
                    GoalPos = new Vector3(9f, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 3:
                    GoalPos = new Vector3(0, 0, -9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
                default:
                    GoalPos = new Vector3(0, 0, 9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
            }
            
        }
        
        if (tileName.Contains("Large Room"))
        {
            int rand = UnityEngine.Random.Range(0, 4);
            switch (rand)
            {
                case 0:
                    GoalPos = new Vector3(0, 0, 9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 1:
                    GoalPos = new Vector3(-9f, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 2:
                    GoalPos = new Vector3(9f, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 3:
                    GoalPos = new Vector3(0, 0, -9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
                default:
                    GoalPos = new Vector3(0, 0, 9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
            }
            
        }
        
        if (tileName.Contains("One Way Room"))
        {
            int rand = UnityEngine.Random.Range(0, 4);
            switch (rand)
            {
                case 0:
                    GoalPos = new Vector3(0, 0, 9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 1:
                    GoalPos = new Vector3(-9f, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 2:
                    GoalPos = new Vector3(9f, 0, 0);
                    Goal.transform.localPosition = GoalPos;
                    break;
                case 3:
                    GoalPos = new Vector3(0, 0, -9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
                default:
                    GoalPos = new Vector3(0, 0, 9f);
                    Goal.transform.localPosition = GoalPos;
                    break;
            }
            
        }
        
    }

    void Update()
    {
        
    }
}
