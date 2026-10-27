using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("SSO")]
    [SerializeField] private SSO_Audio mSSO_Music;
    [SerializeField] private SSO_Audio mSSO_SFX;

    [Header("RSO")]
    [SerializeField] private RSO_Biome mRSO_Biome;

    [Header("RSE")]
    [SerializeField] private RSE_PlaySFX mRSE_PlaySFX;
    [SerializeField] private RSE_LoadScene mRSE_LoadScene;
    [SerializeField] private RSE_MainMenu mRSE_MainMenu;
    [SerializeField] private RSE_StartGame mRSE_StartGame;
    [SerializeField] private RSE_Pause mRSE_Pause;
    [SerializeField] private RSE_GameOver mRSE_GameOver;
    [SerializeField] private RSE_Drink mRSE_Drink;
    [SerializeField] private RSE_Eat mRSE_Eat;
    [SerializeField] private RSE_Heart mRSE_Heart;
    [SerializeField] private RSE_Hit mRSE_Hit;
    [SerializeField] private RSE_Puke mRSE_Puke;

    [Header("Audio Tools")]
    [SerializeField] private AudioMixer mAudioMixer;
    [SerializeField] private AudioSource mAudioSourceMusic1;
    [SerializeField] private AudioSource mAudioSourceMusic2;
    [SerializeField] private AudioSource mAudioSourceSFX;

    private AudioSource mActiveAS;
    private AudioSource mInactiveAS;

    private void OnEnable()
    {
        mRSO_Biome.OnChanged += OnBiomeChanged;

        mRSE_PlaySFX.Event += PlaySFX;

        mRSE_LoadScene.Event += OnLoadScene;
        mRSE_MainMenu.Event += OnMainMenu;
        mRSE_StartGame.Event += OnStartGame;
        mRSE_Pause.Event += OnPause;
        mRSE_GameOver.Event += OnGameOver;
        mRSE_Drink.Event += OnDrink;
        mRSE_Eat.Event += OnEat;
        mRSE_Heart.Event += OnHeart;
        mRSE_Hit.Event += OnHit;
        mRSE_Puke.Event += OnPuke;

        mActiveAS = mAudioSourceMusic1;
        mInactiveAS = mAudioSourceMusic2;
    }

    private void OnDisable()
    {
        mRSO_Biome.OnChanged -= OnBiomeChanged;

        mRSE_PlaySFX.Event -= PlaySFX;

        mRSE_LoadScene.Event -= OnLoadScene;
        mRSE_MainMenu.Event -= OnMainMenu;
        mRSE_StartGame.Event -= OnStartGame;
        mRSE_Pause.Event -= OnPause;
        mRSE_GameOver.Event -= OnGameOver;
        mRSE_Drink.Event -= OnDrink;
        mRSE_Eat.Event -= OnEat;
        mRSE_Heart.Event -= OnHeart;
        mRSE_Hit.Event -= OnHit;
        mRSE_Puke.Event -= OnPuke;
    }

    /*
     * 
     */

    private void StopMusicFade()
    {
        if (!mActiveAS.isPlaying)
            return;

        // Stop with fade
        LMotion.Create(1f, 0f, 0.5f)
            .WithOnComplete(() => mActiveAS.Stop())
            .BindToVolume(mActiveAS);
    }

    private void PlayMusicFade(string musicName)
    {
        AudioClip clip = mSSO_Music.Value[musicName];

        if (clip == null)
        {
            MyUtils.LogWarning("AudioManager: Music " + musicName + "does not exist.");
            return;
        }

        // Start with fade
        mActiveAS.clip = clip;
        mActiveAS.volume = 0f;
        mActiveAS.Play();

        LMotion.Create(0f, 1f, 1f)
            .BindToVolume(mActiveAS);
    }

    private void StopMusic()
    {
        mActiveAS.Stop();
        mActiveAS.clip = null;
    }

    private void PlayMusic(string musicName)
    {
        AudioClip clip = mSSO_Music.Value[musicName];

        if (clip == null)
        {
            MyUtils.LogWarning("AudioManager: Music " + musicName + "does not exist.");
            return;
        }

        mActiveAS.clip = clip;
        mActiveAS.volume = 1f;
        mActiveAS.Play();
    }

    private void MusicTransition(AudioClip clip)
    {
        // Transition fade between musics
        mInactiveAS.clip = clip;
        mInactiveAS.volume = 0f;
        mInactiveAS.Play();

        LSequence.Create()
            .Append(LMotion.Create(1f, 0f, 2f).BindToVolume(mActiveAS))
            .Join(LMotion.Create(0f, 1f, 2f)
                .WithOnComplete(() =>
                {
                    mActiveAS.Stop();
                    AudioSource tmpAS = mActiveAS;
                    mActiveAS = mInactiveAS;
                    mInactiveAS = tmpAS;
                })
                .BindToVolume(mInactiveAS))
            .Run();
    }

    /*
     * 
     */

    private void StopSFX()
    {
        mAudioSourceSFX.Stop();
    }

    private void PlaySFX(string sfxName)
    {
        AudioClip clip = mSSO_SFX.Value[sfxName];

        if (clip == null)
        {
            MyUtils.LogWarning("AudioManager: SFX " + sfxName + "does not exist.");
            return;
        }

        mAudioSourceSFX.PlayOneShot(clip);
    }

    /*
     * 
     */


    private void OnLoadScene(string scene)
    {
        StopMusicFade();
    }


    private void OnMainMenu()
    {
        MyUtils.Log("AudioManager. OnMainMenu");
        PlayMusic("MainMenu");
    }


    private void OnBiomeChanged(int biome)
    {
        if (biome == 0)
            return;

        AudioClip clip = mSSO_Music.Value["Biome" + biome.ToString()];

        if (clip != null && mActiveAS.clip != clip)
        {
            MusicTransition(clip);
        }
    }


    private void OnStartGame()
    {
        PlayMusicFade("Biome0");
    }


    private void OnPause(bool pause)
    {
        if (pause)
        {
            mActiveAS.Pause();
        }
        else
        {
            PlaySFX("UI_Cancel");
            mActiveAS.UnPause();
        }
    }


    private void OnGameOver(GameOverType goType)
    {
        StopMusic();
    }


    private void OnDrink(string drink)
    {
        PlaySFX("Game_Drink");
    }


    private void OnEat()
    {
        PlaySFX("Game_Stand");
    }


    private void OnHeart()
    {
        PlaySFX("Game_Heart");
    }


    private void OnHit()
    {
        PlaySFX("Game_Hit");
    }


    private void OnPuke()
    {
        PlaySFX("Game_Puke");
    }
}
