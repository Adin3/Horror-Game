using UnityEngine;

public class ScrewInteractionManager : MonoBehaviour
{
    public float interactDistance = 3f;
    public VentManager ventManager;

    void Update()
    {
        if (!ventManager.canUnscrew) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (Input.GetMouseButtonDown(0)) // Left mouse click
            {
                Screw screw = hit.collider.GetComponent<Screw>();
                if (screw != null && !screw.isRemoved)
                {
                    screw.Remove();
                    ventManager.ScrewRemoved();
                }
            }
        }
    }
}
