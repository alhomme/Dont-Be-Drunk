using System.Collections.Generic;
using UnityEngine;

public class ObstaclesPool : MonoBehaviour
{
    [SerializeField] private GameObject mPedestrianPrefab;
    [SerializeField] private int mPedestrianAmount;
    [SerializeField] private GameObject mPolicePrefab;
    [SerializeField] private int mPoliceAmount;
    [SerializeField] private GameObject mHotdogPrefab;
    [SerializeField] private int mHotdogAmount;
    [SerializeField] private GameObject mMoversPrefab;
    [SerializeField] private int mMoversAmount;

    private List<GameObject> m_PooledObjects;
    [HideInInspector] public bool pIsInitialized;

    private void Awake()
    {
        MyUtils.Log("ObstaclesPool.Awake");
        pIsInitialized = false;
        InitializePool();
    }

    private void InitializePool()
    {
        m_PooledObjects = new List<GameObject>();
        for (int i = 0; i < mPedestrianAmount; i++)
        {
            GameObject obj = Instantiate(mPedestrianPrefab, transform);
            obj.SetActive(false);
            m_PooledObjects.Add(obj);
        }

        for (int i = 0; i < mPoliceAmount; i++)
        {
            GameObject obj = Instantiate(mPolicePrefab, transform);
            obj.SetActive(false);
            m_PooledObjects.Add(obj);
        }

        for (int i = 0; i < mHotdogAmount; i++)
        {
            GameObject obj = Instantiate(mHotdogPrefab, transform);
            obj.SetActive(false);
            m_PooledObjects.Add(obj);
        }

        for (int i = 0; i < mMoversAmount; i++)
        {
            GameObject obj = Instantiate(mMoversPrefab, transform);
            obj.SetActive(false);
            m_PooledObjects.Add(obj);
        }

        pIsInitialized = true;
    }

    public GameObject GetPooledObjectByName(string name)
    {
        MyUtils.Log("GetPooledObjectByName: " + name);
        for (int i = 0; i < m_PooledObjects.Count; i++)
        {
            if (m_PooledObjects[i].tag == name && !m_PooledObjects[i].activeInHierarchy)
            {
                return m_PooledObjects[i];
            }
        }

        return null;
    }
}
