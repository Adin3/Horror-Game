using UnityEngine;
using Photon.Pun;

public class door_panel_script : InteractableObject
{
    public GameObject interactUiMessage;
    private Outline outline;
    private Animator mAnimator;
    private bool isOpen = false;

    void Start()
    {
        interactUiMessage.SetActive(true);
        interactUiMessage.SetActive(false);
        outline = GetComponent<Outline>();
        mAnimator = GetComponent<Animator>();
        outline.enabled = true;
        outline.enabled = false;
    }

    public virtual void Show()
    {
        interactUiMessage.SetActive(true);
        outline.enabled = true;
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