using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Info")]
public class LevelInfoObject : ScriptableObject
{
    [Header("Types")]
    public int Traps;
    public int AliveMonsters;
    public int DeadMonsters;
    public int Artifacts;
    public int Shinies;

    [Header("Camera")]
    public int PhotosToTake;
}
