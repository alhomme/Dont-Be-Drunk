using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ProcGenManager : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] private GameObject mCharacterPrefab;

    [Header("Camera")]
    [SerializeField] private PositionConstraint mCameraPC;

    [Header("Pools")]
    [SerializeField] private RoadPool mRoadPool;
    [SerializeField] private ItemsPool mItemsPool;
    [SerializeField] private AlcoholPool mAlcoholPool;
    [SerializeField] private BiomePool mBiomePool;

    [Header("RSO")]
    [SerializeField] private RSO_Distance mRSO_Distance;
    [SerializeField] private RSO_Biome mRSO_Biome;
    [SerializeField] private RSO_Seed mRSO_Seed;

    [Header("RSE")]
    [SerializeField] private RSE_StartGame mRSE_StartGame;
    [SerializeField] private RSE_UpdateDistance mRSE_UpdateDistance;

    [Header("SSO")]
    [SerializeField] private SSO_Items mSSO_Obstacles;
    [SerializeField] private SSO_Items mSSO_Collectibles;
    [SerializeField] private SSO_ItemsAlcohol mSSO_ItemsAlcohol;

    [Header("Seed")]
    public int pSeed;

    private GameObject mCharacterObj;
    private float mChunkPosition;
    private bool mGameRunning = false;

    private List<Vector2> mPositionsAvailable = new List<Vector2>();
    private float[] mPossibleX = { -2f, 0f, 2f };
    private float[] mPossibleZ = { -10f, -5, 0, 5, 10, 15 };

    private void OnEnable()
    {
        mRSO_Seed.Value = 0;
    }

    private void OnDisable()
    {
        MyUtils.Log("ProcGenManager.OnDisable", false);
        mGameRunning = false;

        mRSO_Seed.Value = 0;

        // Deactivate all active gameObjects from pools
        //mRoadPool.DeactivatePool();
    }

    private IEnumerator Start()
    {
        MyUtils.Log("ProcGenManager.Start");

        yield return new WaitUntil(() => mRoadPool.pIsInitialized);
        yield return new WaitUntil(() => mItemsPool.pIsInitialized);
        yield return new WaitUntil(() => mBiomePool.pIsInitialized);

        // Create seed
        pSeed = UnityEngine.Random.Range(0, 10000000);
        mRSO_Seed.Value = pSeed;
        MyUtils.Log("Seed: " + pSeed);
        UnityEngine.Random.InitState(pSeed);

        mRSE_UpdateDistance.Dispatch(0);

        // Generate base chunks
        mChunkPosition = -15f;
        // Generate empty road behind player starting position
        GenerateRoad();
        mChunkPosition += 30f;
        // Generate next chunks
        for (int i = 0; i < mRoadPool.GetAmount() - 1; i++)
        {
            GenerateChunk();
        }


        // Instantiate Character
        Vector3 characterPosition = new Vector3(0f, 0.5f, 0f);
        mCharacterObj = Instantiate(mCharacterPrefab, transform);
        mCharacterObj.transform.position = characterPosition;

        // Set Camera Constraint to character
        ConstraintSource cSource = new ConstraintSource { sourceTransform = mCharacterObj.transform, weight = 1f };
        mCameraPC.AddSource(cSource);


        // Everything is ready
        // Check for tutorial

        // Start Game
        mRSE_StartGame.Dispatch();
        mGameRunning = true;
    }

    private void FixedUpdate()
    {
        if (!mGameRunning)
            return;

        // Check distance
        if ((int)(mCharacterObj.transform.position.z) > mRSO_Distance.Value)
        {
            mRSE_UpdateDistance.Dispatch((int)(mCharacterObj.transform.position.z));

            if (mRSO_Distance.Value > 0 && mRSO_Distance.Value % 30 == 0)
            {
                // Deactivate passed chunk(s)
                DeactivateChunk();
                // Generate new one
                GenerateChunk();
            }
        }
    }

    private void GenerateChunk()
    {
        // Get Biome
        int biomeIdx = ((int)mChunkPosition / 510);
        int biomeToPrint = biomeIdx % 4;
        if (biomeIdx > 4)
            biomeIdx = 4;
        

        // Get Road
        GenerateRoad();

        // Generate obstacles at random positions
        mPositionsAvailable.Clear();
        GeneratePositionsAvailable();
        
        GenerateItem(mSSO_Obstacles, biomeIdx);

        // Generate collectibles at random position
        GenerateItem(mSSO_Collectibles, biomeIdx);

        // Generate alcohol items at random position
        GenerateAlcohol(biomeIdx);

        // Generate environment
        GameObject leftBiome = mBiomePool.GetPooledObjectByIdx(biomeToPrint);
        Vector3 leftBiomePosition = new Vector3(-6f, 0, mChunkPosition);
        leftBiome.transform.position = leftBiomePosition;
        leftBiome.SetActive(true);

        GameObject rightBiome = mBiomePool.GetPooledObjectByIdx(biomeToPrint);
        Vector3 rightBiomePosition = new Vector3(6f, 0, mChunkPosition);
        rightBiome.transform.position = rightBiomePosition;
        rightBiome.SetActive(true);

        // Increase chunk position
        mChunkPosition += 30f;
    }

    private void DeactivateChunk()
    {
        // Deactivate road
        List<GameObject> roadObjs = mRoadPool.GetPooledObjectByPosition(mRSO_Distance.Value - 30);
        for (int i = 0; i < roadObjs.Count; i++)
        {
            roadObjs[i].SetActive(false);
        }

        // Deactivate obstacles & collectibles
        List<GameObject> itemsObjs = mItemsPool.GetPooledObjectByPosition(mRSO_Distance.Value - 30);
        for (int i = 0; i < itemsObjs.Count; i++)
        {
            itemsObjs[i].SetActive(false);
        }

        // Deactivate alcohol items
        List<GameObject> alcoholObjs = mAlcoholPool.GetPooledObjectByPosition(mRSO_Distance.Value - 30);
        for (int i = 0; i < alcoholObjs.Count; i++)
        {
            alcoholObjs[i].SetActive(false);
        }

        // Deactivate environment
        List<GameObject> biomeObjs = mBiomePool.GetPooledObjectByPosition(mRSO_Distance.Value - 30);
        for (int i = 0; i < biomeObjs.Count; i++)
        {
            biomeObjs[i].SetActive(false);
        }
    }

    private void GenerateRoad()
    {
        GameObject roadChunk = mRoadPool.GetPooledObject();
        Vector3 roadPosition = new Vector3(0, 0, mChunkPosition);
        roadChunk.transform.position = roadPosition;
        roadChunk.SetActive(true);
    }

    private void GenerateItem(SSO_Items ssoItems, int biomeIdx)
    {
        for (int i = 0; i < ssoItems.Value.Count; i++)
        {
            int nbItem = ssoItems.Value[i].mCountPerBiome[biomeIdx];
            string nameItem = ssoItems.Value[i].mTag;
            for (int j = 0; j < nbItem; j++)
            {
                Vector3 itemPosition = GenerateRandomPosition(ssoItems.Value[i].mPrefab.transform.position.y);
                MyUtils.Log("Item " + nameItem + " at position " + itemPosition);
                GameObject itemObj = mItemsPool.GetPooledObjectByName(nameItem);
                itemObj.transform.position = itemPosition;
                itemObj.SetActive(true);
            }
        }
    }

    private void GenerateAlcohol(int biomeIdx)
    {
        int nbAlcohol = mSSO_ItemsAlcohol.Value.mCountPerBiome[biomeIdx];
        for (int i = 0; i < nbAlcohol; i++)
        {
            GameObject alcoholItem = mAlcoholPool.GetPooledObject();

            Vector3 itemPosition = GenerateRandomPosition(mSSO_ItemsAlcohol.Value.mPrefabs[0].transform.position.y);
            MyUtils.Log("Item " + alcoholItem.tag + " at position " + itemPosition);

            alcoholItem.transform.position = itemPosition;
            alcoholItem.SetActive(true);

        }
    }

    private void GeneratePositionsAvailable()
    {
        // Create all available positions

        foreach (float z in mPossibleZ)
        {
            // Remove one X position so that there are never more than 2 objetcs on the same Z
            List<float> tmpX = new List<float>(mPossibleX);
            int randomIdx = UnityEngine.Random.Range(0, tmpX.Count);
            tmpX.RemoveAt(randomIdx);

            mPositionsAvailable.Add(new Vector2(tmpX[0], mChunkPosition + z));
            mPositionsAvailable.Add(new Vector2(tmpX[1], mChunkPosition + z));
        }

        MyUtils.LogList(mPositionsAvailable);
    }

    private Vector3 GenerateRandomPosition(float objY)
    {
        int randIdx = UnityEngine.Random.Range(0, mPositionsAvailable.Count);
        Vector2 position = mPositionsAvailable[randIdx];
        mPositionsAvailable.RemoveAt(randIdx);

        Vector3 randPositon = new Vector3(position.x, objY, position.y);
        return randPositon;
    }
}
