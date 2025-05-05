using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private InteractableObject currentInteractable;

    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
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
