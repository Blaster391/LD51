using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{

    void Start()
    {
        Physics2D.IgnoreLayerCollision(8, 8);
        Physics2D.IgnoreLayerCollision(8, 9);

        //Physics2D.IgnoreLayerCollision(8, 11);
        Physics2D.IgnoreLayerCollision(9, 11);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
