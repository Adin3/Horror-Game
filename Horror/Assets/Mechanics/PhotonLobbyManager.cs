using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject connectingUI;
    public GameObject playMenu;

    public void ConnectToPhoton()
{
    if (PhotonNetwork.IsConnected)
    {
        Debug.Log("Already connected to Photon. Ignoring reconnect.");

        if (connectingUI != null)
            connectingUI.SetActive(false);
        
        if (playMenu != null)
            playMenu.SetActive(true);

        return;
    }

    Debug.Log("Connecting to Photon...");

    if (connectingUI != null)
        connectingUI.SetActive(true);

    PhotonNetwork.ConnectUsingSettings();
}



    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby.");

        if (connectingUI != null)
            connectingUI.SetActive(false);

        if (playMenu != null)
            playMenu.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected from Photon: " + cause.ToString());
    }
}
