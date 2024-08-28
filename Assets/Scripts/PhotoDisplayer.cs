using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class PhotoDisplayer : MonoBehaviour
{
    public bool isManual;

    public static List<PhotoDisplayer> AutoDisplayers = new List<PhotoDisplayer>();

    private void Awake()
    {
        AutoDisplayers.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (isManual) return;
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(GetImage());
        }
    }
    public IEnumerator GetImage()
    {
        var req = UnityWebRequestTexture.GetTexture(FileManager.instance.PhotoCachePath + "/Photo" + (FileManager.PhotosInCache-AutoDisplayers.IndexOf(this)) + ".png");
        yield return req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success)
        {
            print(req.error);
            yield break;
        }
        var tex = DownloadHandlerTexture.GetContent(req);
        MeshRenderer r = gameObject.GetComponent<MeshRenderer>();
        r.material.mainTexture = tex;
    }
}
