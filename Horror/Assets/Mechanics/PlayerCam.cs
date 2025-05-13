using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCam : MonoBehaviourPun
{
 
    public float sensX;
    public float sensY;
    public Transform orientation;
    private PhotonView photonView;

    float xRotation;
    float yRotation;
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            this.enabled = false;
            return;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            //this.enabled = false;
            return;
        }
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
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

    }

    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    if (hit.gameObject.CompareTag("Obstacle"))
    //    {
    //        Debug.Log("Collided.");
    //    }
    //}
}
