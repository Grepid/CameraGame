using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class PhotoDisplayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(GetImage());
        }
    }
    public IEnumerator GetImage()
    {
        var req = UnityWebRequestTexture.GetTexture(FileManager.instance.PhotoCachePath + "/Photo" + FileManager.PhotosInCache + ".png");
        yield return req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success)
        {
            print(req.error);
        }
        var tex = DownloadHandlerTexture.GetContent(req);
        MeshRenderer r = gameObject.GetComponent<MeshRenderer>();
        r.material.mainTexture = tex;
    }
}
