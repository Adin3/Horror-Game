using System.Collections;
using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject runner;
    public GameObject seeker;
    [Space]
    public Transform spawnPoint;

    public bool isSeeker = true;

    private GameObject _player;

    void Start()
    {
        Debug.Log("Connecting");
        PhotonNetwork.ConnectUsingSettings();
    }

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
        if (isSeeker)
        {
            _player = PhotonNetwork.Instantiate(seeker.name, spawnPoint.position, Quaternion.identity);
            if (_player == null)
            {
                Debug.LogError("Player object is null after instantiation.");
                yield break; // Exit the coroutine if instantiation failed
            }
            Transform pla = _player.transform.Find("Player");
            Transform cameraHolder = pla.transform.Find("CameraHolder");
            Camera camera = cameraHolder.GetComponentInChildren<Camera>();
            PhotonView view = pla.GetComponent<PhotonView>();

            if (view.IsMine)
            {
                // Enable player controls only for the local player
                var playerController = pla.GetComponent<PlayerMovement>(); // Assuming you have a PlayerController component
                if (playerController != null)
                {
                    Debug.Log("Enabling player controller for local player.");
                    playerController.enabled = true;
                }

                var playerInteraction = pla.GetComponent<PlayerInteraction>(); // Assuming you have a PlayerInteraction component
                if (playerInteraction != null)
                {
                    playerInteraction.enabled = true;
                    playerInteraction.playerCam = camera; // Assign the camera to the player interaction script
                }
                if (camera != null)
                {
                    Debug.Log("Enabling camera for local player.");
                    camera.enabled = true; // Enable camera for local player
                }
            }
            else
            {
                // Disable controls for remote players
                var playerController = pla.GetComponent<PlayerMovement>(); // Assuming you have a PlayerController component
                if (playerController != null)
                {
                    playerController.enabled = false;
                }
            }
        }
        else
        {
            _player = PhotonNetwork.Instantiate(runner.name, spawnPoint.position, Quaternion.identity);

            Transform pla = _player.transform.Find("Player");
            Transform cameraHolder = _player.transform.Find("CameraHolder");
            Camera camera = cameraHolder.GetComponentInChildren<Camera>();
            PhotonView view = pla.GetComponent<PhotonView>();

            if (view.IsMine)
            {
                // Enable player controls only for the local player
                var playerController = pla.GetComponent<PlayerMovement>(); // Assuming you have a PlayerController component
                if (playerController != null)
                {
                    playerController.enabled = true;
                }

                var playerInteraction = pla.GetComponent<PlayerInteraction>(); // Assuming you have a PlayerInteraction component
                if (playerInteraction != null)
                {
                    playerInteraction.enabled = true;
                    playerInteraction.playerCam = camera; // Assign the camera to the player interaction script
                }
                if (camera != null)
                {
                    camera.enabled = true; // Enable camera for local player
                }
            }
            else
            {
                // Disable controls for remote players
                var playerController = pla.GetComponent<PlayerMovement>(); // Assuming you have a PlayerController component
                if (playerController != null)
                {
                    playerController.enabled = false;
                }
            }
        }
    }
}