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
    private float m_baseMovementForce = 1.0f;
    [SerializeField]
    private float m_baseJumpForce = 1.0f;

    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_airDampening = 0.45f;

    [SerializeField]
    private float m_groundedEpsilon = 0.05f;

    private Rigidbody2D m_rigidbody2D = null;
    private CapsuleCollider2D m_capsuleCollider2D = null;

    private RaycastHit2D m_lastGroundHit;
    private bool m_isGrounded = true;
    private int m_movingDirection = 0;

    void Start()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        DoGroundCast();

        float horiz = Input.GetAxis("Horizontal");
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

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
        {
            bool doJump = m_isGrounded;

            if (doJump)
            {
                m_rigidbody2D.AddForce(Vector2.up * m_baseJumpForce, ForceMode2D.Impulse);
            }
        }

        m_character.transform.position = transform.position;
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
}
