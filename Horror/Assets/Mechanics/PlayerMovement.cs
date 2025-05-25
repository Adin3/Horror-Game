using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Data;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;

    [Header("Jumping")]
    public float airMultiplier;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    private CapsuleCollider capsuleCollider;
    private float originalHeight;
    private Vector3 originalCenter;
    private float originalRadius;

    public float crouchHeight = 0.779823f;
    public Vector3 crouchCenter;
    public float crouchRadius;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode flashlightToggleKey = KeyCode.F;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Gravity")]
    [SerializeField] public float gravity = 100f;

    [Header("Camera Positioning")]
    public Transform cameraPos;
    public Vector3 normalCameraLocalPos;
    public Vector3 crouchCameraLocalPos;
    public Vector3 crawlCameraLocalPos;
    public float cameraTransitionSpeed = 5f;

    [Header("Flashlight")]
    public Light FlashLight;
    private bool flashlightOn = false;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    PhotonView view;

    [HideInInspector]
    public Animator animator = null;

    private int isRunningHash;
    private int isWalkingHash;
    private int isIdlingHash;
    private int isCrouchingHash;
    private int isCrawlingHash;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air,
        idling
    }

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYScale = transform.localScale.y;
        view = GetComponent<PhotonView>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        originalHeight = capsuleCollider.height;
        originalCenter = capsuleCollider.center;
        originalRadius = capsuleCollider.radius;
        crouchCenter = new Vector3(originalCenter.x, -0.3700758f, originalCenter.z);
        crouchRadius = originalRadius / 2f;

        // Camera positions
        normalCameraLocalPos = cameraPos.localPosition;
        crouchCameraLocalPos = normalCameraLocalPos + new Vector3(0f, -0.7f, 0f);
        crawlCameraLocalPos = normalCameraLocalPos + new Vector3(0f, -0.9f, 0f);

        if (!view.IsMine)
        {
            rb.isKinematic = true;
        }

        if (animator == null)
        {
            Debug.Log("No animator assigned.");
        }

        isRunningHash = Animator.StringToHash("isRunning");
        isWalkingHash = Animator.StringToHash("isWalking");
        isIdlingHash = Animator.StringToHash("isIdling");
        isCrouchingHash = Animator.StringToHash("isCrouching");
        isCrawlingHash = Animator.StringToHash("isCrawling");

        if (FlashLight != null)
        {
            FlashLight.enabled = false; // start turned off
        }
    }

    private void Update()
    {
        if (!view.IsMine) return;

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        HandleFlashlight();

        rb.linearDamping = grounded ? groundDrag : 0f;

        if (state == MovementState.crouching)
        {
            capsuleCollider.center = crouchCenter;
            capsuleCollider.height = crouchHeight;
            capsuleCollider.radius = crouchRadius;
        }
        else
        {
            capsuleCollider.height = originalHeight;
            capsuleCollider.center = originalCenter;
            capsuleCollider.radius = originalRadius;
        }
    }

    private void FixedUpdate()
    {
        if (!view.IsMine) return;

        MovePlayer();
        ApplyGravity();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void StateHandler()
    {
        bool hasInput = horizontalInput != 0 || verticalInput != 0;
        bool wantsToCrouch = Input.GetKey(crouchKey);

        ResetAllAnimatorBools();

        if (grounded)
        {
            if (wantsToCrouch || !CanStandUp())
            {
                animator.SetBool(isCrouchingHash, true);
                state = MovementState.crouching;
                moveSpeed = crouchSpeed;

                if (hasInput)
                {
                    animator.SetBool(isCrawlingHash, true);
                    SmoothCameraTo(crawlCameraLocalPos);
                }
                else
                {
                    SmoothCameraTo(crouchCameraLocalPos);
                }
            }
            else if (Input.GetKey(sprintKey) && hasInput)
            {
                state = MovementState.sprinting;
                moveSpeed = sprintSpeed;
                animator.SetBool(isRunningHash, true);
                SmoothCameraTo(normalCameraLocalPos);
            }
            else if (hasInput)
            {
                state = MovementState.walking;
                moveSpeed = walkSpeed;
                animator.SetBool(isWalkingHash, true);
                SmoothCameraTo(normalCameraLocalPos);
            }
            else
            {
                state = MovementState.idling;
                moveSpeed = 0f;
                animator.SetBool(isIdlingHash, true);
                SmoothCameraTo(normalCameraLocalPos);
            }
        }
        else
        {
            state = MovementState.air;
            SmoothCameraTo(normalCameraLocalPos);
        }
    }

    private bool CanStandUp()
    {
        float checkHeight = originalHeight;
        float checkRadius = capsuleCollider.radius * 0.95f;

        Vector3 start = transform.position + Vector3.up * capsuleCollider.center.y;
        Vector3 end = start + Vector3.up * (checkHeight - capsuleCollider.height);

        return !Physics.CheckCapsule(start, end, checkRadius, whatIsGround);
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection.y = 0f;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void ApplyGravity()
    {
        if (!grounded)
        {
            rb.AddForce(Vector3.down * gravity, ForceMode.Force);
        }
    }

    private void ResetAllAnimatorBools()
    {
        animator.SetBool(isRunningHash, false);
        animator.SetBool(isWalkingHash, false);
        animator.SetBool(isIdlingHash, false);
        animator.SetBool(isCrouchingHash, false);
        animator.SetBool(isCrawlingHash, false);
    }

    private void SmoothCameraTo(Vector3 targetLocalPos)
    {
        cameraPos.localPosition = Vector3.Lerp(cameraPos.localPosition, targetLocalPos, Time.deltaTime * cameraTransitionSpeed);
    }

    private void HandleFlashlight()
    {
        if (!view.IsMine || FlashLight == null) return;

        if (Input.GetKeyDown(flashlightToggleKey))
        {
            flashlightOn = !flashlightOn;
            FlashLight.enabled = flashlightOn;

            view.RPC("RPC_SetFlashlight", RpcTarget.Others, flashlightOn);
        }
    }
    [PunRPC]
    void RPC_SetFlashlight(bool state)
    {
        flashlightOn = state;

        if (FlashLight != null)
        {
            FlashLight.enabled = flashlightOn;
        }
    }
}
