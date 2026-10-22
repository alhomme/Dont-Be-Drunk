using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("RSE")]
    [SerializeField] private RSE_LoadScene mRSE_LoadScene;
    [SerializeField] private RSE_PlayMusic mRSE_PlayMusic;
    [SerializeField] private RSE_StartTutorial mRSE_StartTutorial;

    [Header("RSO")]
    [SerializeField] private RSO_Name mRSO_Name;
    [SerializeField] private RSO_Tutorial mRSO_Tutorial;

    [Header("Elements")]
    [Header("Name Input")]
    [SerializeField] private TMP_InputField mNameInputField;

    [Header("Scores")]
    [SerializeField] private TextMeshProUGUI mGoldScore;
    [SerializeField] private TextMeshProUGUI mSilverScore;
    [SerializeField] private TextMeshProUGUI mBronzeScore;

    private void OnEnable()
    {
        // Play Menu music

        // Retrieve best scores
        string goldName = PlayerPrefs.GetString("GoldName", "");
        if (goldName != "")
        {
            int goldScore = PlayerPrefs.GetInt("GoldScore", 0);
            mGoldScore.text = goldName + " " + goldScore.ToString() + "m";

            string silverName = PlayerPrefs.GetString("SilverName", "");
            if (silverName != "")
            {
                int silverScore = PlayerPrefs.GetInt("SilverScore", 0);
                mSilverScore.text = silverName + " " + silverScore.ToString() + "m";

                string bronzeName = PlayerPrefs.GetString("BronzeName", "");
                if (bronzeName != "")
                {
                    int bronzeScore = PlayerPrefs.GetInt("BronzeScore", 0);
                    mBronzeScore.text = bronzeName + " " + bronzeScore.ToString() + "m";
                }
            }
        }
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

        // RSE LoadScene
        mRSE_LoadScene.Dispatch("GameScene");
    }

    public void OnTutorialClicked()
    {
        MyUtils.Log("OnTutorialClicked");
    }

    public void OnExitClicked()
    {
        MyUtils.Log("OnExitClicked");
        Application.Quit();
    }

}
