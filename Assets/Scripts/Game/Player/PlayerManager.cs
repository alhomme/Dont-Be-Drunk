using UnityEngine;
using UnityEngine.InputSystem;
using LitMotion;
using LitMotion.Extensions;

public class PlayerManager : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference mMoveInput;
    [SerializeField] private InputActionReference mJumpInput;
    [SerializeField] private InputActionReference mSlideInput;
    
    [Header("RSO")]
    [SerializeField] private RSO_Speed mRSO_ForwardSpeed;
    [SerializeField] private RSO_Speed mRSO_LateralSpeed;
    [SerializeField] private RSO_Distance mRSO_Distance;
    [SerializeField] private RSO_Biome mRSO_Biome;

    [Header("RSE")]
    [SerializeField] private RSE_StartGame mRSE_StartGame;
    [SerializeField] private RSE_GameOver mRSE_GameOver;
    [SerializeField] private RSE_Collision mRSE_Collision;

    [Header("Bounce")]
    [SerializeField] private float mBounceHeight = 0.2f;
    [SerializeField] private float mBounceDistance = 1.5f;
    [SerializeField] private Ease mBounceEase = Ease.InOutSine;
    private MotionHandle mBounceMotion;

    [Header("Landing")]
    [SerializeField] private float mLandScale = 0.2f;
    [SerializeField] private float mLandDuration = 0.15f;
    [SerializeField] private Ease mLandEase = Ease.OutQuad;
    private MotionHandle mLandMotion;

    [Header("Jump")]
    [SerializeField] private float mJumpHeight = 1.2f;
    [SerializeField] private float mJumpDistance = 3f;
    [SerializeField] private Ease mJumpEase = Ease.InOutSine;
    private MotionHandle mJumpMotion;

    [Header("Slide")]
    [SerializeField] private Vector3 mSlideScale = new Vector3(0.65f, 0.25f, 0.65f);
    [SerializeField] private float mSlideDistance = 3f;
    [SerializeField] private Ease mSlideEase = Ease.OutQuad;
    private MotionHandle mSlideMotion;

    [Header("Elements")]
    [SerializeField] private float mGroundY = 0.5f;
    [SerializeField] private LayerMask mObstacleLayer;
    [SerializeField] private LayerMask mAlcoholLayer;
    [SerializeField] private LayerMask mCollectibleLayer;
    [SerializeField] private ParticleSystem mObstacleParticle;
    [SerializeField] private ParticleSystem mAlcoholParticle;
    [SerializeField] private ParticleSystem mHeartParticle;
    [SerializeField] private ParticleSystem mStrengthParticle;


    private bool mGameStarted;
    private Vector3 mScaleBase;
    private Material mPlayerMaterial;
    private Vector3 mLastObstacleHit = Vector3.zero;

    private enum PlayerState
    {
        Idle,
        Landing,
        Bouncing,
        Jumping,
        Sliding
    }
    private PlayerState mState;




    private void Awake()
    {
        mScaleBase = transform.localScale;

        mGameStarted = false;

        mState = PlayerState.Idle;

        mPlayerMaterial = GetComponent<MeshRenderer>().material;
    }

    private void OnEnable()
    {
        mJumpInput.action.performed += OnJump;
        mSlideInput.action.performed += OnSlide;
        mRSE_StartGame.Event += OnStartGame;
        mRSE_GameOver.Event += OnGameOver;

        mRSO_Distance.Value = 0;
        mRSO_Biome.Value = 0;
    }

    private void OnDisable()
    {
        mJumpInput.action.performed -= OnJump;
        mSlideInput.action.performed -= OnSlide;
        mRSE_StartGame.Event -= OnStartGame;
        mRSE_GameOver.Event -= OnGameOver;

        StopState(mState);
    }

    private void OnStartGame()
    {
        mGameStarted = true;
        UpdateState(PlayerState.Bouncing);
    }

    private void FixedUpdate()
    {
        if (!mGameStarted)
            return;

        Move();

        if ((int)(transform.position.z) > mRSO_Distance.Value)
        {
            mRSO_Distance.Value = (int)(transform.position.z);
            if (mRSO_Distance.Value % 510 == 0)
                mRSO_Biome.Value = Mathf.Min(mRSO_Biome.Value + 1, 3);
        }
    }


    /*
     * BASE MOVEMENT
     */

    private void Move()
    {
        MyUtils.Log("Speed = " + mRSO_ForwardSpeed.Value, false);

        // Z
        transform.position += Vector3.forward * mRSO_ForwardSpeed.Value * Time.deltaTime;

        // X

        float horizontalInput = mMoveInput.action.ReadValue<Vector2>().x;
        if ((horizontalInput > 0 && transform.position.x < 2.5f)
            || (horizontalInput < 0 && transform.position.x > -2.5f))
        {
            Vector3 lateralDirection = new Vector3(horizontalInput, 0, 0);
            transform.position += lateralDirection * mRSO_LateralSpeed.Value * Time.deltaTime;
        }
    }

    /*
     * MANAGE STATE
     */

    private void UpdateState(PlayerState newState)
    {
        MyUtils.Log("PlayerManager.UpdateState " + mState + " => " + newState, false);

        if (mState == newState)
            return;

        StopState(mState);
        mState = newState;
        StartState(mState);
    }

    private void StopState(PlayerState state)
    {
        MyUtils.Log("PlayerManager.StopState " + state, false);

        switch (state)
        {
            case PlayerState.Bouncing:
                mBounceMotion.TryCancel();
                break;
            case PlayerState.Landing:
                mLandMotion.TryCancel();
                break;
            case PlayerState.Jumping:
                mJumpMotion.TryCancel();
                break;
            case PlayerState.Sliding:
                mSlideMotion.TryCancel();
                break;
        }
    }

    private void StartState(PlayerState state)
    {
        MyUtils.Log("PlayerManager.StartState " + state, false);

        switch (state)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Bouncing:
                BounceUp();
                break;
            case PlayerState.Landing:
                LandDown();
                break;
            case PlayerState.Jumping:
                JumpUp();
                break;
            case PlayerState.Sliding:
                Slide();
                break;
        }
    }


    /*
     * BOUNCE
     * Bounce -> Landing
     */

    private void BounceUp()
    {
        float bounceGround = transform.position.y;
        float bouncePeak = transform.position.y + mBounceHeight;

        float bounceDuration = mBounceDistance / mRSO_ForwardSpeed.Value;


        // Bounce Anim
        // Up & down
        // Then calls landing

        mBounceMotion = LMotion.Create(bounceGround, bouncePeak, bounceDuration / 2)
            .WithEase(mBounceEase)
            .WithOnComplete(() => BounceDown(bounceGround, bouncePeak, bounceDuration))
            .BindToPositionY(transform)
            .AddTo(transform);


        /*  mBounceMotion = LSequence.Create()
              .Append(LMotion.Create(bounceGround, bouncePeak, bounceDuration / 2)
                  .WithEase(mBounceEase)
                  .BindToPositionY(transform))
              .Append(LMotion.Create(bouncePeak, bounceGround, bounceDuration / 2)
                  .WithEase(mBounceEase)
                  .WithOnComplete(() => UpdateState(PlayerState.Landing))
                  .BindToPositionY(transform))
              .Run()
              .AddTo(transform);

          return;*/
    }

    private void BounceDown(float bounceGround, float bouncePeak, float duration)
    {
        mBounceMotion = LMotion.Create(bouncePeak, bounceGround, duration / 2)
            .WithEase(mBounceEase)
            .WithOnComplete(() => UpdateState(PlayerState.Landing))
            .BindToPositionY(transform)
            .AddTo(transform);
    }

    /*
     * LANDING
     */

    private void LandDown()
    {
        Vector3 squashScale = new Vector3(
            mScaleBase.x * (1f + mLandScale * 0.5f),
            mScaleBase.y * (1f - mLandScale),
            mScaleBase.z * (1f + mLandScale * 0.5f));


        // Landing anim
        // Squash in & out
        // Then Bounce

        mLandMotion = LMotion.Create(mScaleBase, squashScale, mLandDuration / 2)
            .WithOnComplete(() => LandUp(squashScale))
            .BindToLocalScale(transform)
            .AddTo(transform);

        /*  mLandMotion = LSequence.Create()
              .Append(LMotion.Create(mScaleBase, squashScale, mLandDuration / 2)
                  .WithEase(mLandEase)
                  .BindToLocalScale(transform))
              .Append(LMotion.Create(squashScale, mScaleBase, mLandDuration / 2)
                  .WithEase(mLandEase)
                  .WithOnComplete(() => UpdateState(PlayerState.Bouncing))
                  .BindToLocalScale(transform))
              .Run()
              .AddTo(transform);
        */
    }

    private void LandUp(Vector3 squashScale)
    {
        mLandMotion = LMotion.Create(squashScale, mScaleBase, mLandDuration / 2)
            .WithEase(mLandEase)
            .WithOnComplete(() => UpdateState(PlayerState.Bouncing))
            .BindToLocalScale(transform)
            .AddTo(transform);
    }

    /*
     * JUMP
     */

    private void OnJump(InputAction.CallbackContext ctx)
    {
        UpdateState(PlayerState.Jumping);
    }

    private void JumpUp()
    {
        float jumpStart = transform.position.y;
        float jumpPeak = transform.position.y + mJumpHeight;

        float jumpDuration = mJumpDistance / mRSO_ForwardSpeed.Value;

        // Jump Anim
        // Up & down
        // Then calls landing

        mJumpMotion = LMotion.Create(jumpStart, jumpPeak, jumpDuration / 2)
            .WithEase(mJumpEase)
            .WithOnComplete(() => JumpDown(jumpPeak, jumpDuration))
            .BindToPositionY(transform)
            .AddTo(transform);

        /*   mJumpMotion = LSequence.Create()
               .Append(LMotion.Create(jumpStart, jumpPeak, jumpDuration / 2)
                   .WithEase(mJumpEase)
                   .BindToPositionY(transform))
               .Append(LMotion.Create(jumpPeak, mGroundY, jumpDuration / 2)
                   .WithEase(mJumpEase)
                   .WithOnComplete(() => UpdateState(PlayerState.Landing))
                   .BindToPositionY(transform))
               .Run()
               .AddTo(transform);*/
    }

    private void JumpDown(float jumpPeak, float jumpDuration)
    {
        mJumpMotion = LMotion.Create(jumpPeak, mGroundY, jumpDuration / 2)
            .WithEase(mJumpEase)
            .WithOnComplete(() => UpdateState(PlayerState.Landing))
            .BindToPositionY(transform)
            .AddTo(transform);
    }

    /*
     * SLIDE
     */

    private void OnSlide(InputAction.CallbackContext ctx)
    {
        UpdateState(PlayerState.Sliding);
    }

    private void Slide()
    {
        float slideDuration = mSlideDistance / mRSO_ForwardSpeed.Value;
        float slidePos = mSlideScale.y / 2;

        // Landing slide
        // Squash in & pos to ground then out
        // Then Bounce
        mSlideMotion = LSequence.Create()
            .Append(LMotion.Create(mScaleBase, mSlideScale, slideDuration / 2)
                .WithEase(mSlideEase)
                .BindToLocalScale(transform))
            .Join(LMotion.Create(transform.position.y, slidePos, slideDuration / 2)
                .BindToPositionY(transform))
            .Append(LMotion.Create(mSlideScale, mScaleBase, slideDuration / 2)
                .WithEase(mSlideEase)
                .BindToLocalScale(transform))
            .Join(LMotion.Create(slidePos, mGroundY, slideDuration / 2)
                .WithOnComplete(() => UpdateState(PlayerState.Bouncing))
                .BindToPositionY(transform))
            .Run()
            .AddTo(transform);
    }


    /*
     * COLLISIONS
     */

    private void OnTriggerEnter(Collider other)
    {
        MyUtils.Log("MovementManager.OnTriggerEnter: " + other.gameObject.tag);

        if (mLastObstacleHit != other.gameObject.transform.position)
        {
            mRSE_Collision.Dispatch(other.gameObject);


            if ((mObstacleLayer & (1 << other.gameObject.layer)) > 0)
            {
                Color baseColor = mPlayerMaterial.color;

                // Add blink effect
                LMotion.Create(1f, 0f, 0.1f)
                    .WithEase(Ease.OutQuad)
                    .WithLoops(6, LoopType.Yoyo)
                    .Bind(a => {
                        Color c = baseColor;
                        c.a = a;
                        mPlayerMaterial.color = c;
                    })
                    .AddTo(gameObject);

                mObstacleParticle.Play();
                if (other.tag == "Stand")
                    mStrengthParticle.Play();
            }
            else if ((mAlcoholLayer & (1 << other.gameObject.layer)) > 0)
            {
                mAlcoholParticle.Play();
            }
            else if ((mCollectibleLayer & (1 << other.gameObject.layer)) > 0)
            {
                if (other.tag == "Heart")
                    mHeartParticle.Play();
            }
        }

        mLastObstacleHit = other.gameObject.transform.position;
    }

    private void OnGameOver(GameOverType type)
    {
        mGameStarted = false;
        UpdateState(PlayerState.Idle);
    }

}
