using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class HeldCam : MonoBehaviour
{
    private string _PhotoCache;
    public int PhotosInCache
    {
        get
        {
            return Directory.GetFiles(_PhotoCache).Length;
        }
    }
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
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakePhoto();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            GiveInfo(LastPhoto);
        }
    }

    public void TakePhoto()
    {
        Photo p = new Photo();
        string photoName = _PhotoCache + "Photo" + (PhotosInCache + 1) + ".png";
        ScreenCapture.CaptureScreenshot(photoName);
        p.photoPath = photoName;
        List<GameObject> InView = GetObjectsInView();
        foreach (GameObject go in InView)
        {
            PhotoTarget target = go.GetComponent<PhotoTarget>();
            if (target != null)
            {
                if (IsObscured(target.gameObject))
                {
                    print(target.info.Type + " was obscured");
                    continue;
                }
                p.targets.Add(target.info);
            }
        }
        LastPhoto = p;
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

    private bool IsObscured(GameObject go)
    {
        Vector3 direction = go.transform.position - cam.transform.position;
        if(Physics.Raycast(cam.transform.position, direction.normalized, out RaycastHit hit))
        {
            if (hit.collider.gameObject == go) return false;
        }
        return true;
    }
}
