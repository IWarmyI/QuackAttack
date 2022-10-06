using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float minY = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float x = player.transform.position.x;
        float y = Mathf.Max(minY, player.transform.position.y);
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
