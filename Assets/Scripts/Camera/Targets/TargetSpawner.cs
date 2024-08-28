using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public List<PhotoTargetType> AllowedTypes = new List<PhotoTargetType>();

    private void Start()
    {
        GameObject toSpawn = TargetManager.GetRandomFromTypes(AllowedTypes);
        if (toSpawn == null) return;
        Instantiate(toSpawn, transform);
    }

}
