using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelection : MonoBehaviour, ISelectHandler
{
    [SerializeField] private RSE_PlaySFX mRSE_PlaySFX;

    public void OnSelect(BaseEventData eventData)
    {
        mRSE_PlaySFX.Dispatch("UI_Select");
    }
}
