using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerPhysics : MonoBehaviour
{
    [SerializeField] protected Vector2 gravity = new Vector2(0.0f, -9.8f);
    [SerializeField] protected float minimumDistance = 0.001f; // Probably need to make all of these fields protected to allow inheritance
    [SerializeField] protected float collisionBuffer = 0.01f;

    // protected Vector2 targetVelocity;
    protected Vector2 velocity;
    protected bool isGrounded;
    protected Rigidbody2D rigidbody;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[10];

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // targetVelocity = new Vector2();
        SetTargetVelocity();
    }

    void FixedUpdate()
    {
        // Set and reset working variables
        velocity += gravity * Time.deltaTime;
        // velocity.x = targetVelocity.x;
        isGrounded = false;
        Vector2 deltaPosition = velocity * Time.deltaTime;

        // Make movement
        if (deltaPosition != null)
        {
            // Horizontal
            Move(Vector2.right * deltaPosition.x, false, true);
            // Vertical
            Move(Vector2.up * deltaPosition.y, true, false);
        }
    }

    void Move(Vector2 move, bool isVertical, bool isHorizontal)
    {
        float distance = move.magnitude;

        if (distance > minimumDistance)
        {
            int count = rigidbody.Cast(move, contactFilter, hitBuffer, distance + collisionBuffer);

            for (int i = 0; i < count; i++)
            {
                Vector2 normal = hitBuffer[i].normal;

                if (isVertical)
                {
                    if (normal.y > 0.9f) { isGrounded = true; velocity = new Vector2(velocity.x, Mathf.Max(0.0f, velocity.y)); }
                    else if (normal.y < -0.9f) velocity = new Vector2(velocity.x, Mathf.Min(0.0f, velocity.y));
                }
                else if (isHorizontal)
                {
                    if (normal.x > 0.9f) velocity = new Vector2(Mathf.Max(0.0f, velocity.x), velocity.y);
                    else if (normal.x < -0.9f) velocity = new Vector2(Mathf.Min(0.0f, velocity.x), velocity.y);
                }

                float projection = Vector2.Dot(velocity, normal);
                if (projection > 0.0f) velocity -= projection * normal;

                float modifiedDistance = hitBuffer[i].distance - collisionBuffer;
                if (distance > modifiedDistance) distance = modifiedDistance;
            }
        }

        rigidbody.position += move.normalized * distance;
        // if (isGrounded) velocity = Vector2.right * velocity.x;
    }

    protected virtual void SetTargetVelocity()
    {

    }
}
