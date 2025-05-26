using UnityEngine;
using Photon.Pun;

public class SeekerAttack : MonoBehaviourPun
{
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public LayerMask runnerLayer; // Assign the "Runner" layer in the Inspector
    public int hitsToTeleport = 3;
    public Vector3 teleportCoordinates = new Vector3(-23f, 2f, -10f);

    private float lastAttackTime = -Mathf.Infinity;
    private int hitsLanded = 0;

    private void Update()
    {
        if (!photonView.IsMine)
            return; // Only the owner controls attack logic

        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                // Check for runners in range
                Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, runnerLayer);
                foreach (var hit in hits)
                {
                    lastAttackTime = Time.time;
                    RegisterHit(hit.gameObject);
                    break; // Only hit one runner per click
                }
            }
        }
    }

    void RegisterHit(GameObject runner)
    {
        hitsLanded++;
        Debug.Log($"Seeker hit {runner.name}. Total hits: {hitsLanded}");

        if (hitsLanded >= hitsToTeleport)
        {
            hitsLanded = 0;
            if (PhotonNetwork.IsConnected)
            {
                PhotonView targetView = runner.GetComponent<PhotonView>();
                if (targetView != null)
                {
                    Debug.Log("Sending TeleportRunner RPC to all clients.");
                    targetView.RPC(nameof(TeleportRunner), RpcTarget.All, teleportCoordinates);
                }
                else
                {
                    Debug.LogWarning("Runner does not have a PhotonView component.");
                }
            }
            else
            {
                TeleportRunnerLocal(runner, teleportCoordinates);
            }
        }
    }

    [PunRPC]
    void TeleportRunner(Vector3 position)
    {
        transform.position = position;
        Debug.Log("Runner teleported to " + position);
    }


    void TeleportRunnerLocal(GameObject runner, Vector3 position)
    {
        runner.transform.position = position;
        Debug.Log("Runner teleported to " + position);
    }

    // Optional: Visualize attack range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public int GetHitsLanded() => hitsLanded;
}
