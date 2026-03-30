using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
public class PlayerInventory : MonoBehaviourPunCallbacks
{
    private int wCondCollected = 0;
    private const int WCOND_TARGET = 5;

    public GameObject collectedItemsUI;
    [SerializeField] public TextMeshProUGUI wCondText; 

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

            collectedItemsUI.SetActive(true);
            if (wCondText != null)
            {
                wCondText.text = $"Items collected:\n {wCondCollected}/{WCOND_TARGET}";
                StartCoroutine(ActivateAndDeactivate());
            }
            else
            {
                Debug.LogWarning("wCondText is not assigned in the inspector.");
            }


            if (wCondCollected >= WCOND_TARGET)
            {
                Debug.Log("Congratulations! You've collected 5 WCond items!");
            }

        }
    }

    public int GetWCondCollected() => wCondCollected;

    private IEnumerator ActivateAndDeactivate()
    {
        collectedItemsUI.SetActive(true);       
        yield return new WaitForSeconds(3f);
        collectedItemsUI.SetActive(false);   
    }
}
