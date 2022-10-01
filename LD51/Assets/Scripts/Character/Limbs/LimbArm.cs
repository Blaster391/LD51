using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbArm : Limb
{
    public override void OnKilled()
    {
        GetComponent<Joint2D>().enabled = false;

        base.OnKilled();
    }
}
