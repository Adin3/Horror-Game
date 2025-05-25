using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCam : MonoBehaviourPun
{
    public float sensX;
    public float sensY;
    public Transform orientation;
    public Transform playerBody;
    public Light flashLight;

    [HideInInspector]
    public string playerType;

    float xRotation;
    float yRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            int playerLayer = LayerMask.NameToLayer(playerType);
            if (playerLayer != -1)
            {
                cam.cullingMask &= ~(1 << playerLayer);
            }
            else
            {
                Debug.LogError("Monster layer not found! Make sure you created the layer.");
            }
        }
        else
        {
            Debug.LogError("No Camera component found! Make sure this script is attached to a Camera.");
        }
    }

    void Update()
    {
        if (PauseMenu.isPaused) return;

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -89.999f, 89.999f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        playerBody.rotation = Quaternion.Euler(0, yRotation, 0);

        photonView.RPC("SyncBodyRotation", RpcTarget.Others, yRotation);

        if (photonView.IsMine && flashLight != null)
        {
            photonView.RPC("SyncFlashlightRotation", RpcTarget.Others, flashLight.transform.rotation);
        }
    }

    [PunRPC]
    void SyncBodyRotation(float rotationY)
    {
        if (!photonView.IsMine)
        {
            playerBody.rotation = Quaternion.Euler(0, rotationY, 0);
        }
    }

    [PunRPC]
    void SyncFlashlightRotation(Quaternion rot)
    {
        if (!photonView.IsMine && flashLight != null)
        {
            flashLight.transform.rotation = rot;
        }
    }
}
