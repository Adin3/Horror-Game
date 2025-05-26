using UnityEngine;
using Photon.Pun;

public enum ItemType
{
    Trap,   // Harmful items
    WCond   // Win condition items
}

public class CollectibleItem : InteractableObject
{
    public GameObject interactUiMessage;
    public ItemType itemType;    // Set this in the inspector
    public int value = 1;        // Value/damage amount

    private Outline outline;
    private bool isCollected = false;

    void Start()
    {
        // Setup UI message
        interactUiMessage.SetActive(true);
        interactUiMessage.SetActive(false);

        // Setup outline
        outline = GetComponent<Outline>();
        outline.enabled = true;
        outline.enabled = false;
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
        if (Input.GetKeyDown(KeyCode.E) && !isCollected)
        {
            CollectItem();
        }
    }

    void CollectItem()
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC(nameof(RPC_CollectItem), RpcTarget.AllBuffered);
        }
        else
        {
            RPC_CollectItem();
        }
    }

    [PunRPC]
    private void RPC_CollectItem()
    {
        if (isCollected) return; // Prevent double collection

        isCollected = true;



        // Find the player's inventory
        PlayerInventory inventory = GameObject.FindGameObjectWithTag("Runner")?.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            Debug.Log("INVENTORY IS NOT NULL");
            inventory.CollectItem(itemType, value);
        } else
        {
            Debug.Log("INVENTORY IS NULL");
        }


        // Hide UI elements
        Hide();

        // Destroy the object
        if (PhotonNetwork.IsConnected && photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else if (!PhotonNetwork.IsConnected)
        {
            Destroy(gameObject);
        }
    }
}