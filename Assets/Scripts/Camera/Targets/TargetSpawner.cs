using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;


public class TargetSpawner : MonoBehaviour
{
    public List<PhotoTargetType> AllowedTypes = new List<PhotoTargetType>();
    public static List<TargetSpawner> Spawners = new List<TargetSpawner>();
    private Vector3 spawnPos;
    private Vector3 lookAtDirection;
    RaycastHit floor;
    public int SpawnPointCount;
    public GameObject pointPrefab;
    private List<GameObject> deletionPoints = new List<GameObject>();
    private int spawnPointsDeferred;

    private void OnValidate()
    {
        if(SpawnPointCount > transform.childCount)
        {
            spawnPointsDeferred = Mathf.Abs(SpawnPointCount-transform.childCount);
        }

        if(SpawnPointCount < transform.childCount)
        {
            for(int i = transform.childCount; i > 0; i--)
            {
                //Debug.Log(i);
                if (i == SpawnPointCount) break;
                GameObject currentSubPointFocus = transform.GetChild(i - 1).gameObject;
                deletionPoints.Add(currentSubPointFocus);
                
            }
        }
        EditorApplication.delayCall += DestroyInEditor;
        EditorApplication.delayCall += MakeSpawnPoints;
    }

    private void MakeSpawnPoints()
    {
        for(int i = 0; i < spawnPointsDeferred; i++)
        {
            var go = Instantiate(pointPrefab, transform);
            go.transform.localPosition = Vector3.down;
        }
        spawnPointsDeferred = 0;
        
    }
    private void DestroyInEditor()
    {
        foreach(var go in deletionPoints)
        {
            DestroyImmediate(go, true);
        }
    }


    private void Awake()
    {
        Spawners.Add(this);
    }

    private void Start()
    {
        SpawnTarget();
        //Add warning log for any unmoved spawn points from 0,-1,0
        CheckSubPoints();
    }
    private void CheckSubPoints()
    {
        Vector3 lastPos = Vector3.zero;
        foreach (Transform t in transform)
        {
            if (t.position == lastPos)
            {
                print(gameObject.name + " has one or more subSpawn points in the same position.");
                return;
            }
            lastPos = t.position;
        }
    }
    private void OldSpawn()
    {
        GameObject toSpawn = TargetManager.GetRandomFromTypes(AllowedTypes).gameObject;
        if (toSpawn == null) return;
        Instantiate(toSpawn, transform);
    }
    public void SpawnTarget()
    {
        ChooseSpot();
        FindRotation();

        PhotoTarget spawnTarget = TargetManager.GetRandomFromTypes(AllowedTypes);
        if(spawnTarget == null)return;
        GameObject toSpawn = spawnTarget.gameObject;
        GameObject spawned = Instantiate(toSpawn,spawnPos,Quaternion.identity);
        float nudgeAmount = spawned.GetComponent<Collider>().bounds.extents.z;
        spawned.transform.LookAt(spawnPos+(lookAtDirection*999f));
        Vector3 randRotation = new Vector3(0, 0, Random.Range(-10f, 10f));
        spawned.transform.Rotate(randRotation);
        //Vector3 nudgeAmount = new Vector3(0, spawned.GetComponent<Collider>().bounds.extents.y, 0);
        spawned.transform.position += (spawned.transform.forward*(nudgeAmount+0.01f));
    }
    private void FindFloor(Vector3 from)
    {
        if (!Physics.Raycast(from, Vector3.down, out RaycastHit hit))
        {
            lookAtDirection = Vector3.zero;
            print("No surface under found");

            return;
        }
        floor = hit;
    }
    private void ChooseSpot()
    {
        Vector3 chosenPoint = Vector3.zero;
        if(transform.childCount == 0)
        {
            chosenPoint = transform.position;
        }
        else
        {
            int randIndex = Random.Range(0, transform.childCount);
            chosenPoint = transform.GetChild(randIndex).position;
        }
        
        FindFloor(chosenPoint);
        spawnPos = floor.collider == null ? chosenPoint : floor.point;
        
    }
    private void FindRotation()
    {
        lookAtDirection = floor.normal;
    }
}
