using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float length, height;
    private float startposX, startposY;
    public float parallaxFactorX;
    public float parallaxFactorY;
    public GameObject cam;

    void Start()
    {
        startposX = transform.position.x;
        startposY = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        height = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void LateUpdate()
    {
        float tempX = cam.transform.position.x * (1 - parallaxFactorX);
        float distanceX = cam.transform.position.x * parallaxFactorX;
        float distanceY = cam.transform.position.y * parallaxFactorY;

        Vector3 newPosition = new Vector3(startposX + distanceX, startposY + distanceY, transform.position.z);

        transform.position = newPosition;

        if (tempX > startposX + (length / 2)) startposX += length;
        else if (tempX < startposX - (length / 2)) startposX -= length;
    }
}