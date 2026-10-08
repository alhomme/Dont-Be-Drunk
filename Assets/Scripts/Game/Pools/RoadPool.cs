using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoadPool : MonoBehaviour
{
    [SerializeField] private GameObject mPrefab;
    [SerializeField] private int mAmount;

    private List<GameObject> m_PooledObjects;
    [HideInInspector] public bool pIsInitialized;

    private void Awake()
    {
        MyUtils.Log("RoadPool.Awake");
        pIsInitialized = false;
        InitializePool();
    }

    private void InitializePool()
    {
        m_PooledObjects = new List<GameObject>();
        for (int i = 0; i < mAmount; i++)
        {
            GameObject obj = Instantiate(mPrefab, transform);
            obj.SetActive(false);
            m_PooledObjects.Add(obj);
        }

        pIsInitialized = true;
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < m_PooledObjects.Count; i++)
        {
            if (!m_PooledObjects[i].activeInHierarchy)
            {
                return m_PooledObjects[i];
            }
        }

        MyUtils.Log("RoadPool: All elements in pool are used, creating a new one");
        GameObject obj = Instantiate(mPrefab, transform);
        obj.SetActive(false);
        m_PooledObjects.Add(obj);

        return obj;
    }

    public List<GameObject> GetPooledObjectByPosition(float positionZ)
    {
        List<GameObject> objs = new List<GameObject>();

        for (int i = 0; i < m_PooledObjects.Count; i++)
        {
            if (m_PooledObjects[i].transform.position.z <= positionZ)
            {
                objs.Add(m_PooledObjects[i]);
            }
        }

        return objs;
    }

    public void DeactivatePool()
    {
        for (int i = 0; i < m_PooledObjects.Count; i++)
        {
            m_PooledObjects[i].SetActive(false);
        }

        pIsInitialized = false;
    }

    public int GetAmount()
    {
        return mAmount;
    }
}
