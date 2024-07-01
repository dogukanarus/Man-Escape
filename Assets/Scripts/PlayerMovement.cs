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
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    [SerializeField] float dashCooldown;

    private bool canDash = true;
    private bool isDashing;


    Vector2 moveInput;
    Rigidbody2D rgbd;
    CapsuleCollider2D capsuleCollider;
    Animator animator;
    TrailRenderer tr;

    void Start()
    {
        rgbd = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        tr = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }
        Run();
        FlipSprite();
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
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rgbd.velocity.y);
        rgbd.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(rgbd.velocity.x) > Mathf.Epsilon;
        animator.SetBool("isRunning", playerHasHorizontalSpeed);

    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rgbd.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rgbd.velocity.x), 1f);
        }
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            rgbd.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    private IEnumerator OnDash()
    {
        canDash = false;
        isDashing = true;
        float orginalGravity = rgbd.gravityScale;
        rgbd.gravityScale = 0f;
        rgbd.velocity = new Vector2(transform.localScale.x * dashSpeed, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashTime);
        tr.emitting = false;
        rgbd.gravityScale = orginalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
