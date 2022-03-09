using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerShell : PlatformerEnemy
{
    // Start is called before the first frame update
    public bool isMoving = false;
    Animator animator;

    [SerializeField] float timer = 8f;

    protected override void Update()
    {
        base.Update();
        if (!isMoving)
        {
            timer -= Time.deltaTime;
            CurrentDir = 0f;
        }
        animator.setFloat("Timer", timer);
        if (timer < 0f)
        {
            Destroy(this.collider);
            //spawn koopa destroy itself
        }
    }

    void Start()
    {
        CurrentDir = 0f;
    }

    public override void OnDeath(bool stomped, float dir)
    {
        CurrentDir = dir;
        isMoving = !isMoving;

    }
}
