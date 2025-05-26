using System.Collections;
using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject runner;
    public GameObject seeker;
    [Space]
    public Transform spawnPoint;

    public bool isSeeker;

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
  
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Is Master Client: " + PhotonNetwork.IsMasterClient);
        isSeeker = PhotonNetwork.IsMasterClient;
        StartCoroutine(spawnPlayer());
    }

    IEnumerator spawnPlayer()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("Spawned " + spawnPoint.position);
        if (!isSeeker)
        {
            isSeeker = false;
            Debug.Log("Spawning Seeker named: [seeker.name]");
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
                if (camera != null)
                {
                    Debug.Log("Enabling camera for local player.");
                    camera.enabled = true; // Enable camera for local player
                }
                var playerController = pla.GetComponent<SeekerMovement>();
                if (playerController != null)
                {
                    Debug.Log("Enabling player controller for local player.");
                    playerController.enabled = true;
                    var playerAnimator = pla.GetComponent<Animator>();
                    playerController.animator = playerAnimator;
                }

                var playerInteraction = pla.GetComponent<PlayerInteraction>(); // Assuming you have a PlayerInteraction component
                if (playerInteraction != null)
                {
                    Debug.Log("Enabling player interaction for local player.");
                    playerInteraction.enabled = true;
                    playerInteraction.playerCam = camera; // Assign the camera to the player interaction script
                }
                var playerCam = cameraHolder.GetComponentInChildren<PlayerCam>();
                playerCam.enabled = true;
                playerCam.playerType = "Seeker";

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
            Transform cameraHolder = pla.transform.Find("CameraHolder");
            Camera camera = cameraHolder.GetComponentInChildren<Camera>();
            PhotonView view = pla.GetComponent<PhotonView>();

            if (view.IsMine)
            {
                camera.enabled = true; // Enable camera for local player
                // Enable player controls only for the local player
                var playerController = pla.GetComponent<PlayerMovement>(); // Assuming you have a PlayerController component
                playerController.enabled = true;
                var playerAnimator = pla.GetComponent<Animator>();
                playerController.animator = playerAnimator;

                var playerInteraction = pla.GetComponent<PlayerInteraction>(); // Assuming you have a PlayerInteraction component
                playerInteraction.enabled = true;
                playerInteraction.playerCam = camera; // Assign the camera to the player interaction script

                var playerCam = cameraHolder.GetComponentInChildren<PlayerCam>();
                playerCam.enabled = true;
                playerCam.playerType = "Runner";
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