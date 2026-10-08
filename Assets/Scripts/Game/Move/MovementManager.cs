using UnityEngine;
using UnityEngine.InputSystem;
using LitMotion;
using LitMotion.Extensions;

public class MovementManager : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference mMoveInput;
    [SerializeField] private InputActionReference mJumpInput;
    [SerializeField] private InputActionReference mSlideInput;
    
    [Header("RSO")]
    [SerializeField] private RSO_Speed mRSO_ForwardSpeed;
    [SerializeField] private RSO_Speed mRSO_LateralSpeed;

    [Header("RSE")]
    [SerializeField] private RSE_StartGame mRSE_StartGame;

    [Header("Bounce")]
    [SerializeField] private float mBounceForce = 2f;

    [Header("Jump")]
    [SerializeField] private float mJumpHeight = 2f;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask mGroundLayer;
    [SerializeField] private float MGroundDistance = 0.15f;

    [Header("Gravity")]
    [SerializeField] private float mGravityBase = 15;
    [SerializeField] private float mGravityMax = 100f;

    [Header("Landing")]
    [SerializeField] private Vector3 mScaleSquashed = new Vector3(0.65f, 0.75f, 0.65f);
    [SerializeField] private float mSquashDuration = 0.08f;
    [SerializeField] private Ease mSquashEase = Ease.OutQuad;


    [Header("Slide")]
    [SerializeField] private Vector3 mScaleSlide = new Vector3(0.65f, 0.25f, 0.65f);
    [SerializeField] private float mSlideDurationBase = 1.2f;
    [SerializeField] private float mSlideDurationMax = 0.5f;
    [SerializeField] private Ease mSlideEase = Ease.OutQuad;


    private bool mGameStarted;
    private Rigidbody mRb;
    private Vector3 mScaleBase;
    //private bool mIsGrounded;
    //private bool mWasGrounded;
    private bool mIsJumping;
    private bool mIsSliding;

    [Header("Speed")]
    public float pMinSpeed = 3f;
    public float pMaxForwardSpeed = 20f;
    public float pMaxLateralSpeed = 10f;


    private void Awake()
    {
        mRb = GetComponent<Rigidbody>();
        mRb.constraints = RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationY
            | RigidbodyConstraints.FreezeRotationZ;

        mScaleBase = transform.localScale;

        mGameStarted = false;
        mIsJumping = false;
        mIsSliding = false;
    }

    private void OnEnable()
    {
        mJumpInput.action.performed += OnJump;
        mSlideInput.action.performed += OnSlide;
        mRSE_StartGame.Event += OnStartGame;
    }

    private void OnDisable()
    {
        mJumpInput.action.performed -= OnJump;
        mSlideInput.action.performed -= OnSlide;
        mRSE_StartGame.Event -= OnStartGame;
    }

    private void FixedUpdate()
    {
        if (!mGameStarted)
            return;

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
        MoveForward();
        ApplyGravity();
    }

    private bool IsGrounded()
    {
        Collider col = GetComponent<Collider>();
        Vector3 origin = col.bounds.center;
        float distance = col.bounds.extents.y + MGroundDistance;

        return Physics.Raycast(origin, 
            Vector3.down,
            distance,
            mGroundLayer,
            QueryTriggerInteraction.Ignore);
    }

    private void ApplyGravity()
    {
        if (IsGrounded())
            return;

        float targetGravity = GetJumpGravity();

        float extraGravity = targetGravity - Mathf.Abs(Physics.gravity.y);

        if (extraGravity > 0f)
            mRb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration);

    }

    private void MoveForward()
    {
        Vector3 newVelocity = mRb.linearVelocity;
        newVelocity.z = mRSO_ForwardSpeed.Value;
        mRb.linearVelocity = newVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        MyUtils.Log("MovementManager.OnCollisionEnter", false);
        if (!mGameStarted)
            return;

        // Landing
        if (((1 << collision.gameObject.layer) & mGroundLayer) != 0)
        {
            mIsJumping = false;
            Bounce();
        }
    }

    private void Bounce()
    {
        if (mIsSliding)
            return;

        //MyUtils.Log("Baby faut que ça bounce");
        Vector3 newVelocity = mRb.linearVelocity;
        newVelocity.y = mBounceForce;
        mRb.linearVelocity = newVelocity;

        PlayLandingAnimation();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (!mIsJumping)
        {
            MyUtils.Log("Position before jump: " + transform.position.z + " with speed " + mRSO_ForwardSpeed.Value);

            float jumpGravity = GetJumpGravity();
            float jumpForce = Mathf.Sqrt(2f * jumpGravity * mJumpHeight);

            Vector3 newVelocity = mRb.linearVelocity;
            newVelocity.y = jumpForce;
            mRb.linearVelocity = newVelocity;

            mIsJumping = true;
        }
    }

    private float GetJumpGravity()
    {
        float speedRatio = Mathf.InverseLerp(3, 20, mRSO_ForwardSpeed.Value);

        // Gravité qui augmente avec la vitesse
        float currentGravity = Mathf.Lerp(
            mGravityBase,
            mGravityMax,
            speedRatio
        );

        return currentGravity;
    }

    private async void PlayLandingAnimation()
    {
        //mCurrentAnim.TryCancel();

        await LMotion.Create(mScaleBase, mScaleSquashed, mSquashDuration)
            .WithEase(mSquashEase)
            .BindToLocalScale(transform);

        await LMotion.Create(mScaleSquashed, mScaleBase, mSquashDuration)
            .WithEase(mSquashEase)
            .BindToLocalScale(transform);
    }



    private void OnSlide(InputAction.CallbackContext ctx)
    {
        if (!mIsJumping)
        {
            Slide();
        }
    }

    private float GetSlideDuration()
    {
        float speedRatio = Mathf.InverseLerp(pMinSpeed, pMaxForwardSpeed, mRSO_ForwardSpeed.Value);

        return Mathf.Lerp(mSlideDurationBase, mSlideDurationMax, speedRatio);
    }

    private async void Slide()
    {
        mIsSliding = true;
        float duration = GetSlideDuration();

        await LMotion.Create(mScaleBase, mScaleSlide, duration / 2)
            .WithEase(mSlideEase)
            .BindToLocalScale(transform);

        await LMotion.Create(mScaleSlide, mScaleBase, duration / 2)
            .WithEase(mSlideEase)
            .BindToLocalScale(transform);

        mIsSliding = false;
        Bounce();
    }

    private void OnStartGame()
    {
        mGameStarted = true;
        Bounce();
    }
}
