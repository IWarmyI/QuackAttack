using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SecretArea : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject wall;
    [SerializeField] GameObject zoneHider;
    bool hitTrigger = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !hitTrigger)
        {
            hitTrigger = true;
            zoneHider.GetComponent<TilemapRenderer>().enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        hitTrigger = false;
        zoneHider.GetComponent<TilemapRenderer>().enabled = true;
    }
}
