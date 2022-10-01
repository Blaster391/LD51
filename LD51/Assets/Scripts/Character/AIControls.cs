using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControls : BaseControls
{
    [SerializeField]
    private float m_reactionSpeed = 1.0f;
    [SerializeField]
    private float m_idleReactionTime = 3.0f;
    [SerializeField]
    private float m_idleReactionTimeBuffer = 1.0f;
    [SerializeField]
    private float m_moveSpeed = 0.25f;

    [SerializeField]
    private float m_accuracy = 1.0f;

    [SerializeField]
    private bool m_patrol = false;

    private Vector2 m_playerLocation = new Vector2();
    private float m_lastUpdatedPlayerLocation = 0.0f;

    private float m_detectionBuffer = 0.0f;
    private AIState m_state = AIState.Idle;

    bool m_walkRight = false;
    float m_waitTime = 0.0f;

    private GameObject m_player = null;
    private CharacterHealth m_health = null;
    private CharacterController m_controller = null;

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
        m_player = GameHelper.GetManager<GameStateManager>().Player;

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

        m_movement = 0.0f;
        m_jump = false;
        m_shoot = false;
        m_throw = false;

        if (m_health.GetHealth() <= 0)
        {
            Enter_Dead();
            return;
        }

        switch(m_state)
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
        if(m_patrol)
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

        if(GameHelper.HasLineOfSight(gameObject, m_player))
        {
            m_detectionBuffer += Time.deltaTime;
        }
        else
        {
            m_detectionBuffer -= Time.deltaTime;
        }

        m_detectionBuffer = Mathf.Max(0.0f, m_detectionBuffer);
        if(m_detectionBuffer > m_idleReactionTimeBuffer)
        {
            Enter_Attack();
            return;
        }
    }

    private void State_Attack()
    {
        if(m_controller.GetBulletCount() == 0)
        {
            Enter_Flee();
            return;
        }
    }

    private void State_Flee()
    {

    }

    private void Enter_Idle()
    {
        m_state = AIState.Idle;
    }

    private void Enter_Attack()
    {
        m_state = AIState.Attack;
    }

    private void Enter_Flee()
    {
        m_state = AIState.Flee;
    }

    private void Enter_Dead()
    {
        m_state = AIState.Dead;
    }
}
