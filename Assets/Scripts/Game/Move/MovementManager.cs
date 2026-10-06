using UnityEngine;
using UnityEngine.InputSystem;
using LitMotion;
using LitMotion.Extensions;

public class MovementManager : MonoBehaviour
{
    [SerializeField] private InputActionReference mMoveInput;
    [SerializeField] private InputActionReference mJumpInput;
    [SerializeField] private InputActionReference mSlideInput;

    [SerializeField] private RSO_Speed mRSO_ForwardSpeed;
    [SerializeField] private RSO_Speed mRSO_LateralSpeed;

    private float countTime;


    public float duration = 1f;
    public float maxRandomDelay = 0.2f;
    public Ease easeType = Ease.OutBounce;
    Vector3 nextPos;


    private void OnEnable()
    {
        mJumpInput.action.performed += OnJump;
        mSlideInput.action.performed += OnSlide;

        countTime = 0f;
    }

    private void OnDisable()
    {
        mJumpInput.action.performed -= OnJump;
        mSlideInput.action.performed -= OnSlide;
    }

    private void Update()
    {
        /* LATERAL MOVEMENT */
        Vector2 stickDirection = mMoveInput.action.ReadValue<Vector2>();
        Vector3 characterDirection = new Vector3(stickDirection.x, 0, 0);
        if (stickDirection.x > 0)
        {
            if (transform.position.x >= 2.5f)
            {
                MyUtils.Log("Right limit reached: " + transform.position.x);
                return;
            }
        }
        else if (stickDirection.x < 0)
        {
            if (transform.position.x <= -2.5f)
            {
                MyUtils.Log("Left limit reached: " + transform.position.x);
                return;
            }
        }
        transform.Translate(characterDirection * mRSO_LateralSpeed.Value * Time.deltaTime);

        /* FORWARD MOVEMENT */
        //        characterDirection = new Vector3(0, 0, 1);
        //        transform.Translate(characterDirection * mRSO_ForwardSpeed.Value * Time.deltaTime);

        countTime += Time.deltaTime;

        if (countTime >= 1f)
        {
            countTime = 0f;

            MoveForward();
        }
    }

    private void MoveForward()
    {
        float nextZ = transform.position.z + mRSO_ForwardSpeed.Value;

        var motionForward = LMotion.Create(transform.position.z, nextZ, 1f)
            .BindToPositionZ(transform);

        var motionUp = LMotion.Create(transform.position.y, transform.position.y + 0.5f, 0.5f)
            .BindToPositionY(transform);

        var motionDown = LMotion.Create(transform.position.y + 0.5f, transform.position.y, 0.5f)
            .BindToPositionY(transform);

        LSequence.Create()
            .Append(motionForward)
            .Join(motionUp)
            .Insert(0.5f, motionDown)
            .Run()
            .AddTo(transform);
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {

    }

    private void OnSlide(InputAction.CallbackContext ctx)
    {

    }
}
