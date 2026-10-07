using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("SSO")]
    [SerializeField] private SSO_Audio mSSO_Music;
    [SerializeField] private SSO_Audio mSSO_SFX;

    [Header("RSE")]
    [SerializeField] private RSE_PlayMusic mRSE_PlayMusic;
    [SerializeField] private RSE_PlaySFX mRSE_PlaySFX;

}
