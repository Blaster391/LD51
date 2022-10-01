using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbTracker : MonoBehaviour
{
    [SerializeField]
    private GameObject m_target;
    [SerializeField]
    private GameObject m_relative;
    [SerializeField]
    private float m_force = 10.0f;

    private Rigidbody2D m_rigidbody = null;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector3 direction = m_target.transform.position - m_relative.transform.position;
        direction.Normalize();
        m_rigidbody.AddForceAtPosition(direction * m_force * Time.fixedDeltaTime, m_relative.transform.position);
    }
}
