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
    private float m_baseMovementForce = 1.0f;

    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float m_airDampening = 0.45f;

    [SerializeField]
    private float m_groundedEpsilon = 0.05f;

    private Rigidbody2D m_rigidbody2D = null;
    private CapsuleCollider2D m_capsuleCollider2D = null;

    private RaycastHit2D m_lastGroundHit;
    private bool m_isGrounded = true;

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

        m_character.transform.position = transform.position;
    }

    private void DoGroundCast()
    {
        var capsuleBounds = m_capsuleCollider2D.bounds;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(m_rigidbody2D.position, capsuleBounds.extents.x, Vector2.down);
        foreach (var hit in hits)
        {
            if (hit.rigidbody == m_rigidbody2D) continue;

            m_lastGroundHit = hit;
            m_isGrounded = (m_rigidbody2D.position.y - hit.point.y) <= (capsuleBounds.extents.y + m_groundedEpsilon);
            break;
        }
    }
}
