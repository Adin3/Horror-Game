using UnityEngine;

public class door_panel_script : MonoBehaviour
{
    private Animator mAnimator;
    private bool isOpen = false;

    public float interactDistance = 3f; // maximum interaction distance
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // Create a ray from the center of the screen
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // Check if this door panel is the one being looked at
            if (hit.collider.gameObject == gameObject)
            {
                // Listen for E key press
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ToggleDoor();
                }
            }
        }
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