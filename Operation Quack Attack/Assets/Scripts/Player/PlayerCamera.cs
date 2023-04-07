using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Camera cam;

    public Transform player;
    [SerializeField] float camSpeed = 10.0f;
    [SerializeField] Vector3 offset;
    private float defaultSize;
    private Vector3 defaultPosition;

    public Transform door;
    private Vector3 start;
    [SerializeField] float panTime = 2.5f;
    [SerializeField] float panSize = 15;
    [SerializeField] float panZoomTime = 1f;
    private float time = 0;
    private float sTime = 0;

    private bool isIntro = true;

    /// <summary>
    /// Instantaneoulsy adjusts the camera's zoom.
    /// </summary>
    /// <param name="orthographicSize">
    /// Zoom value: a smaller value means a higher amount of zoom, and vice versa.
    /// </param>
    private void Zoom(float orthographicSize)
    {
        cam.orthographicSize = orthographicSize;
    }

    /// <summary>
    /// Instantaneously adjusts the camera's position.
    /// </summary>
    /// <param name="newPos">
    /// The position to warp to.
    /// </param>
    private void Pan(Vector3 newPos)
    {
        cam.transform.position = newPos;
    }

    public void Move(float orthographicSize, Vector3 newPos)
    {
        Zoom(orthographicSize);
        Pan(newPos);
    }

    /// <summary>
    /// Restores the camera to its original position and zoom level.
    /// </summary>
    public void Reset()
    {
        cam.orthographicSize = defaultSize;
        cam.transform.position = defaultPosition;
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        defaultPosition = cam.transform.position;
        defaultSize = cam.orthographicSize;
        isIntro = Player.IsIntro;

        if (isIntro && door != null)
        {
            start = door.position + offset;
            cam.orthographicSize = panSize;
        }
        else
        {
            transform.position = player.position + offset;
        }
    }

    private void Update()
    {
        
    }

    void LateUpdate()
    {
        if (isIntro && door != null)
        {
            float futureSize = defaultSize;
            Vector3 futurePos = player.position + offset;

            float t = time / panTime;
            t = t * t * (3f - 2f * t);

            Vector3 lerpPos = Vector3.Lerp(start, futurePos, t);
            transform.position = lerpPos;

            if (time >= panTime - panZoomTime)
            {
                float sT = sTime / panZoomTime;
                sT = sT * sT * (3f - 2f * sT);
                float lerpSize = Mathf.Lerp(panSize, futureSize, sT);
                cam.orthographicSize = lerpSize;

                sTime += Time.deltaTime;

                if (sTime >= panZoomTime) cam.orthographicSize = defaultSize;
            }

            time += Time.deltaTime;

            if (time >= panTime)
            {
                transform.position = futurePos;
                isIntro = false;
            }
        }
        else if (player != null)
        {
            Vector3 futurePos = player.position + offset;
            Vector3 lerpPos = Vector3.Lerp(transform.position, futurePos, camSpeed * Time.deltaTime);
            transform.position = lerpPos;
        }
    }
}
