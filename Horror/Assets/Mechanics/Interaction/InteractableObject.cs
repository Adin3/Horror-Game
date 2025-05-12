using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public GameObject interactUiMessage;
    private Outline outline;

    void Start()
    {
        interactUiMessage.SetActive(true);
        interactUiMessage.SetActive(false);
        outline = GetComponent<Outline>();
        outline.enabled = true;
        outline.enabled = false;
    }

    public void Show()
    {
        interactUiMessage.SetActive(true);
        outline.enabled = true;
    }

    public void Hide()
    {
        interactUiMessage.SetActive(false);
        outline.enabled = false;
    }
}
