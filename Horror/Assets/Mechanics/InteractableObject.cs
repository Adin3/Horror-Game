using UnityEngine;
using Photon.Pun;

public class InteractableObject : MonoBehaviourPun
{
    public virtual void Interact()
    {
        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            photonView.RPC(nameof(RPC_HandleInteraction), RpcTarget.AllBuffered);
        }
    }

    // This is what Photon will call over the network
    [PunRPC]
    private void RPC_HandleInteraction()
    {
        HandleInteraction(); // Calls the virtual method
    }

    // This is meant to be overridden by child classes
    public virtual void HandleInteraction()
    {
        Debug.Log($"[Base] FROM BASE CLASS {gameObject.name}");
    }


}
