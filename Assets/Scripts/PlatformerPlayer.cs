using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using UnityEditor.Animations;
using UnityEngine.InputSystem;
using Blocks;
using DefaultNamespace;
using UnityEngine.SceneManagement;


[Serializable]
public struct SpawnLocations
{
    [SerializeField]private Vector3 _location;
   [SerializeField] private int _key; // 0 = Start Location, 1 = Checkpoint 1, 2 = Checkpoint 2, 3 = OnEnterFromUnderworldPipe, 4 = Checkpoint 4, 5 = Checkpoint 5

    public Vector3 Location => _location;
    public int Key => _key;
    
    public SpawnLocations(Vector3 location, int key)
    {
        _location = location;
        _key = key;
    }
}


public class PlatformerPlayer : PlatformerPhysics
{
    public enum MarioForm { Small, Big, Fire }
    public static MarioForm CurrentForm;
    public int fireBalls = 2;
    
    
    [SerializeField] GameObject fireBall;
    public static bool isInvincible; // Star man
    float invincibilityTimer;
    bool isVulnerable; // I-Frames after being hurt
    float vulnerabilityTimer;
    
    
    
    [Header("Mario Stats")]
    [SerializeField] float maxSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float runInfluence = 1.65f;
    [SerializeField] float jumpForce = 4.5f;
    [SerializeField] float bounceForce = 2.5f;
    [SerializeField] float releasedJumpInfluence = 0.5f;
    [SerializeField] float slowFallInfluence = 0.5f;
    [SerializeField] float jumpSpeedInfluence = 0.30f; // Jump height is multiplied by 1.0 + (this) * (currentSpeed / maxSpeed), so that you jump higher when running

    [Header("Spawn Handling")] 
    [SerializeField] private SpawnLocations[] _spawnLocations;
    [SerializeField] private int _currentCheckpointKey;
    
    
    
    SpriteRenderer spriteRenderer;
    Animator animator;
    [Header("Sprite Handling")]
    [SerializeField] SpriteRenderer throwSpriteRenderer;
    [SerializeField] SpriteMask spriteMask;

    [Header("Animation Handling")]
    [SerializeField] RuntimeAnimatorController[] animators; // small, big, fire, star?
    [SerializeField] Sprite[] powerUpMushroomSprites;
    [SerializeField] Sprite[] powerUpFlowerSprites;
    float powerUpTimer;
    int powerUpStage;

    Vector2 bigMarioColliderScale = new Vector2(0.625f, 1.4375f);
    Vector2 bigMarioColliderOffset = new Vector2(0.0f, -0.21885f);
    Vector2 smallMarioColliderScale = new Vector2(0.5f, 0.6875f);
    Vector2 smallMarioColliderOffset = new Vector2(0.0f, -0.594f);
    BoxCollider2D collider;
    
    [Header("Audio Handling")]
    [SerializeField] AudioClip[] audioClips; // 0 jump, 1 fireBall, 2 powerUp, 3 stomp, 4 pipe (take damage), 5 oneUp
    AudioSource audioSource;

    Vector2 trueGravity;
    float velocityX;
    float currentMaxSpeed;
    bool flipSpriteBuffered;
    bool isCrouching;
    Vector2 moveInput = new Vector2();
    bool jumpDown;
    bool runDown;
    float throwCoolDown;

    private bool _onEnterUnderworldPipe;
    public bool OnPipe
    {
        get => _onEnterUnderworldPipe;
        set
        {
            if (value)
            {
                if (isGrounded)
                {
                    Debug.Log($"can go in pipe");
                    _onEnterUnderworldPipe = true;
                }
                else
                    _onEnterUnderworldPipe = false;
            }
            else
                _onEnterUnderworldPipe = false;
        }
        
    }
    
    private bool _onExitUnderworldPipe;
    
    public bool OnExitUnderworldPipe
    {
        get => _onExitUnderworldPipe;
        set
        {
            if (value)
            {
                if (isGrounded)
                {
                    Debug.Log($"can go in pipe");
                    _onExitUnderworldPipe = true;
                }
                else
                    _onExitUnderworldPipe = false;
            }
            else
                _onExitUnderworldPipe = false;
        }
        
    }

    public void OnMove(InputValue value) { moveInput = value.Get<Vector2>(); }
    
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
            velocity = new Vector2(velocity.x, jumpForce * (1.0f + (jumpSpeedInfluence * (Mathf.Abs(velocity.x) / (maxSpeed * runInfluence)))));
            gravity *= slowFallInfluence;
            PlaySound(0);
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
        OSTManager.isAlive = true;
        trueGravity = gravity;
        currentMaxSpeed = maxSpeed;

        collider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        throwSpriteRenderer.enabled = false;
        spriteMask.enabled = false;
    }


    private void OnEnable()
    {
        if (!PlayerPrefs.HasKey("MarioLives"))
        {
            PlayerPrefs.SetInt("MarioLives",3);
        }
        
        SceneManager.sceneLoaded += MoveMarioToCheckpoint;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= MoveMarioToCheckpoint;
    }

    [ContextMenu("Debug/ResetCheckpoint")]
    public void ResetCheckpointCounter() => PlayerPrefs.DeleteKey("CheckpointKey");
    
    [ContextMenu("Debug/ReseMariotLives")]
    public void RestMarioLives() => PlayerPrefs.DeleteKey("MarioLives");

    [ContextMenu("Debug/ResetAll")]
    public void ResetAllPlayerPrefs()
    {
        RestMarioLives();
        ResetCheckpointCounter();
    }

    private void MoveMarioToCheckpoint(Scene scene, LoadSceneMode mode)
    {

        if (!PlayerPrefs.HasKey("CheckpointKey"))
        {
            Debug.Log("Does not have key");
            _currentCheckpointKey = 0;
        }
        else
        {
            _currentCheckpointKey = PlayerPrefs.GetInt("CheckpointKey");
            Debug.Log("Current key : " + _currentCheckpointKey);

        }
       

        if (scene == SceneManager.GetSceneByBuildIndex(1))
        {

            SpawnLocations toSpawnLocation = _spawnLocations.Single(x => x.Key == _currentCheckpointKey);
            transform.position = toSpawnLocation.Location;

            Debug.Log(toSpawnLocation.Key);
            
            if (_currentCheckpointKey == 3)//3 is the number for the spawn location that exited the underworld pipe
            {
                StartCoroutine(EnterFromUnderworldPipeCoroutine());
            } 
            
        }
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

        // Decrement invincibility timers && handle animations for these
        if (isVulnerable)
        {
            vulnerabilityTimer -= Time.deltaTime;
            if (vulnerabilityTimer <= 0.0f)
            {
                isVulnerable = false;
                vulnerabilityTimer = 0.0f;
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;

            // Exit invinsibility state
            if (invincibilityTimer <= 0.0f)
            {
                isInvincible = false;
                invincibilityTimer = 0.0f;
                // Resume normal music
                // Reset visuals
                spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }

            // Animate invincibility
            else
            {
                float playRate = 1;
                if (invincibilityTimer <= 2.0f) playRate = 0.5f;

                switch (Mathf.FloorToInt((invincibilityTimer % (0.1999f / playRate)) * (20 * playRate)))
                {
                    case 0:
                        spriteRenderer.color = new Color(0, 0, 0);
                        throwSpriteRenderer.color = new Color(0, 0, 0);
                        break;
                    case 1:
                        spriteRenderer.color = new Color(1, 0, 0);
                        throwSpriteRenderer.color = new Color(1, 0, 0);
                        break;
                    case 2:
                        spriteRenderer.color = new Color(1, 1, 1);
                        throwSpriteRenderer.color = new Color(1, 1, 1);
                        break;
                    case 3:
                        spriteRenderer.color = new Color(0, 1, 0);
                        throwSpriteRenderer.color = new Color(0, 1, 0);
                        break;
                    default:
                        ;
                        break;
                }
            }
        }

        // Animate power-up
        if (powerUpStage >= 0)
        {
            powerUpTimer += Time.unscaledDeltaTime;
            powerUpStage += Mathf.FloorToInt(powerUpTimer / 0.1f);
            powerUpTimer %= 0.1f;
            if (CurrentForm == MarioForm.Small)
            {
                // Update sprite for hurt transistion
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
                        break;
                }
            }
            else if (CurrentForm == MarioForm.Big)
            {
                // Update sprite for mushroom transistion
                switch (powerUpStage)
                {
                    case 0:
                    case 2:
                    case 4:
                    case 7:
                    case 10:
                        spriteRenderer.sprite = powerUpMushroomSprites[0];
                        break;
                    case 1:
                    case 3:
                    case 5:
                    case 8:
                        spriteRenderer.sprite = powerUpMushroomSprites[1];
                        break;
                    case 6:
                    case 9:
                        spriteRenderer.sprite = powerUpMushroomSprites[2];
                        break;
                    default:
                        powerUpStage = -1;
                        powerUpTimer = 0.0f;
                        Time.timeScale = 1.0f;
                        animator.enabled = true;
                        break;
                }
            }
            else if (CurrentForm == MarioForm.Fire)
            {
                // Update sprite for flower transistion
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
                // Switch case will interupt anything beyond this point!
            }
        }
    }

    protected override void SetTargetVelocity()
    {
        velocityX = velocity.x;

        if (Mathf.Abs(moveInput.x) > 0.1f && !isCrouching)
        {
            if (moveInput.x > 0.0f)
            {
                velocityX += acceleration * Time.deltaTime;
                
                if (_onExitUnderworldPipe)
                    StartCoroutine(ExitUnderworldAnimationCoroutine());

            }
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
            if (moveInput.y < -0.1f) 
            { 
                animator.SetBool("isCrouching", true); 
                isCrouching = true;

                if (_onEnterUnderworldPipe)
                    StartCoroutine(EnterUnderworldAnimationCoroutine());
               
            }
            else
            {
                animator.SetBool("isCrouching", false); 
                isCrouching = false;
            }
        }

        // Visual code
        animator.SetBool("isGrounded", isGrounded);
        if (!animator.GetBool("isTurning")) animator.SetBool("isTurning", moveInput.x * velocity.x < 0 && Mathf.Abs(velocity.x) > 2.0f && isGrounded);
        else { animator.SetBool("isTurning", moveInput.x * velocity.x < 0 && isGrounded); if (!isGrounded) spriteRenderer.flipX = !spriteRenderer.flipX; }
        animator.SetBool("isMoving", Mathf.Abs(moveInput.x) > 0.1f || Mathf.Abs(velocity.x) > 0.1f);
        if (jumpDown && !isGrounded && !isCrouching && !animator.GetBool("inJump")) { animator.SetBool("inJump", true); if (moveInput.x > 0.0f) spriteRenderer.flipX = false; else if (moveInput.x < 0.0f) spriteRenderer.flipX = true; }
        else if (isGrounded) animator.SetBool("inJump", false);
        animator.speed = isGrounded ? Mathf.Clamp(Mathf.Abs(velocity.x) / maxSpeed * 1.75f, 1.0f, 1.75f) : 0.0f;
        if (isGrounded && Mathf.Abs(velocity.x) > 0.1f) spriteRenderer.flipX = (velocity.x < 0.0f);
    }

   IEnumerator EnterUnderworldAnimationCoroutine()
    {
        Time.timeScale = 0.0f;
        float timer = 0;
        float waitBeforeRestart = 1f;

        PlaySound(4);
        while (timer < waitBeforeRestart)
        {
            transform.position += Vector3.up * -0.3f * Time.unscaledDeltaTime;
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync("Underground");
    }
    
   IEnumerator ExitUnderworldAnimationCoroutine()
   {
       Time.timeScale = 0.0f;

       float timer = 0;
       float waitBeforeRestart = 1f;
       PlaySound(4);

       while (timer < waitBeforeRestart)
       {
           transform.position += Vector3.left * -0.1f * Time.unscaledDeltaTime;
           timer += Time.unscaledDeltaTime;
           yield return null;
       }
        
       PlayerPrefs.SetInt("CheckpointKey",3);
       PlayerPrefs.Save();
       Time.timeScale = 1f;
       SceneManager.LoadSceneAsync("Level 1-1");
   }
   
   IEnumerator EnterFromUnderworldPipeCoroutine()
   {
       Time.timeScale = 0.0f;
        Debug.Log("started this");
       float timer = 0;
       float waitBeforeRestart = 0.5f;
       PlaySound(4);

       while (timer < waitBeforeRestart)
       {
           transform.position += Vector3.down * -5f * Time.unscaledDeltaTime;
           timer += Time.unscaledDeltaTime;
           yield return null;
       }
        
       PlayerPrefs.SetInt("CheckpointKey",4);
       PlayerPrefs.Save();

       Time.timeScale = 1f;
   }
   
    void ThrowAction()
    {
        if (throwCoolDown <= 0.0f)
        {
            PlaySound(1);
            fireBalls--;
            spriteMask.enabled = true;
            throwSpriteRenderer.enabled = true;
            throwSpriteRenderer.flipX = spriteRenderer.flipX;
            throwCoolDown = 0.2f;
            GameObject temp = Instantiate(fireBall, transform.position + (new Vector3(0.75f, 0.35f) * (spriteRenderer.flipX ? -1 : 1)), Quaternion.identity);
            temp.GetComponent<PlatformerFireBall>().SetDirection(spriteRenderer.flipX ? -1 : 1);
        }
    }

    public void UpdateCheckpoint(int key)
    {
        print("updating checkpoint...");
        PlayerPrefs.SetInt("CheckpointKey",key);
        PlayerPrefs.Save();
        _currentCheckpointKey = PlayerPrefs.GetInt("CheckpointKey");
        
    }
    protected override void HitWall(int direction, RaycastHit2D hit)
    {
        base.HitWall(direction, hit);

        PlatformerEnemy enemy = hit.collider.gameObject.GetComponent<PlatformerEnemy>(); // Currently, Mario must be moving for him to get hurt
        if (hit.normal.y > 0.9f && enemy != null)
        {
            // Hurt enemy and bounce Mario
            enemy.OnDeath(true, 1);
            velocity = new Vector2(velocity.x, bounceForce);
            PlaySound(3);
        }
        else if (enemy != null)
        {
            PlatformerShell shell = hit.collider.gameObject.GetComponent<PlatformerShell>();
            bool notAShell = true;
            if (shell != null)
            {
                if (!shell.isMoving)
                {
                    enemy.OnDeath(false, velocity.x / Math.Abs(velocity.x));
                    isVulnerable = true;
                    vulnerabilityTimer = 0.1f;
                    notAShell = false;
                }
            }
            if (notAShell)
            {
                if (isInvincible)
                {
                    // Hurt enemy
                    enemy.OnDeath(false, velocity.x / Math.Abs(velocity.x));
                }
                else if (isVulnerable)
                {
                    // Nothing?
                }
                else if ((int)CurrentForm > 0)
                {
                    // Hurt Mario
                    Time.timeScale = 0.0f;
                    CurrentForm = MarioForm.Small;
                    powerUpStage = 0;
                    powerUpTimer = 0.0f;
                    animator.runtimeAnimatorController = animators[0];
                    animator.enabled = false;
                    collider.offset = smallMarioColliderOffset;
                    collider.size = smallMarioColliderScale;
                    spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                    isVulnerable = true;
                    vulnerabilityTimer = 2.0f;
                    PlaySound(4, true);
                }
                else
                {
                    PlayerDeath.Instance.Die();
                }
            }
        }

        Block block = hit.collider.gameObject.GetComponent<Block>();
        if (hit.normal.y < -0.9f && block != null) block.Trigger();

    }

    public void GetPowerUp(int powerUpType) // Types are as follows: 0 mushroom, 1 fire flower, 2 star, 3 one-up mushroom
    {
        ScoreManager.Instance.AddScore(1000);
        
        switch (powerUpType)
        {
            case 0: // Mushroom
                if (((int)CurrentForm) == 0)
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
                    PlaySound(2);
                }
                else
                {
                    // Increase score
                }
                break;
            case 1: // Fire Flower
                if (((int)CurrentForm) <= 1)
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
                    PlaySound(2);
                }
                else
                {
                    // Increase score
                }
                break;
            case 2: // Star man
                // Make call to music player to play invincibility music
                invincibilityTimer = 9.0f;
                isInvincible = true;
                PlaySound(2);
                break;
            case 3: // One-Up mushroom
                // Increment lives
                PlayerPrefs.SetInt("MarioLives", PlayerPrefs.GetInt("MarioLives") + 1);
                PlayerPrefs.Save();
                PlaySound(5);
                break;
            default:
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PowerUpInfo powerUp = collision.gameObject.GetComponent<PowerUpInfo>();
        if (powerUp != null)
        {
            GetPowerUp((int)powerUp.powerUpType);
            Destroy(powerUp.gameObject);
        }
    }

    void PlaySound(int index)
    {
        if (index < audioClips.Length)
        {
            audioSource.pitch = 1.0f;
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }
    }

    void PlaySound(int index, bool shiftPitch)
    {
        if (index < audioClips.Length)
        {
            if (true) audioSource.pitch = 1.5f;
            else audioSource.pitch = 1.0f;
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }
    }
}
