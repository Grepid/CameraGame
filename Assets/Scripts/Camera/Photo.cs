using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Photo
{
    public string photoPath;
    public PhotoTarget target;
    public Dictionary<PhotoTarget,Vector3> positionOnScreen = new Dictionary<PhotoTarget,Vector3>();
}
