using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResetTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference to the main camera in the scene.")]
    private PlayerCamera mainCamera;

    /// <summary>
    /// Tells the main camera to zoom and pan back to its defaults.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            mainCamera.Reset();
    }
}
