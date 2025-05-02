using UnityEngine;

public class Screw : MonoBehaviour
{
    public bool isRemoved = false;
    public float interactDistance = 3f;
    public VentManager ventManager;
    public LayerMask interractableLayer;

    public void Remove()
    {
        isRemoved = true;
        gameObject.SetActive(false); // Hide the screw
    }
    void Update()
    {
        if (!ventManager.canUnscrew) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interractableLayer))
        {
            if (Input.GetMouseButtonDown(0) && hit.collider.gameObject == gameObject) // Left mouse click
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
