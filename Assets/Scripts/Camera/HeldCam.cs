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

                pointsOnCollider.Add(col.ClosestPoint(col.bounds.center + (col.transform.right * extents.x)));
                pointsOnCollider.Add(col.ClosestPoint(col.bounds.center + ((col.transform.right + col.transform.forward) * (extents.x + extents.z))));


                pointsOnCollider.Add(col.ClosestPoint(col.bounds.center + (col.transform.up*extents.y)));
                pointsOnCollider.Add(col.ClosestPoint(col.bounds.center + (col.transform.forward*extents.z)));
            }

            AddPointOnCollider(col.bounds.extents);
            AddPointOnCollider(-col.bounds.extents);

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
