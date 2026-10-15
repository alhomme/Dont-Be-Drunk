using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [Header("RSE")]
    [SerializeField] private RSE_GameOver mRSE_GameOver;

    [Header("RSO")]
    [SerializeField] private RSO_Life mRSO_Life;

    private void OnEnable()
    {
        mRSO_Life.OnChanged += OnLifeChanged;
    }

    private void OnDisable()
    {
        mRSO_Life.OnChanged -= OnLifeChanged;
    }

    private void OnLifeChanged(int life)
    {
        if (life == 0)
            mRSE_GameOver.Dispatch(GameOverType.Life);
    }
}
