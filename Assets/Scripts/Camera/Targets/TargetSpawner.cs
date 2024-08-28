using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public List<PhotoTargetType> AllowedTypes = new List<PhotoTargetType>();
    public static List<TargetSpawner> Spawners = new List<TargetSpawner>();
    private Vector3 spawnPos;
    private Vector3 lookAtDirection;
    RaycastHit floor;

    private void Awake()
    {
        Spawners.Add(this);
    }

    private void Start()
    {
        SpawnTarget();
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

        GameObject spawned = Instantiate(TargetManager.GetRandomFromTypes(AllowedTypes).gameObject,spawnPos,Quaternion.identity);
        spawned.transform.LookAt(transform.position+(lookAtDirection*999f));
        Vector3 randRotation = new Vector3(0, 0, Random.Range(-10f, 10f));
        spawned.transform.Rotate(randRotation);
        //spawned.transform.Translate(new Vector3(0, spawned.GetComponent<Collider>().bounds.extents.y, 0));
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
