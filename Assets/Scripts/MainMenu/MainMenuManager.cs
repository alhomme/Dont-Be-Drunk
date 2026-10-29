using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("RSE")]
    [SerializeField] private RSE_LoadScene mRSE_LoadScene;
    [SerializeField] private RSE_PlaySFX mRSE_PlaySFX;
    [SerializeField] private RSE_MainMenu mRSE_MainMenu;
    [SerializeField] private RSE_StartTutorial mRSE_StartTutorial;

    [Header("RSO")]
    [SerializeField] private RSO_Name mRSO_Name;
    [SerializeField] private RSO_SaveData mRSO_SaveData;

    [Header("Elements")]
    [Header("Name Input")]
    [SerializeField] private TMP_InputField mNameInputField;

    [Header("Scores")]
    [SerializeField] private TextMeshProUGUI mGoldScore;
    [SerializeField] private TextMeshProUGUI mSilverScore;
    [SerializeField] private TextMeshProUGUI mBronzeScore;

    private void OnEnable()
    {
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

    private void Start()
    {
        mRSE_MainMenu.Dispatch();
    }

    public void OnStartClicked()
    {
        MyUtils.Log("OnStartClicked: " + mNameInputField.text);

        // Store Name in RSO
        if (mNameInputField.text == "")
            mRSO_Name.Value = "Inconnu";
        else
            mRSO_Name.Value = mNameInputField.text;

        // Stop Menu music
        mRSE_PlaySFX.Dispatch("UI_Start");

        // RSE LoadScene
        mRSE_LoadScene.Dispatch("GameScene");
    }

    public void OnTutorialClicked()
    {
        MyUtils.Log("OnTutorialClicked");
        mRSE_PlaySFX.Dispatch("UI_Confirm");
    }

    public void OnExitClicked()
    {
        MyUtils.Log("OnExitClicked");
        Application.Quit();
    }
}
