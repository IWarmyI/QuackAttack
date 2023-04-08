using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraActivateTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the main camera in the scene.")]
    private PlayerCamera mainCamera;

    [SerializeField]
    [Tooltip("Zoom level to set the camera to.")]
    private float orthographicSize;

    [SerializeField]
    [Tooltip("Position to set the camera to.")]
    private Vector3 newPos;

    /// <summary>
    /// Tells the main camera to zoom and pan to the orthographic size and 
    /// position specified in this class's fields.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            mainCamera.Move(orthographicSize, newPos);
    }
}
