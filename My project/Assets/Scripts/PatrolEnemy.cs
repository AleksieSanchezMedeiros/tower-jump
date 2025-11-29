using UnityEngine;
using System.Collections;

public class PatrolEnemy : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float xVelocity = 1.2f;
    bool isGoingRight;
    bool isStunned;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Hitbox.OnStun += Stun;
        Hitbox.OnDeath += Die;
        isStunned = false;
    }

    void OnDisable()
    {
        StopAllCoroutines();
        Hitbox.OnStun -= Stun;
        Hitbox.OnDeath -= Die;
    }

    // Go in one steady direction, switch left from right accordingly.
    void Update()
    {
        if (!isStunned)
        {
            Vector3 moveDirect = (isGoingRight) ? transform.right : -transform.right;
            moveDirect *= Time.deltaTime * xVelocity;
            transform.Translate(moveDirect);
        }
    }
    // Check for bound collision, bounds are what define the patrol area
    void OnTriggerEnter2D(Collider2D other)
    {
        string name = other.name;
        if (name.Contains("Bounds"))
        {
            isGoingRight = !isGoingRight;
            transform.Rotate(0, 180, 0);
        }
    }

    void Stun()
    {
        Debug.Log("Stunned");
        StartCoroutine(InflictStun());
    }

    void Die()
    {
        Destroy(gameObject);
    }

    IEnumerator InflictStun()
    {
        float timer = 2f;
        isStunned = true;
        while (timer > 0)
        {
            timer -= 1f;
            yield return new WaitForSeconds(1f);
        }
        isStunned = false;
    }

}

