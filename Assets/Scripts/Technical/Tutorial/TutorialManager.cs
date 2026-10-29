using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [Header("RSO")]
    [SerializeField] private RSO_InputSource mRSO_InputSource;
    [SerializeField] private RSO_SaveData mRSO_SaveData;

    [Header("RSE")]
    [SerializeField] private RSE_StartTutorial mRSE_StartTutorial;
    [SerializeField] private RSE_EndTutorial mRSE_EndTutorial;
    [SerializeField] private RSE_Save mRSE_Save;

    [Header("Inputs")]
    [SerializeField] private InputActionReference mBackInput;
    [SerializeField] private InputActionReference mNextInput;

    [Header("Panels")]
    [SerializeField] private GameObject mBackground;
    [SerializeField] private List<GameObject> mPanels;

    [Header("Commands")]
    [SerializeField] private TextMeshProUGUI mLateralText;
    [SerializeField] private TextMeshProUGUI mJumpText;
    [SerializeField] private TextMeshProUGUI mSlideText;


    private int mCurrentPanel;
    private int mLastPanel;

    private void OnEnable()
    {
        mRSO_InputSource.OnChanged += OnInputSourceChanged;

        mRSE_StartTutorial.Event += StartTutorial;

        DeactivatePanels();

        // Retrieve Tutorial value from SaveData
    }

    private void OnDisable()
    {
        mRSO_InputSource.OnChanged -= OnInputSourceChanged;

        mRSE_StartTutorial.Event -= StartTutorial;

        DeactivatePanels();
    }

    private void DeactivatePanels()
    {
        mBackInput.action.performed -= OnBackInput;
        mNextInput.action.performed -= OnNextInput;

        mBackground.SetActive(false);
        for (int i = 0; i < mPanels.Count; i++)
        {
            mPanels[i].SetActive(false);
        }
    }

    private void StartTutorial()
    {
        mBackInput.action.performed += OnBackInput;
        mNextInput.action.performed += OnNextInput;

        mBackground.SetActive(true);
        mCurrentPanel = 0;
        mLastPanel = mPanels.Count - 1;
        mPanels[mCurrentPanel].SetActive(true);
    }

    public void OnNext()
    {
        mPanels[mCurrentPanel].SetActive(false);
        mCurrentPanel++;
        mPanels[mCurrentPanel].SetActive(true);
    }

    public void OnBack()
    {
        mPanels[mCurrentPanel].SetActive(false);
        mCurrentPanel--;
        mPanels[mCurrentPanel].SetActive(true);
    }


    public void OnValidateButton()
    {
        if (!mRSO_SaveData.Value.pTutorialSeen)
        {
            // Save
            SaveData newSaveData = new SaveData
            {
                pGoldScore = mRSO_SaveData.Value.pGoldScore,
                pSilverScore = mRSO_SaveData.Value.pSilverScore,
                pBronzeScore = mRSO_SaveData.Value.pBronzeScore,
                pTutorialSeen = true
            };

            mRSE_Save.Dispatch(newSaveData);
        }

        DeactivatePanels();
        mRSE_EndTutorial.Dispatch(true);
    }

    public void OnCloseButton()
    {
        DeactivatePanels();
        mRSE_EndTutorial.Dispatch(false);
    }

    private void OnBackInput(InputAction.CallbackContext ctx)
    {
        if (mCurrentPanel == 0)
            OnCloseButton();
        else
            OnBack();
    }

    private void OnNextInput(InputAction.CallbackContext ctx)
    {
        if (mCurrentPanel == mLastPanel)
            OnValidateButton();
        else
            OnNext();
    }

    private void OnInputSourceChanged(InputSource source)
    {
        if (source == InputSource.Keyboard)
        {
            mLateralText.text = "<sprite name=kl> <sprite name=kr>\r\n<sprite name=dl> <sprite name=dr>";
            mJumpText.text = "<sprite name=space> <sprite name=ku> <sprite name=z>";
            mSlideText.text = "<sprite name=shift> <sprite name=kd> <sprite name=s>";
        }
        else if (source == InputSource.Gamepad)
        {
            mLateralText.text = "<sprite name=jl>\r\n<sprite name=q> <sprite name=d>";
            mJumpText.text = "<sprite name=fs>";
            mSlideText.text = "<sprite name=fe>";
        }
    }
}
