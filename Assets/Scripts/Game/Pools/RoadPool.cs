using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoadPool : MonoBehaviour
{
    [SerializeField] private GameObject mPrefab;
    [SerializeField] private int mAmount;

    private List<GameObject> mPooledObjects;
    [HideInInspector] public bool pIsInitialized;

    private void Awake()
    {
        MyUtils.Log("RoadPool.Awake");
        pIsInitialized = false;
        InitializePool();
    }

    private void InitializePool()
    {
        mPooledObjects = new List<GameObject>();
        for (int i = 0; i < mAmount; i++)
        {
            GameObject obj = Instantiate(mPrefab, transform);
            obj.SetActive(false);
            mPooledObjects.Add(obj);
        }

        pIsInitialized = true;
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < mPooledObjects.Count; i++)
        {
            if (!mPooledObjects[i].activeInHierarchy)
            {
                return mPooledObjects[i];
            }
        }

        //MyUtils.Log("RoadPool: All elements in pool are used, creating a new one");
        GameObject obj = Instantiate(mPrefab, transform);
        obj.SetActive(false);
        mPooledObjects.Add(obj);

        return obj;
    }

    public List<GameObject> GetPooledObjectByPosition(float positionZ)
    {
        List<GameObject> objs = new List<GameObject>();

        for (int i = 0; i < mPooledObjects.Count; i++)
        {
            if (mPooledObjects[i].transform.position.z <= positionZ)
            {
                objs.Add(mPooledObjects[i]);
            }
        }

        return objs;
    }

    public void DeactivatePool()
    {
        for (int i = 0; i < mPooledObjects.Count; i++)
        {
            mPooledObjects[i].SetActive(false);
        }

        pIsInitialized = false;
    }

    public int GetAmount()
    {
        return mAmount;
    }
}
