using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEmitter : MonoBehaviour
{
    [SerializeField]
    private GameObject m_bloodPrefab;
    [SerializeField]
    private float m_emmitTimer = 1.0f;
    [SerializeField]
    private float m_spread = 0.1f;
    [SerializeField]
    private float m_force = 10.0f;
    [SerializeField]
    private float m_forceVaraince = 1.0f;
    [SerializeField]
    private float m_maxStartDelay = 0.5f;

    private float m_timeToEmmission = 0.0f;

    private bool m_isActive = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isActive)
        {
            m_timeToEmmission -= Time.deltaTime;
            if(m_timeToEmmission < 0.0f)
            {
                m_timeToEmmission = m_emmitTimer;

                Vector3 variation = transform.up * (Random.value - 0.5f) * m_spread;

                var blood = Instantiate(m_bloodPrefab);
                blood.transform.position = transform.position;
                blood.transform.rotation = Quaternion.AngleAxis(360.0f * Random.value, Vector3.forward);
                blood.GetComponent<Rigidbody2D>().AddForce((transform.right + variation) * (m_force + m_forceVaraince * Random.value), ForceMode2D.Impulse);
            }
        }
    }

    public void Activate()
    {
        m_isActive = true;
        m_timeToEmmission = m_maxStartDelay * Random.value;
    }
}
