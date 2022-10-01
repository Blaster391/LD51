using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLineRenderer : MonoBehaviour
{
    private LineRenderer m_lineRenderer = null;

    [SerializeField]
    private float m_lifeTime = 2.0f;

    private float m_lifeTimeRemaining = 0.0f;

    private void Awake()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        m_lifeTimeRemaining = m_lifeTime;
    }

    public void DrawLine(Vector3 from, Vector3 to)
    {
        Vector3[] positions = new Vector3[2];
        positions[0] = from;
        positions[1] = to;
        m_lineRenderer.SetPositions(positions);
    }

    // Update is called once per frame
    void Update()
    {
        m_lifeTimeRemaining -= Time.deltaTime;
        if(m_lifeTimeRemaining < 0.0f)
        {
            Destroy(gameObject);
            return;
        }

        float alpha = m_lifeTimeRemaining / m_lifeTime;
        Color colour = m_lineRenderer.startColor;
        colour.a = alpha;
        m_lineRenderer.startColor = colour;
        m_lineRenderer.endColor = colour;
    }
}
