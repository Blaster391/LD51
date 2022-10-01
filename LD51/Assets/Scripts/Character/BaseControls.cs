using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseControls : MonoBehaviour
{
    protected bool m_jump = false;
    protected float m_movement = 0.0f;
    protected Vector2 m_targetPosition = Vector2.zero;
    protected bool m_shoot = false;
    protected bool m_throw = false;

    public bool GetJump() { return m_jump; }
    public bool GetShoot() { return m_shoot; }
    public bool GetThrow() { return m_throw; }
    public float GetMovement() { return m_movement; }
    public Vector2 GetTargetPosition() { return m_targetPosition; }

}
