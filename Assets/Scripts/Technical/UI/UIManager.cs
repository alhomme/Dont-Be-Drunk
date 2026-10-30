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
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UIManager : MonoBehaviour
{
    [Header("RSE")]
    [SerializeField] private RSE_Save mRSE_Save;
    [SerializeField] private RSE_StartTutorial mRSE_StartTutorial;
    [SerializeField] private RSE_EndTutorial mRSE_EndTutorial;
    [SerializeField] private RSE_StartGame mRSE_StartGame;
    [SerializeField] private RSE_GameOver mRSE_GameOver;
    [SerializeField] private RSE_Pause mRSE_Pause;
    [SerializeField] private RSE_LoadScene mRSE_LoadScene;
    [SerializeField] private RSE_PlaySFX mRSE_PlaySFX;
    [SerializeField] private RSE_Puke mRSE_Puke;

    [Header("RSO")]
    [SerializeField] private RSO_SaveData mRSO_SaveData;
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

    [Header("Puke")]
    [SerializeField] private GameObject mPukePanel;
    [SerializeField] private Image[] mPukeImages;

    [Header("Pause")]
    [SerializeField] private GameObject mPausePanel;
    [SerializeField] private GameObject mPauseTutorialButton;
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
        mRSE_EndTutorial.Event += OnTutorialEnded;
        mRSE_StartGame.Event += OnStartGame;
        mRSE_GameOver.Event += OnGameOver;
        mRSE_Puke.Event += OnPuke;

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

        mRSE_EndTutorial.Event -= OnTutorialEnded;
        mRSE_StartGame.Event -= OnStartGame;
        mRSE_GameOver.Event -= OnGameOver;
        mRSE_Puke.Event -= OnPuke;

        mRSO_Distance.OnChanged -= OnDistanceChanged;
        mRSO_Biome.OnChanged -= OnBiomeChanged;
        mRSO_Seed.OnChanged -= OnSeedChanged;
        mRSO_Life.OnChanged -= OnLifeChanged;
        mRSO_AlcoholLevel.OnChanged -= OnAlcoholLevelChanged;
    }

    private void DisableAll()
    {
        DisableEventSystem();
        DisableScore();
        DisableSeed();
        DisableLife();
        DisableAlcoholBar();
        DisableGameOver();
        DisableGoPolice();
        DisableGoAlcohol();
        DisablePause();
        DisablePuke();
    }

    private void DisableEventSystem()
    {
        mEventSystem.SetSelectedGameObject(null);
        mEventSystem.enabled = false;
    }

    private void OnStartGame()
    {
        EnableScore();
        EnableSeed();
        EnableLife();
        EnableAlcoholBar();

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

    private void EnableScore()
    {
        mScoreText.enabled = true;
    }

    private void OnSeedChanged(int seed)
    {
        mSeedText.text = seed.ToString();
    }

    private void DisableSeed()
    {
        mSeedText.enabled = false;
    }

    private void EnableSeed()
    {
        mSeedText.enabled = true;
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
        //for (int i = 0; i < mLifeImages.Length; i++)
        //    mLifeImages[i].enabled = false;
    }

    private void EnableLife()
    {
        mLife.SetActive(true);
    }

    private void OnAlcoholLevelChanged(int alcoholLevel)
    {
        //MyUtils.Log("UI Manager. OnAlcoholLevelChanged: " + alcoholLevel);

        float value = (float)alcoholLevel / 100;

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

    private void EnableAlcoholBar()
    {
        mAlcoholGo.SetActive(true);
    }

    private void UpdateAlcoholBarColor(float value)
    {
        int idx = 3;
        if (value < 0.25f)
            idx = 0;
        else if (value < 0.5f)
            idx = 1;
        else if (value < 0.75f)
            idx = 2;

        mSliderFill.color = mFillColors[mRSO_Biome.Value, idx];
    }

    private void OnBiomeChanged(int biome)
    {
       //MyUtils.Log("UIManager.OnBiomeChanged");

        UpdateAlcoholBarColor(mAlcoholBar.value);

        mBiomeTitleObject.GetComponent<TextMeshProUGUI>().text = mBiomeTitles[biome];

        Vector3 basePosition = mBiomeTitleObject.transform.localPosition;

        LSequence.Create()
            .Append(LMotion.Create(mBiomeTitleObject.transform.localPosition.x, 0f, 1f)
                .WithEase(Ease.OutExpo)
                .BindToLocalPositionX(mBiomeTitleObject.transform))
            .Append(LMotion.Create(0f, mBiomeTitleObject.transform.localPosition.x * -1f, 1f)
                .WithEase(Ease.InExpo)
                .WithOnComplete(() => mBiomeTitleObject.transform.localPosition = basePosition)
                .BindToLocalPositionX(mBiomeTitleObject.transform))
            .Run();
    }

    /*
     * PUKE
     */

    private void OnPuke()
    {
        mPukePanel.SetActive(true);

        // Set random position for 3 images
        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = mPukeImages[i].transform.localPosition;

            pos.y = 100 + (UnityEngine.Random.Range(0, 5) * 50);
            mPukeImages[i].transform.localPosition = pos;
        }

        // Animate down for 1.5 seconds: Wait 0.5 and down 1 ?
        LSequence.Create()
            .AppendInterval(1.5f)
            .Append(LMotion.Create(0f, mPukePanel.transform.localPosition.y - 1080, 0.5f)
                .WithOnComplete(() => DisablePuke())
                .BindToLocalPositionY(mPukePanel.transform))
            .Run();
    }

    private void DisablePuke()
    {
        mPukePanel.transform.localPosition = Vector3.zero;
        mPukePanel.SetActive(false);
    }

    /*
     * GAME OVER
     */

    private void OnGameOver(GameOverType goType)
    {
        //MyUtils.Log("UIManager.OnGameOver: " + goType);

        DisableScore();
        DisablePause();

        // Check for high score & save
        mGameOverScore.text = "Score: " + mScoreText.text + " m";
        UpdateBestScores();

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
        SaveData newSaveData = new SaveData
        {
            pTutorialSeen = mRSO_SaveData.Value.pTutorialSeen
        };

        if (mRSO_Distance.Value > mRSO_SaveData.Value.pGoldScore.pScore)
        {
            newSaveData.pGoldScore.pName = mRSO_Name.Value;
            newSaveData.pGoldScore.pScore = mRSO_Distance.Value;

            newSaveData.pSilverScore.pName = mRSO_SaveData.Value.pGoldScore.pName;
            newSaveData.pSilverScore.pScore = mRSO_SaveData.Value.pGoldScore.pScore;

            newSaveData.pBronzeScore.pName = mRSO_SaveData.Value.pSilverScore.pName;
            newSaveData.pBronzeScore.pScore = mRSO_SaveData.Value.pSilverScore.pScore;

            // Image = medal gold
            mGameOverMedal.sprite = mMedalSprites[0];
            mGameOverMedal.enabled = true;

            mRSE_Save.Dispatch(newSaveData);
        }
        else if (mRSO_Distance.Value > mRSO_SaveData.Value.pSilverScore.pScore)
        {
            newSaveData.pGoldScore.pName = mRSO_SaveData.Value.pGoldScore.pName;
            newSaveData.pGoldScore.pScore = mRSO_SaveData.Value.pGoldScore.pScore;

            newSaveData.pSilverScore.pName = mRSO_Name.Value;
            newSaveData.pSilverScore.pScore = mRSO_Distance.Value;

            newSaveData.pBronzeScore.pName = mRSO_SaveData.Value.pSilverScore.pName;
            newSaveData.pBronzeScore.pScore = mRSO_SaveData.Value.pSilverScore.pScore;

            // Image = medal silver
            mGameOverMedal.sprite = mMedalSprites[1];
            mGameOverMedal.enabled = true;

            mRSE_Save.Dispatch(newSaveData);
        }
        else if (mRSO_Distance.Value > mRSO_SaveData.Value.pBronzeScore.pScore)
        {
            newSaveData.pGoldScore.pName = mRSO_SaveData.Value.pGoldScore.pName;
            newSaveData.pGoldScore.pScore = mRSO_SaveData.Value.pGoldScore.pScore;

            newSaveData.pSilverScore.pName = mRSO_SaveData.Value.pSilverScore.pName;
            newSaveData.pSilverScore.pScore = mRSO_SaveData.Value.pSilverScore.pScore;

            newSaveData.pBronzeScore.pName = mRSO_Name.Value;
            newSaveData.pBronzeScore.pScore = mRSO_Distance.Value;

            // Image = medal bronze
            mGameOverMedal.sprite = mMedalSprites[2];
            mGameOverMedal.enabled = true;

            mRSE_Save.Dispatch(newSaveData);
        }
        else
        {
            // No Image of medal
            mGameOverMedal.enabled = false;
        }
    }

    public void ReturnToMainMenu()
    {
        mRSE_PlaySFX.Dispatch("UI_Confirm");
        mRSE_LoadScene.Dispatch("MainMenuScene");
        DisableAll();
    }


    /*
     * PAUSE
     */

    private void OnPauseInput(InputAction.CallbackContext ctx)
    {
        //MyUtils.Log("UIManager.OnOpenMenu: " + mPausePanel.activeInHierarchy);

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
        //MyUtils.Log("UIManager.OnCloseMenu");

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

    public void TutorialPauseMenu()
    {
        //MyUtils.Log("UIManger.TutorialPauseMenu");

        DisableScore();
        DisableSeed();
        DisableLife();
        DisableAlcoholBar();

        mEventSystem.enabled = false;
        mPausePanel.SetActive(false);

        mRSE_PlaySFX.Dispatch("UI_Confirm");

        mRSE_StartTutorial.Dispatch(TutorialSource.GamePause);
    }

    private void OnTutorialEnded(bool status, TutorialSource source)
    {
        if (source == TutorialSource.GamePause)
        {
            EnableScore();
            EnableSeed();
            EnableLife();
            EnableAlcoholBar();

            mEventSystem.enabled = true;
            mPausePanel.SetActive(true);
        }
    }

    private void DisablePause()
    {
        mPauseInput.action.performed -= OnPauseInput;
        mReturnInput.action.performed -= OnReturnInput;

        mPausePanel.SetActive(false);
        mEventSystem.SetSelectedGameObject(null);
        mEventSystem.enabled = false;
    }
}
