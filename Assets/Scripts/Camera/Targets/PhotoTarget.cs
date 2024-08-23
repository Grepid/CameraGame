using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoTarget : MonoBehaviour
{
    public PhotoTargetInfo info;

    public static List<PhotoTarget> targetsInScene = new List<PhotoTarget>();

    private void Start()
    {
        targetsInScene.Add(this);
    }
}
