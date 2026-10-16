using Unity.VisualScripting;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [Header("RSO")]
    [SerializeField] private RSO_Life mRSO_Life;
    [SerializeField] private RSO_Life mRSO_LifeMax;
    [SerializeField] private RSO_Biome mRSO_Biome;

    [Header("RSE")]
    [SerializeField] private RSE_StartGame mRSE_StartGame;
    [SerializeField] private RSE_Collision mRSE_Collision;

    [Header("Layers")]
    [SerializeField] private LayerMask mObstacleLayer;
    [SerializeField] private LayerMask mCollectibleLayer;

    private void OnEnable()
    {
        mRSE_StartGame.Event += OnStartGame;
        mRSE_Collision.Event += OnCollision;
        mRSO_Biome.OnChanged += OnBiomeChanged;
    }

    private void OnDisable()
    {
        mRSE_StartGame.Event -= OnStartGame;
        mRSE_Collision.Event -= OnCollision;
        mRSO_Biome.OnChanged -= OnBiomeChanged;
    }

    private void OnStartGame()
    {
        mRSO_LifeMax.Value = 3;
        mRSO_Life.Value = mRSO_LifeMax.Value;
    }

    private void OnBiomeChanged(int biome)
    {
        if (mRSO_LifeMax.Value != (biome + 3))
        {
            mRSO_LifeMax.Value = 3 + biome;
            mRSO_Life.Value++;
        }
    }

    private void OnCollision(GameObject go)
    {
        if ((mObstacleLayer & (1 << go.layer)) > 0)
        {
            MyUtils.Log("Collision with Obstacle");
            mRSO_Life.Value--;
            return;
        }

        if ((mCollectibleLayer & (1 << go.layer)) > 0)
        {
            if (go.tag == "Heart")
            {
                MyUtils.Log("Collision with Collectible Heart");
                if (mRSO_Life.Value < mRSO_LifeMax.Value)
                    mRSO_Life.Value++;
            }
            return;
        }
    }
}
