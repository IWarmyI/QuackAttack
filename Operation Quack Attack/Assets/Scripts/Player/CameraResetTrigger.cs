using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResetTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the main camera in the scene.")]
    private PlayerCamera mainCamera;

    [SerializeField]
    [Tooltip("Whether to zoom back to the original orthographic size.")]
    private bool resetZoom;

    [SerializeField]
    [Tooltip("Whether to pan back to the original offset.")]
    private bool resetPan;

    /// <summary>
    /// Tells the main camera to zoom and pan back to its defaults.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (resetZoom) mainCamera.ResetZoom();
            if (resetPan) mainCamera.ResetPan();
        }
    }
}
