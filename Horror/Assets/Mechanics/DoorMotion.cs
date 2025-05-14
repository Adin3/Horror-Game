using UnityEngine;
using Photon.Pun;

public class door_panel_script : InteractableObject
{
    private Animator mAnimator;
    private bool isOpen = false;

    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }


    public override void HandleInteraction()
    {
        ToggleDoor();
    }

    void ToggleDoor()
    {
        if (mAnimator != null && PhotonNetwork.IsConnected)
        {
            photonView.RPC(nameof(RPC_ToggleDoor), RpcTarget.AllBuffered);
        } else
        {
            RPC_ToggleDoor();
        }
    }

    [PunRPC]
    private void RPC_ToggleDoor()
    {
        isOpen = !isOpen;
        mAnimator.SetBool("isOpen", isOpen);
    }
}