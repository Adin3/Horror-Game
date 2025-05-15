using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInteraction : MonoBehaviour
{
    public float playerReach = 3f;
    public Camera playerCam = null;
    PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();

    }

    private void Update()
    {
        if (view.IsMine)
        {
            Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 3f))
            {
                InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
                if (interactable != null)
                {
                    interactable.Show(); // This handles the correct override via polymorphism
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        interactable.HandleInteraction();
                    }
                }

            }
        }

    }

    //void CheckInteraction()
    //{
    //    RaycastHit hit;
    //    Ray ray = new Ray(playerCam.transform.position, Camera.main.transform.forward);

    //    if (Physics.Raycast(ray, out hit, playerReach))
    //    {
    //        if (hit.collider.tag == "Interactable")
    //        {
    //            Interactable newInteractable = hit.collider.GetComponent<Interactable>();

    //            if (currentInteractable && newInteractable != currentInteractable)
    //            {
    //                currentInteractable.DisableOutline();
    //            }

    //            if (newInteractable.enabled)
    //            {
    //                SetNewCurrentInteractable(newInteractable);
    //            }
    //            else
    //            {
    //                DisableCurrentInteractable();
    //            }
    //        }
    //        else
    //        {
    //            DisableCurrentInteractable();
    //        }
    //    }
    //    else
    //    {
    //        DisableCurrentInteractable();
    //    }
    //}

    //void SetNewCurrentInteractable(Interactable newInteractable)
    //{
    //    currentInteractable = newInteractable;
    //    currentInteractable.EnableOutline();
    //}

    //void DisableCurrentInteractable()
    //{
    //    if (currentInteractable)
    //    {
    //        currentInteractable.DisableOutline();
    //        currentInteractable = null;
    //    }
    //}
}