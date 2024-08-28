using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public List<GameObject> TargetPrefabs = new List<GameObject>(); 
    //public Dictionary<GameObject,PhotoTargetType> 



    public static TargetManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }


    }
    public static GameObject GetRandomFromTypes(ICollection<PhotoTargetType> types)
    {
        if (types.Count <= 0) return null;
        List<PhotoTarget> result = new List<PhotoTarget>();
        foreach (var x in instance.TargetPrefabs) result.Add(x.GetComponent<PhotoTarget>());
        List<PhotoTarget> filtered = result.FindAll(t => types.Contains(t.info.Type));

        int index = UnityEngine.Random.Range(0, filtered.Count);
        PhotoTarget chosenTarget = filtered.ElementAt(index);
        int orginialIndex = result.IndexOf(chosenTarget);

        return instance.TargetPrefabs.ElementAt(orginialIndex);
    }
}
