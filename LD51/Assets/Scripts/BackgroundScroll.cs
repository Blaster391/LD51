using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [SerializeField]
    private Vector2 m_offTrack = new Vector2(-1, -1);
    [SerializeField]
    private float m_speed = 1.0f;
    [SerializeField]
    private float m_resetDistance = 1000.0f;

    private float m_distance = 0.0f;

    // Update is called once per frame
    void Update()
    {
        float distance = 0.0f;
        distance += Time.deltaTime * m_speed;

        transform.Translate(m_offTrack * distance);

        m_distance += distance;

        if(m_distance > m_resetDistance)
        {
            transform.Translate(-m_offTrack * m_resetDistance);
            m_distance -= m_resetDistance;
        }

    }
}
