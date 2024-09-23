using AudioSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class HeldCam : MonoBehaviour
{
    FileManager fm => FileManager.instance;
    GameObject obscureMatrix;

    public Photo LastPhoto;

    public int PhotosLeft;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        // Create the "Parent" transform of points
        obscureMatrix = new GameObject("obscureMatrix");
        for(int i = 0; i<27; i++)
        {
            GameObject go = new GameObject("matrixPoint" + i);
            go.transform.parent = obscureMatrix.transform;
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TakePhoto();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            GiveInfo(LastPhoto);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach(PhotoTarget t in PhotoTarget.targetsInScene)
            {
                print("A " + t.info.Type + " was left unphotoed");
            }
        }
    }

    public void TakePhoto()
    {
        if (PhotosLeft <= 0) return;
        PhotosLeft--;
        print("Photo taken");
        Photo p = new Photo();
        string photoName = fm.PhotoCachePath + "/Photo" + (FileManager.PhotosInCache + 1) + ".png";
        ScreenCapture.CaptureScreenshot(photoName);
        p.photoPath = photoName;
        List<GameObject> InView = GetObjectsInView();
        foreach (GameObject go in InView)
        {
            //Make a list of the visible ones
            // Make this foreach the one that holds the targets and grabs all their stats
            // make the outside of this loop run all the numbers and find the best one
            PhotoTarget target = go.GetComponent<PhotoTarget>();
            if (target != null)
            {
                float amountVisible = ObscureCheck(target);
                if (amountVisible == 0)
                {
                    print(target.info.Type + " was obscured");
                    continue;
                }
                p.target = target;
                PhotoTarget.targetsInScene.Remove(target);
                Destroy(target.gameObject.GetComponent<PhotoTarget>());
                p.positionOnScreen.Add(target, cam.WorldToScreenPoint(target.gameObject.transform.position));
            }
        }
        LastPhoto = p;
    }



    public void GiveInfo(Photo photo)
    {
        if (LastPhoto == null) return;
        print(photo.target.info.Type + " " + photo.target.info.weight);
        print(photo.positionOnScreen[photo.target]);
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

    private float ObscureCheck(PhotoTarget target)
    {
        Collider col = target.GetComponent<Collider>();
        Collider col2 = Instantiate(col.gameObject,col.gameObject.transform.position,Quaternion.Euler(Vector3.zero)).GetComponent<Collider>();
            
        List<Vector3> pointsOnCollider = new List<Vector3>();

        Vector3 ext = col2.bounds.extents;
        Vector3 currentPoint = col2.bounds.extents;
        col2.enabled = false;
        Destroy(col2.gameObject);

        int m1 = 1;
        int m2 = 1;
        int m3 = 1;
        for (int x = 0; x < 3; x++)
        {
            currentPoint = new Vector3(ext.x*m1, currentPoint.y, currentPoint.z);
            if (x == 0) m1 = 0;
            if (x == 1) m1 = -1;
            if (x == 2) m1 = 1;
            for(int y = 0; y < 3; y++)
            {
                currentPoint = new Vector3(currentPoint.x, ext.y * m2, currentPoint.z);
                if (y == 0) m2 = 0;
                if (y == 1) m2 = -1;
                if (y == 2) m2 = 1;
                for (int z = 0; z < 3; z++)
                {
                    currentPoint = new Vector3(currentPoint.x, currentPoint.y, ext.z * m3);
                    pointsOnCollider.Add(currentPoint);
                    if(z == 0) m3 = 0;
                    if (z == 1) m3 = -1;
                    if(z == 2) m3 = 1;
                }
            }
        }

        obscureMatrix.transform.position = col.bounds.center;
        int w = 0;
        foreach(Transform t in obscureMatrix.transform)
        {
            t.localPosition = pointsOnCollider[w];
            w++;
        }
        obscureMatrix.transform.eulerAngles = Vector3.RotateTowards(obscureMatrix.transform.eulerAngles, col.transform.eulerAngles, 999, 999);
        foreach(Transform t in obscureMatrix.transform)
        {
            t.position = col.ClosestPoint(t.position);
            //t.position = Vector3.MoveTowards(t.position, col.bounds.center, 0.1f);
        }

        int pointsInView = 0;
        foreach(Transform point in obscureMatrix.transform)
        {
            Ray ray = new Ray(cam.transform.position, point.position - cam.transform.position);
            Debug.DrawRay(ray.origin, ray.direction*Vector3.Distance(cam.transform.position,point.position), Color.red, 5f);
            if(Physics.Raycast(ray,out RaycastHit hitInfo))
            {
                //print(hitInfo.collider.gameObject.name);
                if(hitInfo.collider.gameObject == target.gameObject)
                {
                    pointsInView++;
                }
            }
        }
        print(pointsInView);


        return (float)pointsInView / 27f;    
    }
}
