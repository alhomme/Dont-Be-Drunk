using UnityEngine;

public class SpeedManager : MonoBehaviour
{
    [Header("RSO")]
    [SerializeField] private RSO_Speed mRSO_ForwardSpeed;
    [SerializeField] private RSO_Speed mRSO_LateralSpeed;

    [Header("RSE")]
    [SerializeField] private RSE_StartGame mRSE_StartGame;


    [Header("Speed Values")]
    [SerializeField] private float mMinSpeed = 3f;
    [SerializeField] private float mMaxForwardSpeed = 20f;
    [SerializeField] private float mIncFowardSpeed = 0.05f;
    [SerializeField] private float mMaxLateralSpeed = 10f;
    [SerializeField] private float mIncLateralSpeed = 0.02f;

    private float countTime;

    private void OnEnable()
    {
        //MyUtils.Log("SpeedManager.OnEnable", false);

        mRSE_StartGame.Event += OnStartGame;

        mRSO_ForwardSpeed.Value = mMinSpeed;
        mRSO_LateralSpeed.Value = mMinSpeed;
        countTime = -1f;
    }

    private void OnDisable()
    {
        mRSE_StartGame.Event -= OnStartGame;
        countTime = -1f;
    }

    private void OnStartGame()
    {
        mRSO_ForwardSpeed.Value = mMinSpeed;
        mRSO_LateralSpeed.Value = mMinSpeed;
        countTime = 0f;
    }

    private void FixedUpdate()
    {
        if (countTime < 0f)
            return;

        countTime += Time.deltaTime;

        if (countTime >= 1f)
        {
            if (mRSO_ForwardSpeed.Value < mMaxForwardSpeed)
                mRSO_ForwardSpeed.Value += mIncFowardSpeed;

            if (mRSO_LateralSpeed.Value < mMaxLateralSpeed)
                mRSO_LateralSpeed.Value += mIncLateralSpeed;

            countTime = 0f;
        }
    }
}
