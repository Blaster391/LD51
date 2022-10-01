using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControls : BaseControls
{

    [SerializeField]
    private float m_idleReactionTime = 3.0f;
    [SerializeField]
    private float m_idleReactionTimeBuffer = 1.0f;
    [SerializeField]
    private float m_attackGiveUpBuffer = 3.0f;
    [SerializeField]
    private float m_fleeGiveUpBuffer = 10.0f;
    [SerializeField]
    private float m_moveSpeed = 0.25f;
    [SerializeField]
    private float m_attackDelay = 2.0f;

    [SerializeField]
    private float m_accuracy = 1.0f;
    [SerializeField]
    private float m_playerLocationUpdateTime = 1.0f;

    [SerializeField]
    private bool m_patrol = false;

    private Vector2 m_playerLocationOld = new Vector2();
    private Vector2 m_playerLocation = new Vector2();

    private float m_lastUpdatedPlayerLocation = 0.0f;

    private float m_detectionBuffer = 0.0f;
    private float m_attackTime = 0.0f;
    private AIState m_state = AIState.Idle;

    bool m_walkRight = false;
    float m_waitTime = 0.0f;

    private CharacterHealth m_playerHealth = null;
    private CharacterController m_playerController = null;

    private CharacterHealth m_health = null;
    private CharacterController m_controller = null;
    private bool m_hasLOS = false;

    public enum AIState
    {
        Idle,
        Attack,
        Flee,
        Dead
    }

    void Start()
    {
        m_lastUpdatedPlayerLocation = Random.value * m_idleReactionTime;
        m_playerHealth = GameHelper.GetManager<GameStateManager>().Player.GetComponent<CharacterHealth>();
        m_playerController = GameHelper.GetManager<GameStateManager>().PlayerController;

        m_health = GetComponentInParent<CharacterHealth>();
        m_controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_state == AIState.Dead)
        {
            return;
        }

        if(!m_playerHealth.IsAlive() && m_state != AIState.Idle)
        {
            Enter_Idle();
        }

        m_movement = 0.0f;
        m_jump = false;
        m_shoot = false;
        m_throw = false;

        if (m_health.GetHealth() <= 0)
        {
            Enter_Dead();
            return;
        }

        if(m_playerHealth.IsAlive())
        {
            if (GameHelper.HasLineOfSight(gameObject, m_playerController.gameObject))
            {
                m_detectionBuffer += Time.deltaTime;
                m_hasLOS = true;
            }
            else
            {
                m_detectionBuffer -= Time.deltaTime;
                m_hasLOS = false;
            }
            m_detectionBuffer = Mathf.Max(0.0f, m_detectionBuffer);
        }

        switch (m_state)
        {
            case AIState.Idle:
                State_Idle();
                break;
            case AIState.Attack:
                State_Attack();
                break;
            case AIState.Flee:
                State_Flee();
                break;
            case AIState.Dead:
                break;
        }
    }

    private void State_Idle()
    {
        if (m_detectionBuffer > m_idleReactionTimeBuffer)
        {
            Enter_Attack();
            return;
        }

        if (m_patrol)
        {
            if(m_waitTime > 0.0f)
            {
                m_waitTime -= Time.deltaTime;
            }
            else
            {
                if (m_walkRight)
                {
                    m_movement = m_moveSpeed;
                }
                else
                {
                    m_movement = -m_moveSpeed;
                }
            }
        }
    }

    private void State_Attack()
    {
        if(m_controller.GetBulletCount() == 0)
        {
            Enter_Flee();
            return;
        }

        if (m_detectionBuffer > m_attackGiveUpBuffer)
        {
            Enter_Idle();
            return;
        }

        if(m_hasLOS)
        {
            m_lastUpdatedPlayerLocation += Time.deltaTime;
            if(m_lastUpdatedPlayerLocation > m_playerLocationUpdateTime)
            {
                m_lastUpdatedPlayerLocation = 0.0f;
                m_playerLocationOld = m_playerLocation;
                m_playerLocation = m_playerController.transform.position;
            }

            float targetLerp = m_lastUpdatedPlayerLocation / m_playerLocationUpdateTime;
   
            m_targetPosition = Vector2.Lerp(m_playerLocationOld, m_playerLocation, targetLerp);

            m_attackTime += Time.deltaTime;
            if(m_attackTime > m_attackDelay)
            {
                m_shoot = true;
                m_attackTime = 0.0f;
            }
        }

    }

    private void State_Flee()
    {
        if (m_detectionBuffer > m_fleeGiveUpBuffer)
        {
            Enter_Idle();
            return;
        }
    }

    private void Enter_Idle()
    {
        m_state = AIState.Idle;
        m_detectionBuffer = 0.0f;
    }

    private void Enter_Attack()
    {
        m_state = AIState.Attack;
        m_detectionBuffer = 0.0f;
        m_attackTime = 0.0f;

        m_playerLocationOld = m_playerController.transform.position;
        m_playerLocation = m_playerController.transform.position;
    }

    private void Enter_Flee()
    {
        m_state = AIState.Flee;
        m_detectionBuffer = 0.0f;
    }

    private void Enter_Dead()
    {
        m_state = AIState.Dead;
    }
}
