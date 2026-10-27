using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class ProcGenManager : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] private GameObject mCharacterPrefab;

    [Header("Pools")]
    [SerializeField] private RoadPool mRoadPool;
    [SerializeField] private ItemsPool mItemsPool;
    [SerializeField] private AlcoholPool mAlcoholPool;
    [SerializeField] private BiomePool mBiomePool;

    [Header("RSO")]
    [SerializeField] private RSO_Seed mRSO_Seed;
    [SerializeField] private RSO_Distance mRSO_Distance;
    [SerializeField] private RSO_Player mRSO_Player;

    [Header("RSE")]
    [SerializeField] private RSE_StartGame mRSE_StartGame;

    [Header("SSO")]
    [SerializeField] private SSO_Items mSSO_Obstacles;
    [SerializeField] private SSO_Items mSSO_Collectibles;
    [SerializeField] private SSO_ItemsAlcohol mSSO_ItemsAlcohol;

    [Header("Seed")]
    public int pSeed;

    private float mChunkPosition;
    private int mChunkBiome;
    private int mChunkBiomeAsset;
    private int mChunkCount;

    private List<List<int>> mCollectiblesChunks;

    private List<Vector2> mPositionsAvailable = new List<Vector2>();
    private float[] mPossibleX = { -2f, 0f, 2f };
    private float[] mPossibleZ = { -12f, -9f, -6f, -3f, 0f, 3f, 6f, 9f, 12f, 15f };

    private void OnEnable()
    {
        mRSO_Distance.OnChanged += OnDistanceChanged;

        mRSO_Seed.Value = 0;
        mChunkPosition = -15f;
        mChunkCount = 0;

    }

    private void OnDisable()
    {
        MyUtils.Log("ProcGenManager.OnDisable", false);

        mRSO_Distance.OnChanged -= OnDistanceChanged;
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
        yield return new WaitUntil(() => mAlcoholPool.pIsInitialized);

        // Create seed
        pSeed = UnityEngine.Random.Range(0, 10000000);
        mRSO_Seed.Value = pSeed;
        MyUtils.Log("Seed: " + pSeed);
        UnityEngine.Random.InitState(pSeed);

        // Generate base chunks     
        // Generate empty road behind player starting position
        GenerateRoad();
        GenerateBiome();
        mChunkPosition += 30f;

        // Generate next chunks
        mCollectiblesChunks = new List<List<int>>();
        DetermineCollectibles();
        for (int i = 0; i < mRoadPool.GetAmount() - 1; i++)
        {
            GenerateChunk();
        }


        // Instantiate Character
        Vector3 characterPosition = new Vector3(0f, 0.5f, -5f);
        mRSO_Player.Value = Instantiate(mCharacterPrefab, transform);
        mRSO_Player.Value.transform.position = characterPosition;


        // Everything is ready
        // Check for tutorial here ?

        // Start Game
        mRSE_StartGame.Dispatch();
    }

    private void GenerateChunk()
    {
        // Get Road
        GenerateRoad();

        // Generate obstacles at random positions
        mPositionsAvailable.Clear();
        GeneratePositionsAvailable();

        GenerateObstacles();

        // Generate collectibles at random position
        GenerateCollectible();

        // Generate alcohol items at random position
        GenerateAlcohol();

        // Generate environment
        GenerateBiome();

        // Increase chunk position
        UpdateChunkPosition();
    }

    private void DeactivateChunk(float position)
    {
        //MyUtils.Log("ProcGenManager.DeactivateChunk: " + position);

        // Deactivate road
        List<GameObject> roadObjs = mRoadPool.GetPooledObjectByPosition(position);
        for (int i = 0; i < roadObjs.Count; i++)
        {
            roadObjs[i].SetActive(false);
        }

        // Deactivate obstacles & collectibles
        List<GameObject> itemsObjs = mItemsPool.GetPooledObjectByPosition(position);
        for (int i = 0; i < itemsObjs.Count; i++)
        {
            itemsObjs[i].SetActive(false);
        }

        // Deactivate alcohol items
        List<GameObject> alcoholObjs = mAlcoholPool.GetPooledObjectByPosition(position);
        for (int i = 0; i < alcoholObjs.Count; i++)
        {
            alcoholObjs[i].SetActive(false);
        }

        // Deactivate environment
        List<GameObject> biomeObjs = mBiomePool.GetPooledObjectByPosition(position);
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

    private void GenerateBiome()
    {
        GameObject leftBiome = mBiomePool.GetPooledObjectByIdx(mChunkBiomeAsset);
        Vector3 leftBiomePosition = new Vector3(-6f, 0, mChunkPosition);
        leftBiome.transform.position = leftBiomePosition;
        leftBiome.SetActive(true);

        GameObject rightBiome = mBiomePool.GetPooledObjectByIdx(mChunkBiomeAsset);
        Vector3 rightBiomePosition = new Vector3(6f, 0, mChunkPosition);
        rightBiome.transform.position = rightBiomePosition;
        rightBiome.SetActive(true);
    }

    private void GenerateObstacles()
    {
        for (int i = 0; i < mSSO_Obstacles.Value.Count; i++)
        {
            int nbObstacle = mSSO_Obstacles.Value[i].pCountPerBiome[mChunkBiome];
            string nameObstacle = mSSO_Obstacles.Value[i].pTag;
            for (int j = 0; j < nbObstacle; j++)
            {
                GameObject obstacleObj = mItemsPool.GetPooledObjectByName(nameObstacle);

                Vector3 obstaclePosition = GenerateRandomPosition(obstacleObj.transform.position.y);
                //MyUtils.Log("Obstacle " + nameItem + " at position " + itemPosition);

                obstacleObj.transform.position = obstaclePosition;
                obstacleObj.SetActive(true);
            }
        }
    }

    private void GenerateAlcohol()
    {
        int nbAlcohol = mSSO_ItemsAlcohol.Value.pCountPerBiome[mChunkBiome];
        for (int i = 0; i < nbAlcohol; i++)
        {
            GameObject alcoholItem = mAlcoholPool.GetPooledObject();

            Vector3 itemPosition = GenerateRandomPosition(alcoholItem.transform.position.y);
            //MyUtils.Log("Item " + alcoholItem.tag + " at position " + itemPosition);

            // Random Y position between 0.5 (base) & 1
            if (UnityEngine.Random.Range(0, 2) == 0)
                itemPosition.y = 1f;
            else
                itemPosition.y = 0.25f;

            alcoholItem.transform.position = itemPosition;
            alcoholItem.SetActive(true);
        }
    }

    private void GenerateCollectible()
    {
        for (int i = 0; i < mSSO_Collectibles.Value.Count; i++)
        {
            for (int j = 0; j < mCollectiblesChunks[i].Count; j++)
            {
                if (mChunkCount == mCollectiblesChunks[i][j])
                {
                    string nameCollectible = mSSO_Collectibles.Value[i].pTag;
                    GameObject collectibleObj = mItemsPool.GetPooledObjectByName(nameCollectible);

                    Vector3 collectiblePosition = GenerateRandomPosition(collectibleObj.transform.position.y);
                    //MyUtils.Log("Collectible " + nameCollectible + " at position " + collectiblePosition);

                    // Random Y position between 0.5 (base) & 1
                    if (UnityEngine.Random.Range(0, 2) == 0)
                        collectiblePosition.y = 1f;
                    else
                        collectiblePosition.y = 0.25f;

                    collectibleObj.transform.position = collectiblePosition;
                    collectibleObj.SetActive(true);
                }
            }
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

        //MyUtils.LogList(mPositionsAvailable);
    }

    private Vector3 GenerateRandomPosition(float objY)
    {
        int randIdx = UnityEngine.Random.Range(0, mPositionsAvailable.Count);

        //MyUtils.Log("ProcGenManager.GenerateRandomPosition: " + randIdx + " / " + mPositionsAvailable.Count);

        Vector2 position = mPositionsAvailable[randIdx];
        mPositionsAvailable.RemoveAt(randIdx);

        Vector3 randPositon = new Vector3(position.x, objY, position.y);
        return randPositon;
    }

    private void UpdateChunkPosition()
    {
        mChunkPosition += 30f;
        mChunkCount++;

        if (mChunkCount == 17)
        {
            ChangeBiome();
        }
    }

    private void ChangeBiome()
    {
        MyUtils.Log("ProcGenManager.ChangeBiome");

        mChunkBiome++;
        if (mChunkBiome > 4)
            mChunkBiome = 4;

        mChunkBiomeAsset++;
        if (mChunkBiomeAsset > 3)
            mChunkBiomeAsset = 0;

        DetermineCollectibles();

        // Add Life here

        mChunkCount = 0;
    }

    private void DetermineCollectibles()
    {
        // Determine in which chunk collectibles go

        mCollectiblesChunks.Clear();

        for (int i = 0; i < mSSO_Collectibles.Value.Count; i++)
        {
            List<int> collectibleChunks = new List<int>();
            List<int> chunkNbs = Enumerable.Range(0, 17).ToList();

            int nbItem = mSSO_Collectibles.Value[i].pCountPerBiome[mChunkBiome];
            for (int j = 0; j < nbItem; j++)
            {
                int rnd = UnityEngine.Random.Range(0, chunkNbs.Count);
                collectibleChunks.Add(chunkNbs[rnd]);
                chunkNbs.RemoveAt(rnd);
            }

            mCollectiblesChunks.Add(collectibleChunks);

//            MyUtils.LogList(collectibleChunks);
        }
    }

    private void OnDistanceChanged(int distance)
    {
        if (distance > 0 && distance % 30 == 0)
        {
            // Deactivate passed chunk(s) 
            DeactivateChunk((float)distance - 30);
            // Generate new one
            GenerateChunk();
        }
    }
}
