using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("RSO")]
    [SerializeField] private RSO_Tutorial mRSO_Tutorial;
    [SerializeField] private RSO_InputSource mRSO_InputSource;

    [Header("RSE")]
    [SerializeField] private RSE_StartTutorial mRSE_StartTutorial;

    [Header("Panels")]
    [SerializeField] private List<GameObject> mPanels;

    [Header("Commands")]
    [SerializeField] private TextMeshProUGUI mLateralText;
    [SerializeField] private TextMeshProUGUI mJumpText;
    [SerializeField] private TextMeshProUGUI mSlideText;


    private int mCurrentPanel;

    private void OnEnable()
    {
        mRSO_InputSource.OnChanged += OnInputSourceChanged;

        mRSE_StartTutorial.Event += StartTutorial;
    }

    private void OnDisable()
    {
        mRSO_InputSource.OnChanged -= OnInputSourceChanged;

        mRSE_StartTutorial.Event -= StartTutorial;
    }

    private void StartTutorial()
    {
        mCurrentPanel = 0;
    }

    private void OnNext()
    {
        mCurrentPanel++;
    }

    private void OnBack()
    {
        mCurrentPanel--;
    }


    private void OnValidateButton()
    {
        mRSO_Tutorial.Value = true;
    }

    private void OnCloseButton()
    {

    }

    private void OnInputSourceChanged(InputSource source)
    {
        if (source == InputSource.Keyboard)
        {
            mLateralText.text = "<sprite name=kl> <sprite name=kr>\r\n<sprite name=q> <sprite name=d>";
            mJumpText.text = "<sprite name=space> <sprite name=ku> <sprite name=z>";
            mSlideText.text = "<sprite name=shift> <sprite name=kd> <sprite name=s>";
        }
        else if (source == InputSource.Gamepad)
        {

        }
    }
}
