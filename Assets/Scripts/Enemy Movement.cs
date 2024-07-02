using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float enemySpeed;
    Rigidbody2D rb;
    CapsuleCollider2D cc;
    BoxCollider2D bc;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        rb.velocity = new Vector2(enemySpeed, 0f);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        enemySpeed = -enemySpeed;
        FlipEnemyFacing();
    }

    void FlipEnemyFacing()
    {
        transform.localScale = new Vector2(-Mathf.Sign(rb.velocity.x), 1f);
    }
}
