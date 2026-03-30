using System.Linq;
using UnityEngine;
using Photon.Pun;

public class WallPhasingAbility : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public KeyCode phaseKey = KeyCode.LeftShift;
    public LayerMask wallLayer;
    public LayerMask roomLayer;
    public Camera playerCam;

    private bool isPhasing = false;
    private Collider playerCollider;
    private Vector3 phasingDirection;
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
        if (!photonView.IsMine) return;

        //DrawPhasingRay();

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
        if (!IsTouchingWall())
        {
            Debug.Log("Not touching a wall, cannot phase.");
            return;
        }

        if (!IsLookingAtRoom())
        {
            Debug.Log("Not looking at another room, cannot phase.");
            return;
        }

        Debug.Log("THE PLAYER IS READY FOR PHASING");
        photonView.RPC(nameof(RPC_StartPhasing), RpcTarget.All, playerCam.transform.forward);
    }

    bool IsTouchingWall()
    {
        return Physics.CheckSphere(transform.position, 0.8f, wallLayer);
    }

    bool IsLookingAtRoom()
    {
        int layerMask = roomLayer.value & ~LayerMask.GetMask("Wall");
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        RaycastHit hit;
        bool didHit = Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);

        if (didHit)
        {
            return hit.collider.GetComponent<RoomVolume>() != null;
        }

        return false;
    }

    [PunRPC]
    void RPC_StartPhasing(Vector3 direction)
    {
        phasingYLevel = transform.position.y;
        isPhasing = true;
        phasingDirection = direction;
        playerCollider.isTrigger = true;

        if (rb != null)
        {
            rb.detectCollisions = false;
            rb.linearVelocity = Vector3.zero;
        }
    }

    void MoveForwardWhilePhasing()
    {
        Vector3 movement = phasingDirection * moveSpeed * Time.deltaTime;
        Vector3 targetPosition = transform.position + movement;
        targetPosition.y = phasingYLevel;

        if (rb != null)
            rb.MovePosition(targetPosition);
        else
            transform.position = targetPosition;

        if (ReachedOtherRoom())
        {
            photonView.RPC(nameof(RPC_StopPhasing), RpcTarget.All);
        }
    }

    [PunRPC]
    void RPC_StopPhasing()
    {
        isPhasing = false;
        playerCollider.isTrigger = false;

        if (rb != null)
            rb.detectCollisions = true;
    }

    bool ReachedOtherRoom()
    {
        return Physics.CheckSphere(transform.position, 0.5f, roomLayer);
    }

    void DrawPhasingRay()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        float rayLength = 10f;
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.cyan);
    }
}
