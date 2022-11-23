using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    // [SerializeField] GameObject player;
    // [SerializeField] float minY = 0;

    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (player == null) return;

    //     float x = player.transform.position.x;
    //     float y = Mathf.Max(minY, player.transform.position.y);
    //     transform.position = new Vector3(x, y, transform.position.z);
    // }

    public Transform player;
    [SerializeField] float camSpeed = 10.0f;
    [SerializeField] Vector3 offset;

    void Start()
    {
        transform.position = player.position + offset;
    }

    void LateUpdate()
    {
        if (player == null) return;
        Vector3 futurePos = player.position + offset;
        Vector3 lerpPos = Vector3.Lerp(transform.position, futurePos, camSpeed * Time.deltaTime);
        //if ((lerpPos - futurePos).magnitude < 0.001) lerpPos = futurePos;
        transform.position = lerpPos;
    }
}
