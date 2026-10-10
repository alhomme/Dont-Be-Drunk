using System.Collections.Generic;
using UnityEngine;

public class ItemsPool : MonoBehaviour
{
    [SerializeField] private SSO_Items mSSO_Obstacles;
    [SerializeField] private SSO_Items mSSO_Collectibles;
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

        // Obstacles
        for (int i = 0; i < mSSO_Obstacles.Value.Count; i++)
        {
            int lastIdx = mSSO_Obstacles.Value[i].mCountPerBiome.Length - 1;
            int maxElem = mAmount * mSSO_Obstacles.Value[i].mCountPerBiome[lastIdx];
            for (int j = 0; j < maxElem; j++)
            {
                GameObject obj = Instantiate(mSSO_Obstacles.Value[i].mPrefab, transform);
                obj.SetActive(false);
                mPooledObjects.Add(obj);
            }
        }

        // Collectibles
        for (int i = 0; i < mSSO_Collectibles.Value.Count; i++)
        {
            int lastIdx = mSSO_Collectibles.Value[i].mCountPerBiome.Length - 1;
            int maxElem = mAmount * mSSO_Collectibles.Value[i].mCountPerBiome[lastIdx];
            for (int j = 0; j < maxElem; j++)
            {
                GameObject obj = Instantiate(mSSO_Collectibles.Value[i].mPrefab, transform);
                obj.SetActive(false);
                mPooledObjects.Add(obj);
            }
        }

        pIsInitialized = true;
    }

    public GameObject GetPooledObjectByName(string name)
    {
        MyUtils.Log("GetPooledObjectByName: " + name);
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
            if (mPooledObjects[i].transform.position.z <= positionZ)
            {
                objs.Add(mPooledObjects[i]);
            }
        }

        return objs;
    }
}
