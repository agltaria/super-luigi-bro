using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerEnemy : PlatformerPhysics
{
    // Start is called before the first frame update
    void Start()
    {

    }

    [SerializeField] float CurrentDir = -1f;
    [SerializeField] float Boost = 0f;
    public float Speed;
    [SerializeField] int ScoreValue;
    [SerializeField] new Collider2D collider;

    [SerializeField] bool kill = false;


    // Update is called once per frame

    Vector2 move = new Vector2();
    protected override void SetTargetVelocity()
    {
        velocity = new Vector2(CurrentDir * Speed, velocity.y + Boost);
        //Debug.Log("we trying to move");
        if (kill)
        {
            kill = false;
            OnDeath(false, 1f);
        }

    }

    protected override void HitWall(int direction, RaycastHit2D hit)
    {
        if ((direction == 2 || direction == 3) && (hit.collider.gameObject.tag == "Untagged" || hit.collider.gameObject.tag == "Enemy" || hit.collider.gameObject.tag == "PowerUp"))
        {
            CurrentDir *= -1f;
        }
    }

    public virtual void OnDeath(bool stomped, float dir)
    {
        Invoke("DIE", 1.5f);
        Speed = 0;
        if (!stomped)
        {
            Speed = 1.5f;
            CurrentDir = dir;
            //Boost = 0.01f;
            gravity = new Vector2(0, 1f);
            velocity = new Vector2(velocity.x, velocity.y + 5f);
            Destroy(collider);
        }
    }

    protected void DIE()
    {
        //ScoreManager.AddScore(ScoreValue);
        Destroy(this.gameObject);
    }
}
