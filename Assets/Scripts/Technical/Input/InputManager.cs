using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("RSO")]
    [SerializeField] private RSO_InputSource mRSO_InputSource;

    [SerializeField] private InputActionReference m_Input;

    private void OnEnable()
    {
        mRSO_InputSource.Value = InputSource.Keyboard;
        m_Input.action.performed += OnInput;
    }

    private void OnDisable()
    {
        m_Input.action.performed -= OnInput;
    }

    private void OnInput(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device is Keyboard)
        {
            mRSO_InputSource.Value = InputSource.Keyboard;
        }
        else if (ctx.control.device is Gamepad)
        {
            mRSO_InputSource.Value = InputSource.Gamepad;
        }
    }
}
