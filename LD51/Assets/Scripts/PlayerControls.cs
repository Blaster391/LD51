using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : BaseControls
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_movement = Input.GetAxis("Horizontal");
        m_jump = Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump");
        m_targetPosition = GameHelper.MouseToWorldPosition();
    }
}
