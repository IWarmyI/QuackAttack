using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBounds : MonoBehaviour
{
    [SerializeField] private Player player;
    private PolygonCollider2D boundingBox;
    private Vector2 bottom = Vector2.zero;
    private int bottomIndex;
    [SerializeField] private float offset = 1;

    // Start is called before the first frame update
    void Start()
    {
        boundingBox = GetComponent<PolygonCollider2D>();
        bottom = boundingBox.points[1];
        bottomIndex = 0;
        for (int i = 0; i < boundingBox.points.Length; i++)
        {
            Vector2 point = boundingBox.points[i];
            if (point.y < bottom.y)
            {
                bottom = point;
                bottomIndex = i;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player == null) return;

        if (player.State == Player.PlayerState.Normal)
        {
            if (player.OnGround || (player.Velocity.y < 0 && !player.OnGround))
            {
                GrowBox();
            }
            else
            {
                ShrinkBox();
            }
        }
        else if (player.State == Player.PlayerState.Dashing)
        {
            ShrinkBox();
        }
        else
        {
            GrowBox();
        }
    }

    private void ShrinkBox()
    {
        Vector2[] points = boundingBox.points;
        points[bottomIndex] = bottom + new Vector2(0, offset);
        boundingBox.points = points;
    }

    private void GrowBox()
    {
        Vector2[] points = boundingBox.points;
        points[bottomIndex] = bottom;
        boundingBox.points = points;
    }
}
