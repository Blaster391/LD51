using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbLeg : Limb
{
    public override void OnKilled()
    {
        GetComponent<Joint2D>().enabled = false;

        base.OnKilled();
    }
}
