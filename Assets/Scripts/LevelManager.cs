using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour
{
    public LevelInfoObject Info {  get; private set; }
    public bool IsInitialised { get; private set; }

    private void Start()
    {
        
    }

    public void InitialiseLevel(LevelInfoObject info)
    {
        if (IsInitialised) return;

        Info = info;


        IsInitialised = true;
    }

}


