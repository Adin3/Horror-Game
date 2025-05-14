using UnityEngine;
using Photon.Pun;

public class door_panel_script : InteractableObject
{
    private Animator mAnimator;
    private bool isOpen = false;

    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }


    public override void HandleInteraction()
    {
        Debug.Log("Opening door...");
        ToggleDoor();
    }
   
    void ToggleDoor()
    {
        if (mAnimator != null)
        {
            isOpen = !isOpen;
            mAnimator.SetBool("isOpen", isOpen);
        }
    }
}