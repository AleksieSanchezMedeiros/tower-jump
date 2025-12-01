using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //changed this up for smoother following
    //yuh

    // references
    [SerializeField] private Transform player;
    [SerializeField] private float yOffset = 2f;
    [SerializeField] private float minCameraY = 0f; //dont go below 0

    private float highestYReached = 0f;
    private float velocityY = 0f;

    private void LateUpdate()
    {
        // safety check
        if (player == null)
        {
            Debug.LogWarning("Player transform is not assigned in CameraFollow script.");
            return;
        }

        // target camera Y
        float targetY = player.position.y + yOffset;

        // update highest reached but allow downward movement smoothly
        if (targetY > highestYReached)
            highestYReached = targetY;

        // clamp so camera never goes below min or above highest reached
        float clampedY = Mathf.Clamp(targetY, minCameraY, highestYReached);

        // smooth camera movement using SmoothDamp
        float newY = Mathf.SmoothDamp(transform.position.y, clampedY, ref velocityY, 0.1f);

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}