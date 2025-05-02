using UnityEngine;

public class VentManager : MonoBehaviour
{
    public bool canUnscrew = false; // de pus conditie daca are surubelnita
    public GameObject ventCover;
    public int totalScrews = 4;
    private int screwsRemoved = 0;

    public void ScrewRemoved()
    {
        screwsRemoved++;
        if (screwsRemoved >= totalScrews)
        {
            Rigidbody rb = ventCover.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Let it fall
            }
        }
    }
}
