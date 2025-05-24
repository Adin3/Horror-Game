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
            // This will keep all current layers but remove the Monster layer
            int playerLayer = LayerMask.NameToLayer(playerType);
            if (playerLayer != -1) // Make sure the layer exists
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

        if (PauseMenu.isPaused)
        {
            return; 
        }

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        //pus limita la 89.99 ca sa nu se mai blocheze cand se uita fix in jos/sus
        xRotation = Mathf.Clamp(xRotation, -89.999f, 89.999f);

        //rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        //camHolder.localRotation = Quaternion.Euler(0, yRotation, 0);

        playerBody.rotation = Quaternion.Euler(0, yRotation, 0);
        photonView.RPC("SyncBodyRotation", RpcTarget.Others, yRotation);
        //camHolder.rotation = Quaternion.Euler(0, yRotation, 0);

    }
    [PunRPC]
    void SyncBodyRotation(float rotationY)
    {
        // Apply the body rotation for remote players (without affecting their camera)
        if (!photonView.IsMine)
        {
            Debug.Log("Applied ROTATION" + rotationY);
            playerBody.rotation = Quaternion.Euler(0, rotationY, 0);
        }
    }
}
