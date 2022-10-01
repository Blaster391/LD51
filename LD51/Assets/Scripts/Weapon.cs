using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private GameObject m_firingPoint = null;

    [SerializeField]
    private bool m_twoHanded = false;

    [SerializeField]
    private int m_ammo = 6;

    private GameObject m_thrownBy = null;

    public Vector2 GetFiringPoint()
    {
        return m_firingPoint.transform.position;
    }

    public bool IsTwoHanded()
    {
        return m_twoHanded;
    }

    public int GetAmmo()
    {
        return m_ammo;
    }
    public void FireWeapon()
    {
        --m_ammo;
    }

    public void Throw(GameObject thrower)
    {
        m_thrownBy = thrower;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(m_thrownBy != null)
        {
            Vector2 weaponVelocity = GetComponent<Rigidbody2D>().velocity;
            float throwSpeed = weaponVelocity.magnitude;

            if(throwSpeed < 1.0f)
            {
                m_thrownBy = null;
                return;
            }

            CharacterHealth hitCharacterHealth = collision.collider.GetComponentInParent<CharacterHealth>();
            Limb hitLimb = collision.collider.GetComponent<Limb>();
            if(hitLimb != null && hitCharacterHealth.gameObject != m_thrownBy)
            {
                hitCharacterHealth.TakeDamage(hitLimb, weaponVelocity, collision.GetContact(0).point, 2.0f);
                m_thrownBy = null;
            }
        }
    }
}
