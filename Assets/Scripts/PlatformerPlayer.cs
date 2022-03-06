using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformerPlayer : PlatformerPhysics
{
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float runInfluence = 1.65f;
    [SerializeField] float jumpForce = 4.5f;
    [SerializeField] float releasedJumpInfluence = 0.5f;
    [SerializeField] float slowFallInfluence = 0.5f;
    [SerializeField] float jumpSpeedInfluence = 0.30f; // Jump height is multiplied by 1.0 + (this) * (currentSpeed / maxSpeed), so that you jump higher when running

    SpriteRenderer spriteRenderer;
    Animator animator;

    Vector2 trueGravity;
    float velocityX;
    float currentMaxSpeed;

    Vector2 moveInput = new Vector2();
    bool jumpDown;
    public void OnMove(InputValue value) { moveInput = value.Get<Vector2>(); }
    public void OnRun(InputValue value)
    {
        if (value.Get<float>() > 0.1f) currentMaxSpeed = maxSpeed * runInfluence;
        else currentMaxSpeed = maxSpeed;
    }
    public void OnJump(InputValue value)
    {
        if (value.Get<float>() > 0.1f && isGrounded && !jumpDown)
        {
            jumpDown = true;
            velocity = new Vector2(velocity.x, jumpForce * (1.0f + (jumpSpeedInfluence * (Mathf.Abs(velocity.x) / (maxSpeed * runInfluence)))));
            gravity *= slowFallInfluence;
        }
        else if (value.Get<float>() < 0.1f)
        {
            if (!isGrounded && jumpDown && velocity.y > 0.0f)
            {
                gravity = trueGravity;
                velocity = new Vector2(velocity.x, velocity.y * releasedJumpInfluence);
            }
            jumpDown = false;
        }
    }

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        trueGravity = gravity;
        currentMaxSpeed = maxSpeed;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        if (gravity != trueGravity && velocity.y < 0.0f && !isGrounded) gravity = trueGravity;
    }

    protected override void SetTargetVelocity()
    {
        velocityX = velocity.x;

        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            if (moveInput.x > 0.0f) velocityX += acceleration * Time.deltaTime;
            else velocityX -= acceleration * Time.deltaTime;

            if (Mathf.Abs(velocityX) > currentMaxSpeed) velocityX = (velocityX / Mathf.Abs(velocityX)) * currentMaxSpeed;
        }
        if (isGrounded)
        {
            if (velocityX > 0.0f && moveInput.x < 0.1f)
            {
                velocityX -= deceleration * Time.deltaTime;
                if (velocityX < 0.0f) velocityX = 0.0f;
            }
            else if (velocityX < 0.0f && moveInput.x > -0.1f)
            {
                velocityX += deceleration * Time.deltaTime;
                if (velocityX > 0.0f) velocityX = 0.0f;
            }
        }

        velocity = new Vector2(velocityX, velocity.y);

        // Visual code
        animator.SetBool("isGrounded",  isGrounded);
        if (!animator.GetBool("isTurning")) animator.SetBool("isTurning", moveInput.x * velocity.x < 0 && Mathf.Abs(velocity.x) > 2.0f);
        else animator.SetBool("isTurning", moveInput.x * velocity.x < 0);
        animator.SetBool("isMoving", Mathf.Abs(moveInput.x) > 0.1f || Mathf.Abs(velocity.x) > 0.1f);
        if (jumpDown && !isGrounded) animator.SetBool("inJump", true); else if (isGrounded) animator.SetBool("inJump", false);
        animator.speed = isGrounded ? Mathf.Abs(velocity.x) / maxSpeed * 1.5f : 0.0f;
        if (isGrounded && Mathf.Abs(velocity.x) > 0.1f) spriteRenderer.flipX = (velocity.x < 0.0f);
    }
}
