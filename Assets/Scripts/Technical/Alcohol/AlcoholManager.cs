using UnityEngine;

public class AlcoholManager : MonoBehaviour
{
    [Header("SSO")]
    [SerializeField] private SSO_ItemsAlcohol mSSO_ItemsAlcohol;

    [Header("RSO")]
    [SerializeField] private RSO_AlcoholLevel mRSO_AlcoholLevel;

    [Header("RSE")]
    [SerializeField] private RSE_StartGame mRSE_StartGame;
    [SerializeField] private RSE_Collision mRSE_Collision;
    [SerializeField] private RSE_Puke mRSE_Puke;

    [Header("Layers")]
    [SerializeField] private LayerMask mCollectibleLayer;



    private void OnEnable()
    {
        mRSE_StartGame.Event += OnStartGame;
        mRSE_Collision.Event += OnCollision;
    }

    private void OnDisable()
    {
        mRSE_StartGame.Event -= OnStartGame;
        mRSE_Collision.Event -= OnCollision;
    }

    private void OnStartGame()
    {
        mRSO_AlcoholLevel.Value = 0;
    }

    private void OnCollision(GameObject go)
    {
        if ((mSSO_ItemsAlcohol.Value.pLayer & (1 << go.layer)) > 0)
        {
            for (int i = 0; i < mSSO_ItemsAlcohol.Value.pTags.Length; i++)
            {
                if (go.tag == mSSO_ItemsAlcohol.Value.pTags[i])
                {
                    mRSO_AlcoholLevel.Value += mSSO_ItemsAlcohol.Value.pEffects[i] * 10;
                    MyUtils.Log("New Alcohol value: " + mRSO_AlcoholLevel.Value);
                    return;
                }
            }
        }

        MyUtils.Log("AlcoholManager.OnCollision: " + go.tag);

        if ((mCollectibleLayer & (1 << go.layer)) > 0)
        {
            if (go.tag == "Puke")
            {
                MyUtils.Log("AlcoholManager: Collision with Collectible Puke");
                mRSE_Puke.Dispatch();

                mRSO_AlcoholLevel.Value -= 25;
                if (mRSO_AlcoholLevel.Value < 0)
                    mRSO_AlcoholLevel.Value = 0;
                MyUtils.Log("New Alcohol value: " + mRSO_AlcoholLevel.Value);
            }
            return;
        }

        if (go.tag == "Stand")
        {
            mRSO_AlcoholLevel.Value -= 20;
            if (mRSO_AlcoholLevel.Value < 0)
                mRSO_AlcoholLevel.Value = 0;
            MyUtils.Log("New Alcohol value: " + mRSO_AlcoholLevel.Value);
        }
    }
}
