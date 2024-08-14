using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoTarget : MonoBehaviour
{
    public PhotoTargetInfo info;
    public List<Transform> capturePoints = new List<Transform>();

    private void Start()
    {
        foreach(Transform t in transform)
        {
            if (t.name.Contains("CapturePoint"))
            {
                capturePoints.Add(t);
            }
        }
    }
}
