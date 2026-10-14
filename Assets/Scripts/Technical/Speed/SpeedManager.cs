using UnityEngine;

public class SpeedManager : MonoBehaviour
{
    [Header("RSO")]
    [SerializeField] private RSO_Speed mRSO_ForwardSpeed;
    [SerializeField] private RSO_Speed mRSO_LateralSpeed;

    private float countTime;

    public float pMinSpeed = 3f;
    public float pMaxForwardSpeed = 20f;
    public float pMaxLateralSpeed = 10f;

    private void OnEnable()
    {
        MyUtils.Log("SpeedManager.OnEnable", false);

        mRSO_ForwardSpeed.Value = pMinSpeed;
        mRSO_LateralSpeed.Value = pMinSpeed;
        countTime = 0f;
    }

    private void FixedUpdate()
    {
        countTime += Time.deltaTime;

        if (countTime >= 1f)
        {
            if (mRSO_ForwardSpeed.Value < pMaxForwardSpeed)
                mRSO_ForwardSpeed.Value += 0.05f;

            if (mRSO_LateralSpeed.Value < pMaxLateralSpeed)
                mRSO_LateralSpeed.Value += 0.02f;

            MyUtils.Log("SpeedManager: One second", false);
            MyUtils.Log("SpeedManager: Forward speed = " + mRSO_ForwardSpeed.Value, false);
            MyUtils.Log("SpeedManager: Lateral speed = " + mRSO_LateralSpeed.Value, false);

            countTime = 0f;
        }
    }
}
