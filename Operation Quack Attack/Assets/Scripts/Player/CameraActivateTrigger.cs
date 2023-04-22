using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    [Tooltip("Whether to pan the camera.")]
    private bool pan;

    [SerializeField]
    [Tooltip("Position to set the camera to.")]
    private Vector3 newOffset;

    /// <summary>
    /// Tells the main camera to zoom and pan to the orthographic size and 
    /// position specified in this class's fields.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (pan && mainCamera.Offset != newOffset) mainCamera.Pan(newOffset);
            if (orthographicSize != 0 && mainCamera.NewOrthographicSize != orthographicSize) mainCamera.Zoom(orthographicSize);
        }
    }
}

