using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_bulletHolder;
    [SerializeField]
    private GameObject m_bulletPrefab;
    [SerializeField]
    private float m_bulletSpacing = 1.0f;

    [SerializeField]
    private GameObject m_lifeHolder;
    [SerializeField]
    private GameObject m_lifePrefab;
    [SerializeField]
    private float m_lifeSpacing = 75.0f;

    private List<GameObject> m_bullets = new List<GameObject>();
    private List<GameObject> m_health = new List<GameObject>();

    private CharacterHealth m_playerHealth = null;
    private CharacterController m_playerController = null;
    private GameStateManager m_gameStateManager = null;

    // Start is called before the first frame update
    void Start()
    {
        m_gameStateManager = GameHelper.GetManager<GameStateManager>();
        m_playerController = m_gameStateManager.PlayerController;
        m_playerHealth = m_gameStateManager.Player.GetComponent<CharacterHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        int bulletCount = m_playerController.GetBulletCount();
        if (m_bullets.Count != bulletCount)
        {
            while(m_bullets.Count > 0)
            {
                Destroy(m_bullets[0]);
                m_bullets.RemoveAt(0);
            }

            for(int i = 0; i < bulletCount; ++i)
            {
                var newBullet = Instantiate(m_bulletPrefab, m_bulletHolder.transform, false);
                newBullet.transform.Translate(new Vector3(m_bulletSpacing * i, 0, 0));
                m_bullets.Add(newBullet);
            }
        }

        int healthCount = m_playerHealth.GetHealth();
        if (m_health.Count != healthCount)
        {
            while (m_health.Count > 0)
            {
                Destroy(m_health[0]);
                m_health.RemoveAt(0);
            }

            for (int i = 0; i < healthCount; ++i)
            {
                var newLife = Instantiate(m_lifePrefab, m_lifeHolder.transform, false);
                newLife.transform.Translate(new Vector3(m_lifeSpacing * i, 0, 0));
                m_health.Add(newLife);
            }
        }
    }
}
