using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerKoopa : PlatformerEnemy
{
    // Start is called before the first frame update
    [SerializeField] Animator animator;
    [SerializeField] Sprite sprite;
    public override void OnDeath(bool stomped, float dir)
    {
        spr.sprite = sprite;
        base.OnDeath(stomped, dir);
        if (stomped)
        {
            Speed = 0f;

            animator.SetBool("Stomped", stomped);
            //insert shell
        }
    }
}
