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

        var characterController = GetComponent<CharacterController>();
        if (characterController.GetEquippedWeapon() != null && characterController.GetEquippedWeapon().IsAutomatic())
        {
            m_shoot = Input.GetMouseButton(0);
        }
        else
        {
            m_shoot = Input.GetMouseButtonDown(0);
        }

        m_throw = Input.GetMouseButtonDown(1);
        m_targetPosition = GameHelper.MouseToWorldPosition();
    }
}
