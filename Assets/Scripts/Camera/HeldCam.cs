using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class HeldCam : MonoBehaviour
{
    FileManager fm => FileManager.instance;
    public int PhotosInCache
    {
        get
        {
            return Directory.GetFiles(fm.PhotoCachePath).Length;
        }
    }
    public Photo LastPhoto;

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
        string photoName = fm.PhotoCachePath + "Photo" + (PhotosInCache + 1) + ".png";
        ScreenCapture.CaptureScreenshot(photoName);
        p.photoPath = photoName;
        List<GameObject> InView = GetObjectsInView();
        foreach (GameObject go in InView)
        {
            PhotoTarget target = go.GetComponent<PhotoTarget>();
            if (target != null)
            {
                float amountVisible = ObscureCheck(target);
                if (amountVisible == 0)
                {
                    print(target.info.Type + " was obscured");
                    continue;
                }
                p.targets.Add(target);
            }
        }
        LastPhoto = p;
    }

    public void GiveInfo(Photo photo)
    {
        if (LastPhoto == null) return;
        foreach(var info in photo.targets)
        {
            print(info.info.Type + " " + info.info.weight);
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

    private float ObscureCheck(PhotoTarget target)
    {
        int visibleCount = 0;
        foreach(Transform t in target.capturePoints)
        {
            Vector3 direction = t.position - cam.transform.position;
            if(Physics.Raycast(cam.transform.position, direction.normalized, out RaycastHit hit))
            {
                if(hit.collider.gameObject == target.gameObject) visibleCount++;
            }
        }

        //Casts ray to 
        /*if(target.capturePoints.Count == 0)
        {
            Vector3 direction = target.gameObject.transform.position - cam.transform.position;
            if (Physics.Raycast(cam.transform.position, direction.normalized, out RaycastHit hit))
            {
                if (hit.collider.gameObject == target.gameObject) return 1;
            }
            return 0;
        }*/


        if (target.capturePoints.Count == 0)
        {
            Vector3 direction = target.gameObject.transform.position - cam.transform.position;

            List<RaycastHit> Hits = new List<RaycastHit>();
            Collider col = target.GetComponent<Collider>();

            List<Vector3> pointsOnCollider = new List<Vector3>();

            print(col.bounds.extents);

            void AddPointOnCollider(Vector3 extents)
            {
                //pointsOnCollider.Add(col.ClosestPoint(col.bounds.center + (col.transform.right * extents.x)));
                //pointsOnCollider.Add(col.ClosestPoint(col.bounds.center + (col.transform.up*extents.y)));
                //pointsOnCollider.Add(col.ClosestPoint(col.bounds.center + (col.transform.forward*extents.z)));
                
                Vector3 localisedPoint = extents;

                //localisedPoint.x = (col.gameObject.transform.right * extents.x).x;
                //localisedPoint.y = (col.gameObject.transform.up * extents.y).y;
                //localisedPoint.z = (col.gameObject.transform.forward * extents.z).z;

                print(localisedPoint.x);
                localisedPoint.x = (col.gameObject.transform.right * extents.x).x;
                print(localisedPoint.x);
                localisedPoint.y = (col.gameObject.transform.up * extents.y).y;
                localisedPoint.z = (col.gameObject.transform.forward * extents.z).z;

                //pointsOnCollider.Add(col.ClosestPoint(col.bounds.center + localisedPoint));
                pointsOnCollider.Add(col.bounds.center + localisedPoint);
                //pointsOnCollider.Add(col.bounds.center + extents);

            }

            Vector3 ext = col.bounds.extents;
            Vector3 currentPoint = col.bounds.extents;

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
                        AddPointOnCollider(currentPoint);
                        if(z == 0) m3 = 0;
                        if (z == 1) m3 = -1;
                        if(z == 2) m3 = 1;
                    }
                }
            }


            //AddPointOnCollider(col.bounds.extents);
            //AddPointOnCollider(-col.bounds.extents);

            foreach (Vector3 v3 in pointsOnCollider)
            {
                GameObject go = new GameObject();
                go.transform.position = v3;
            }

            int pointsInView = 0;
            foreach(Vector3 point in pointsOnCollider)
            {
                if(Physics.Raycast(cam.transform.position,point-cam.transform.position,out RaycastHit hitInfo))
                {
                    if(hitInfo.collider.gameObject == target.gameObject)
                    {
                        pointsInView++;
                    }
                }
            }
            print(pointsInView);

            if (Physics.Raycast(cam.transform.position, direction.normalized, out RaycastHit hit))
            {
                if (hit.collider.gameObject == target.gameObject) return 1;
            }


            return 0;
        }


        return (float)visibleCount / target.capturePoints.Count;    
    }
}
