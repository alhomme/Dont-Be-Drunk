using LitMotion;
using LitMotion.Extensions;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UIManager : MonoBehaviour
{
    [Header("RSE")]
    [SerializeField] private RSE_StartTutorial mRSE_StartTutorial;
    [SerializeField] private RSE_StartGame mRSE_StartGame;
    [SerializeField] private RSE_GameOver mRSE_GameOver;
    [SerializeField] private RSE_Pause mRSE_Pause;
    [SerializeField] private RSE_LoadScene mRSE_LoadScene;
    [SerializeField] private RSE_PlayMusic mRSE_PlayMusic;
    [SerializeField] private RSE_PlaySFX mRSE_PlaySFX;

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

    [Header("Biome Title")]
    [SerializeField] private GameObject mBiomeTitleObject;
    [SerializeField] private string[] mBiomeTitles;

    [Header("Pause")]
    [SerializeField] private GameObject mPausePanel;
    [SerializeField] private GameObject mPauseCloseButton;

    [Header("Game Over")]
    [SerializeField] private GameObject mGameOverPanel;
    [SerializeField] private Image mGameOverIcon;
    [SerializeField] private Dictionary<string, Sprite> mGameOverIconSprites;
    [SerializeField] private TextMeshProUGUI mGameOverCause;
    [SerializeField] private TextMeshProUGUI mGameOverScore;
    [SerializeField] private Image mGameOverMedal;
    [SerializeField] private Sprite[] mMedalSprites;
    [SerializeField] private GameObject mGameOverQuit;
    [SerializeField] private GameObject mGoPrisonCell;
    [SerializeField] private GameObject mGoFaintUp;
    [SerializeField] private GameObject mGoFaintDown;


    [Header("Alcohol Bar")]
    [SerializeField] private GameObject mAlcoholGo;
    [SerializeField] private Slider mAlcoholBar;
    [SerializeField] private Image mSliderFill;

    private Color[,] mFillColors = {
        { Color.green, Color.yellow, Color.orange, Color.red },
        { Color.yellow, Color.orange, Color.red, Color.red },
        { Color.orange, Color.red, Color.red, Color.red },
        { Color.red, Color.red, Color.red, Color.red }
    };

    private Vector3 mCellBasePosition = new Vector3(0f, 1080f, 0f);
    private Vector3 mFaintUpBasePosition = new Vector3(0f, 540, 0f);
    private Vector3 mFaintDownBasePosition = new Vector3(0f, -540, 0f);


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

        DisableAll();
    }

    private void OnDisable()
    {
        DisableAll();

        mRSE_StartGame.Event -= OnStartGame;
        mRSE_GameOver.Event -= OnGameOver;

        mRSO_InputSource.OnChanged -= OnInputSourceChange;
        mRSO_Distance.OnChanged -= OnDistanceChanged;
        mRSO_Biome.OnChanged -= OnBiomeChanged;
        mRSO_Seed.OnChanged -= OnSeedChanged;
        mRSO_Life.OnChanged -= OnLifeChanged;
        mRSO_AlcoholLevel.OnChanged -= OnAlcoholLevelChanged;
    }

    private void DisableAll()
    {
        DisableScore();
        DisableSeed();
        DisableLife();
        DisableAlcoholBar();
        DisableGameOver();
        DisableGoPolice();
        DisableGoAlcohol();
        DisablePause();
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

    private void DisableScore()
    {
        mScoreText.enabled = false;
    }

    private void OnSeedChanged(int seed)
    {
        mSeedText.text = seed.ToString();
    }

    private void DisableSeed()
    {
        mSeedText.enabled = false;
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

    private void DisableLife()
    {
        mLife.SetActive(false);
        for (int i = 0; i < mLifeImages.Length; i++)
            mLifeImages[i].enabled = false;
    }

    private void OnAlcoholLevelChanged(int alcoholLevel)
    {
        MyUtils.Log("UI Manager. OnAlcoholLevelChanged: " + alcoholLevel);

        float value = (float)alcoholLevel / 100;
        //mAlcoholBar.value = value;

        LMotion.Create(mAlcoholBar.value, value, 0.5f)
            .WithEase(Ease.OutQuad)
            .Bind(x => {
                UpdateAlcoholBarColor(x);
                mAlcoholBar.value = x;
                })
            .AddTo(mAlcoholBar);
    }

    private void DisableAlcoholBar()
    {
        mAlcoholGo.SetActive(false);
    }

    private void UpdateAlcoholBarColor(float value)
    {
        int idx = 3;
        if (value <= 0.25f)
            idx = 0;
        else if (value <= 0.5f)
            idx = 1;
        else if (value <= 0.75f)
            idx = 2;

        mSliderFill.color = mFillColors[mRSO_Biome.Value, idx];
    }

    private void OnBiomeChanged(int biome)
    {
       MyUtils.Log("UIManager.OnBiomeChanged");

        UpdateAlcoholBarColor(mAlcoholBar.value);

        mBiomeTitleObject.GetComponent<TextMeshProUGUI>().text = mBiomeTitles[biome];

        Vector3 basePosition = mBiomeTitleObject.transform.localPosition;

        LSequence.Create()
            .Append(LMotion.Create(mBiomeTitleObject.transform.localPosition.x, 0f, 1f)
                .WithEase(Ease.OutBack)
                .BindToLocalPositionX(mBiomeTitleObject.transform))
            .AppendInterval(1f)
            .Append(LMotion.Create(0f, mBiomeTitleObject.transform.localPosition.x * -1f, 1f)
                .WithEase(Ease.InBack)
                .WithOnComplete(() => mBiomeTitleObject.transform.localPosition = basePosition)
                .BindToLocalPositionX(mBiomeTitleObject.transform))
            .Run();
    }

    /*
     * GAME OVER
     */

    private void OnGameOver(GameOverType goType)
    {
        DisableScore();
        DisablePause();

        // Check for high score & save
        mGameOverScore.text = "Score: " + mScoreText.text + " m";
        UpdateBestScores();

        mRSE_PlayMusic.Dispatch("");

        switch (goType)
        {
            case GameOverType.Life:
                GameOverLife();
                break;
            case GameOverType.Alcohol:
                GameOverAlcohol();
                break;
            case GameOverType.Police:
                GameOverPolice();
                break;
        }

    }

    private void ShowGameOverPanel()
    {
        // Show Panel & activate Button
        mGameOverPanel.SetActive(true);
        mEventSystem.enabled = true;
        mEventSystem.SetSelectedGameObject(mGameOverQuit);

        float curX = mGameOverPanel.transform.localPosition.x;
        LMotion.Create(curX, 0, 1f)
            .WithEase(Ease.OutElastic)
            .BindToLocalPositionX(mGameOverPanel.transform)
            .AddTo(mGameOverPanel);
    }

    private void DisableGameOver()
    {
        mEventSystem.SetSelectedGameObject(null);
        mEventSystem.enabled = false;
        mGameOverPanel.SetActive(false);
    }

    private void GameOverPolice()
    {
        mGameOverCause.text = "Tu t'es fait arrêter !";
        mGameOverIcon.sprite = mGameOverIconSprites["Police"];

        float curY = mGoPrisonCell.transform.localPosition.y;

        mGoPrisonCell.SetActive(true);

        LMotion.Create(curY, 0, 1f)
            .WithOnComplete(() => {
                ShowGameOverPanel();
                mRSE_PlaySFX.Dispatch("GameOver_Police");
            })
            .BindToLocalPositionY(mGoPrisonCell.transform)
            .AddTo(mGoPrisonCell);   
    }

    private void DisableGoPolice()
    {
        mGoPrisonCell.SetActive(false);
        mGoPrisonCell.transform.localPosition = mCellBasePosition;
    }

    private void GameOverLife()
    {
        mGameOverCause.text = "Tu t'es blessé !";
        mGameOverIcon.sprite = mGameOverIconSprites["Life"];

        mRSE_PlaySFX.Dispatch("GameOver_Life");
        ShowGameOverPanel();
    }

    private void GameOverAlcohol()
    {
        mGameOverCause.text = "Tu t'es évanoui !";
        mGameOverIcon.sprite = mGameOverIconSprites["Alcohol"];

        mGoFaintUp.SetActive(true);
        mGoFaintDown.SetActive(true);

        LSequence.Create()
            .Append(LMotion.Create(mFaintUpBasePosition.y, 0f, 0.5f)
                .BindToLocalPositionY(mGoFaintUp.transform))
            .Join(LMotion.Create(mFaintDownBasePosition.y, 0f, 0.5f)
                .BindToLocalPositionY(mGoFaintDown.transform))

            .Append(LMotion.Create(0f, mFaintUpBasePosition.y / 2, 1f)
                .BindToLocalPositionY(mGoFaintUp.transform))
            .Join(LMotion.Create(0f, mFaintDownBasePosition.y / 2, 1f)
                .BindToLocalPositionY(mGoFaintDown.transform))

            .Append(LMotion.Create(mFaintUpBasePosition.y / 2, 0f, 0.25f)
                .BindToLocalPositionY(mGoFaintUp.transform))
            .Join(LMotion.Create(mFaintDownBasePosition.y / 2, 0f, 0.25f)
                .WithOnComplete(() => { 
                    ShowGameOverPanel();
                    mRSE_PlaySFX.Dispatch("GameOver_Alcohol");
                })
                .BindToLocalPositionY(mGoFaintDown.transform))
            .Run();
    }


    private void DisableGoAlcohol()
    {
        mGoFaintUp.transform.localPosition = mFaintUpBasePosition;
        mGoFaintUp.SetActive(false);

        mGoFaintDown.transform.localPosition = mFaintDownBasePosition;
        mGoFaintDown.SetActive(false);
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
        DisableAll();
        mRSE_PlaySFX.Dispatch("UI_Confirm");
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
        mRSE_PlayMusic.Dispatch("Pause");

        mPausePanel.SetActive(true);
        mEventSystem.SetSelectedGameObject(mPauseCloseButton);
        mEventSystem.enabled = true;

        mRSE_Pause.Dispatch(true);
    }

    public void ClosePauseMenu()
    {
        mRSE_PlaySFX.Dispatch("UI_Cancel");
        mRSE_PlayMusic.Dispatch("UnPause");

        mEventSystem.SetSelectedGameObject(null);
        mEventSystem.enabled = false;
        mPausePanel.SetActive(false);

        mRSE_Pause.Dispatch(false);
    }

    private void DisablePause()
    {
        mPauseInput.action.performed -= OnPauseInput;
        mReturnInput.action.performed -= OnReturnInput;

        mEventSystem.SetSelectedGameObject(null);
        mEventSystem.enabled = false;
    }
}
