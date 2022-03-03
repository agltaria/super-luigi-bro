using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class PCPlatformer2D : MonoBehaviour
{
    [SerializeField] float speed = 12;
    [SerializeField] float walkAcceleration = 50;
    [SerializeField] float airAcceleration = 35;
    [SerializeField] float jumpHeight = 8;

    [SerializeField] float fallSpeedMinimum = 15;
    [SerializeField] float fallSpeedMaximum = 45;
    //[SerializeField] float wallSlideSpeed = 50;

    [SerializeField] float groundedTurnSpeed = 5;
    [SerializeField] float defaultTurnSpeed = 2;

    [SerializeField] float coyoteTime = 0.15f;
    [SerializeField] float runMultiplier = 2.0f;
    float coyoteTimer;
    enum BufferedJump { jump, wallLeft, wallRight };
    BufferedJump buffer;

    Rigidbody2D rigidbody;

    Vector2 targetInput;
    float targetInputInfluence; // This is for wall jumping, clamping your input in the direction of the wall jump

    Vector2 moveInput;
    bool jumping;
    bool jumpDown;
    bool runDown;

    public bool grounded;
    public bool wallLeft;
    public bool wallRight;

    public void OnMove(InputValue value) { moveInput = value.Get<Vector2>(); }

    public void OnJump(InputValue value)
    {
        if (value.Get<float>() > 0)
        {
            if (!jumpDown)
            {
                jumpDown = true;
                if (coyoteTimer > 0) jumping = true;
            }
        }
        else jumpDown = false;
    }

    public void OnRun(InputValue value)
    {
        if (value.Get<float>() > 0)
        {
            if (!runDown)
            {
                runDown = true;
            }
        }
        else runDown = false;
    }

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Debug.Log(rigidbody.velocity.x);

        // Set up momentum n' input variables
        float acceleration = grounded ? walkAcceleration : airAcceleration;
        Vector2 modifiedInput = moveInput;
        if (runDown && grounded) modifiedInput *= runMultiplier; else if (grounded) modifiedInput *= 1f; else modifiedInput *= 0.75f;
        modifiedInput = new Vector2(Mathf.Clamp(modifiedInput.x, (-1 * (1 - targetInputInfluence)) + (targetInput.x * targetInputInfluence), (1 * (1 - targetInputInfluence)) + (targetInput.y * targetInputInfluence)), modifiedInput.y);
        if (targetInputInfluence > 0) targetInputInfluence -= Time.deltaTime;
        if (targetInputInfluence < 0) targetInputInfluence = 0;

        // Clamp input based on wall triggers
        if (wallLeft) modifiedInput = new Vector2(Mathf.Clamp(modifiedInput.x, 0, 1), modifiedInput.y); 
        if (wallRight) modifiedInput = new Vector2(Mathf.Clamp(modifiedInput.x, -1, 0), modifiedInput.y); 

        // Walk left/right
        float turnSpeed = 1;
        if ((modifiedInput.x > 0 && rigidbody.velocity.x < 0) || (modifiedInput.x < 0 && rigidbody.velocity.x > 0))
        {
            if (grounded) turnSpeed = groundedTurnSpeed; // Change directions faster if on ground
            else turnSpeed = defaultTurnSpeed;
        }
        rigidbody.velocity = new Vector2(Mathf.MoveTowards(rigidbody.velocity.x, (modifiedInput.x * speed), Time.deltaTime * acceleration * turnSpeed), rigidbody.velocity.y);

        // Buffer jump / reset coyote time
        if (coyoteTimer > 0) coyoteTimer -= Time.deltaTime;
        if (coyoteTimer < 0) coyoteTimer = 0;
        if (grounded) // Regular Jump
        {
            buffer = BufferedJump.jump;
            coyoteTimer = coyoteTime;
        }
        //else if (wallLeft && moveInput.x < 0) // Wall Jump Right
        //{
        //    buffer = BufferedJump.wallRight;
        //    coyoteTimer = coyoteTime;
        //}
        //else if (wallRight && moveInput.x > 0) // Wall Jump Left
        //{
        //    buffer = BufferedJump.wallLeft;
        //    coyoteTimer = coyoteTime;
        //}

        // Jump
        if (jumping)
        {
            jumping = false;
            if (buffer == BufferedJump.jump) // Regular Jump
            {
                coyoteTimer = 0;
                grounded = false;
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y)));
            }
            else if (buffer == BufferedJump.wallRight) // Wall Jump Right
            {
                coyoteTimer = 0;
                targetInput = new Vector2(1, 1);
                targetInputInfluence = 1;
                rigidbody.velocity = new Vector2(speed, Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y)));
            }
            else if (buffer == BufferedJump.wallLeft) { //Wall Jump Left
                coyoteTimer = 0;
                targetInput = new Vector2(-1, -1);
                targetInputInfluence = 1;
                rigidbody.velocity = new Vector2(-speed, Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y)));
            }
        }

        //// Wall slide
        //if (wallLeft && moveInput.x < 0 && rigidbody.velocity.y < -3) { rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y + ((-rigidbody.velocity.y -3) * Time.deltaTime * wallSlideSpeed)); }
        //else if (wallRight && moveInput.x > 0 && rigidbody.velocity.y < -3) { rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y + ((-rigidbody.velocity.y - 3) * Time.deltaTime * wallSlideSpeed)); }

        // Fall faster
        else if (rigidbody.velocity.y > -10) { // Set maximum fall velocity
            if (rigidbody.velocity.y < 0 || (rigidbody.velocity.y > 0 && !jumpDown))
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y - (Time.deltaTime * fallSpeedMaximum));  // On the downward or if jump not down
            }
            else rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y - (Time.deltaTime * fallSpeedMinimum)); // On the upward and jump down
        }

        //
    }
}