using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField]
    private int m_hp = 3;

    [SerializeField]
    private Limb m_head;
    [SerializeField]
    private Rigidbody2D m_body;
    [SerializeField]
    private GameObject m_controller;

    [SerializeField]
    private bool m_isPlayer = false;

    private GameStateManager m_gameStateManager = null;

    [SerializeField]
    private bool m_explodeOnSpawn = false;

    private void Awake()
    {
        if(m_explodeOnSpawn)
        {
            return;
        }

        m_gameStateManager = GameHelper.GetManager<GameStateManager>();

        if (m_isPlayer)
        {
            m_gameStateManager.RegisterPlayer(gameObject);
        }
        else
        {
            m_gameStateManager.RegisterEnemy(gameObject);
        }
    }

    private void Start()
    {
        if (m_explodeOnSpawn)
        {
            DestroyHead();
        }
    }

    public bool IsPlayer()
    {
        return m_isPlayer;
    }

    public bool IsAlive()
    {
        return m_hp > 0;
    }

    public int GetHealth()
    {
        return m_hp;
    }

    public void TakeDamage(Limb hitLimb, Vector2 hitDirection, Vector2 hitPosition, float hitForce)
    {
        bool justKilled = IsAlive();
        if (IsAlive())
        {
            m_hp -= hitLimb.GetDamageFromHit();
            if (m_hp <= 0)
            {
                if(!IsPlayer() && !m_explodeOnSpawn)
                {
                    m_gameStateManager.AddScore(hitLimb.GetScore() * m_gameStateManager.TimeRemaining);
                }

                hitLimb.OnKilled();
                Die();

                m_body.AddForceAtPosition(hitDirection * hitForce, hitPosition, ForceMode2D.Impulse);
            }
        }

        if(!IsAlive())
        {
            if (!justKilled || hitLimb.GetComponent<Rigidbody2D>() != m_body)
            {
                hitLimb.GetComponent<Rigidbody2D>().AddForceAtPosition(hitDirection * hitForce, hitPosition, ForceMode2D.Impulse);
            }
        }
    }

    private void Die()
    {
        m_controller.GetComponent<CharacterController>().OnDeath();
        Destroy(m_controller);

        RigidbodyConstraints2D constraints = new RigidbodyConstraints2D();
        m_body.constraints = constraints;

        if (m_explodeOnSpawn)
        {
            return;
        }

        if (m_isPlayer)
        {
            m_gameStateManager.LoseGame();
        }
        else
        {
            
            m_gameStateManager.RemoveEnemy(gameObject);
            m_gameStateManager.ResetTimer();
        }

 
        Camera.main.gameObject.GetComponent<ScreenFXManager>().TriggerScreenshot();
        

    }

    public void DestroyHead()
    {
        while(IsAlive())
        {
            TakeDamage(m_head, Vector2.up, m_head.transform.position, 10.0f);
        }
    }
}
