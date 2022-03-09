using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PowerUpMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private bool jumping;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    
    private bool movingLeft;
    private new Rigidbody2D rigidbody;
    private float timeSinceLastJump;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private Vector2 Direction => movingLeft ? Vector2.left : Vector2.right;

    private void FixedUpdate()
    {
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, Direction, wallCheckDistance);
            
        if (wallHit.collider != null)
        {
            if (wallHit.collider.gameObject.tag != "Player")
            {
                movingLeft = !movingLeft;
            }
        }

        transform.position += (Vector3) Direction * moveSpeed * Time.fixedDeltaTime;
        
        if (!jumping) return;

        timeSinceLastJump += Time.fixedDeltaTime;

        if (timeSinceLastJump < jumpCooldown) return;
        
        RaycastHit2D floorHit = Physics2D.Raycast(transform.position, Vector2.down, wallCheckDistance);
        
        if (floorHit.collider != null)
        {
            rigidbody.velocity = Vector2.zero;
            
            rigidbody.AddForce(Vector2.up * jumpForce);

            timeSinceLastJump = 0;
        }
    }
}
