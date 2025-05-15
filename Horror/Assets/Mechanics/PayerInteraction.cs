using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInteraction : MonoBehaviour
{
    public float playerReach = 3f;
    public Camera playerCam = null;
    private InteractableObject currentInteractable;
    PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();

    }

    private void Update()
    {
        if (view.IsMine)
        {
            if (playerCam == null)
            {
                return;
            }
            Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 3f))
            {
                InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();

                if (interactable != null)
                {
                    if (interactable != currentInteractable)
                    {
                        if (currentInteractable != null)
                            currentInteractable.Hide();

                        currentInteractable = interactable;
                        currentInteractable.Show();
                    }
                    currentInteractable.HandleInteraction();
                }
                else if (currentInteractable != null)
                {
                    currentInteractable.Hide();
                    currentInteractable = null;
                }
            }
            else if (currentInteractable != null)
            {
                currentInteractable.Hide();
                currentInteractable = null;
            }
        }
    }
}
