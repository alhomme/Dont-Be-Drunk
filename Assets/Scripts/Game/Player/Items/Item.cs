using System;
using UnityEngine;

[Serializable]
public struct Item
{
    public string mTag;
    public LayerMask mLayer;
    public int[] mCountPerBiome;
    public GameObject mPrefab;
}

[Serializable]
public struct ItemAlcohol
{

    public LayerMask mLayer;
    public int[] mCountPerBiome;

    public string[] mTags;
    public GameObject[] mPrefabs;
    public int[] mRates;
    public int[] mEffects;
}
