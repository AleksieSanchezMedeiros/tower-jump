using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RockHazard : MonoBehaviour
{
    public enum State
    {
        Falling,
        Rolling
    }

    [SerializeField] private State defaultState;
    public State state;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdateState(defaultState);
    }

    private void UpdateState(State newState)
    {
        state = newState;

        switch (state) {
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
        yield return null;
    }

    private IEnumerator RollingSequence()
    {
        yield return null;
    }
}
