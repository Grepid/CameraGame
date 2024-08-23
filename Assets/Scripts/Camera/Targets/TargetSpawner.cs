using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public List<PhotoTargetType> AllowedTypes = new List<PhotoTargetType>();

    private void Start()
    {
        Instantiate(TargetManager.GetRandomFromTypes(AllowedTypes), transform);
    }

}
