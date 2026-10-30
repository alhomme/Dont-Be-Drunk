using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("RSE")]
    [SerializeField] private RSE_LoadScene mRSE_LoadScene;
    [SerializeField] private RSE_PlaySFX mRSE_PlaySFX;
    [SerializeField] private RSE_MainMenu mRSE_MainMenu;
    [SerializeField] private RSE_StartTutorial mRSE_StartTutorial;
    [SerializeField] private RSE_EndTutorial mRSE_EndTutorial;

    [Header("RSO")]
    [SerializeField] private RSO_Name mRSO_Name;
    [SerializeField] private RSO_SaveData mRSO_SaveData;

    [Header("Elements")]
    [SerializeField] private EventSystem mEventSystem;
    [SerializeField] private GameObject mBackground;
    [SerializeField] private GameObject mContent;
    [Header("Name Input")]
    [SerializeField] private TMP_InputField mNameInputField;
    [Header("Buttons")]
    [SerializeField] private Button mStartButton;
    [SerializeField] private Button mTutorialButton;

    [Header("Scores")]
    [SerializeField] private TextMeshProUGUI mGoldScore;
    [SerializeField] private TextMeshProUGUI mSilverScore;
    [SerializeField] private TextMeshProUGUI mBronzeScore;

    private void OnEnable()
    {
        mRSE_EndTutorial.Event += OnTutorialEnded;

        // Retrieve best scores
        if (mRSO_SaveData.Value.pGoldScore.pScore > 0)
        {
            mGoldScore.text = mRSO_SaveData.Value.pGoldScore.pName
                + " "
                + mRSO_SaveData.Value.pGoldScore.pScore.ToString() + "m";
        }
        else
            mGoldScore.text = "";

        if (mRSO_SaveData.Value.pSilverScore.pScore > 0)
        {
            mSilverScore.text = mRSO_SaveData.Value.pSilverScore.pName
                + " "
                + mRSO_SaveData.Value.pSilverScore.pScore.ToString() + "m";
        }
        else
            mSilverScore.text = "";

        if (mRSO_SaveData.Value.pBronzeScore.pScore > 0)
        {
            mBronzeScore.text = mRSO_SaveData.Value.pBronzeScore.pName
                + " "
                + mRSO_SaveData.Value.pBronzeScore.pScore.ToString() + "m";
        }
        else
            mBronzeScore.text = "";
    }

    private void OnDisable()
    {
        mRSE_EndTutorial.Event -= OnTutorialEnded;
    }

    private void Start()
    {
        mContent.SetActive(true);
        mRSE_MainMenu.Dispatch();
    }

    public void OnStartClicked()
    {
        //MyUtils.Log("OnStartClicked: " + mNameInputField.text);

        // Store Name in RSO
        if (mNameInputField.text == "")
            mRSO_Name.Value = "Inconnu";
        else
            mRSO_Name.Value = mNameInputField.text;


        // Show Tutorial if it's the first time playing
        if (mRSO_SaveData.Value.pTutorialSeen)
        {
            // Stop Menu music
            mRSE_PlaySFX.Dispatch("UI_Start");

            // RSE LoadScene
            mRSE_LoadScene.Dispatch("GameScene");
        }
        else
        {
            mContent.SetActive(false);
            mRSE_StartTutorial.Dispatch(TutorialSource.MainMenuStart);
        }
    }

    public void OnTutorialClicked()
    {
        //MyUtils.Log("OnTutorialClicked");

        mBackground.SetActive(false);
        mContent.SetActive(false);
        mEventSystem.enabled = false;

        mRSE_PlaySFX.Dispatch("UI_Confirm");

        mRSE_StartTutorial.Dispatch(TutorialSource.MainMenuTuto);
    }

    private void OnTutorialEnded(bool status, TutorialSource source)
    {
        mEventSystem.enabled = true;
        mBackground.SetActive(true);
        mContent.SetActive(true);

        if (status && source == TutorialSource.MainMenuStart)
        {
            // Stop Menu music
            mRSE_PlaySFX.Dispatch("UI_Start");

            // RSE LoadScene
            mRSE_LoadScene.Dispatch("GameScene");
        }
    }

    public void OnExitClicked()
    {
        //MyUtils.Log("OnExitClicked");
        Application.Quit();
    }
}
