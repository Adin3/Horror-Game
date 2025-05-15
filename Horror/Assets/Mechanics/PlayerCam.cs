using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
 
    public float sensX;
    public float sensY;
    public Transform orientation;

    float xRotation;
    float yRotation;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
