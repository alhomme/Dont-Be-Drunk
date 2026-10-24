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
    [SerializeField] private RSE_Pause mRSE_Pause;
    [SerializeField] private RSE_Collision mRSE_Collision;
    [SerializeField] private RSE_PlayMusic mRSE_PlayMusic;
    [SerializeField] private RSE_PlaySFX mRSE_PlaySFX;

    [Header("Bounce")]
    [SerializeField] private float mBounceHeight = 0.2f;
    [SerializeField] private float mBounceDistance = 1.5f;
    [SerializeField] private Ease mBounceEase = Ease.InOutSine;

    [Header("Landing")]
    [SerializeField] private float mLandScale = 0.2f;
    [SerializeField] private float mLandDuration = 0.15f;
    [SerializeField] private Ease mLandEase = Ease.OutQuad;

    [Header("Jump")]
    [SerializeField] private float mJumpHeight = 1.2f;
    [SerializeField] private float mJumpDistance = 3f;
    [SerializeField] private Ease mJumpEase = Ease.InOutSine;

    [Header("Slide")]
    [SerializeField] private Vector3 mSlideScale = new Vector3(0.65f, 0.25f, 0.65f);
    [SerializeField] private float mSlideDistance = 3f;
    [SerializeField] private Ease mSlideEase = Ease.OutQuad;

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
    private MotionHandle mCurrentMotion;
    private MotionHandle mBlinkMotion;
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
        mRSE_Pause.Event += OnPause;

        mRSO_Distance.Value = 0;
        mRSO_Biome.Value = 0;
    }

    private void OnDisable()
    {
        mJumpInput.action.performed -= OnJump;
        mSlideInput.action.performed -= OnSlide;
        mRSE_StartGame.Event -= OnStartGame;
        mRSE_GameOver.Event -= OnGameOver;
        mRSE_Pause.Event -= OnPause;

        StopState();
    }

    private void OnStartGame()
    {
        mGameStarted = true;
        mRSE_PlayMusic.Dispatch("Biome0");
        UpdateState(PlayerState.Bouncing);
    }

    private void OnPause(bool pauseStatus)
    {

        if (pauseStatus)
        {
            PauseState(0f);
            if (mBlinkMotion.IsActive())
                mBlinkMotion.PlaybackSpeed = 0f;
        }
        else
        {
            PauseState(1f);
            if (mBlinkMotion.IsActive())
                mBlinkMotion.PlaybackSpeed = 1f;
        }

        mGameStarted = !pauseStatus;
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
            {
                mRSO_Biome.Value = Mathf.Min(mRSO_Biome.Value + 1, 3);
                mRSE_PlayMusic.Dispatch("Biome" + mRSO_Biome.Value.ToString());
            }
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

        StopState();
        mState = newState;
        StartState(mState);
    }

    private void StopState()
    {
        MyUtils.Log("PlayerManager.StopState", false);

        mCurrentMotion.TryCancel();
    }

    private void StartState(PlayerState state)
    {
        MyUtils.Log("PlayerManager.StartState " + state, false);

        switch (state)
        {
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

    private void PauseState(float resume)
    {
        MyUtils.Log("PlayerManager.StartState", false);

        if (mCurrentMotion.IsActive())
            mCurrentMotion.PlaybackSpeed = resume;
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

        mCurrentMotion = LMotion.Create(bounceGround, bouncePeak, bounceDuration / 2)
            .WithEase(mBounceEase)
            .WithOnComplete(() => BounceDown(bounceGround, bouncePeak, bounceDuration))
            .BindToPositionY(transform)
            .AddTo(transform);
    }

    private void BounceDown(float bounceGround, float bouncePeak, float duration)
    {
        mCurrentMotion = LMotion.Create(bouncePeak, bounceGround, duration / 2)
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
        mCurrentMotion = LMotion.Create(mScaleBase, squashScale, mLandDuration / 2)
            .WithOnComplete(() => LandUp(squashScale))
            .BindToLocalScale(transform)
            .AddTo(transform);
    }

    private void LandUp(Vector3 squashScale)
    {
        mCurrentMotion = LMotion.Create(squashScale, mScaleBase, mLandDuration / 2)
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
        if (mGameStarted)
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

        mCurrentMotion = LMotion.Create(jumpStart, jumpPeak, jumpDuration / 2)
            .WithEase(mJumpEase)
            .WithOnComplete(() => JumpDown(jumpPeak, jumpDuration))
            .BindToPositionY(transform)
            .AddTo(transform);
    }

    private void JumpDown(float jumpPeak, float jumpDuration)
    {
        mCurrentMotion = LMotion.Create(jumpPeak, mGroundY, jumpDuration / 2)
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
        if (mGameStarted)
            UpdateState(PlayerState.Sliding);
    }

    private void Slide()
    {
        float slideDuration = mSlideDistance / mRSO_ForwardSpeed.Value;
        float slidePos = mSlideScale.y / 2;

        // Landing slide
        // Squash in & pos to ground then out
        // Then Bounce
        mCurrentMotion = LSequence.Create()
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
            if ((mObstacleLayer & (1 << other.gameObject.layer)) > 0)
            {
                Color baseColor = mPlayerMaterial.color;

                // Add blink effect
                mBlinkMotion = LMotion.Create(1f, 0f, 0.1f)
                    .WithEase(Ease.OutQuad)
                    .WithLoops(6, LoopType.Yoyo)
                    .Bind(a => {
                        Color c = baseColor;
                        c.a = a;
                        mPlayerMaterial.color = c;
                    })
                    .AddTo(gameObject);

                mObstacleParticle.Play();
                mRSE_PlaySFX.Dispatch("Game_Hit");
                if (other.tag == "Stand")
                {
                    mStrengthParticle.Play();
                    mRSE_PlaySFX.Dispatch("Game_Eat");
                }
            }
            else if ((mAlcoholLayer & (1 << other.gameObject.layer)) > 0)
            {
                mAlcoholParticle.Play();
                mRSE_PlaySFX.Dispatch("Game_Drink");
            }
            else if ((mCollectibleLayer & (1 << other.gameObject.layer)) > 0)
            {
                if (other.tag == "Heart")
                {
                    mHeartParticle.Play();
                    mRSE_PlaySFX.Dispatch("Game_Heart");
                }
            }

            mRSE_Collision.Dispatch(other.gameObject);
        }

        mLastObstacleHit = other.gameObject.transform.position;
    }

    private void OnGameOver(GameOverType type)
    {
        mGameStarted = false;
        mBlinkMotion.TryCancel();
        UpdateState(PlayerState.Idle);
    }

}
