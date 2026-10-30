using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraManager : MonoBehaviour
{
    [Header("RSO")]
    [SerializeField] private RSO_Player mRSO_Player;
    [SerializeField] private RSO_AlcoholLevel mRSO_AlcoholLevel;

    [Header("RSE")]
    [SerializeField] private RSE_StartGame mRSE_StarGame;
    [SerializeField] private RSE_Pause mRSE_Pause;

    [Header("Camera")]
    [SerializeField] private Volume mCameraVolume;
    [SerializeField] private Transform mCameraTransform;
    [SerializeField] private PositionConstraint mCameraPC;
    private DepthOfField mDepthOfField;

    private Quaternion baseRotation;
    private MotionHandle mCameraMotion;
    private int mAlcoholZone;


    private void OnEnable()
    {
        mRSE_StarGame.Event += OnStartGame;
        mRSE_Pause.Event += OnPause;

        mRSO_AlcoholLevel.OnChanged += OnAlcoholLevelChanged;

        mCameraVolume.profile.TryGet(out mDepthOfField);

        baseRotation = mCameraTransform.localRotation;
    }

    private void OnDisable()
    {
        mRSE_StarGame.Event -= OnStartGame;
        mRSE_Pause.Event -= OnPause;

        mRSO_AlcoholLevel.OnChanged -= OnAlcoholLevelChanged;
    }


    private void OnStartGame()
    {
        // Set Camera Constraint to character
        ConstraintSource cSource = new ConstraintSource { sourceTransform = mRSO_Player.Value.transform, weight = 1f };
        mCameraPC.AddSource(cSource);

        mDepthOfField.focalLength.value = 100f;

        mAlcoholZone = mRSO_AlcoholLevel.Value / 25;
    }

    private void OnPause(bool pauseStatus)
    {
        if (!mCameraMotion.IsActive())
            return;

        if (pauseStatus)
        {
            mCameraMotion.PlaybackSpeed = 0f;
        }
        else
        {
            mCameraMotion.PlaybackSpeed = 1f;
        }
    }

    private void OnAlcoholLevelChanged(int alcoholLevel)
    {
        int newZone = alcoholLevel / 25;

        if (mAlcoholZone == newZone)
            return;

        mCameraMotion.TryCancel();

        switch (newZone)
        {
            case 0:
                mDepthOfField.focalLength.value = 100f;
                break;
            case 1:
                mDepthOfField.focalLength.value = 150f;
                ShakeCamera(2);
                break;
            case 2:
                mDepthOfField.focalLength.value = 200f;
                ShakeCamera(6);
                break;     
            default:
                mDepthOfField.focalLength.value = 250f;
                ShakeCamera(10);
                break;

        }

        mAlcoholZone = newZone;
    }

    private void ShakeCamera(int freq)
    {
        mCameraMotion = LMotion.Punch.Create(0f, 1f, 1f)
            .WithLoops(-1)
            .WithFrequency(freq)
            .WithDampingRatio(0f)
            .Bind(angle =>
            {
                mCameraTransform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, angle);
            });
    }
}
