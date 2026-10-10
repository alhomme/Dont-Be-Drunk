using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("RSE")]
    [SerializeField] private RSE_StartTutorial mRSE_StartTutorial;
    [SerializeField] private RSE_StartGame mRSE_StartGame;
    [SerializeField] private RSE_GameOver mRSE_GameOver;

    [Header("RSO")]
    [SerializeField] private RSO_Tutorial mRSO_Tutorial;
    [SerializeField] private RSO_Name mRSO_Name;
    [SerializeField] private RSO_Distance mRSO_Distance;
    [SerializeField] private RSO_Seed mRSO_Seed;

    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI mScoreText;
    [SerializeField] private TextMeshProUGUI mSeedText;


    private void OnEnable()
    {
        mRSE_StartGame.Event += OnStartGame;
        mRSE_GameOver.Event += OnGameOver;

        mRSO_Distance.OnChanged += OnDistanceChanged;
        mRSO_Seed.OnChanged += OnSeedChanged;
    }

    private void OnDisable()
    {
        mRSE_StartGame.Event -= OnStartGame;
        mRSE_GameOver.Event -= OnGameOver;

        mRSO_Distance.OnChanged -= OnDistanceChanged;
        mRSO_Seed.OnChanged -= OnSeedChanged;
    }

    private void OnDistanceChanged(int distance)
    {
        mScoreText.text = distance.ToString();
        // Add anim
    }

    private void OnSeedChanged(int seed)
    {
        mSeedText.text = seed.ToString();
    }

    private void OnStartGame()
    {
        mScoreText.enabled = true;
        mSeedText.enabled = true;
    }

    private void OnGameOver(GameOverType goType)
    {
        mScoreText.enabled = false;
        mSeedText.enabled = false;
    }
}
