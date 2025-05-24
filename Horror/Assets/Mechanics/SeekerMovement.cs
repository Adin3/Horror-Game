using UnityEngine;
using Photon.Pun;

public class SeekerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float walkSpeed;
    public float groundDrag;
    public float sprintSpeed;
    public float airMultiplier = 0.4f; // Add this for better air control

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    [Header("Gravity")]
    [SerializeField] public float gravity = 100f;

    [Header("Keybinds")]
    public KeyCode sprintKey = KeyCode.LeftShift;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    PhotonView view;

    [HideInInspector]
    int isRunningHash;
    int isWalkingHash;
    int isIdlingHash;

    [HideInInspector]
    public Animator animator = null;


    public MovementState state;
    public enum MovementState
    {
        walking,
        air,
        idling,
        sprinting
    }

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        view = GetComponent<PhotonView>();

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

            // Handle drag
            if (grounded)
            {
                rb.linearDamping = groundDrag;
            }
            else
            {
                rb.linearDamping = 0;
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
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void StateHandler()
    {
        bool hasInput = horizontalInput != 0 || verticalInput != 0;
        // Mode - Walking
        if (grounded)
        {
            if (hasInput)
            {
                if (Input.GetKey(sprintKey))
                {
                    state = MovementState.sprinting;
                    animator.SetBool(isRunningHash, true);
                    animator.SetBool(isWalkingHash, false);
                    animator.SetBool(isIdlingHash, false);
                    moveSpeed = sprintSpeed;
                }
                else
                {
                    state = MovementState.walking;
                    animator.SetBool(isWalkingHash, true);
                    animator.SetBool(isRunningHash, false);
                    animator.SetBool(isIdlingHash, false);
                    moveSpeed = walkSpeed;
                }
            } else
            {
                state = MovementState.idling;
                moveSpeed = 0;
                animator.SetBool(isWalkingHash, false);
                animator.SetBool(isIdlingHash, true);
                animator.SetBool(isRunningHash, false);
            }
        }
        // Mode - Air
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection.y = 0f; // Prevent vertical movement from camera
        moveDirection = moveDirection.normalized;

        // Apply movement force
        if (grounded)
        {
            rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
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
}