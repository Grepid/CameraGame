using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    public static FileManager instance;

    public string PPath {  get; private set; }
    public string PhotoCachePath { get; private set; }
    public string SavedPhotosPath { get; private set; }
    public string SaveDataPath { get; private set; }

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
