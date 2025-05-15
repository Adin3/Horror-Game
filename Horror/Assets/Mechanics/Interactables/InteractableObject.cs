using UnityEngine;
using Photon.Pun;

public class InteractableObject : MonoBehaviourPun
{
    // This is overridden in child classes
    public virtual void HandleInteraction()
    {
        Debug.Log($"[Base] Interacted with {gameObject.name}");
    }

    public virtual void Show()
    {
        // Show outline and UI
    }
}
