using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWobble : MonoBehaviour
{
    [SerializeField]
    private float m_speed = 1.0f;
    [SerializeField]
    private float m_bounds = 30.0f;

    private float m_prop = 0.0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_prop += Time.deltaTime * m_speed;
        if (m_prop > 1000.0f)
        {
            m_prop -= 1000.0f;
        }

        float angle = Mathf.Sin(m_prop) * m_bounds;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


    }
}
