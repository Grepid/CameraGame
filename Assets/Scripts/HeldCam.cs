using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HeldCam : MonoBehaviour
{
    public const string _PhotoCache = "PhotoCache/";
    // Start is called before the first frame update
    void Start()
    {
        ScreenCapture.CaptureScreenshot(_PhotoCache+"test.png");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
