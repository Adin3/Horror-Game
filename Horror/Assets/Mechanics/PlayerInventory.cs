using UnityEngine;
using Photon.Pun;

public class PlayerInventory : MonoBehaviourPunCallbacks
{
    private int wCondCollected = 0;
    private const int WCOND_TARGET = 5; // Target number of WCond items

    public void CollectItem(ItemType type, int value)
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC(nameof(RPC_CollectItem), RpcTarget.All, (int)type, value);
        }
        else
        {
            RPC_CollectItem((int)type, value);
        }
    }

    [PunRPC]
    private void RPC_CollectItem(int typeInt, int value)
    {
        ItemType type = (ItemType)typeInt;

        Debug.Log($"Collected item of type: {type} (value: {value})");

        if (type == ItemType.WCond)
        {
            wCondCollected += value;
            Debug.Log($"WCond collected! Total: {wCondCollected}");

            if (wCondCollected >= WCOND_TARGET)
            {
                Debug.Log("Congratulations! You've collected 5 WCond items!");
            }
        }
        // We'll keep the Trap type but not implement it yet
    }

    // Helper method to get current WCond count
    public int GetWCondCollected() => wCondCollected;
}