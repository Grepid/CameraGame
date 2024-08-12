using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HeldCam : MonoBehaviour
{
    private string _PhotoCache;
    public Photo LastPhoto;

    private void Awake()
    {
        _PhotoCache = Application.persistentDataPath + "/PhotoCache/";
    }
    // Start is called before the first frame update
    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        if (!Directory.Exists(_PhotoCache))
        {
            Directory.CreateDirectory(_PhotoCache);
        }
        ScreenCapture.CaptureScreenshot(_PhotoCache+"test.png");
        IOExtension.ClearAllFiles(Directory.GetParent(_PhotoCache));
        print("Hello hehe :)");
    }

    public void TakePhoto()
    {
        Photo p = new Photo();
        ScreenCapture.CaptureScreenshot(_PhotoCache + "Photo.png");
        p.photoPath = _PhotoCache+"Photo.png";
        Collider[] colliders = GetObjectsInCameraView();
        foreach (Collider col in colliders)
        {
            print(col.name);
            PhotoTarget target = col.GetComponent<PhotoTarget>();
            if(target != null)
            {
                p.targets.Add(target.info);
            }
        }
        LastPhoto = p;
    }

    public void TakePhoto2()
    {
        Photo p = new Photo();
        ScreenCapture.CaptureScreenshot(_PhotoCache + "Photo.png");
        p.photoPath = _PhotoCache + "Photo.png";
        List<GameObject> InView = GetObjectsInView();
        foreach (GameObject go in InView)
        {
            print(go.name);
            PhotoTarget target = go.GetComponent<PhotoTarget>();
            if (target != null)
            {
                p.targets.Add(target.info);
            }
        }
        LastPhoto = p;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakePhoto2();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            GiveInfo(LastPhoto);
        }
    }
    public void GiveInfo(Photo photo)
    {
        if (LastPhoto == null) return;
        foreach(var info in photo.targets)
        {
            print(info.Type + " " + info.weight);
        }
    }
    public Camera cam;
    public float maxDistance = 100f; // Max distance for detection


    Collider[] GetObjectsInCameraView()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        Bounds bounds = new Bounds();

        foreach (var plane in planes)
        {
            bounds.Encapsulate(plane.ClosestPointOnPlane(Vector3.zero));
            
        }

        Vector3 center = cam.transform.position + cam.transform.forward * maxDistance / 2;
        Vector3 size = new Vector3(bounds.size.x, bounds.size.y, maxDistance);

        return Physics.OverlapBox(center, size / 2, cam.transform.rotation);
    }

    List<GameObject> GetObjectsInView()
    {
        List<GameObject> objectsInView = new List<GameObject>();

        // Get the bounding box for the camera's frustum
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        Collider[] colliders = Physics.OverlapBox(cam.transform.position + cam.transform.forward * maxDistance / 2,
                                                  new Vector3(maxDistance, maxDistance, maxDistance) / 2);

        foreach (Collider col in colliders)
        {
            if (GeometryUtility.TestPlanesAABB(planes, col.bounds))
            {
                objectsInView.Add(col.gameObject);
            }
        }

        return objectsInView;
    }
}
