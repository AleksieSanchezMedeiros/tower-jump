using UnityEngine;

public class BreakableFloor : MonoBehaviour
{
    [SerializeField] private float breakDelay = 1f;     // how long player must stay on it
    [SerializeField] private float respawnDelay = 3f;   // set to 0 if you don't want it to come back
    [SerializeField] private bool respawns = true; //should this platform be allowed to respawn?

    private Collider2D col;
    private SpriteRenderer sr; //using sprite renderer for visuals for now, im sure there's an animation to be made in the future

    private void Start()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    //start a timer when player steps on it
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // start breaking timer
            Invoke(nameof(Break), breakDelay);
        }
    }

    /// removed for now cause it kinda sucks if you step on it and then it doesnt break
    //cancel the timer if they leave early 
    /*
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // player left early
            CancelInvoke(nameof(Break));
        }
    }
    */

    //break the floor
    private void Break()
    {
        // Disable visuals & collision
        col.enabled = false;
        sr.enabled = false;

        //if this floor is allowed to respawn, set a timer for it to come back
        if (respawns)
        {
            Invoke(nameof(Respawn), respawnDelay);
        }
    }

    //frankenstein that gahdamn floor cuh
    private void Respawn()
    {
        col.enabled = true;
        sr.enabled = true;
    }
}
