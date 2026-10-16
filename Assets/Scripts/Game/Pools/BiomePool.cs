using System.Collections.Generic;
using UnityEngine;

public class BiomePool : MonoBehaviour
{
    [SerializeField] private GameObject[] mBiomesPrefab;
    [SerializeField] private int mAmount;

    private List<List<GameObject>> mPooledObjects;
    [HideInInspector] public bool pIsInitialized;

    private void Awake()
    {
        MyUtils.Log("BiomePool.Awake");
        pIsInitialized = false;
        InitializePool();
    }

    private void InitializePool()
    {
        mPooledObjects = new List<List<GameObject>>();
        for (int i = 0; i < mBiomesPrefab.Length; i++)
        {
            List<GameObject> biomeList = new List<GameObject>();
            for (int j = 0; j < mAmount; j++)
            {
                GameObject obj = Instantiate(mBiomesPrefab[i], transform);
                obj.SetActive(false);
                biomeList.Add(obj);
            }
            mPooledObjects.Add(biomeList);
        }

        pIsInitialized = true;
    }

    public GameObject GetPooledObjectByIdx(int idx)
    {
        for (int i = 0; i < mPooledObjects[idx].Count; i++)
        {
            if (!mPooledObjects[idx][i].activeInHierarchy)
            {
                return mPooledObjects[idx][i];
            }
        }

        //MyUtils.Log("BiomePool: All elements in pool are used, creating a new one");
        GameObject obj = Instantiate(mBiomesPrefab[idx], transform);
        obj.SetActive(false);
        mPooledObjects[idx].Add(obj);

        return obj;
    }

    public List<GameObject> GetPooledObjectByPosition(float positionZ)
    {
        List<GameObject> objs = new List<GameObject>();

        for (int i = 0; i < mPooledObjects.Count; i++)
        {
            for (int j = 0; j < mPooledObjects[i].Count; j++)
            {
                if (mPooledObjects[i][j].transform.position.z <= positionZ
                    && mPooledObjects[i][j].activeInHierarchy)
                {
                    objs.Add(mPooledObjects[i][j]);
                }
            }
        }

        return objs;
    }
}
