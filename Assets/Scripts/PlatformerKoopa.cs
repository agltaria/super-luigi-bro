using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerKoopa : PlatformerEnemy
{
    // Start is called before the first frame update
    [SerializeField] Animator animator;
    [SerializeField] Sprite abc;
    [SerializeField] GameObject shell;
    public override void OnDeath(bool stomped, float dir)
    {
        //Debug.Log("i should change my sprite but i wont cause im a bitch");
        //spr.sprite = abc;
        animator.SetBool("Stomped", true);
        base.OnDeath(stomped, dir);
        if (stomped)
        {
            //INSERT STOMP KOOPA NOISE
            Debug.Log("IVE BEEN STOMPED");
            Speed = 0f;
            //insert shell
            Instantiate(shell, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
