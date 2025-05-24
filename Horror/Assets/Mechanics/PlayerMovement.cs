using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Data;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
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

    public float crouchHeight = 1.279823f;
    public Vector3 crouchCenter;
    public float crouchRadius;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Gravity")]
    [SerializeField] public float gravity = 100f;

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

        // Only enable physics simulation for the local player
        if (!view.IsMine)
        {
            rb.isKinematic = true;
        }
        if (animator == null)
        {
            Debug.Log("No animATOR");
        }
        isRunningHash = Animator.StringToHash("isRunning");
        isWalkingHash = Animator.StringToHash("isWalking");
        isIdlingHash = Animator.StringToHash("isIdling");
        isCrouchingHash = Animator.StringToHash("isCrouching");
        isCrawlingHash = Animator.StringToHash("isCrawling");
    }

    private void Update()
    {
        // Only process input and movement for the local player
        if (view.IsMine)
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

            MyInput();
            SpeedControl();
            StateHandler();

            if (grounded)
            {
                rb.linearDamping = groundDrag;
            }
            else
            {
                rb.linearDamping = 0;
            }
            if (state == MovementState.crouching)
            {
                capsuleCollider.center = crouchCenter; // Example center
                capsuleCollider.height = crouchHeight;
            } else
            {
                capsuleCollider.height = originalHeight;
                capsuleCollider.center = originalCenter;
            }
        }
    }

    private void FixedUpdate()
    {
        // Only move the local player
        if (view.IsMine)
        {
            MovePlayer();
            ApplyGravity();
        }
    }

    private void MyInput()
    {
        // Only process input for the local player (redundant check since this is only called if view.IsMine)
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void StateHandler()
    {
        bool hasInput = horizontalInput != 0 || verticalInput != 0;

        if (grounded)
        {
            ResetAllAnimatorBools();

            if (Input.GetKey(crouchKey))
            {
                animator.SetBool(isCrouchingHash, true);
                state = MovementState.crouching;
                if (hasInput)
                {
                    animator.SetBool(isCrawlingHash, true);
                }
                moveSpeed = crouchSpeed;
            }
            else if (Input.GetKey(sprintKey) && hasInput)
            {
                state = MovementState.sprinting;
                moveSpeed = sprintSpeed;
                animator.SetBool(isRunningHash, true);
            }
            else if (hasInput)
            {
                state = MovementState.walking;
                moveSpeed = walkSpeed;
                animator.SetBool(isWalkingHash, true);
            }
            else
            {
                state = MovementState.idling;
                moveSpeed = 0;
                animator.SetBool(isIdlingHash, true);
            }
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection.y = 0f; // Prevent vertical movement from camera
        moveDirection = moveDirection.normalized;

        // on ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void ApplyGravity()
    {
        // Apply custom gravity if not grounded
        if (!grounded)
        {
            rb.AddForce(Vector3.down * gravity, ForceMode.Force); // Adjust gravity force if needed
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
}