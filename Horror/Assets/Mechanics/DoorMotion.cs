using UnityEngine;
using Photon.Pun;

public class door_panel_script : InteractableObject
{
    public GameObject interactUiMessage;
    private Outline outline;
    private bool isOpen = false;
    private Animator mAnimator;

    [HideInInspector]
    private int isOpenHash;

    void Start()
    {
        interactUiMessage.SetActive(true);
        interactUiMessage.SetActive(false);
        outline = GetComponent<Outline>();
        mAnimator = GetComponent<Animator>();
        outline.enabled = true;
        outline.enabled = false;
        isOpenHash = Animator.StringToHash("isOpen");
    }

    public override void Show()
    {
        interactUiMessage.SetActive(true);
        outline.enabled = true;
    }

    public override void Hide()
    {
        interactUiMessage.SetActive(false);
        outline.enabled = false;
    }


    public override void HandleInteraction()
    {
        Debug.Log("PLAYER INTERACTED WITH AN OBJECT");
        if (Input.GetKeyDown(KeyCode.E))
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
        mAnimator.SetBool(isOpenHash, isOpen);
    }
}