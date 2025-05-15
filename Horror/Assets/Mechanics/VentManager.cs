using UnityEngine;
using Photon.Pun;

public class VentManager : MonoBehaviourPun
{
    public bool canUnscrew = false; // de pus conditie daca are surubelnita
    public int totalScrews = 4;
    private int screwsRemoved = 0;
    public GameObject vent;

    public void ScrewRemoved()
    {
        screwsRemoved++;
        if (screwsRemoved >= totalScrews)
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log($"[VentManager] All screws removed, removing vent.");
                photonView.RPC(nameof(RemoveVent), RpcTarget.AllBuffered);
            }
            else
            {
                RemoveVent();
            }
        }
    }
    [PunRPC]
    private void RemoveVent()
    {
        Debug.Log($"[VentManager] All screws removed, removing vent.");
        vent.SetActive(false); // Disable the vent object
    }
}
