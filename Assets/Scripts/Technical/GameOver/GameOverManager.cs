using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [Header("RSE")]
    [SerializeField] private RSE_GameOver mRSE_GameOver;
    [SerializeField] private RSE_Collision mRSE_Collision;

    [Header("RSO")]
    [SerializeField] private RSO_Biome mRSO_Biome;
    [SerializeField] private RSO_Life mRSO_Life;
    [SerializeField] private RSO_AlcoholLevel mRSO_AlcoholLevel;

    private void OnEnable()
    {
        mRSE_Collision.Event += OnCollision;
        mRSO_Life.OnChanged += OnLifeChanged;
        mRSO_AlcoholLevel.OnChanged += OnAlcoholLevelChanged;
    }

    private void OnDisable()
    {
        mRSE_Collision.Event -= OnCollision;
        mRSO_Life.OnChanged -= OnLifeChanged;
        mRSO_AlcoholLevel.OnChanged -= OnAlcoholLevelChanged;
    }

    private void OnCollision(GameObject go)
    {
        if (go.tag == "Police")
        {
            // Alcootest
            switch (mRSO_Biome.Value)
            {
                case 0:
                    if (mRSO_AlcoholLevel.Value > 75)
                        mRSE_GameOver.Dispatch(GameOverType.Police);
                    break;

                case 1:
                    if (mRSO_AlcoholLevel.Value > 50)
                        mRSE_GameOver.Dispatch(GameOverType.Police);
                    break;

                case 2:
                    if (mRSO_AlcoholLevel.Value > 25)
                        mRSE_GameOver.Dispatch(GameOverType.Police);
                    break;

                case 3:
                    if (mRSO_AlcoholLevel.Value > 0)
                        mRSE_GameOver.Dispatch(GameOverType.Police);
                    break;
            }
        }
    }

    private void OnLifeChanged(int life)
    {
        if (life == 0)
            mRSE_GameOver.Dispatch(GameOverType.Life);
    }

    private void OnAlcoholLevelChanged(int alcoholLevel)
    {
        if (alcoholLevel >= 100)
            mRSE_GameOver.Dispatch(GameOverType.Alcohol);
    }
}
