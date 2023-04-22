/* Camera Regions by Stephan Zuidam
www.szuidam.weebly.com
*/

using System;
using UnityEngine;

/*  The Region class is a simplified Rect class that stores the information for a region and can return the width and height of the region.
 Contains returns a true or false wether a position, given as parameter, is within the boundaries of this region.

*/

[Serializable]
public class Region {
    public Vector2 p0, p1;

    public Region(Vector2 p0, Vector2 p1) {
        this.p0 = p0;
        this.p1 = p1;
    }

    public Region(float p0_x, float p0_y, float p1_x, float p1_y) {
        this.p0 = new Vector2(p0_x, p0_y);
        this.p1 = new Vector2(p1_x, p1_y);
    }

    public float Width
    {
        get { return Mathf.Abs(p1.x - p0.x); }
    }

    public float Height
    {
        get { return Mathf.Abs(p1.y - p0.y); }
    }

    public bool Contains(Vector3 _position) {
        return (_position.x > p0.x && _position.x < p1.x && _position.y > p1.y && _position.y < p0.y);
    }
}
