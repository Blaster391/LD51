using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private bool m_twoHanded = false;

    [SerializeField]
    private int m_ammo = 6;

    [SerializeField]
    private float m_fadeTime = 0.2f;

    [SerializeField]
    private float m_spread = 0.0f;
    [SerializeField]
    private int m_shotsPerBurst = 1;
    [SerializeField]
    private float m_delay = 0.0f;
    [SerializeField]
    private bool m_isAutomatic = false;

    private float m_currentDelay = 0.0f;


    private float m_fadeRemaining = 0.0f;

    private GameObject m_heldBy = null;
    private GameObject m_thrownBy = null;

    public Vector2 GetFiringPoint()
    {
        return transform.position;
    }

    public bool IsTwoHanded()
    {
        return m_twoHanded;
    }

    public bool IsAutomatic()
    {
        return m_isAutomatic;
    }

    public int GetAmmo()
    {
        return m_ammo;
    }
    public void FireWeapon()
    {
        m_ammo -= m_shotsPerBurst;
        m_currentDelay = m_delay;
    }

    public int GetShotsPerBurst()
    {
        return m_shotsPerBurst;
    }

    public bool CanFire()
    {
        return m_currentDelay <= 0.0f;
    }

    public float GetSpread()
    {
        return m_spread;
    }

    public void Pickup(GameObject holder)
    {
        if(m_heldBy != null)
        {
            m_heldBy.GetComponent<CharacterController>().DropWeapon();
        }

        m_heldBy = holder;
    }

    public void Throw(GameObject thrower)
    {
        GetComponent<Collider2D>().isTrigger = true;
        m_heldBy = null;
        m_thrownBy = thrower;
        m_fadeRemaining = m_fadeTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(m_thrownBy != null)
        {
            Vector2 weaponVelocity = GetComponent<Rigidbody2D>().velocity;
            float throwSpeed = weaponVelocity.magnitude;

            if(throwSpeed < 0.25f)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 10)
        {
            m_fadeRemaining = 0.0f;
            GetComponent<Collider2D>().isTrigger = false;
            return;
        }

        CharacterHealth hitCharacterHealth = collision.gameObject.GetComponentInParent<CharacterHealth>();
        Limb hitLimb = collision.gameObject.GetComponent<Limb>();
        if (hitLimb != null && hitCharacterHealth.gameObject != m_thrownBy)
        {
            m_fadeRemaining = 0.0f;
            GetComponent<Collider2D>().isTrigger = false;
            return;
        }
    }


    private void Update()
    {
        m_currentDelay -= Time.deltaTime;
        if (m_fadeRemaining > 0.0f)
        {
            m_fadeRemaining -= Time.deltaTime;
            GetComponent<Collider2D>().isTrigger = (m_fadeRemaining > 0.0f);
        }
    }
}
