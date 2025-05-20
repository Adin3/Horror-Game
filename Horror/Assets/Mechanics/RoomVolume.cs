using UnityEngine;

public class RoomVolume : MonoBehaviour
{
    [Tooltip("Unique identifier for this room")]
    public int roomId;

    private void OnDrawGizmos()
    {
        // Visualize the room volume in the editor
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}