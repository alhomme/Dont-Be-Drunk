using UnityEngine;

public class SpeedManager : MonoBehaviour
{
    [SerializeField] private RSO_Speed mRSO_ForwardSpeed;
    [SerializeField] private RSO_Speed mRSO_LateralSpeed;

    private float countTime;

    private void OnEnable()
    {
        MyUtils.Log("SpeedManager.OnEnable");

        mRSO_ForwardSpeed.Value = 3f;
        mRSO_LateralSpeed.Value = 3f;
        countTime = 0f;
    }

    private void Update()
    {
        countTime += Time.deltaTime;

        if (countTime >= 1f)
        {
            if (mRSO_ForwardSpeed.Value < 20f)
                mRSO_ForwardSpeed.Value += 0.05f;

            if (mRSO_LateralSpeed.Value < 10f)
                mRSO_LateralSpeed.Value += 0.02f;

            MyUtils.Log("SpeedManager: One second");
            MyUtils.Log("SpeedManager: Forward speed = " + mRSO_ForwardSpeed.Value);
            MyUtils.Log("SpeedManager: Lateral speed = " + mRSO_LateralSpeed.Value);

            countTime = 0f;
        }
    }
}
