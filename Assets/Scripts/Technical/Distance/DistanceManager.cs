using UnityEngine;

public class DistanceManager : MonoBehaviour
{
    [Header("RSO")]
    [SerializeField] private RSO_Distance mRSO_Distance;
    [SerializeField] private RSO_Biome mRSO_Biome;

    [Header("RSE")]
    [SerializeField] private RSE_UpdateDistance mRSE_UpdateDistance;

    private void OnEnable()
    {
        mRSO_Distance.Value = 0;
        mRSO_Biome.Value = 0;

        mRSE_UpdateDistance.Event += OnUpdateDistance;
    }

    private void OnDisable()
    {
        mRSE_UpdateDistance.Event -= OnUpdateDistance;
    }

    private void OnUpdateDistance(int newDistance)
    {
        mRSO_Distance.Value = newDistance;

        if (newDistance == 0)
        {
            mRSO_Biome.Value = 0;
        }
        else if (newDistance % 510 == 0)
        {
            mRSO_Biome.Value += 1;
        }
    }
}
