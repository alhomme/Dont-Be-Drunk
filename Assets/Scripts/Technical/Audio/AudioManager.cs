using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("SSO")]
    [SerializeField] private SSO_Audio mSSO_Music;
    [SerializeField] private SSO_Audio mSSO_SFX;

    [Header("RSE")]
    [SerializeField] private RSE_PlayMusic mRSE_PlayMusic;
    [SerializeField] private RSE_PlaySFX mRSE_PlaySFX;

    [Header("Audio Tools")]
    [SerializeField] private AudioMixer mAudioMixer;
    [SerializeField] private AudioSource mAudioSourceMusic1;
    [SerializeField] private AudioSource mAudioSourceMusic2;
    [SerializeField] private AudioSource mAudioSourceSFX;

    private AudioSource mActiveAS;
    private AudioSource mInactiveAS;

    private void OnEnable()
    {
        mRSE_PlayMusic.Event += OnPlayMusic;
        mRSE_PlaySFX.Event += OnPlaySFX;

        mActiveAS = mAudioSourceMusic1;
        mInactiveAS = mAudioSourceMusic2;
    }

    private void OnDisable()
    {
        mRSE_PlayMusic.Event -= OnPlayMusic;
        mRSE_PlaySFX.Event -= OnPlaySFX;
    }

    private void OnPlayMusic(string music)
    {
        MyUtils.Log("OnPlayMusic: " + music);

        if (music == "")
        {
            mActiveAS.Stop();            
            mActiveAS.clip = null;
        }
        else if (music == "Pause")
        {
            mActiveAS.Pause();
        }
        else if (music == "UnPause")
        {
            mActiveAS.UnPause();
        }
        else
        {
            AudioClip clip = mSSO_Music.Value[music];

            if (clip != null && mActiveAS.clip != clip)
            {
                if (mActiveAS.isPlaying)
                {
                    // Transition fade between musics

                    mInactiveAS.clip = clip;
                    mInactiveAS.volume = 0f;
                    mInactiveAS.Play();

                    LMotion.Create(1f, 0f, 2f).BindToVolume(mActiveAS);

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
                else
                {
                    mActiveAS.clip = clip;
                    mActiveAS.volume = 1f;
                    mActiveAS.Play();
                }
            }
        }
    }

    private void OnPlaySFX(string sfx)
    {
        MyUtils.Log("OnPlaySFX: " + sfx);

        if (sfx == "")
        {
            mAudioSourceSFX.Stop();
        }
        else
        {
            AudioClip clip = mSSO_SFX.Value[sfx];
            if (clip != null)
            {
//                mAudioSourceSFX.clip = clip;
                mAudioSourceSFX.PlayOneShot(clip);
            }
        }
    }
}
