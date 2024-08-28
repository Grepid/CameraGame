using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class FileManager : MonoBehaviour
{
    public static FileManager instance;

    public string PPath {  get; private set; }
    public string PhotoCachePath { get; private set; }
    public string SavedPhotosPath { get; private set; }
    public string SaveDataPath { get; private set; }

    public static int PhotosInCache
    {
        get
        {
            return Directory.GetFiles(instance.PhotoCachePath).Length;
        }
    }


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        PPath = Application.persistentDataPath;
        PhotoCachePath = PPath + "/Saves" + "/PhotoCache";
        SavedPhotosPath = PPath + "/Saves" + "/SavedPhotos";
        SaveDataPath = PPath + "/Saves" + "/SaveData";
        TryAccessDirectories();
        /*UnityWebRequestTexture.GetTexture("");
        var tex = DownloadHandlerTexture.GetContent(^);
        MeshRenderer r = new MeshRenderer();
        r.material.mainTexture = tex;*/
    }
    private void TryAccessDirectories()
    {
        if (!Directory.Exists(PhotoCachePath))
        {
            Directory.CreateDirectory(PhotoCachePath);
        }

        if (!Directory.Exists(SavedPhotosPath))
        {
            Directory.CreateDirectory(SavedPhotosPath);
        }

        if (!Directory.Exists(SaveDataPath))
        {
            Directory.CreateDirectory(SaveDataPath);
        }
    }
}
