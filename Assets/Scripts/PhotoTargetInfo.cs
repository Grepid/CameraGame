using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PhotoTargetType{Trap,MonsterAlive,MonsterDead,Artifact,Shiny}

[System.Serializable]
public class PhotoTargetInfo
{
    public PhotoTargetType Type;
    [Range(0,1)]
    public float weight;
}
