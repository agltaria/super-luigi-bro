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
        }
        animator.SetFloat("Timer", timer);
        if (timer < 0f)
        {
            Destroy(this.GetComponent<Collider>());
            Instantiate(koopa, this.transform.position + new Vector3(0f, 0.131993f, 0f), Quaternion.identity);
            Debug.Log("timer over");
            Destroy(this.gameObject);
        }

        if (!spr.isVisible)
        {
            Destroy(this.gameObject);
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
