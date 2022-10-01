using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbHead : Limb
{
    public override int GetDamageFromHit() { return 3; }

    public override void OnKilled()
    {
        GetComponent<Joint2D>().enabled = false;

        base.OnKilled();
    }
}
