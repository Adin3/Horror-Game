using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public GameObject interactUiMessage;
    private Outline outline;
    public LayerMask interactableLayer;
    public Camera playerCamera;

    void Start()
    {
        interactUiMessage.SetActive(false);
        outline = GetComponent<Outline>();
        outline.enabled = true;
        outline.enabled = false;
    }

    void Update()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f, interactableLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                //interactUiMessage.SetActive(true);
                outline.enabled = true;
            }
            else
            {
                //interactUiMessage.SetActive(false);
                outline.enabled = false;
            }
        }
        else
        {
            //interactUiMessage.SetActive(false);
            outline.enabled = false;
        }
    }
}
