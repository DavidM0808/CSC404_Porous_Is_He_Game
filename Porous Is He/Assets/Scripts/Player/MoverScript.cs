using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;

[RequireComponent(typeof(CharacterController))]

public class MoverScript : MonoBehaviour
{
    public CharacterController controller;
    private PlayerInputActions playerInputActions;
    private Transform cameraMainTransform;

    private static float verticalMod;

    // Jump variables
    private float playerVelocity;
    private float gravityValue;
    private float jumpPower;
    private int numberOfJumps = 0;
    private int maxNumberOfJumps = 2;

    // Player Movement variables
    private Vector2 inputVector;
    private Vector3 direction;
    [SerializeField] private float playerSpeed = 6.5f;
    private float turnSmoothTime = 0.05f;
    private float turnSmoothVelocity;

    public bool cannotMove = false;

    // Variables that deals with knockback
    public float knockBackForce = 4f;
    public float knockBackForceHorizontal = 2f;
    public float knockBackTime = 0.2f;

    public float knockBackForceUP = 4f;
    public float knockBackForceHorizontalUP = 2f;
    public float knockBackTimeUP = 0.2f;

    private float knockBackCounter;

    // Variable that toggles movement
    private bool enableMovement = true;

    // Variable that deals with aiming
    public bool aiming = false;

    private Vector3 hitNormal;
    private GameObject hitObject;

    public bool isGrounded_Custom;
    private float slopeLimit;
    public float slideSpeed = 1.0f;

    // used in seesaw script
    public bool onMovingPlatform;
    private bool isJumping = false;

    [SerializeField] private Lander landingHitbox;
    private float lastJumped = 0;

    private DifficultyManager diffm;
    [SerializeField] private AudioSource WetRunningSound;
    [SerializeField] private AudioSource OilRunningSound;
    [SerializeField] private AudioSource RunningSound;

    [SerializeField] private Animator playerAnimator;

    private float lastGrounded = 0;


    [SerializeField] private LiquidTracker liquidTracker;
    [SerializeField] private float knockbackMultiWithOil = 1f;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.started += Jump;
    }
    private void Start()
    {
        cameraMainTransform = Camera.main.transform;
        slopeLimit = controller.slopeLimit;

        //AssignMovement();
        diffm = GameObject.Find("GameController").GetComponent<DifficultyManager>();
        diffm.loadedEvent.AddListener(AssignMovement);
    }

    private void AssignMovement()
    {
        float jumpMulti = 1;
        if (diffm.difficultyAssigned)
        {
            if (diffm.isHardmode)
            {
                verticalMod = 0.9f;
            }
            else
            {
                jumpMulti = 1.05f;
                verticalMod = 0.75f;
            }
        }
        else
        {
            verticalMod = 0.9f;
        }

        gravityValue = -8.8f * verticalMod;
        jumpPower = 2.5f * verticalMod * jumpMulti;



    }

    void FixedUpdate()
    {
        Vector3 slidingMovement = new Vector3();


        if (isGrounded_Custom)
        {
            lastGrounded = Time.time;
        }

        if (landingHitbox.IsGrounded() && Time.time - lastJumped > 0.1f)
        {
            StopFallAnim();
        }
        else
        {
            if (Time.time - lastGrounded > 0.1f && numberOfJumps == 0)
            {
                PlayFallAnim(false, false);
            }
        }


        //grounded detection
        if (!isGrounded_Custom)
        {
            float slideMulti = 1f + 1f * Mathf.Min((Time.time - lastGrounded) * 5f, 10f);
            slidingMovement.x += (1f - hitNormal.y) * hitNormal.x * slideSpeed * slideMulti;
            slidingMovement.z += (1f - hitNormal.y) * hitNormal.z * slideSpeed * slideMulti;
        }
        if (!(Vector3.Angle(Vector3.up, hitNormal) <= slopeLimit))
        {
            isGrounded_Custom = false;
        }
        else
        {
            isGrounded_Custom = controller.isGrounded;
        }


        if (hitObject)
        {
            SpinRollingPin spinRolling = hitObject.GetComponent("SpinRollingPin") as SpinRollingPin;
            if (spinRolling != null)
            {
                slidingMovement += -hitObject.transform.parent.transform.right * spinRolling.rotationSpeed;
            }

        }

        if (LevelComplete.LevelEnd) return;

        if (knockBackCounter <= 0)
        {
            if (!aiming && enableMovement)
            {
                Move();
                ApplyGravity();
                controller.Move((direction * playerSpeed + slidingMovement) * Time.deltaTime);
            }
            else
            {
                MoveWhileAiming();
                controller.Move((direction * playerSpeed + slidingMovement) * Time.deltaTime);
            }
        }
        else
        {
            knockBackCounter -= Time.deltaTime;
            ApplyGravity();
            controller.Move(direction * playerSpeed * Time.deltaTime);
            if (IsGrounded())
            {
                direction.x = 0.0f;
                direction.z = 0.0f;
            }
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;

        hitObject = hit.gameObject;


    }

    private void PlayRunningSound()
    {

        if (!IsGrounded() && Time.time - lastGrounded > 0.1f )
        {
            StopRunningSound();
            return;
        }

        if (liquidTracker.GetLiquidAmountFromIndex(1) > 0)
        {
            if (!OilRunningSound.isPlaying)
            {
                OilRunningSound.Play();
            }
        } else if (liquidTracker.CalcWeight() > 0)
        {
            if (!WetRunningSound.isPlaying)
            {
                WetRunningSound.Play();
            }
        }
        else
        {
            if (!RunningSound.isPlaying)
            {
                RunningSound.Play();
            }
        }
    }
    private void StopRunningSound()
    {
        if (RunningSound.isPlaying)
        {
            RunningSound.Stop();
        }
        if (WetRunningSound.isPlaying)
        {
            WetRunningSound.Stop();
        }
        if (OilRunningSound.isPlaying)
        {
            OilRunningSound.Stop();
        }
    }

    private void PlayFallAnim(bool doJump, bool doDoubleJump)
    {
        if (doDoubleJump)
        {
            playerAnimator.SetBool("IsDoubleJumping", true);
        }

        if (doJump)
        {
            playerAnimator.SetBool("IsJumping", true);
        }

        playerAnimator.SetBool("IsFalling", true);
    }
    private void StopFallAnim()
    {
        playerAnimator.SetBool("IsDoubleJumping", false);
        playerAnimator.SetBool("IsJumping", false);
        playerAnimator.SetBool("IsFalling", false);

        playerAnimator.SetBool("IsBurning", false);
    }




    private void MoveWhileAiming()
    {
        playerAnimator.SetBool("IsRunning", false);
        StopRunningSound();

        // To be implemented
        if (IsGrounded() && playerVelocity < 0f)
        {
            playerVelocity = 0.0f;
            direction.x = 0f;
            direction.z = 0f;
        }
        else
        {
            playerVelocity += gravityValue * Time.deltaTime;
        }

        direction.y = playerVelocity;
    }

    private void Move()
    {
        if (cannotMove)
        {
            return;
        }

        inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        direction = new Vector3(inputVector.x, 0f, inputVector.y);

        Vector3 XZCMTForward = new Vector3(cameraMainTransform.forward.x, 0, cameraMainTransform.forward.z).normalized;
        Vector3 XZCMTRight = new Vector3(cameraMainTransform.right.x, 0, cameraMainTransform.right.z).normalized;
        direction = XZCMTForward * direction.z + XZCMTRight  * direction.x;

        if (direction.magnitude >= 0.1f)
        {
            playerAnimator.SetBool("IsRunning", true);
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            PlayRunningSound();
        }
        else
        {
            playerAnimator.SetBool("IsRunning", false);
            StopRunningSound();
        }
    }

    private void ApplyGravity()
    {
        if (onMovingPlatform && !isJumping)
            playerVelocity = gravityValue;
        else if (IsGrounded() && playerVelocity < 0f)
            playerVelocity = 0.0f;
        else
            playerVelocity += gravityValue * Time.deltaTime;

        direction.y = playerVelocity;
    }


    public void Jump(InputAction.CallbackContext context)
    {
        if (!enableMovement) return;
        if ((!IsGrounded() && numberOfJumps >= maxNumberOfJumps) || aiming) return;
        isJumping = true;
        if (numberOfJumps == 0) StartCoroutine(WaitForLanding());


        float playerWeight = liquidTracker.CalcWeight();
        if (playerWeight > 0)
        {
            playerVelocity = jumpPower * (8.0f / 10.0f);
        }
        else
        {
            playerVelocity = jumpPower;
        }

        if (numberOfJumps == 0) {
            lastJumped = Time.time;

            gameObject.GetComponent<PoSoundManager>().PlaySound("Jump");
            PlayFallAnim(true, false);
        } else
        {
            gameObject.GetComponent<PoSoundManager>().PlaySound("DoubleJump");
            PlayFallAnim(true, true);
        }

        numberOfJumps++;
    }

    private IEnumerator WaitForLanding()
    {
        yield return new WaitUntil(() => !IsGrounded());
        yield return new WaitUntil(IsGrounded);
        numberOfJumps = 0;
        isJumping = false;
    }

    //private bool IsGrounded() => controller.isGrounded;
    private bool IsGrounded() => isGrounded_Custom;

    public void KnockBack(Vector3 moveDirection, bool propelUp)
    {
        lastJumped = Time.time;

        float knockbackMulti = 1f;
        if (liquidTracker.GetLiquidAmountFromIndex(1) > 0)
        {
            knockbackMulti = knockbackMultiWithOil;
        }

        if (propelUp)
        {
            knockBackCounter = knockBackTimeUP;
            direction = moveDirection * knockBackForceHorizontalUP * knockbackMulti;
            direction.y = knockBackForceUP * knockbackMulti;
            playerVelocity = knockBackForceUP * knockbackMulti;
        } else
        {
            knockBackCounter = knockBackTime;
            direction = moveDirection * knockBackForceHorizontal * knockbackMulti;
            direction.y = knockBackForce * knockbackMulti;
            playerVelocity = knockBackForce * knockbackMulti;
        }

        playerAnimator.SetBool("IsBurning", true);
    }
    private void OnDestroy()
    {
        playerInputActions.Player.Disable();
    }


}
