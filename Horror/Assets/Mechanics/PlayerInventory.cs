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

    [SerializeField] private TextMeshProUGUI wCondText; 

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


            if (wCondText != null)
            {
                wCondText.text = $"Items collected:\n w{wCondCollected}/{WCOND_TARGET}";
                StartCoroutine(ActivateAndDeactivate());
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
