using System;
using UnityEngine;

[Serializable]
public struct Item
{
    public string pTag;
    public LayerMask pLayer;
    public int[] pCountPerBiome;
    public GameObject pPrefab;
}

[Serializable]
public struct ItemAlcohol
{

    public LayerMask pLayer;
    public int[] pCountPerBiome;

    public string[] pTags;
    public GameObject[] pPrefabs;
    public int[] pRates;
    public int[] pEffects;
}
