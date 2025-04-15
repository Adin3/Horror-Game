using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    [Header("Gravity")]
    [SerializeField] public float gravity = 100f;
    [SerializeField] public float playerHeight = 2;


    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // Ground check using raycast
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 1.5f + 0.2f, whatIsGround);

        MyInput();

        // Apply ground drag when grounded
        if (grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplyGravity();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        // Calculate movement direction based on the camera's yaw (horizontal rotation only)
        Vector3 forward = orientation.forward;
        Vector3 right = orientation.right;

        // Ignore vertical movement (pitch) by flattening the direction vectors to the XZ plane
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();


        // Combine the movement directions
        moveDirection = forward * verticalInput + right * horizontalInput;

        // Apply movement force (keep y velocity unaffected by horizontal movement)
        Vector3 targetVelocity = moveDirection.normalized * moveSpeed;
        targetVelocity.y = rb.linearVelocity.y; // Preserve the y-velocity for gravity
        rb.linearVelocity = targetVelocity;
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