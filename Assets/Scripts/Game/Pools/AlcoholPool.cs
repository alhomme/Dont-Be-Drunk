using System.Collections.Generic;
using UnityEngine;

public class AlcoholPool : MonoBehaviour
{
    [SerializeField] private SSO_ItemsAlcohol mSSO_ItemsAlcohol;
    [SerializeField] private int mAmount; // = 10

    private List<GameObject> mPooledObjects;
    [HideInInspector] public bool pIsInitialized;

    private void Awake()
    {
        MyUtils.Log("ObstaclesPool.Awake");
        pIsInitialized = false;
        InitializePool();
    }

    private void InitializePool()
    {
        mPooledObjects = new List<GameObject>();

        int lastIdx = mSSO_ItemsAlcohol.Value.pCountPerBiome.Length - 1;
        int maxElem = mAmount * mSSO_ItemsAlcohol.Value.pCountPerBiome[lastIdx];

        for (int i = 0; i < mSSO_ItemsAlcohol.Value.pTags.Length; i++)
        {
            for (int j = 0; j < maxElem; j++)
            {
                GameObject obj = Instantiate(mSSO_ItemsAlcohol.Value.pPrefabs[i], transform);
                obj.SetActive(false);
                mPooledObjects.Add(obj);
            }
        }

        pIsInitialized = true;
    }

    public GameObject GetPooledObject()
    {
        int randNb = UnityEngine.Random.Range(0, 100);
        //MyUtils.Log("AlcoholPool.GetPooledObject: Random number is " + randNb);

        for (int i = 0; i < mSSO_ItemsAlcohol.Value.pRates.Length; i++)
        {
            if (randNb < mSSO_ItemsAlcohol.Value.pRates[i])
            {
                return GetPooledObjectByName(mSSO_ItemsAlcohol.Value.pTags[i]);
            }
        }

        return null;
    }

    public GameObject GetPooledObjectByName(string name)
    {
        //MyUtils.Log("GetPooledObjectByName: " + name);
        for (int i = 0; i < mPooledObjects.Count; i++)
        {
            if (mPooledObjects[i].tag == name && !mPooledObjects[i].activeInHierarchy)
            {
                return mPooledObjects[i];
            }
        }

        return null;
    }

    public List<GameObject> GetPooledObjectByPosition(float positionZ)
    {
        List<GameObject> objs = new List<GameObject>();

        for (int i = 0; i < mPooledObjects.Count; i++)
        {
            if (mPooledObjects[i].transform.position.z <= positionZ
                && mPooledObjects[i].activeInHierarchy)
            {
                objs.Add(mPooledObjects[i]);
            }
        }

        return objs;
    }
}
