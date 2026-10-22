using LitMotion;
using LitMotion.Extensions;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("RSE")]
    [SerializeField] private RSE_StartTutorial mRSE_StartTutorial;
    [SerializeField] private RSE_StartGame mRSE_StartGame;
    [SerializeField] private RSE_GameOver mRSE_GameOver;
    [SerializeField] private RSE_Pause mRSE_Pause;
    [SerializeField] private RSE_LoadScene mRSE_LoadScene;

    [Header("RSO")]
    [SerializeField] private RSO_InputSource mRSO_InputSource;
    [SerializeField] private RSO_Tutorial mRSO_Tutorial;
    [SerializeField] private RSO_Name mRSO_Name;
    [SerializeField] private RSO_Distance mRSO_Distance;
    [SerializeField] private RSO_Biome mRSO_Biome;
    [SerializeField] private RSO_Seed mRSO_Seed;
    [SerializeField] private RSO_Life mRSO_Life;
    [SerializeField] private RSO_Life mRSO_LifeMax;
    [SerializeField] private RSO_AlcoholLevel mRSO_AlcoholLevel;

    [Header("Inputs")]
    [SerializeField] private InputActionReference mPauseInput;
    [SerializeField] private InputActionReference mReturnInput;

    [Header("Elements")]
    [SerializeField] private EventSystem mEventSystem;

    [Header("Info")]
    [SerializeField] private TextMeshProUGUI mScoreText;
    [SerializeField] private TextMeshProUGUI mSeedText;

    [Header("Life")]
    [SerializeField] private GameObject mLife;
    [SerializeField] private Image[] mLifeImages;
    [SerializeField] private Sprite mHeartFullSprite;
    [SerializeField] private Sprite mHeartEmptySprite;

    [Header("Pause")]
    [SerializeField] private GameObject mPausePanel;
    [SerializeField] private GameObject mPauseCloseButton;

    [Header("Game Over")]
    [SerializeField] private GameObject mGameOverPanel;
    [SerializeField] private TextMeshProUGUI mGameOverCause;
    [SerializeField] private TextMeshProUGUI mGameOverScore;
    [SerializeField] private Image mGameOverMedal;
    [SerializeField] private Sprite[] mMedalSprites;
    [SerializeField] private GameObject mGameOverQuit;

    [Header("Alcohol Bar")]
    [SerializeField] private GameObject mAlcoholGo;
    [SerializeField] private Slider mAlcoholBar;
    [SerializeField] private Image mSliderFill;


    private void OnEnable()
    {
        mRSE_StartGame.Event += OnStartGame;
        mRSE_GameOver.Event += OnGameOver;

        mRSO_InputSource.OnChanged += OnInputSourceChange;
        mRSO_Distance.OnChanged += OnDistanceChanged;
        mRSO_Biome.OnChanged += OnBiomeChanged;
        mRSO_Seed.OnChanged += OnSeedChanged;
        mRSO_Life.OnChanged += OnLifeChanged;
        mRSO_AlcoholLevel.OnChanged += OnAlcoholLevelChanged;

        DisableElements(true);
    }

    private void OnDisable()
    {
        mPauseInput.action.performed -= OnPauseInput;
        mReturnInput.action.performed -= OnReturnInput;

        mRSE_StartGame.Event -= OnStartGame;
        mRSE_GameOver.Event -= OnGameOver;

        mRSO_InputSource.OnChanged -= OnInputSourceChange;
        mRSO_Distance.OnChanged -= OnDistanceChanged;
        mRSO_Biome.OnChanged -= OnBiomeChanged;
        mRSO_Seed.OnChanged -= OnSeedChanged;
        mRSO_Life.OnChanged -= OnLifeChanged;
        mRSO_AlcoholLevel.OnChanged -= OnAlcoholLevelChanged;
    }

    private void DisableElements(bool disableLife)
    {
        mPauseInput.action.performed -= OnPauseInput;
        mReturnInput.action.performed -= OnReturnInput;

        mEventSystem.SetSelectedGameObject(null);
        mEventSystem.enabled = false;
        mGameOverPanel.SetActive(false);

        mPausePanel.SetActive(false);

        mScoreText.enabled = false;
        mSeedText.enabled = false;

        if (disableLife)
        {
            mLife.SetActive(false);
            for (int i = 0; i < mLifeImages.Length; i++)
                mLifeImages[i].enabled = false;

            mAlcoholGo.SetActive(false);
        }
    }

    private void OnInputSourceChange(InputSource source)
    {

    }

    private void OnStartGame()
    {
        mScoreText.enabled = true;
        mSeedText.enabled = true;

        mLife.SetActive(true);

        mAlcoholGo.SetActive(true);

        mPauseInput.action.performed += OnPauseInput;
        mReturnInput.action.performed += OnReturnInput;
    }

    /*
     * GAME UI
     */

    private void OnDistanceChanged(int distance)
    {
        mScoreText.text = distance.ToString();
    }

    private void OnSeedChanged(int seed)
    {
        mSeedText.text = seed.ToString();
    }

    private void OnLifeChanged(int life)
    {
        for (int i = 0; i < mRSO_LifeMax.Value; i++)
        {
            if (i < life)
                mLifeImages[i].sprite = mHeartFullSprite;
            else
                mLifeImages[i].sprite = mHeartEmptySprite;

            mLifeImages[i].enabled = true;
        }
    }

    private void OnAlcoholLevelChanged(int alcoholLevel)
    {
        MyUtils.Log("UI Manager. OnAlcoholLevelChanged: " + alcoholLevel);

        float value = (float)alcoholLevel / 100;
        //mAlcoholBar.value = value;

        LMotion.Create(mAlcoholBar.value, value, 0.5f)
            .WithEase(Ease.OutQuad)
            .Bind(x => {
                OnBiomeChanged(mRSO_Biome.Value);
                mAlcoholBar.value = x;
                })
            .AddTo(mAlcoholBar);
    }

    private void OnBiomeChanged(int biome)
    {
        // Update Alcohol bar color
        float value = mAlcoholBar.value;

        switch (mRSO_Biome.Value)
        {
            case 0:
                if (value <= 0.25f)
                    mSliderFill.color = Color.green;
                else if (value <= 0.5f)
                    mSliderFill.color = Color.yellow;
                else if (value <= 0.75f)
                    mSliderFill.color = Color.orange;
                else
                    mSliderFill.color = Color.red;
                break;

            case 1:
                if (value <= 0.25f)
                    mSliderFill.color = Color.yellow;
                else if (value <= 0.5f)
                    mSliderFill.color = Color.orange;
                else
                    mSliderFill.color = Color.red;
                break;

            case 2:
                if (value <= 0.25f)
                    mSliderFill.color = Color.orange;
                else
                    mSliderFill.color = Color.red;
                break;

            case 3:
                mSliderFill.color = Color.red;
                break;

        }
    }

    /*
     * GAME OVER
     */

    private void OnGameOver(GameOverType goType)
    {
        DisableElements(false);

        switch (goType)
        {
            case GameOverType.Life:
                mGameOverCause.text = "Tu t'es blessé !";
                break;
            case GameOverType.Alcohol:
                mGameOverCause.text = "Tu t'es évanoui !";
                break;
            case GameOverType.Police:
                mGameOverCause.text = "Tu t'es fait arrêter !";
                break;
        }

        // Check for high score & save
        mGameOverScore.text = "Score: " + mScoreText.text + " m";
        UpdateBestScores();

        // Show Panel & activate Button
        mGameOverPanel.SetActive(true);
        mEventSystem.enabled = true;
        mEventSystem.SetSelectedGameObject(mGameOverQuit);
    }

    private void UpdateBestScores()
    {
        string goldName = PlayerPrefs.GetString("GoldName", "");
        int goldScore = PlayerPrefs.GetInt("GoldScore", 0);
        string silverName = PlayerPrefs.GetString("SilverName", "");
        int silverScore = PlayerPrefs.GetInt("SilverScore", 0);
        string bronzeName = PlayerPrefs.GetString("BronzeName", "");
        int bronzeScore = PlayerPrefs.GetInt("BronzeScore", 0);

        if (mRSO_Distance.Value > goldScore)
        {
            bronzeName = silverName;
            bronzeScore = silverScore;

            silverName = goldName;
            silverScore = goldScore;

            goldName = mRSO_Name.Value;
            goldScore = mRSO_Distance.Value;

            // Image = medal gold
            mGameOverMedal.sprite = mMedalSprites[0];
            mGameOverMedal.enabled = true;
        }
        else if (mRSO_Distance.Value > silverScore)
        {
            bronzeName = silverName;
            bronzeScore = silverScore;

            silverName = mRSO_Name.Value;
            silverScore = mRSO_Distance.Value;

            // Image = medal silver
            mGameOverMedal.sprite = mMedalSprites[1];
            mGameOverMedal.enabled = true;
        }
        else if (mRSO_Distance.Value > bronzeScore)
        {
            bronzeName = mRSO_Name.Value;
            bronzeScore = mRSO_Distance.Value;

            // Image = medal bronze
            mGameOverMedal.sprite = mMedalSprites[2];
            mGameOverMedal.enabled = true;
        }
        else
        {
            // No Image of medal
            mGameOverMedal.enabled = false;
        }

        PlayerPrefs.SetString("GoldName", goldName);
        PlayerPrefs.SetInt("GoldScore", goldScore);
        PlayerPrefs.SetString("SilverName", silverName);
        PlayerPrefs.SetInt("SilverScore", silverScore);
        PlayerPrefs.SetString("BronzeName", bronzeName);
        PlayerPrefs.SetInt("BronzeScore", bronzeScore);
    }

    public void ReturnToMainMenu()
    {
        DisableElements(true);
        mRSE_LoadScene.Dispatch("MainMenuScene");
    }


    /*
     * PAUSE
     */

    private void OnPauseInput(InputAction.CallbackContext ctx)
    {
        MyUtils.Log("UIManager.OnOpenMenu: " + mPausePanel.activeInHierarchy);

        if (mPausePanel.activeInHierarchy)
        {
            ClosePauseMenu();
        }
        else
        {
            OpenPauseMenu();
        }
    }

    private void OnReturnInput(InputAction.CallbackContext ctx)
    {
        MyUtils.Log("UIManager.OnCloseMenu");

        if (mPausePanel.activeInHierarchy)
        {
            ClosePauseMenu();
        }
    }

    private void OpenPauseMenu()
    {
        mPausePanel.SetActive(true);
        mEventSystem.SetSelectedGameObject(mPauseCloseButton);
        mEventSystem.enabled = true;

        mRSE_Pause.Dispatch(true);
    }

    public void ClosePauseMenu()
    {
        mEventSystem.SetSelectedGameObject(null);
        mEventSystem.enabled = false;
        mPausePanel.SetActive(false);

        mRSE_Pause.Dispatch(false);
    }
}
