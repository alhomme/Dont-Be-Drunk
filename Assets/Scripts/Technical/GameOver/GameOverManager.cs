using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [Header("RSE")]
    [SerializeField] private RSE_GameOver mRSE_GameOver;
    [SerializeField] private RSE_StartGame mRSE_StartGame;
    [SerializeField] private RSE_Police mRSE_Police;

    [Header("RSO")]
    [SerializeField] private RSO_Biome mRSO_Biome;
    [SerializeField] private RSO_Life mRSO_Life;
    [SerializeField] private RSO_AlcoholLevel mRSO_AlcoholLevel;

    private bool mIsGameOver;

    private void OnEnable()
    {
        mRSE_StartGame.Event += OnStartGame;
        mRSE_Police.Event += OnPolice;
        mRSO_Life.OnChanged += OnLifeChanged;
        mRSO_AlcoholLevel.OnChanged += OnAlcoholLevelChanged;

        mIsGameOver = false;
    }

    private void OnDisable()
    {
        mRSE_StartGame.Event -= OnStartGame;
        mRSE_Police.Event -= OnPolice;
        mRSO_Life.OnChanged -= OnLifeChanged;
        mRSO_AlcoholLevel.OnChanged -= OnAlcoholLevelChanged;
    }

    private void OnStartGame()
    {
        mIsGameOver = false;
    }

    private void OnPolice()
    {
        if (mIsGameOver)
            return;

        // Alcootest
        if ((mRSO_Biome.Value == 0 && mRSO_AlcoholLevel.Value > 75)
            || (mRSO_Biome.Value == 1 && mRSO_AlcoholLevel.Value > 50)
            || (mRSO_Biome.Value == 2 && mRSO_AlcoholLevel.Value > 25)
            || (mRSO_Biome.Value == 3 && mRSO_AlcoholLevel.Value > 0))
        {
            mRSE_GameOver.Dispatch(GameOverType.Police);
            mIsGameOver = true;
        }
    }

    private void OnLifeChanged(int life)
    {
        MyUtils.Log("GameOverManager.OnLifeChanged");

        if (mIsGameOver)
            return;

        if (life == 0)
        {
            mRSE_GameOver.Dispatch(GameOverType.Life);
            mIsGameOver = true;
        }
    }

    private void OnAlcoholLevelChanged(int alcoholLevel)
    {
        if (mIsGameOver)
            return;

        if (alcoholLevel >= 100)
        {
            mRSE_GameOver.Dispatch(GameOverType.Alcohol);
            mIsGameOver = true;
        }
    }
}
