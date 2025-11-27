using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RockHazard : MonoBehaviour
{
    public enum State
    {
        None,
        Falling,
        Rolling
    }

    [SerializeField] private State defaultState;
    public State currentState;

    [SerializeField] private float fallSpeed = 10f;
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.5f);
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Space(10)]
    [SerializeField] private float rollSpeed = 5f;
    [SerializeField] private float rotationMultiplier = 10f;
    [SerializeField] private int rollDirection = 1; // 1 = right, -1 = left

    [Space(10)]
    [SerializeField] private float lifetime = 15f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdateState(defaultState);
    }

    private void UpdateState(State newState)
    {
        StopAllCoroutines(); // Stop any previous state behaviour before starting another
        currentState = newState;

        switch (currentState) {
            case State.None:
                break;
            case State.Falling:
                StartCoroutine(FallingSequence());
                break;
            case State.Rolling:
                StartCoroutine(RollingSequence());
                break;
            default:
                break;
        }
    }

    private IEnumerator FallingSequence()
    {
        rb.gravityScale = 1f;

        while (currentState == State.Falling) {
            rb.linearVelocity = new Vector2(0f, -fallSpeed);
            if (IsGrounded()) {
                rb.linearVelocity = Vector2.zero;
                UpdateState(State.Rolling);
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator RollingSequence()
    {
        rb.gravityScale = 1f; // Sorta repetitive but might adjust implementation to require this

        while (currentState == State.Rolling) {
            rb.AddForce(new Vector2(rollSpeed * rollDirection, 0f), ForceMode2D.Force);
            
            // Cap the max roll speed
            if (Mathf.Abs(rb.linearVelocity.x) > rollSpeed) {
                rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * rollSpeed, rb.linearVelocity.y);
            }

            transform.Rotate(0f, 0f, rb.linearVelocity.x * rotationMultiplier * -rollDirection * Time.deltaTime);

            yield return null;
        }
    }

    private bool IsGrounded()
    {
        Vector2 origin = (Vector2)transform.position + groundCheckOffset; // below the boulder
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);

        return hit.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            UpdateState(State.Falling);
            Destroy(transform.GetChild(0).gameObject);
        }
    }
}
