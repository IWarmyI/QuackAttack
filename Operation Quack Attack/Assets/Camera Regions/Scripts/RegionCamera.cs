/* Camera Regions by Stephan Zuidam
www.szuidam.weebly.com
*/

using System.Collections;
using UnityEngine;

/* The RegionCamera class should be added to whatever camera should be controlled within defined regions. This component therefor
requires a camera component to properly work. The class needs a type of RegionHandler, assigned in the inspector. This means it's possible to 
assign different regions to different cameras within the same scene. 

bool lerpToTargetPosition can be toggled to make the camera smoothly lerp towards the target instead of instantly moving to the target. 
float smoothTime is paired with lerpToTargetPosition and is the smoothing speed at which the camera lerps towards the target position.

float zoomSpeed is used to set the speed at which the camera zooms in and out.

*/

[RequireComponent(typeof(Camera))]
public class RegionCamera : MonoBehaviour {
    private Camera cam;
    [SerializeField]
    private RegionHandler regionHandler;
    [SerializeField]
    private bool lerpToTargetPosition;

    [SerializeField]
    private float smoothTime;
    [SerializeField]
    private float zoomSpeed;

    private float orthographicSizeStart;
    public float CameraStartSize { get { return orthographicSizeStart; } }

    private Vector3 positionToMoveTo;
    [SerializeField] private GameObject objectToFollow;
    private Vector3 positionToFollow;

    IEnumerator zoomRoutine;

    void Start() {
        if (regionHandler == null) {
            throw new System.Exception("RegionCamera needs a type of RegionHandler. Either create or assign a region handler object in the scene.");
        }

        cam = GetComponent<Camera>();
        orthographicSizeStart = cam.orthographicSize;
    }

    void Update() {
        positionToMoveTo = (objectToFollow == null ? positionToFollow : objectToFollow.transform.position);
        float cameraAspectSize = cam.orthographicSize * cam.aspect;

        Vector3 newCameraPosition = Vector3.zero;

        if (regionHandler.Regions.Count > 0) {

            Region region = regionHandler.ActiveRegion;

            // The following block holds the logic for moving the camera on the horizontal axis. If the current regions width is small than that of the width of the
            // camera, the camera stays fixed at the center of the region. Otherwise it'll move towards the target on the x-axis.
            if (region.Width < cameraAspectSize * 2) {
                newCameraPosition.x = region.p0.x + region.Width / 2;
            }
            else {
                newCameraPosition.x = Mathf.Clamp(positionToMoveTo.x, region.p0.x + cameraAspectSize, region.p1.x - cameraAspectSize);
            }

            // The same logic but this time for the vertical axis. If the active region is smaller in height than the height of the camera, the camera stays fixed in the
            // center of the region.
            if (region.Height < cam.orthographicSize * 2) {
                newCameraPosition.y = region.p1.y + region.Height / 2;
            }
            else {
                newCameraPosition.y = Mathf.Clamp(positionToMoveTo.y, region.p1.y + cam.orthographicSize, region.p0.y - cam.orthographicSize);
            }

            if (!region.Contains(positionToMoveTo)) {
                regionHandler.SetActiveRegion(positionToMoveTo);
            }
        }
        else {
            newCameraPosition = positionToMoveTo;
        }

        // Restrict the camera to only move on the x- and y-axis. 
        newCameraPosition.z = transform.position.z;

        // Move towards the new target position that has been defined above. If lerpToTargetPosition is enabled, the camera lerps towards the target.
        if (lerpToTargetPosition) {
            transform.position = Vector3.Lerp(transform.position, newCameraPosition, smoothTime);
        }
        else {
            transform.position = newCameraPosition;
        }
    }

    // Call this function to set an object the camera should follow every frame.
    public void SetFollowObject(GameObject _objectToFollow) {
        objectToFollow = _objectToFollow;
    }

    // Call this function to set a fixed position for the camera to move to and focus on. Useful for showing points of interest.
    public void SetFollowPosition(Vector3 _positionToFollow) {
        positionToFollow = _positionToFollow;
        objectToFollow = null;
    }

    // Call this function to switch to a completely different region handler
    public void SetRegionHandler(RegionHandler _regionHandler) {
        regionHandler = _regionHandler;
    }

    // Call this function to zoom the camera in or out. The function takes a float type as value to which the camera should zoom in or out.
    public void Zoom(float _zoomValue) {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);
        zoomRoutine = ZoomRoutine(_zoomValue);
        StartCoroutine(zoomRoutine);
    }

    // Coroutine holding the logic for zooming in and out. It checks if the float given in the parameters is higher or lower than the current
    // size of the camera and from there determines if the camera should zoom in or out. The speed with which the camera should zoom can be
    // adjusted with the public field zoomSpeed from the inspector.
    private IEnumerator ZoomRoutine(float _zoomValue) {
        int zoomDirection = (_zoomValue > cam.orthographicSize ? 1 : -1);
        float i = Mathf.Abs(cam.orthographicSize - _zoomValue);

        while (i > 0) {
            cam.orthographicSize += Time.deltaTime * zoomDirection * zoomSpeed;

            i -= Time.deltaTime * zoomSpeed;
            yield return new WaitForEndOfFrame();
        }

        cam.orthographicSize = _zoomValue;

        yield return null;
    }
}