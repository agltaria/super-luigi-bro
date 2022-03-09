using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerGoomba : PlatformerEnemy
{
    // Start is called before the first frame update
    public override void OnDeath(bool stomped, float dir)
    {
        base.OnDeath(stomped, dir);
        if (stomped)
        {
            Speed = 0f;
        }
    }
}
