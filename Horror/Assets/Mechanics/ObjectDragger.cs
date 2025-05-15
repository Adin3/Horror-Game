using UnityEngine;
using System.Collections;

public class ObjectDragger : MonoBehaviour
{
    public float grabDistance = 3f;
    public float moveSpeed = 10f;
    public LayerMask draggableLayer;
    public Camera playerCam = null;

    private Rigidbody grabbedRigidbody;
    private Vector3 grabOffset;
    private Coroutine dragCoroutine;

    void Update()
    {
        // Start dragging when left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, grabDistance, draggableLayer))
            {

                if (hit.rigidbody != null && !hit.rigidbody.isKinematic)
                {
                    grabbedRigidbody = hit.rigidbody;


                    grabOffset = hit.point - grabbedRigidbody.transform.position;
                    grabbedRigidbody.useGravity = false;
                    grabbedRigidbody.linearDamping = 10f;

                    dragCoroutine = StartCoroutine(DragObject());
                }
            }
        }

        // Stop dragging when left mouse button is released
        if (Input.GetMouseButtonUp(0) && grabbedRigidbody != null)
        {
            StopCoroutine(dragCoroutine);
            grabbedRigidbody.useGravity = true;
            grabbedRigidbody.linearDamping = 0f;
            grabbedRigidbody = null;
        }
    }

    IEnumerator DragObject()
    {
        while (Input.GetMouseButton(0))
        {
            Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Vector3 targetPoint = ray.origin + ray.direction * grabDistance - grabOffset;
            Vector3 force = (targetPoint - grabbedRigidbody.position) * moveSpeed;
            grabbedRigidbody.linearVelocity = force;

            yield return null; // Wait for next frame
        }
    }
}
