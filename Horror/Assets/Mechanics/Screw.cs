using UnityEngine;
using Photon.Pun;

public class Screw : InteractableObject
{
    public bool isRemoved = false;
    public VentManager ventManager;
    public LayerMask interractableLayer;
    public GameObject interactUiMessage;
    private Outline outline;

    void Start()
    {
        if (!isRemoved)
        {
            interactUiMessage.SetActive(true);
            interactUiMessage.SetActive(false);
            outline = GetComponent<Outline>();
            outline.enabled = true;
            outline.enabled = false;
        }
    }

    public override void Show()
    {
        if (isRemoved) return;
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
        if (isRemoved) return;
        if (Input.GetMouseButtonDown(0) && ventManager.canUnscrew)
            removeScrew();
    }

    void removeScrew()
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC(nameof(RPC_Remove), RpcTarget.AllBuffered);
        }
        else
        {
            RPC_Remove();
        }
        ventManager.ScrewRemoved();
    }

    [PunRPC]
    public void RPC_Remove()
    {
        isRemoved = true;
        gameObject.SetActive(false); // Hide the screw
    }
}
