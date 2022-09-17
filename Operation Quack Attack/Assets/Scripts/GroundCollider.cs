using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollider : MonoBehaviour
{
    private bool onGround = false;
    public bool OnGround { get { return onGround; } set { onGround = value; } }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.bounds.max.y < collision.otherCollider.bounds.min.y)
        {
            onGround = true;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        onGround = false;
    }
}
