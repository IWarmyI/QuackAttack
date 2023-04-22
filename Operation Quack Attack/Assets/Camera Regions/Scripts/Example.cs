/* Camera Regions by Stephan Zuidam
www.szuidam.weebly.com
*/

using UnityEngine;
using UnityEngine.UI;

/* The following class is created for the example scene to operate the buttons on the UI canvas. All the buttons are 
bound to public functions in the RegionCamera class that controls the camera. These functions can be called from other classes in the same
manner as in this example class.

This class can safely be removed from the project if it is no longer necessary for reference. Don't forget to remove the canvas object aswell from the 
example scene if you so wish to use the scene.
*/

public class Example : MonoBehaviour {
    RegionCamera cam;
    [SerializeField] Button followPlayer;
    [SerializeField] Button followFixedObject;

    void Start() {
        cam = Camera.main.GetComponent<RegionCamera>();
        cam.SetFollowObject(GameObject.Find("Player"));
    }

    public void OnButton_FollowPlayer() {
        cam.SetFollowObject(GameObject.Find("Player"));

        followPlayer.interactable = false;
        followFixedObject.interactable = true;
    }

    public void OnButton_FollowFixedObject() {
        cam.SetFollowPosition(GameObject.Find("Fixed Object").transform.position);

        followPlayer.interactable = true;
        followFixedObject.interactable = false;
    }

    public void OnButton_ZoomIn() {
        cam.Zoom(2);
    }

    public void OnButton_ZoomOut() {
        cam.Zoom(6);
    }

    public void OnButton_ZoomReset() {
        cam.Zoom(cam.CameraStartSize);
    }
}
