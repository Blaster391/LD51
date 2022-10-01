using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbNeck : Limb
{
    public override int GetDamageFromHit() { return 5; }

    public override void OnKilled()
    {
        GetComponent<Joint2D>().enabled = false;

        base.OnKilled();
    }
}
