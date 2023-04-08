using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Camera cam;

    public Transform player;
    [SerializeField] float panSpeed;
    [SerializeField] float zoomSpeed;
    [SerializeField] Vector3 offset;
    private Vector3 defaultOffset;
    private float defaultZoom;
    private float newOrthographicSize;

    public Transform door;
    private Vector3 start;
    [SerializeField] float panTime = 2.5f;
    [SerializeField] float panSize = 15;
    [SerializeField] float panZoomTime = 1f;
    private float time = 0;
    private float sTime = 0;

    private bool isIntro = true;

    /// <summary>
    /// Adjusts the camera's zoom variable, so that the update loop handles the
    /// zooming accordingly.
    /// </summary>
    /// <param name="orthographicSize">
    /// Zoom value: a smaller value means a higher amount of zoom, and vice versa.
    /// </param>
    private void Zoom(float orthographicSize)
    {
        newOrthographicSize = orthographicSize;
    }

    /// <summary>
    /// Adjusts the camera's position variable, so that the update loop handles
    /// the panning accordingly.
    /// </summary>/
    /// <param name="newPos">
    /// The position to pan to.
    /// </param>
    private void Pan(Vector3 newPos)
    {
        // Offset is relative to player's position in the game world.
        offset = newPos - transform.position;
    }

    /// <summary>
    /// Sets the camera's zoom and position variables, so that the update loop
    /// handles zooming and panning accordingly.
    /// </summary>
    /// <param name="orthographicSize">
    /// Zoom value: a smaller valye means a higher amount of zoom, and vice versa.
    /// </param>
    /// <param name="newPos">
    /// The position to pan to.
    /// </param>
    public void Move(float orthographicSize, Vector3 newPos)
    {
        Zoom(orthographicSize);
        Pan(newPos);
    }

    /// <summary>
    /// Restores the camera's zoom and position variables to their original values.
    /// </summary>
    public void Reset()
    {
        newOrthographicSize = defaultZoom;
        offset = defaultOffset;
    }

    void Start()
    {
        cam = GetComponent<Camera>();

        // Get default offset and zoom values.
        defaultOffset = offset;
        defaultZoom = newOrthographicSize = cam.orthographicSize;

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

    void LateUpdate()
    {
        if (isIntro && door != null)
        {
            float futureSize = defaultZoom;
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

                if (sTime >= panZoomTime) cam.orthographicSize = defaultZoom;
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
            // This code handles the camera following the player as they move.
            // Camera movement is lerped for smoothness.
            // offset is adjusted above to effectively change how far the player
            // can see ahead of themselves. Useful when encountering long jumps,
            // to avoid the need for leaps of faith.
            Vector3 futurePos = player.position + offset;
            Vector3 lerpPos = Vector3.Lerp(transform.position, futurePos, panSpeed * Time.deltaTime);
            transform.position = lerpPos;

            // This check ensures that zooming only happens when there is a change
            // to the zoom variable's value.
            if (cam.orthographicSize != newOrthographicSize)
            {
                // Using the same logic as panning above, lerp the camera's zoom.
                float lerpZoom = Mathf.Lerp(cam.orthographicSize, newOrthographicSize, zoomSpeed * Time.deltaTime);
                cam.orthographicSize = lerpZoom;
            }
        }
    }
}
