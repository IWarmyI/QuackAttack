using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Player;

public class PlayerCollision : MonoBehaviour
{
    public enum CollisionEvent
    {
        Land,
        Roof,
        Slide,
        Wall,
        WallSide,
        WallBonk,
        WallLat,
        WallVert
    }

    [SerializeField] private Player player;
    [SerializeField] private Rigidbody2D rb;
    private Dictionary<CollisionEvent, int> events = new()
    {
        { CollisionEvent.Land, 0},
        { CollisionEvent.Roof, 0},
        { CollisionEvent.Slide, 0},
        { CollisionEvent.Wall, 0},
        { CollisionEvent.WallSide, 0},
        { CollisionEvent.WallBonk, 0},
        { CollisionEvent.WallLat, 0},
        { CollisionEvent.WallVert, 0},
    };

    private void Start()
    {
        if (player == null) player = GetComponentInParent<Player>();
        rb = player.GetComponent<Rigidbody2D>();
    }

    public Dictionary<CollisionEvent, bool> CalculateCollision()
    {
        Dictionary<CollisionEvent, bool> data = new();

        foreach(CollisionEvent ce in events.Keys)
        {
            data.Add(ce, events[ce] > 0);
        }

        foreach (CollisionEvent key in events.Keys.ToList())
        {
            events[key] = 0;
        }

        return data;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider.isTrigger) return;

        if (collision.gameObject.CompareTag("Stage"))
        {
            // OtherCollider (Player)
            float aLeft = collision.otherCollider.bounds.min.x;
            float aRight = collision.otherCollider.bounds.max.x;
            float aBottom = collision.otherCollider.bounds.min.y;
            float aTop = collision.otherCollider.bounds.max.y;
            // Collider (Wall)
            float bLeft = collision.collider.bounds.min.x;
            float bRight = collision.collider.bounds.max.x;
            float bBottom = collision.collider.bounds.min.y;
            float bTop = collision.collider.bounds.max.y;

            // If bottom of player collider is over top of platform colllider
            if (bTop <= aBottom)
            {
                events[CollisionEvent.Land]++;
            }

            // If top of player collider is under bottom of platform colllider
            else if (bBottom > aTop)
            {
                // When bumping head, kill vertical velocity
                events[CollisionEvent.Roof]++;
            }

            // If player bumps into wall/side of a platform
            else if (bBottom <= aTop)
            {
                // If air dashing, bonk off of the wall
                if (!player.OnGround && (player.State == PlayerState.Dashing))
                {
                    events[CollisionEvent.WallBonk]++;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.otherCollider.isTrigger) return;

        if (collision.gameObject.CompareTag("Stage"))
        {
            // OtherCollider (Player)
            float aLeft = collision.otherCollider.bounds.min.x;
            float aRight = collision.otherCollider.bounds.max.x;
            float aBottom = collision.otherCollider.bounds.min.y;
            float aTop = collision.otherCollider.bounds.max.y;
            // Collider (Wall)
            float bLeft = collision.collider.bounds.min.x;
            float bRight = collision.collider.bounds.max.x;
            float bBottom = collision.collider.bounds.min.y;
            float bTop = collision.collider.bounds.max.y;

            ContactPoint2D cp = collision.GetContact(0);

            // If bottom of player collider is over top of platform colllider
            if (aBottom >= bTop)
            {
                events[CollisionEvent.Land]++;
            }

            // If player bumps into wall/side of a platform
            else
            {
                // If in the air...
                if (!player.OnGround)
                {
                    // If on wall, allow walljump
                    if (bBottom <= aTop)
                    {
                        events[CollisionEvent.Wall]++;

                        // Determine which side the wall is on
                        if (cp.point.x > rb.position.x) // Rightside Wall
                            events[CollisionEvent.WallSide] = -1;
                        else if (cp.point.x < rb.position.x) // Leftside Wall
                            events[CollisionEvent.WallSide] = 1;
                    }

                    // When going agsint wall...
                    float delta = player.Velocity.x * Time.deltaTime;
                    if ((aRight + delta >= bLeft && aLeft < bLeft) ||
                        (aLeft + delta <= bRight && aRight > bRight))
                    {
                        // If air dashing, bonk off of the wall
                        if (player.State == PlayerState.Dashing)// || Math.Abs(vel.x) >= topSpeed))
                        {
                            if (bBottom <= aTop)
                            {
                                events[CollisionEvent.WallBonk]++;
                            }
                        }
                        else
                        {
                            // Otherwise, if in the air, reduces lateral velocity as if on the ground
                            events[CollisionEvent.WallLat]++;
                        }
                    }

                    // If rising, reduce vertical velocity
                    events[CollisionEvent.WallVert]++;
                }

                // If on the ground, but not 100% on top, slide down
                else
                {
                    if (aTop > bTop && aBottom <= bTop)
                    {
                        events[CollisionEvent.Slide]++;
                    }
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.otherCollider.isTrigger) return;

        if (collision.gameObject.CompareTag("Stage"))
        {
        }
    }
}
