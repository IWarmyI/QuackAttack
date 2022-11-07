using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class PlayerCollision : MonoBehaviour
{
    public struct CollisionData
    {
        public bool land;
        public bool roof;
        public bool slide;

        public bool wall;
        public bool wallBonk;
        public bool wallLat;
        public bool wallVert;

        public int wallSide;
    }

    [SerializeField] private Player player;
    [SerializeField] private Rigidbody2D rb;

    private int _wallSide = 0;

    public int _land = 0;
    public int _roof = 0;
    public int _slide = 0;
    public int _wall = 0;
    public int _wallBonk = 0;
    public int _wallLat = 0;
    public int _wallVert = 0;

    private void Start()
    {
        if (player == null) player = GetComponentInParent<Player>();
        rb = player.GetComponent<Rigidbody2D>();
    }

    public CollisionData CalculateCollision()
    {
        CollisionData data = new()
        {
            land = _land > 0,
            roof = _roof > 0,
            slide = _slide > 0,

            wall = _wall > 0,
            wallBonk = _wallBonk > 0,
            wallLat = _wallLat > 0,
            wallVert = _wallVert > 0,

            wallSide = _wallSide
        };

        _land = 0;
        _roof = 0;
        _slide = 0;
        _wall = 0;
        _wallBonk = 0;
        _wallLat = 0;
        _wallVert = 0;

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
                _land++;
            }

            // If top of player collider is under bottom of platform colllider
            else if (bBottom > aTop)
            {
                // When bumping head, kill vertical velocity
                _roof++;
            }

            // If player bumps into wall/side of a platform
            else if (bBottom <= aTop)
            {
                // If air dashing, bonk off of the wall
                if (!player.OnGround && (player.State == PlayerState.Dashing))
                {
                    _wallBonk++;
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
                _land++;
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
                        _wall++;

                        // Determine which side the wall is on
                        if (cp.point.x > rb.position.x) // Rightside Wall
                            _wallSide = -1;
                        else if (cp.point.x < rb.position.x) // Leftside Wall
                            _wallSide = 1;
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
                                _wallBonk++;
                            }
                        }
                        else
                        {
                            // Otherwise, if in the air, reduces lateral velocity as if on the ground
                            _wallLat++;
                        }
                    }

                    // If rising, reduce vertical velocity
                    _wallVert++;
                }

                // If on the ground, but not 100% on top, slide down
                else
                {
                    if (aTop > bTop && aBottom <= bTop)
                    {
                        _slide++;
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
