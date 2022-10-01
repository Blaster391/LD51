using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField]
    private int m_hp = 3;

    [SerializeField]
    private Rigidbody2D m_body;
    [SerializeField]
    private GameObject m_controller;

    public bool IsAlive()
    {
        return m_hp > 0;
    }

    public void TakeDamage(Limb hitLimb, Vector2 hitDirection, Vector2 hitPosition, float hitForce)
    {
        bool justKilled = IsAlive();
        if (IsAlive())
        {
            m_hp -= hitLimb.GetDamageFromHit();
            if (m_hp <= 0)
            {
                hitLimb.OnKilled();
                Die();

                m_body.AddForceAtPosition(hitDirection * hitForce, hitPosition, ForceMode2D.Impulse);
            }
        }

        if(!IsAlive())
        {
            if (!justKilled || hitLimb.GetComponent<Rigidbody2D>() != m_body)
            {
                hitLimb.GetComponent<Rigidbody2D>().AddForceAtPosition(hitDirection * hitForce, hitPosition, ForceMode2D.Impulse);
            }
        }
    }

    private void Die()
    {
        m_controller.GetComponent<CharacterController>().OnDeath();
        Destroy(m_controller);

        RigidbodyConstraints2D constraints = new RigidbodyConstraints2D();
        m_body.constraints = constraints;
    }
}
