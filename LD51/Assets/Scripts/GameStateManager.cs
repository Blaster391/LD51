using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameObject Player { get; private set; }
    public CharacterController PlayerController 
    { 
        get
        {
            return Player.GetComponentInChildren<CharacterController>();
        }
    }

    public void RegisterPlayer(GameObject player)
    {
        Player = player;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
