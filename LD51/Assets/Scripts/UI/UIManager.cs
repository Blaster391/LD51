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

    private List<GameObject> m_bullets = new List<GameObject>();

    private CharacterController m_playerController = null;
    private GameStateManager m_gameStateManager = null;

    // Start is called before the first frame update
    void Start()
    {
        m_gameStateManager = GameHelper.GetManager<GameStateManager>();
        m_playerController = GameHelper.GetManager<GameStateManager>().PlayerController;
    }

    // Update is called once per frame
    void Update()
    {
        int bulletCount = m_playerController.GetBulletCount();
        if (m_bullets.Count != m_playerController.GetBulletCount())
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
    }
}
