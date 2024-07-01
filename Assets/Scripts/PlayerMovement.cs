using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float climbSpeed;

    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    [SerializeField] float dashCooldown;

    private bool canDash = true;
    private bool isDashing;
    float gravity;


    Vector2 moveInput;
    Rigidbody2D rb;
    CapsuleCollider2D cc;
    BoxCollider2D bc;
    Animator animator;
    TrailRenderer tr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        bc = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        tr = GetComponent<TrailRenderer>();
        gravity = rb.gravityScale;
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }
        Run();
        FlipSprite();
        ClimbLadder();
    }

    void FixedUpdate()
    {
        if (isDashing) { return; }
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        animator.SetBool("isRunning", playerHasHorizontalSpeed);

    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
        }
    }

    void OnJump(InputValue value)
    {
        if (!bc.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }
        if (value.isPressed)
        {
            rb.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    private IEnumerator OnDash()
    {
        if (canDash)
        {
            canDash = false;
            isDashing = true;
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0f);
            tr.emitting = true;
            yield return new WaitForSeconds(dashTime);
            tr.emitting = false;
            rb.gravityScale = gravity;
            isDashing = false;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }

    void ClimbLadder()
    {
        if (!cc.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            rb.gravityScale = gravity;

            animator.SetBool("isClimbing", false);

            return;
        }

        Vector2 climbLadder = new Vector2(rb.velocity.x, moveInput.y * climbSpeed);
        rb.velocity = climbLadder;
        rb.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(moveInput.y) > Mathf.Epsilon;
        animator.SetBool("isClimbing", playerHasVerticalSpeed);
    }
}
