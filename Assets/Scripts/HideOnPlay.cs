using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    public bool Recurssive;

    // Start is called before the first frame update
    void Start()
    {
        if (Recurssive) HideAllChildren(gameObject);
        else HideObject(gameObject);
    }

    public static void HideAllChildren(GameObject go)
    {
        foreach(Transform t in go.transform)
        {
            HideAllChildren(t.gameObject);
        }
        HideObject(go);
    }

    public static void HideObject(GameObject go)
    {
        MeshRenderer mr = go.GetComponent<MeshRenderer>();
        Collider collider = go.GetComponent<Collider>();
        if(mr != null) mr.enabled = false;
        if (collider != null) collider.enabled = false;
    }

}
