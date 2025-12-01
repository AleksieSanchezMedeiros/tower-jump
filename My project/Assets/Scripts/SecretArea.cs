using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class SecretArea : MonoBehaviour
{
    [SerializeField] private float fadeZone = 0.5f;
    [SerializeField] private float minAlpha = 0.1f;


    private SpriteRenderer sr;
    private Color originalColor;
    private Transform player;
    private float halfWidth;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        halfWidth = sr.bounds.size.x / 2;
    }

    private void Update()
    {
        if (!player) return; // Don't do anything is the player is out of range

        HandleAreaOpacity();
    }

    private void HandleAreaOpacity()
    {
        float distanceFromCenter = Mathf.Abs(transform.position.x - player.position.x);
        float distanceFromEdge = halfWidth - distanceFromCenter;

        if (distanceFromEdge <= 0) {
            SetAlpha(originalColor.a);
            return;
        }

        float t = distanceFromEdge / fadeZone;
        float alpha = Mathf.Lerp(originalColor.a, minAlpha, t);

        SetAlpha(alpha);
    }

    private void SetAlpha(float a)
    {
        Color color = sr.color;
        color.a = a;
        sr.color = color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            player = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && player == other.transform) {
            player = null;
            SetAlpha(originalColor.a);
        }
    }
}
