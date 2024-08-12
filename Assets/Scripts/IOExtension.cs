using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class IOExtension
{
    private static bool HasBannedDirect(string filepath)
    {
        if (filepath.Contains("System32") || filepath.Contains("Windows")) return true;
        return false;
    }

    public static void DirectCheck(string filepath)
    {
        if (HasBannedDirect(filepath))
        {
            throw new Exception("You are attempting to access an off limits File Path. Access Denied.");
        }
    }

    public static void ClearAllFiles(DirectoryInfo direct)
    {
        DirectCheck(direct.FullName);
        var files = direct.GetFiles();
        foreach(var file in files)
        {
            file.Delete();
            //Debug.Log(file.Name);
        }
    }
}
