using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerShell : PlatformerEnemy
{
    // Start is called before the first frame update
    public bool isMoving = false;
    [SerializeField] GameObject koopa;
    [SerializeField] Animator animator;

    [SerializeField] float timer = 8f;

    protected override void Update()
    {
        base.Update();
        if (!isMoving)
        {
            timer -= Time.deltaTime;
            CurrentDir = 0f;
        }
        else
        {
            timer = 8f;
            if (!spr.isVisible)
            {
                Debug.Log("MY FINAL MESSAGE");
                Destroy(this.gameObject);
            }
        }
        animator.SetFloat("Timer", timer);
        if (timer < 0f)
        {
            Destroy(this.GetComponent<Collider>());
            Instantiate(koopa, this.transform.position + new Vector3(0f, 0.131993f, 0f), Quaternion.identity);
            Debug.Log("timer over");
            Destroy(this.gameObject);
        }

    }

    void Start()
    {
        CurrentDir = 0f;
        Debug.Log("IVE BEEN BIRTHED");
    }

    public override void OnDeath(bool stomped, float dir)
    {
        CurrentDir = dir;
        isMoving = !isMoving;

    }
    protected override void HitWall(int direction, RaycastHit2D hit)
    {
        if (hit.collider.gameObject.tag == "Enemy")
        {
            PlatformerEnemy enemy = hit.collider.gameObject.GetComponent<PlatformerEnemy>();
            enemy.OnDeath(false, CurrentDir);
        }
        else if ((direction == 2 || direction == 3))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                CurrentDir = direction;
                isMoving = true;
            }
            else
            {
                CurrentDir *= -1f;
                flipped = !flipped;
                spr.flipX = flipped;
            }
        }
    }


}
