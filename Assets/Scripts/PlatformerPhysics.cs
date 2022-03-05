using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerPhysics : MonoBehaviour
{
    [SerializeField] Vector2 gravity = new Vector2(0.0f, -9.8f);
    [SerializeField] float minimumDistance = 0.001f; // Probably need to make all of these fields protected to allow inheritance
    [SerializeField] float collisionBuffer = 0.01f;

    Vector2 targetVelocity;
    Vector2 velocity;
    bool isGrounded;
    Rigidbody2D rigidbody;
    ContactFilter2D contactFilter;
    RaycastHit2D[] hitBuffer = new RaycastHit2D[10];

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update()
    {
        targetVelocity = new Vector2();
        SetTargetVelocity();
    }

    void FixedUpdate()
    {
        // Set and reset working variables
        velocity += gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;
        isGrounded = false;
        Vector2 deltaPosition = velocity * Time.deltaTime;

        // Make movement
        Move(deltaPosition);
    }

    void Move(Vector2 move)
    {
        float distance = move.magnitude;

        if (distance > minimumDistance)
        {
            int count = rigidbody.Cast(move, contactFilter, hitBuffer, distance + collisionBuffer);

            for (int i = 0; i < count; i++)
            {
                Vector2 normal = hitBuffer[i].normal;
                if (normal.y > 0.9f) isGrounded = true;

                float projection = Vector2.Dot(velocity, normal);
                if (projection > 0.0f) velocity -= projection * normal;

                float modifiedDistance = hitBuffer[i].distance - collisionBuffer;
                if (distance > modifiedDistance) distance = modifiedDistance;
            }
        }

        rigidbody.position += move.normalized * distance;
        if (isGrounded) velocity = Vector2.right * velocity.x;
    }

    protected virtual void SetTargetVelocity()
    {

    }
}
