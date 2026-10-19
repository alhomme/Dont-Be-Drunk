using LitMotion;
using LitMotion.Extensions;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("RSE")]
    [SerializeField] private RSE_StartTutorial mRSE_StartTutorial;
    [SerializeField] private RSE_StartGame mRSE_StartGame;
    [SerializeField] private RSE_GameOver mRSE_GameOver;

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

    [Header("Elements")]
    [Header("Info")]
    [SerializeField] private TextMeshProUGUI mScoreText;
    [SerializeField] private TextMeshProUGUI mSeedText;

    [Header("Life")]
    [SerializeField] private GameObject mLife;
    [SerializeField] private Image[] mLifeImages;
    [SerializeField] private Sprite mHeartFullSprite;
    [SerializeField] private Sprite mHeartEmptySprite;

    [Header("Game Over")]
    [SerializeField] private GameObject mGameOverPanel;
    [SerializeField] private TextMeshProUGUI mGameOverCause;
    [SerializeField] private TextMeshProUGUI mGameOverScore;
    [SerializeField] private TextMeshProUGUI mGameOverReturn;

    [Header("Alcohol Bar")]
    [SerializeField] private GameObject mAlcoholGo;
    [SerializeField] private Slider mAlcoholBar;



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
        mGameOverPanel.SetActive(false);

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
        if (source == InputSource.Keyboard)
        {
            mGameOverReturn.text = "PRESS <sprite name=enter> TO RETURN TO MENU";
        }
        else if (source == InputSource.Gamepad)
        {
            mGameOverReturn.text = "PRESS <sprite name=xa> TO RETURN TO MENU";
        }
    }

    private void OnStartGame()
    {
        mScoreText.enabled = true;
        mSeedText.enabled = true;

        mLife.SetActive(true);

        mAlcoholGo.SetActive(true);
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
            .WithOnComplete(() => OnBiomeChanged(mRSO_Biome.Value))
            .Bind(x => mAlcoholBar.value = x)
            .AddTo(mAlcoholBar);
    }

    private void OnBiomeChanged(int biome)
    {
        // Update Alcohol bar color
        Image sliderFill = mAlcoholBar.fillRect.GetComponent<Image>();
        float value = mAlcoholBar.value;

        switch (mRSO_Biome.Value)
        {
            case 0:
                if (value <= 0.25f)
                    sliderFill.color = Color.green;
                else if (value <= 0.5f)
                    sliderFill.color = Color.yellow;
                else if (value <= 0.75f)
                    sliderFill.color = Color.orange;
                else
                    sliderFill.color = Color.red;
                break;

            case 1:
                if (value <= 0.25f)
                    sliderFill.color = Color.yellow;
                else if (value <= 0.5f)
                    sliderFill.color = Color.orange;
                else
                    sliderFill.color = Color.red;
                break;

            case 2:
                if (value <= 0.25f)
                    sliderFill.color = Color.orange;
                else
                    sliderFill.color = Color.red;
                break;

            case 3:
                sliderFill.color = Color.red;
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
                mGameOverCause.text = "You got hurt !";
                break;
            case GameOverType.Alcohol:
                mGameOverCause.text = "You passed out !";
                break;
            case GameOverType.Police:
                mGameOverCause.text = "You got arrested !";
                break;
        }

        // Check for high score & save

        mGameOverScore.text = "Score: " + mScoreText.text + " m";

        mGameOverPanel.SetActive(true);
    }


}
