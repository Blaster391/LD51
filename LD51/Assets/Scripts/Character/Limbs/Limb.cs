using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb : MonoBehaviour
{
    [SerializeField]
    private List<BloodEmitter> m_emitters = new List<BloodEmitter>();

    public virtual int GetDamageFromHit() { return 1; }
    public virtual int GetScore() { return 1; }
    public virtual void OnKilled()
    {
        
    }
}
