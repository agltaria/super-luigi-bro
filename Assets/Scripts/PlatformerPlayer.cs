using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformerPlayer : PlatformerPhysics
{
    [SerializeField] float speed;
    [SerializeField] float acceleration;
    [SerializeField] float runInfluence = 0.65f;
    [SerializeField] float minimumJump = 1.5f;
    [SerializeField] float maximumJump = 4.5f;
    [SerializeField] float jumpSpeedInfluence = 0.30f; // Jump height is multiplied by 1.0 + (this) * (currentSpeed / maxSpeed), so that you jump higher when running

    Vector2 moveInput;
    public void OnMove(InputValue value) { moveInput = value.Get<Vector2>(); }

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    protected override void SetTargetVelocity()
    {
        
    }
}
