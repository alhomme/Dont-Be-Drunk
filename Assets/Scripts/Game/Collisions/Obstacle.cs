using System;
using UnityEngine;

[Serializable]
public struct Obstacle
{
    public string mTag;
    public LayerMask mLayer;
    public int[] mCountPerBiome;
    public GameObject mObj;
}
