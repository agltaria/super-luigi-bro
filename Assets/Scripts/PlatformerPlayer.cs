using System;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.InputSystem;

namespace DefaultNamespace
{
    [RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer), typeof(Animator))]
    public class PlatformerPlayer : PlatformerPhysics
    {
        bool isInvincible; // Star man
        float invincibilityTimer;
        bool isVulnerable; // I-Frames after being hurt
        float vulnerabilityTimer;
    
        public enum MarioForm
        {
            Small,
            Big,
            Fire
        }

        public static MarioForm CurrentForm;
        public int fireBalls = 2;
        [SerializeField] private GameObject fireBall;

        [SerializeField] private float maxSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float deceleration;
        [SerializeField] private float runInfluence = 1.65f;
        [SerializeField] private float jumpForce = 4.5f;
        [SerializeField] private float bounceForce = 2.5f;
        [SerializeField] private float releasedJumpInfluence = 0.5f;
        [SerializeField] private float slowFallInfluence = 0.5f;

        // Jump height is multiplied by 1.0 + (this) * (currentSpeed / maxSpeed), so that you jump higher when running
        [SerializeField] private float jumpSpeedInfluence = 0.30f;

        private SpriteRenderer spriteRenderer;
        private Animator animator;
        [SerializeField] private SpriteRenderer throwSpriteRenderer;
        [SerializeField] private SpriteMask spriteMask;

        [SerializeField] private AnimatorController[] animators; // small, big, fire, star?
        [SerializeField] private Sprite[] powerUpMushroomSprites;
        [SerializeField] private Sprite[] powerUpFlowerSprites;
        private float powerUpTimer;
        private int powerUpStage;

        private readonly Vector2 bigMarioColliderScale = new Vector2(0.75f, 1.4375f);
        private readonly Vector2 bigMarioColliderOffset = new Vector2(0.0f, -0.21885f);
        private readonly Vector2 smallMarioColliderScale = new Vector2(0.63f, 0.6875f);
        private readonly Vector2 smallMarioColliderOffset = new Vector2(0.0f, -0.594f);
        private new BoxCollider2D collider;

        private Vector2 trueGravity;
        private float velocityX;
        private float currentMaxSpeed;
        private bool flipSpriteBuffered;
        private bool isCrouching;
        private Vector2 moveInput;
        private bool jumpDown;
        private bool runDown;
        private float throwCoolDown;

        private static readonly int IsCrouching = Animator.StringToHash("isCrouching");
        private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
        private static readonly int IsTurning = Animator.StringToHash("isTurning");
        private static readonly int IsMoving = Animator.StringToHash("isMoving");
        private static readonly int InJump = Animator.StringToHash("inJump");

        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        public void OnRun(InputValue value)
        {
            if (value.Get<float>() > 0.1f)
            {
                currentMaxSpeed = maxSpeed * runInfluence;
                if (!runDown)
                {
                    runDown = true;
                    if (CurrentForm == MarioForm.Fire && fireBalls > 0) ThrowAction();
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
                velocity = new Vector2(velocity.x,
                    jumpForce * (1.0f + (jumpSpeedInfluence * (Mathf.Abs(velocity.x) / (maxSpeed * runInfluence)))));
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

        protected override void Awake()
        {
            base.Awake();
            trueGravity = gravity;
            currentMaxSpeed = maxSpeed;

            collider = GetComponent<BoxCollider2D>();
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
            
            AnimateThrow();
            
            AnimatePowerUp();
        }

        private void AnimateThrow()
        {
            if (throwCoolDown <= 0.0f) return;
            
            throwCoolDown -= Time.deltaTime;
            throwSpriteRenderer.flipX = spriteRenderer.flipX;

            if (throwCoolDown > 0.0f) return;
            
            spriteMask.enabled = false;
            throwSpriteRenderer.enabled = false;
            throwCoolDown = 0.0f;
        }

        private void AnimatePowerUp()
        {
            if (powerUpStage < 0) return;
            
            powerUpTimer += Time.unscaledDeltaTime;
            powerUpStage += Mathf.FloorToInt(powerUpTimer / 0.1f);
            powerUpTimer %= 0.1f;
            
            if (CurrentForm == MarioForm.Small)
            {
                // Update sprite for mushroom transition
                switch (powerUpStage)
                {
                    case 0:
                    case 2:
                    case 4:
                    case 7:
                    case 10:
                        spriteRenderer.sprite = powerUpMushroomSprites[2];
                        break;
                    case 1:
                    case 3:
                    case 5:
                    case 8:
                        spriteRenderer.sprite = powerUpMushroomSprites[1];
                        break;
                    case 6:
                    case 9:
                        spriteRenderer.sprite = powerUpMushroomSprites[0];
                        break;
                    default:
                        powerUpStage = -1;
                        powerUpTimer = 0.0f;
                        Time.timeScale = 1.0f;
                        animator.enabled = true;
                        spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        break;
                }
            }
            else if (CurrentForm == MarioForm.Big)
            {
                // Update sprite for flower transition
                switch (powerUpStage)
                {
                    case 0:
                    case 2:
                    case 4:
                    case 7:
                    case 10:
                        spriteRenderer.sprite = powerUpFlowerSprites[0];
                        break;
                    case 1:
                    case 3:
                    case 5:
                    case 8:
                        spriteRenderer.sprite = powerUpFlowerSprites[1];
                        break;
                    case 6:
                    case 9:
                        spriteRenderer.sprite = powerUpFlowerSprites[2];
                        break;
                    default:
                        powerUpStage = -1;
                        powerUpTimer = 0.0f;
                        Time.timeScale = 1.0f;
                        animator.enabled = true;
                        break;
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
                    if (Mathf.Abs(velocityX) - currentMaxSpeed < acceleration * Time.deltaTime)
                        velocityX = (velocityX / Mathf.Abs(velocityX)) * currentMaxSpeed;
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
                if (moveInput.y < -0.1f)
                {
                    animator.SetBool(IsCrouching, true);
                    isCrouching = true;
                }
                else
                {
                    animator.SetBool(IsCrouching, false);
                    isCrouching = false;
                }
            }

            // Visual code
            animator.SetBool(IsGrounded, isGrounded);
            if (!animator.GetBool(IsTurning))
            {
                animator.SetBool(IsTurning, moveInput.x * velocity.x < 0 && Mathf.Abs(velocity.x) > 2.0f && isGrounded);
            }
            else
            {
                animator.SetBool(IsTurning, moveInput.x * velocity.x < 0 && isGrounded);
                if (!isGrounded) spriteRenderer.flipX = !spriteRenderer.flipX;
            }

            animator.SetBool(IsMoving, Mathf.Abs(moveInput.x) > 0.1f || Mathf.Abs(velocity.x) > 0.1f);

            if (jumpDown && !isGrounded && !isCrouching && !animator.GetBool(InJump))
            {
                animator.SetBool(InJump, true);
                if (moveInput.x > 0.0f) spriteRenderer.flipX = false;
                else if (moveInput.x < 0.0f) spriteRenderer.flipX = true;
            }
            else if (isGrounded)
            {
                animator.SetBool(InJump, false);
            }

            animator.speed = isGrounded ? Mathf.Abs(velocity.x) / maxSpeed * 1.5f : 0.0f;

            if (isGrounded && Mathf.Abs(velocity.x) > 0.1f) spriteRenderer.flipX = velocity.x < 0.0f;
        }

        private void ThrowAction()
        {
            if (throwCoolDown > 0.0f) return;

            var flipX = spriteRenderer.flipX;

            fireBalls--;
            spriteMask.enabled = true;
            throwSpriteRenderer.enabled = true;
            throwSpriteRenderer.flipX = flipX;
            throwCoolDown = 0.2f;
            GameObject temp = Instantiate(fireBall, transform.position + new Vector3(0.75f, 0.35f) * (flipX ? -1 : 1),
                Quaternion.identity);
            temp.GetComponent<PlatformerFireBall>().SetDirection(flipX ? -1 : 1);
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
            //    Time.timeScale = 0.0f;
            //    currentForm = MarioForm.Small;
            //    powerUpStage = 0;
            //    powerUpTimer = 0.0f;
            //    animator.runtimeAnimatorController = animators[0];
            //    animator.enabled = false;
            //    collider.offset = smallMarioColliderOffset;
            //    collider.size = smallMarioColliderScale;
            //    spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            //}
        }

        public void GetPowerUp(PowerUpInfo.PowerUpType powerUpType) 
        {
            switch (powerUpType)
            {
                case PowerUpInfo.PowerUpType.Mushroom:
                    if ((int)CurrentForm == 0)
                    {
                        // Power up to big form
                        Time.timeScale = 0.0f;
                        CurrentForm = MarioForm.Big;
                        powerUpStage = 0;
                        powerUpTimer = 0.0f;
                        animator.runtimeAnimatorController = animators[1];
                        animator.enabled = false;
                        collider.offset = bigMarioColliderOffset;
                        collider.size = bigMarioColliderScale;
                    }
                    else
                    {
                        // Increase score
                    }

                    break;
                case PowerUpInfo.PowerUpType.FireFlower:
                    if ((int)CurrentForm <= 1)
                    {
                        // Power up to fire form
                        Time.timeScale = 0.0f;
                        CurrentForm = MarioForm.Fire;
                        powerUpStage = 0;
                        powerUpTimer = 0.0f;
                        animator.runtimeAnimatorController = animators[2];
                        animator.enabled = false;
                        collider.offset = bigMarioColliderOffset;
                        collider.size = bigMarioColliderScale;
                    }
                    else
                    {
                        // Increase score
                    }

                    break;
                case PowerUpInfo.PowerUpType.StarMan:
                    break;
                case PowerUpInfo.PowerUpType.OneUpMushroom:
                    // Increment lives
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            PowerUpInfo powerUp = collision.gameObject.GetComponent<PowerUpInfo>();
            
            if (powerUp == null) return;
            
            GetPowerUp(powerUp.powerUpType);
            Destroy(powerUp.gameObject);
            Debug.Log("Collected power of type " + (int)powerUp.powerUpType);
        }
    }
}
