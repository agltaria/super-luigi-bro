using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformerPlayer : PlatformerPhysics
{
    public enum MarioForm { Small, Big, Fire }
    public MarioForm currentForm;
    public int fireBalls = 2;
    [SerializeField] GameObject fireBall;

    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float runInfluence = 1.65f;
    [SerializeField] float jumpForce = 4.5f;
    [SerializeField] float bounceForce = 2.5f;
    [SerializeField] float releasedJumpInfluence = 0.5f;
    [SerializeField] float slowFallInfluence = 0.5f;
    [SerializeField] float jumpSpeedInfluence = 0.30f; // Jump height is multiplied by 1.0 + (this) * (currentSpeed / maxSpeed), so that you jump higher when running

    SpriteRenderer spriteRenderer;
    Animator animator;
    [SerializeField] SpriteRenderer throwSpriteRenderer;
    [SerializeField] SpriteMask spriteMask;

    Vector2 trueGravity;
    float velocityX;
    float currentMaxSpeed;
    bool flipSpriteBuffered;
    bool isCrouching;
    Vector2 moveInput = new Vector2();
    bool jumpDown;
    bool runDown;
    float throwCoolDown;

    public void OnMove(InputValue value) { moveInput = value.Get<Vector2>(); }
    public void OnRun(InputValue value)
    {
        if (value.Get<float>() > 0.1f)
        {
            currentMaxSpeed = maxSpeed * runInfluence;
            if (!runDown)
            {
                runDown = true;
                if (currentForm == MarioForm.Fire && fireBalls > 0) ThrowAction();
            }
        }
        else
        {
            currentMaxSpeed = maxSpeed;
            if (runDown) runDown = false;
        }
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
        throwSpriteRenderer.enabled = false;
        spriteMask.enabled = false;

        //currentForm = MarioForm.Big;
    }

    protected override void Update()
    {
        base.Update();

        if (gravity != trueGravity && velocity.y < 0.0f && !isGrounded) gravity = trueGravity;

        // Animate throw
        if (throwCoolDown > 0.0f)
        {
            throwCoolDown -= Time.deltaTime;
            throwSpriteRenderer.flipX = spriteRenderer.flipX;

            if (throwCoolDown <= 0.0f)
            {
                spriteMask.enabled = false;
                throwSpriteRenderer.enabled = false;
                throwCoolDown = 0.0f;
            }
        }
    }

    protected override void SetTargetVelocity()
    {
        velocityX = velocity.x;

        if (Mathf.Abs(moveInput.x) > 0.1f && !isCrouching)
        {
            if (moveInput.x > 0.0f) velocityX += acceleration * Time.deltaTime;
            else velocityX -= acceleration * Time.deltaTime;

            if (Mathf.Abs(velocityX) > currentMaxSpeed)
            {
                if (Mathf.Abs(velocityX) - currentMaxSpeed < acceleration * Time.deltaTime) velocityX = (velocityX / Mathf.Abs(velocityX)) * currentMaxSpeed;
                else velocityX -= (velocityX / Mathf.Abs(velocityX)) * acceleration * 2.0f * Time.deltaTime;
            }
        }
        if (isGrounded)
        {
            if (velocityX > 0.0f && (moveInput.x < 0.1f || isCrouching))
            {
                velocityX -= deceleration * Time.deltaTime;
                if (velocityX < 0.0f) velocityX = 0.0f;
            }
            else if (velocityX < 0.0f && (moveInput.x > -0.1f || isCrouching))
            {
                velocityX += deceleration * Time.deltaTime;
                if (velocityX > 0.0f) velocityX = 0.0f;
            }
        }

        velocity = new Vector2(velocityX, velocity.y);

        // Crouching
        if (isGrounded)
        {
            if (moveInput.y < -0.1f) { animator.SetBool("isCrouching", true); isCrouching = true; }
            else { animator.SetBool("isCrouching", false); isCrouching = false; }
        }

        // Visual code
        animator.SetBool("isGrounded",  isGrounded);
        if (!animator.GetBool("isTurning")) animator.SetBool("isTurning", moveInput.x * velocity.x < 0 && Mathf.Abs(velocity.x) > 2.0f && isGrounded);
        else { animator.SetBool("isTurning", moveInput.x * velocity.x < 0 && isGrounded); if (!isGrounded) spriteRenderer.flipX = !spriteRenderer.flipX; }
        animator.SetBool("isMoving", Mathf.Abs(moveInput.x) > 0.1f || Mathf.Abs(velocity.x) > 0.1f);
        if (jumpDown && !isGrounded && !isCrouching && !animator.GetBool("inJump")) { animator.SetBool("inJump", true); if (moveInput.x > 0.0f) spriteRenderer.flipX = false; else if (moveInput.x < 0.0f) spriteRenderer.flipX = true; }
        else if (isGrounded) animator.SetBool("inJump", false);
        animator.speed = isGrounded ? Mathf.Abs(velocity.x) / maxSpeed * 1.5f : 0.0f;
        if (isGrounded && Mathf.Abs(velocity.x) > 0.1f) spriteRenderer.flipX = (velocity.x < 0.0f);
    }

    void ThrowAction()
    {
        if (throwCoolDown <= 0.0f)
        {
            fireBalls--;
            spriteMask.enabled = true;
            throwSpriteRenderer.enabled = true;
            throwSpriteRenderer.flipX = spriteRenderer.flipX;
            throwCoolDown = 0.2f;
            GameObject temp = Instantiate(fireBall, transform.position + (new Vector3(0.75f, 0.35f) * (spriteRenderer.flipX ? -1 : 1)), Quaternion.identity);
            temp.GetComponent<PlatformerFireBall>().SetDirection(spriteRenderer.flipX ? -1 : 1);
        }
    }
    protected override void HitWall(int direction, RaycastHit2D hit)
    {
        base.HitWall(direction, hit);

        //PlatformerEnemy enemy = hit.collider.gameObject.GetComponent<PlatformerEnemy>();
        //if (hit.normal.y > 0.9f && enemy != null)
        //{
        //    // Hurt enemy and bounce Mario
        //    enemy.GetHurt(true);
        //    velocity = new Vector2(bounceForce, velocity.x);
        //}
        //else if (enemy != null)
        //{
        //    // Hurt Mario
        //}
    }
}
