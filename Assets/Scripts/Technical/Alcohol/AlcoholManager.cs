using UnityEngine;

public class AlcoholManager : MonoBehaviour
{
    [Header("SSO")]
    [SerializeField] private SSO_ItemsAlcohol mSSO_ItemsAlcohol;

    [Header("RSO")]
    [SerializeField] private RSO_AlcoholLevel mRSO_AlcoholLevel;

    [Header("RSE")]
    [SerializeField] private RSE_StartGame mRSE_StartGame;
    [SerializeField] private RSE_Drink mRSE_Drink;
    [SerializeField] private RSE_Eat mRSE_Eat;
    [SerializeField] private RSE_Puke mRSE_Puke;

    [Header("Layers")]
    [SerializeField] private LayerMask mCollectibleLayer;



    private void OnEnable()
    {
        mRSE_StartGame.Event += OnStartGame;
        mRSE_Drink.Event += OnDrink;
        mRSE_Eat.Event += OnEat;
        mRSE_Puke.Event += OnPuke;
    }

    private void OnDisable()
    {
        mRSE_StartGame.Event -= OnStartGame;
        mRSE_Drink.Event -= OnDrink;
        mRSE_Eat.Event -= OnEat;
        mRSE_Puke.Event -= OnPuke;
    }

    private void OnStartGame()
    {
        mRSO_AlcoholLevel.Value = 0;
    }

    private void OnDrink(string drink)
    {
        for (int i = 0; i < mSSO_ItemsAlcohol.Value.pTags.Length; i++)
        {
            if (drink == mSSO_ItemsAlcohol.Value.pTags[i])
            {
                mRSO_AlcoholLevel.Value += mSSO_ItemsAlcohol.Value.pEffects[i] * 10;
                //MyUtils.Log("New Alcohol value: " + mRSO_AlcoholLevel.Value);
                return;
            }
        }
    }

    private void OnEat()
    {
        mRSO_AlcoholLevel.Value -= 20;
        if (mRSO_AlcoholLevel.Value < 0)
            mRSO_AlcoholLevel.Value = 0;
    }

    private void OnPuke()
    {
        mRSO_AlcoholLevel.Value -= 25;
        if (mRSO_AlcoholLevel.Value < 0)
            mRSO_AlcoholLevel.Value = 0;
    }
}
