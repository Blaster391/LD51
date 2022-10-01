using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseControls : MonoBehaviour
{
    protected bool m_jump = false;
    protected float m_movement = 0.0f;
    protected Vector2 m_targetPosition = Vector2.zero;

    public bool GetJump() { return m_jump; }
    public float GetMovement() { return m_movement; }
    public Vector2 GetTargetPosition() { return m_targetPosition; }

}
