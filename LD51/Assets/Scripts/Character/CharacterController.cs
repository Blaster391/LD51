using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_character;
    [SerializeField]
    private GameObject m_visuals;
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

    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_airDampening = 0.45f;

    [SerializeField]
    private float m_groundedEpsilon = 0.05f;

    [SerializeField]
    private Vector2 m_weaponHoldOffset = new Vector2(0, 0.15f);

    private Rigidbody2D m_rigidbody2D = null;
    private CapsuleCollider2D m_capsuleCollider2D = null;
    private BaseControls m_controls = null;
    private CharacterHealth m_health = null;

    private RaycastHit2D m_lastGroundHit;
    private bool m_isGrounded = true;
    private int m_movingDirection = 0;

    void Start()
    {
        m_health = GetComponentInParent<CharacterHealth>();
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        m_controls = GetComponent<BaseControls>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_health.IsAlive())
        {
            return;
        }

        DoGroundCast();

        float horiz = m_controls.GetMovement();
        float damp = m_isGrounded ? 1.0f : m_airDampening;
        m_rigidbody2D.AddForce(Vector2.right * m_baseMovementForce * horiz * damp * Time.deltaTime * 60);

        if(Mathf.Abs(horiz) < 0.1f)
        {
            if(m_movingDirection != 0)
            {
                m_animator.Play("Idle");
                m_movingDirection = 0;
            }

        }
        else
        {
            if(horiz > 0.1f)
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

        Vector2 myPosition = transform.position;
        var directionToTarget = m_controls.GetTargetPosition() - myPosition;
        directionToTarget.Normalize();
        var angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        m_itemHolder.transform.position = myPosition + directionToTarget * m_weaponHoldDistance + m_weaponHoldOffset;

        m_arm1.transform.position = myPosition + directionToTarget * m_weaponHoldDistance * 0.4f + m_weaponHoldOffset;
        m_arm1.transform.rotation = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);

        m_hand1.transform.position = m_itemHolder.transform.position;
        m_hand1.transform.rotation = Quaternion.AngleAxis(angle + 90.0f, Vector3.forward);

        m_character.transform.position = transform.position;

        if (m_controls.GetThrow())
        {

        }

        if (m_controls.GetShoot())
        {
            Vector2 weaponPosition = m_itemHolder.transform.position;
            ContactFilter2D filter = new ContactFilter2D();
            RaycastHit2D[] results = new RaycastHit2D[10];
            var raycastHit = Physics2D.Raycast(weaponPosition, directionToTarget, filter, results);

            foreach(var hit in results)
            {
                if(hit.collider == null ||  hit.collider.gameObject.layer == 10)
                {
                    break;
                }

                CharacterHealth hitCharacterHealth = hit.collider.GetComponentInParent<CharacterHealth>();
                Limb hitLimb = hit.collider.GetComponent<Limb>();
                if (hitLimb != null && hitCharacterHealth != null && hitCharacterHealth != m_health)
                {

                    hitCharacterHealth.TakeDamage(hitLimb, directionToTarget, hit.point, 10.0f); 
                    break;
                }
            }
        }

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
    }

    public void OnDeath()
    {

    }
}
