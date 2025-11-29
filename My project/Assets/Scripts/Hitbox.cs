using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hitbox : MonoBehaviour
{
    Rigidbody2D rb;
    Vector3 direction;
    GameObject enemy;
    [Header("Hitbox properties")]
    public int riccochet = 10;
    public int hitCount { get; private set; } = 0;
    [SerializeField] HitState state;

    public delegate void HitEvent();
    public static event HitEvent OnDeath;
    public static event HitEvent OnStun;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        direction = Vector3.up;
        enemy = transform.parent.gameObject;
        state = HitState.Alive;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Make player shoot up in air
            GameObject player = collision.gameObject;
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            Vector2 bounceDirection = direction * riccochet;
            playerRb.linearVelocity = bounceDirection;

            hitCount++;
            CheckHits();
        }
    }

    private void CheckHits()
    {
        if (hitCount == 1)
        {
            StunEvent();
        }
        else if (hitCount >= 2) {
            DeathEvent();
        }
    }

    public void DeathEvent()
    {
        OnDeath?.Invoke();
        state = HitState.Dead;
    }

    public void StunEvent()
    {
        OnStun?.Invoke();
        state = HitState.Stunned;
    }
}

public enum HitState
{
    Alive,
    Stunned, // Hit once
    Dead     // Hit twice
}