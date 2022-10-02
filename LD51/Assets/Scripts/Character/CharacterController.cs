﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_character;
    [SerializeField]
    private GameObject m_visuals;
    [SerializeField]
    private GameObject m_body;
    [SerializeField]
    private Animator m_animator;
    [SerializeField]
    private GameObject m_itemHolder;
    [SerializeField]
    private GameObject m_arm1;
    [SerializeField]
    private GameObject m_arm2;
    [SerializeField]
    private GameObject m_hand1;
    [SerializeField]
    private GameObject m_hand2;

    [SerializeField]
    private float m_baseMovementForce = 1.0f;
    [SerializeField]
    private float m_baseJumpForce = 1.0f;
    [SerializeField]
    private float m_weaponHoldDistance = 1.0f;
    [SerializeField]
    private float m_throwForce = 10.0f;

    [SerializeField]
    private float m_maxSpeed = 10.0f;

    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_airDampening = 0.45f;

    [SerializeField]
    private float m_groundedEpsilon = 0.05f;

    [SerializeField]
    private Vector2 m_weaponHoldOffset = new Vector2(0, 0.15f);

    [SerializeField]
    private GameObject m_bulletLineRenderer = null;

    [SerializeField]
    private Weapon m_startingWeapon = null;

    private Rigidbody2D m_rigidbody2D = null;
    private CapsuleCollider2D m_capsuleCollider2D = null;
    private BaseControls m_controls = null;
    private CharacterHealth m_health = null;
    private GameStateManager m_gameStateManager = null;

    private RaycastHit2D m_lastGroundHit;
    private bool m_isGrounded = true;
    private int m_movingDirection = 0;

    private Weapon m_equippedWeapon;
    private Weapon m_groundWeapon;

    void Start()
    {
        m_health = GetComponentInParent<CharacterHealth>();
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        m_controls = GetComponent<BaseControls>();
        m_gameStateManager = GameHelper.GetManager<GameStateManager>();

        if(m_startingWeapon != null)
        {
            m_equippedWeapon = m_startingWeapon;

            m_arm1.GetComponent<Joint2D>().enabled = false;
            m_hand1.GetComponent<Joint2D>().enabled = false;

            if (m_equippedWeapon.IsTwoHanded())
            {
                m_arm2.GetComponent<Joint2D>().enabled = false;
                m_hand2.GetComponent<Joint2D>().enabled = false;
            }

            m_equippedWeapon.Pickup(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_health.IsAlive())
        {
            return;
        }

        if(!m_gameStateManager.GameActive && m_health.IsPlayer())
        {
            if (m_movingDirection != 0)
            {
                m_animator.Play("Idle");
                m_movingDirection = 0;
            }

            DropWeapon();
            m_character.transform.position = transform.position;
            m_body.transform.position = transform.position;
            return;
        }

        DoGroundCast();

        float horiz = m_controls.GetMovement();
        float damp = m_isGrounded ? 1.0f : m_airDampening;
        m_rigidbody2D.AddForce(Vector2.right * m_baseMovementForce * horiz * damp * Time.deltaTime * 60);

        Vector2 velocity = m_rigidbody2D.velocity;
        velocity.x = Mathf.Clamp(velocity.x, -m_maxSpeed, m_maxSpeed);
        m_rigidbody2D.velocity = velocity;

        if (Mathf.Abs(horiz) < 0.1f)
        {
            if (m_movingDirection != 0)
            {
                m_animator.Play("Idle");
                m_movingDirection = 0;
            }

        }
        else
        {
            if (horiz > 0.1f)
            {
                if (m_movingDirection != 1)
                {
                    m_animator.Play("Walk");
                    m_movingDirection = 1;
                }
            }

            if (horiz < -0.1f)
            {
                if (m_movingDirection != -1)
                {
                    m_animator.Play("WalkBackwards");
                    m_movingDirection = -1;
                }
            }
        }

        if (m_controls.GetJump())
        {
            bool doJump = m_isGrounded;

            if (doJump)
            {
                m_rigidbody2D.AddForce(Vector2.up * m_baseJumpForce, ForceMode2D.Impulse);
            }
        }

        m_character.transform.position = transform.position;
        m_body.transform.position = transform.position;

        if (m_equippedWeapon != null)
        {
            Vector2 myPosition = transform.position;
            var directionToTarget = m_controls.GetTargetPosition() - myPosition;
            directionToTarget.Normalize();
            var angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            if(angle < 0.0f)
            {
                angle += 360.0f;
            }

            m_itemHolder.transform.position = myPosition + directionToTarget * m_weaponHoldDistance + m_weaponHoldOffset;
            m_equippedWeapon.transform.position = m_itemHolder.transform.position;
            m_equippedWeapon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            m_equippedWeapon.GetComponent<SpriteRenderer>().flipY = (angle < 270.0f && angle > 90.0f);

            m_arm1.transform.position = myPosition + directionToTarget * m_weaponHoldDistance * 0.35f + m_weaponHoldOffset;
            m_arm1.transform.rotation = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);

            m_hand1.transform.position = myPosition + directionToTarget * m_weaponHoldDistance * 0.8f + m_weaponHoldOffset;
            m_hand1.transform.rotation = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);

            if(m_equippedWeapon.IsTwoHanded())
            {
                m_arm2.transform.position = myPosition + directionToTarget * m_weaponHoldDistance * 0.35f + m_weaponHoldOffset;
                m_arm2.transform.rotation = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);

                m_hand2.transform.position = myPosition + directionToTarget * m_weaponHoldDistance * 0.8f + m_weaponHoldOffset;
                m_hand2.transform.rotation = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);
            }

            if (m_controls.GetThrow())
            {
                TryThrow();
            }
            else if (m_controls.GetShoot() && m_equippedWeapon.CanFire())
            {
                TryShoot();
            }

        }
        else
        {
            if (m_controls.GetThrow())
            {
                TryPickup();
            }
        }
    }

    private void TryShoot()
    {
        if(m_equippedWeapon == null || m_equippedWeapon.GetAmmo() <= 0)
        {
            return;
        }

        Vector2 weaponPosition = m_equippedWeapon.GetFiringPoint();
        var initialDireactionToTarget = m_controls.GetTargetPosition() - weaponPosition;
        initialDireactionToTarget.Normalize();

        for (int i = 0; i < m_equippedWeapon.GetShotsPerBurst(); ++i)
        {
            var directionToTarget = initialDireactionToTarget + new Vector2((Random.value - 0.5f) * m_equippedWeapon.GetSpread(), (Random.value - 0.5f) * m_equippedWeapon.GetSpread());

            directionToTarget.Normalize();

            Vector2 hitPosition = weaponPosition + directionToTarget * 100.0f;

            ContactFilter2D filter = new ContactFilter2D();
            RaycastHit2D[] results = new RaycastHit2D[10];
            var raycastHit = Physics2D.Raycast(weaponPosition, directionToTarget, filter, results);


            foreach (var hit in results)
            {
                if (hit.collider == null)
                {
                    break;
                }

                if (hit.collider.gameObject.layer == 10)
                {
                    hitPosition = hit.point;
                    break;
                }

                CharacterHealth hitCharacterHealth = hit.collider.GetComponentInParent<CharacterHealth>();
                Limb hitLimb = hit.collider.GetComponent<Limb>();
                if (hitLimb != null && hitCharacterHealth != null && hitCharacterHealth != m_health)
                {
                    hitPosition = hit.point;
                    hitCharacterHealth.TakeDamage(hitLimb, directionToTarget, hit.point, 10.0f);
                    break;
                }
            }

            var lineRendererObject = Instantiate(m_bulletLineRenderer);
            BulletLineRenderer lineRenderer = lineRendererObject.GetComponent<BulletLineRenderer>();
            lineRenderer.DrawLine(weaponPosition, hitPosition);
        }

        m_equippedWeapon.FireWeapon();
    }

    private void TryPickup()
    {
        if (m_equippedWeapon != null)
        {
            return;
        }

        if (m_groundWeapon == null)
        {
            return;
        }

        m_groundWeapon.Pickup(gameObject);

        m_equippedWeapon = m_groundWeapon;

        m_arm1.GetComponent<Joint2D>().enabled = false;
        m_hand1.GetComponent<Joint2D>().enabled = false;

        if (m_equippedWeapon.IsTwoHanded())
        {
            m_arm2.GetComponent<Joint2D>().enabled = false;
            m_hand2.GetComponent<Joint2D>().enabled = false;
        }
    }

    private void TryThrow()
    {
        if (m_equippedWeapon == null)
        {
            return;
        }

        m_equippedWeapon.Throw(m_health.gameObject);

        m_arm1.GetComponent<Joint2D>().enabled = true;
        m_arm2.GetComponent<Joint2D>().enabled = true;

        m_hand1.GetComponent<Joint2D>().enabled = true;
        m_hand2.GetComponent<Joint2D>().enabled = true;

        //Vector2 weaponPosition = m_itemHolder.transform.position;
        //var directionToTarget = m_controls.GetTargetPosition() - weaponPosition;

        Vector2 myPosition = transform.position;
        var directionToTarget = m_controls.GetTargetPosition() - myPosition;
        directionToTarget.Normalize();

        Vector2 upwardsForce = Vector2.up * m_throwForce * 0.1f;

        m_equippedWeapon.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        m_equippedWeapon.GetComponent<Rigidbody2D>().AddForce(directionToTarget * m_throwForce + upwardsForce, ForceMode2D.Impulse);

        m_equippedWeapon = null;
    }

    public void DropWeapon()
    {
        m_arm1.GetComponent<Joint2D>().enabled = true;
        m_arm2.GetComponent<Joint2D>().enabled = true;

        m_hand1.GetComponent<Joint2D>().enabled = true;
        m_hand2.GetComponent<Joint2D>().enabled = true;

        m_equippedWeapon = null;
    }


    private void DoGroundCast()
    {
        var capsuleBounds = m_capsuleCollider2D.bounds;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(m_rigidbody2D.position, capsuleBounds.extents.x * 0.9f, Vector2.down);
        foreach (var hit in hits)
        {

            if (hit.collider.gameObject.layer != 10) continue;

            m_lastGroundHit = hit;
            m_isGrounded = (m_rigidbody2D.position.y - hit.point.y) <= (capsuleBounds.extents.y + m_groundedEpsilon);
            break;
        }

        m_groundWeapon = null;
        hits = Physics2D.CircleCastAll(m_rigidbody2D.position + Vector2.up * 2.0f, capsuleBounds.extents.x * 4.0f, Vector2.down, 4.0f);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.layer == 11)
            {
                m_groundWeapon = hit.collider.gameObject.GetComponent<Weapon>();
                break;
            }
        }
    }

    public void OnDeath()
    {
        DropWeapon();
    }

    public bool HasWeapon()
    {
        return m_equippedWeapon != null;
    }

    public Weapon GetEquippedWeapon()
    {
        return m_equippedWeapon;
    }

    public int GetBulletCount()
    {
        if(m_equippedWeapon != null)
        {
            return m_equippedWeapon.GetAmmo();
        }

        return 0;
    }
}
