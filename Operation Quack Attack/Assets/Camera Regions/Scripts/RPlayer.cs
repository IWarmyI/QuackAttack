/* Camera Regions by Stephan Zuidam
www.szuidam.weebly.com
*/

using UnityEngine;

/* The Player class has been created without much further thought for the purpose
of showing the workings of the RegionCamera class in regards to following a moving/player controlled object. 
This class can be safely removed from the project when importing the asset into your own project*/

public class RPlayer : MonoBehaviour {

    public float walkSpeed;
    private new Rigidbody2D rigidbody;

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        rigidbody.velocity = new Vector2(Mathf.Lerp(0, Input.GetAxis("Horizontal") * walkSpeed, 2f), Mathf.Lerp(0, Input.GetAxis("Vertical") * walkSpeed, 2f));
    }
}
