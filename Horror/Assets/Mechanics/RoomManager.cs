using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInput;
    public TMP_InputField roomNameInputJoin;


    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            Debug.Log("Creating room: " + roomNameInput.text);
            PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
            Debug.Log("Creating room: " + roomNameInput.text);

        }
    }


    public void JoinRoom()
    {
        Debug.Log("AAAAAAAA");

        if (!string.IsNullOrEmpty(roomNameInputJoin.text))
        {
            Debug.Log("ASDASD");

            PhotonNetwork.JoinRoom(roomNameInputJoin.text);
        }
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("scena_lu_alex");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Create Room Failed: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Join Room Failed: " + message);
    }

}