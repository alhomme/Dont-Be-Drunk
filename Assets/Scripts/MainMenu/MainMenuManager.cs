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
    }

    private void OnDisable()
    {
        
    }

    public void OnStartClicked()
    {
        MyUtils.Log("OnStartClicked: " + mNameInputField.text);

        // Store Name in RSO

        // Stop Menu music

        // RSE LoadScene
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
