using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerEnemy : PlatformerPhysics
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public float CurrentDir = -1f;
    [SerializeField] float Boost = 0f;
    public float Speed;

    public bool flipped;
    [SerializeField] int ScoreValue;
    [SerializeField] new Collider2D collider;

    [SerializeField] bool kill = false;

    [SerializeField] bool stomp = false;
    public SpriteRenderer spr;


    // Update is called once per frame

    Vector2 move = new Vector2();
    protected override void SetTargetVelocity()
    {

        if (spr.isVisible)
        {

            velocity = new Vector2(CurrentDir * Speed, velocity.y + Boost);
            //Debug.Log("we trying to move");
            if (kill)
            {
                kill = false;
                OnDeath(false, 1f);
            }

            if (stomp)
            {
                stomp = false;
                OnDeath(true, 1f);
            }
        }
        else
        {
            velocity = new Vector2(0, 0);
        }


    }

    protected override void HitWall(int direction, RaycastHit2D hit)
    {
        if ((direction == 2 || direction == 3) && (hit.collider.gameObject.tag == "Untagged" || hit.collider.gameObject.tag == "Enemy" || hit.collider.gameObject.tag == "PowerUp"))
        {

            CurrentDir *= -1f;
            flipped = !flipped;
        }
        spr.flipX = flipped;
    }

    public virtual void OnDeath(bool stomped, float dir)
    {
        Invoke("DIE", 2.0f);
        Speed = 0;
        Destroy(collider);
        gravity.y = 0f;
        velocity = new Vector2(velocity.x, 0);
        if (!stomped)
        {
            gravity.y = -15f;
            Speed = 1.5f;
            CurrentDir = dir;
            //Boost = 0.01f;
            spr.flipY = true;
        }
    }

    protected void DIE()
    {
        //ScoreManager.AddScore(ScoreValue);
        Destroy(this.gameObject);
    }
}
