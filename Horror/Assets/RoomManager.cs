using System.Collections;

using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{

    public GameObject player;
    [Space]
    public Transform spawnPoint;
    private GameObject _player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Debug.Log("Connecting");

        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("Connected");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        PhotonNetwork.JoinOrCreateRoom("Lobby", null, null);

        Debug.Log("We joined a room");

        StartCoroutine(spawnPlayer());
    }

    IEnumerator spawnPlayer()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Spawned " + spawnPoint.position);

        _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);

        // Access the CameraHolder and find the PlayerCam
        //Transform cameraHolder = _player.transform.Find("CameraHolder");
        //if (cameraHolder != null)
        //{
        //    Camera playerCam = cameraHolder.GetComponentInChildren<Camera>(); // Get the camera inside CameraHolder
        //    if (playerCam != null)
        //    {
        //        playerCam.enabled = true; // Enable the camera
        //    }
        //    else
        //    {
        //        Debug.LogWarning("PlayerCam not found inside CameraHolder");
        //    }
        //}
        //else
        //{
        //    Debug.LogWarning("CameraHolder not found on the player");
        //}
    }

}