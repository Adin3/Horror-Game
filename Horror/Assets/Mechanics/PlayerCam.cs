using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
 
    public float sensX;
    public float sensY;
    public Transform orientation;
    public Transform playerBody;
    public Transform camHolder;
    public LayerMask seeker;

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
            int monsterLayer = LayerMask.NameToLayer("Seeker");
            if (monsterLayer != -1) // Make sure the layer exists
            {
                cam.cullingMask &= ~(1 << monsterLayer);
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

        playerBody.rotation = Quaternion.Euler(-90, yRotation, 0);
        //camHolder.rotation = Quaternion.Euler(0, yRotation, 0);

    }

    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    if (hit.gameObject.CompareTag("Obstacle"))
    //    {
    //        Debug.Log("Collided.");
    //    }
    //}
}
