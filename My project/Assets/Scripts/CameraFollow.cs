using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float yOffset;
    [SerializeField] private float minCameraY = 0f; // lowest the camera is allowed to go

    private float highestYReached = 0f;

    private void LateUpdate()
    {
        if (player == null){
            Debug.LogWarning("Player transform is not assigned in CameraFollow script.");
            return;
        }

        float targetY = player.position.y + yOffset;

        // update highest reached
        if (targetY > highestYReached)
            highestYReached = targetY;

        // allow camera to go down but not below minimum
        float newY = Mathf.Clamp(targetY, minCameraY, highestYReached);

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}