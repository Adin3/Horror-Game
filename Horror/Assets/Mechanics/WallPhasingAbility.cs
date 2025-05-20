using System.Linq;
using UnityEngine;

public class WallPhasingAbility : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the player moves while phasing
    public KeyCode phaseKey = KeyCode.LeftShift; // Key to activate phasing
    public LayerMask wallLayer; // Layer for walls
    public LayerMask roomLayer; // Layer for rooms

    private bool isPhasing = false; // Tracks whether the player is phasing
    private Collider playerCollider;
    private Vector3 phasingDirection; // Direction to move during phasing
    public Camera playerCam;
    private Rigidbody rb;
    private float phasingYLevel;

    void Start()
    {
        playerCollider = GetComponentsInChildren<CapsuleCollider>()
            .FirstOrDefault(c => c.gameObject != this.gameObject);
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Draw the phasing ray always in the Scene view
        DrawPhasingRay();

        if (Input.GetKeyDown(phaseKey))
        {
            TryStartPhasing();
        }

        if (isPhasing)
        {
            MoveForwardWhilePhasing();
        }
    }

    void TryStartPhasing()
    {
        // Check if the player is touching a wall
        if (!IsTouchingWall())
        {
            Debug.Log("Not touching a wall, cannot phase.");
            return;
        } else
        {
            Debug.Log("Touching a wall.");
        }

        // Check if the player is looking at another room
        if (!IsLookingAtRoom())
        {
            Debug.Log("Not looking at another room, cannot phase.");
            return;
        }

        Debug.Log("THE PLAYER IS READY FOR PHASING");
        // Start phasing
        StartPhasing();
    }

    bool IsTouchingWall()
    {
        // Use a capsule cast or check collisions to determine if touching a wall
        return Physics.CheckSphere(transform.position, 0.8f, wallLayer);
    }

    bool IsLookingAtRoom()
    {
        // Define a layer mask that excludes the "Wall" layer
        int layerMask = roomLayer.value & ~LayerMask.GetMask("Wall");

        // Use the camera's center for the ray
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        float rayLength = 10f; // Set a reasonable length for visualization

        // Draw the ray in the Scene view (green if hit, red if not)
        RaycastHit hit;
        bool didHit = Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);

        Color rayColor = didHit ? Color.green : Color.red;
        Debug.DrawRay(ray.origin, ray.direction * rayLength, rayColor);

        if (didHit)
        {
            RoomVolume roomVolume = hit.collider.GetComponent<RoomVolume>();
            if (roomVolume != null)
            {
                Debug.Log("Looking at room ID: " + roomVolume.roomId);
                return true;
            }
        }

        Debug.Log("No room in sight.");
        return false;
    }


    void StartPhasing()
    {
        phasingYLevel = transform.position.y;

        isPhasing = true;
        phasingDirection = playerCam.transform.forward; // Use camera direction
        playerCollider.isTrigger = true;
        if (rb != null)
        {
            rb.detectCollisions = false; // Optional: disables all collisions
            rb.linearVelocity = Vector3.zero;
        }
    }

    void MoveForwardWhilePhasing()
    {
        Vector3 movement = phasingDirection * moveSpeed * Time.deltaTime;
        Vector3 targetPosition = transform.position + movement;
        targetPosition.y = phasingYLevel; // Lock vertical position

        if (rb != null)
            rb.MovePosition(targetPosition);
        else
            transform.position = targetPosition;

        if (ReachedOtherRoom())
        {
            StopPhasing();
        }
    }

    void StopPhasing()
    {
        isPhasing = false;
        playerCollider.isTrigger = false;
        if (rb != null)
            rb.detectCollisions = true;
    }

    bool ReachedOtherRoom()
    {
        // Logic to determine if the player has reached another room
        // For example, check if the player's position overlaps with a "Room" collider
        return Physics.CheckSphere(transform.position, 0.5f, roomLayer);
    }

    void DrawPhasingRay()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        float rayLength = 10f;
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.cyan);
    }
}