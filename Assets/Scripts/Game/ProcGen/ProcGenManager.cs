using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class ProcGenManager : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] private GameObject mCharacterPrefab;

    [Header("Camera")]
    [SerializeField] private PositionConstraint mCameraPC;

    [Header("Pools")]
    [SerializeField] private RoadPool mRoadPool;
    [SerializeField] private ObstaclesPool mObstaclesPool;

    [Header("RSO")]
    [SerializeField] private RSO_Distance mRSO_Distance;
    [SerializeField] private RSO_Biome mRSO_Biome;

    [Header("RSE")]
    [SerializeField] private RSE_StartGame mRSE_StartGame;
    [SerializeField] private RSE_UpdateDistance mRSE_UpdateDistance;

    [Header("SSO")]
    [SerializeField] private SSO_Obstacles mSSO_Obstacles;

    [Header("Seed")]
    public int pSeed;

    private GameObject mCharacterObj;
    private float mChunkPosition;
    private bool mGameStarted = false;

    private List<Vector2> mPositionsAvailable = new List<Vector2>();

    private IEnumerator Start()
    {
        MyUtils.Log("ProcGenManager.Start");

        yield return new WaitUntil(() => mRoadPool.pIsInitialized);
        yield return new WaitUntil(() => mObstaclesPool.pIsInitialized);

        // Create seed
        pSeed = UnityEngine.Random.Range(0, 1000000);
        MyUtils.Log("Seed: " + pSeed);
        UnityEngine.Random.InitState(pSeed);

        mRSE_UpdateDistance.Dispatch(0);

        // Generate base chunks
        mChunkPosition = -15f;
        for (int i = 0; i < mRoadPool.GetAmount(); i++)
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
        //mRSE_StartGame.Dispatch();
        //mGameStarted = true;
    }

    private void OnDisable()
    {
        MyUtils.Log("ProcGenManager.OnDisable", false);
        mGameStarted = false;

        // Deactivate all active gameObjects from pools
        //mRoadPool.DeactivatePool();
    }

    private void FixedUpdate()
    {
        if (!mGameStarted)
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
        int currentBiome = mRSO_Biome.Value;

        // Get Road
        GameObject roadChunk = mRoadPool.GetPooledObject();
        Vector3 roadPosition = new Vector3(0, 0, mChunkPosition);
        roadChunk.transform.position = roadPosition;
        roadChunk.SetActive(true);

        // Generate obstacles at random positions
        mPositionsAvailable.Clear();
        GeneratePositionsAvailable();

        for (int i = 0; i < mSSO_Obstacles.Value.Count; i++)
        {
            int nbObstacle = mSSO_Obstacles.Value[i].mCountPerBiome[currentBiome];
            string nameObstacle = mSSO_Obstacles.Value[i].mTag;
            for (int j = 0; j < nbObstacle; j++)
            {
                Vector3 obstaclePosition = GenerateRandomPosition(mSSO_Obstacles.Value[i].mObj.transform.position.y);
                MyUtils.Log("Obstacle " + nameObstacle + " at position " + obstaclePosition);
                GameObject obstacleObj = mObstaclesPool.GetPooledObjectByName(nameObstacle);
                obstacleObj.transform.position = obstaclePosition;
                obstacleObj.SetActive(true);
            }
        }

        // Generate items at position

        // Generate environment
        
        // Increase chunk position
        mChunkPosition += 30f;
    }

    private void DeactivateChunk()
    {
        // Deactivate road
        List<GameObject> objs = mRoadPool.GetPooledObjectByPosition(mRSO_Distance.Value - 30);
        for (int i = 0; i < objs.Count; i++)
        {
            objs[i].SetActive(false);
        }

        // Deactivate obstacles

        // Deactivate collectibles

        // Deactivate environment
    }

    private void GeneratePositionsAvailable()
    {
        // Create all available positions
        float[] possibleX = { -2f, 0f, 2f };
        float[] possibleZ = { mChunkPosition - 10,
            mChunkPosition - 5,
            mChunkPosition + 0,
            mChunkPosition + 5,
            mChunkPosition + 10,
            mChunkPosition + 15 };

        foreach (float z in possibleZ)
        {
            foreach (float x in possibleX)
            {
                mPositionsAvailable.Add(new Vector2(x, z));
            }
        }
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
