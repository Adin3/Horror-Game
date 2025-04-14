using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public GameObject interactUiMessage;
    private Outline outline;

    void Start()
    {
        interactUiMessage.SetActive(false);
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f))
        {
            if (hit.collider.gameObject == gameObject)
            {
                interactUiMessage.SetActive(true);
                outline.enabled = true;
            }
            else
            {
                interactUiMessage.SetActive(false);
                outline.enabled = false;
            }
        }
        else
        {
            interactUiMessage.SetActive(false);
            outline.enabled = false;
        }
    }
}
